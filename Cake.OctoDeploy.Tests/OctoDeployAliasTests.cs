using Cake.Core;
using Cake.Core.IO;
using Cake.OctoDeploy.Tests.Fixtures;
using Cake.OctoDeploy.Tests.Helpers;
using FluentAssertions;
using HttpMock;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using Cake.OctoDeploy;

namespace Cake.OctoDeploy.Tests
{
    public class OctoDeployAliasTests
    {
        #region Public Methods

        [Test]
        public void ErrorDuringAssetUploadThrowsAnException()
        {
            // arrange
            var fixture = new CakeOctoDeployAliasFixture();
            var responseDummy = new GitHubResponseHelper($"{GitHubRequestFixture.BaseUrl}/repos/{fixture.OctoSettingMock.Owner}/{fixture.OctoSettingMock.Repository}/releases/{GitHubResponseHelper.ReleaseId}/assets{{?name,label}}");

            OctoDeployAlias.GitHubApiBaseUrl = GitHubRequestFixture.BaseUrl;
            var httpMock = HttpMockRepository.At(GitHubRequestFixture.BaseUrl);
            responseDummy.SetupHttpMockWithValidResponseForReleaseRetrieval(httpMock, fixture.OctoSettingMock);
            responseDummy.SetupHttpMockWithInvalidAssetUploadResponse(httpMock, fixture.OctoSettingMock);

            var act = new Action(() => fixture.GetCakeContext.UploadArtifact(GitHubResponseHelper.ReleaseId, GitHubRequestFixture.Artifact1FilePath, GitHubRequestFixture.Artifact1Name, GitHubRequestFixture.MimeType, fixture.OctoSettingMock.GetSettings));

            // act
            // assert
            act.ShouldThrow<CakeException>("Unknown error occured while creating release");
        }

        [Test]
        public void PublishBasicReleaseThrowsAnExceptionIfAServerErrorOccurs()
        {
            // arrange
            var fixture = new CakeOctoDeployAliasFixture();
            OctoDeployAlias.GitHubApiBaseUrl = GitHubRequestFixture.BaseUrl;
            var httpMock = HttpMockRepository.At(GitHubRequestFixture.BaseUrl);
            httpMock.Stub(x => x.Post($"/api/v3/repos/{fixture.OctoSettingMock.Owner}/{fixture.OctoSettingMock.Repository}/releases"))
                .WithStatus(HttpStatusCode.BadRequest);

            var act = new Action(() => fixture.GetCakeContext.PublishRelease(GitHubRequestFixture.Tag, GitHubRequestFixture.Title, GitHubRequestFixture.ReleaseNotes, GitHubRequestFixture.IsDraft, GitHubRequestFixture.IsPreRelease, fixture.OctoSettingMock.GetSettings));

            // act
            // assert
            act.ShouldThrow<CakeException>();
        }

        [Test]
        public void PublishBasicReleaseWorksCorrectly()
        {
            // arrange
            var fixture = new CakeOctoDeployAliasFixture();
            var responseFixture = new GitHubResponseHelper();

            OctoDeployAlias.GitHubApiBaseUrl = GitHubRequestFixture.BaseUrl;
            var httpMock = HttpMockRepository.At(GitHubRequestFixture.BaseUrl);
            responseFixture.SetupHttpMockWithValidResponseForReleaseCreation(httpMock, fixture.OctoSettingMock);

            // act
            var result = fixture.GetCakeContext.PublishRelease(GitHubRequestFixture.Tag, GitHubRequestFixture.Title, GitHubRequestFixture.ReleaseNotes,
                GitHubRequestFixture.IsDraft, GitHubRequestFixture.IsPreRelease, fixture.OctoSettingMock.GetSettings);

            // assert
            result.Should().Be(GitHubResponseHelper.ReleaseId);
        }

        [Test]
        public void UploadArtifactWorksCorrectly()
        {
            // arrange
            var fixture = new CakeOctoDeployAliasFixture();
            var responseDummy = new GitHubResponseHelper($"{GitHubRequestFixture.BaseUrl}/repos/{fixture.OctoSettingMock.Owner}/{fixture.OctoSettingMock.Repository}/releases/{GitHubResponseHelper.ReleaseId}/assets{{?name,label}}");

            OctoDeployAlias.GitHubApiBaseUrl = GitHubRequestFixture.BaseUrl;
            var httpMock = HttpMockRepository.At(GitHubRequestFixture.BaseUrl);
            responseDummy.SetupHttpMockWithValidResponseForReleaseRetrieval(httpMock, fixture.OctoSettingMock);
            responseDummy.SetupHttpMockWithValidAssetUploadResponse(httpMock, fixture.OctoSettingMock);

            // act
            fixture.GetCakeContext.UploadArtifact(GitHubResponseHelper.ReleaseId, GitHubRequestFixture.Artifact1FilePath, GitHubRequestFixture.Artifact1Name, GitHubRequestFixture.MimeType, fixture.OctoSettingMock.GetSettings);

            // assert
            fixture.GetCakeLog.Messages.Last()
                .Arguments.First().Should()
                .Be($"Uploaded artifact {GitHubRequestFixture.Artifact1FilePath.FullPath} to GitHub. Id {123}");
        }

        [Test]
        public void PublishReleaseWithReleaseNotesInFileWorksCorrectly()
        {
            // arrange
            var fixture = new CakeOctoDeployAliasFixture();
            var responseFixture = new GitHubResponseHelper();

            OctoDeployAlias.GitHubApiBaseUrl = GitHubRequestFixture.BaseUrl;
            var httpMock = HttpMockRepository.At(GitHubRequestFixture.BaseUrl);
            responseFixture.SetupHttpMockWithValidResponseForReleaseCreation(httpMock, fixture.OctoSettingMock);

            // act
            var result = fixture.GetCakeContext.PublishRelease(GitHubRequestFixture.Tag, GitHubRequestFixture.Title, GitHubRequestFixture.ReleaseNotesFilePath,
                GitHubRequestFixture.IsDraft, GitHubRequestFixture.IsPreRelease, fixture.OctoSettingMock.GetSettings);

            // assert
            result.Should().Be(GitHubResponseHelper.ReleaseId);
        }

        [Test]
        public void PublishWithArtifactWithReleaseNotesInFileWorksCorrectly()
        {
            // arrange
            var fixture = new CakeOctoDeployAliasFixture();
            var responseFixture = new GitHubResponseHelper($"{GitHubRequestFixture.BaseUrl}/repos/{fixture.OctoSettingMock.Owner}/{fixture.OctoSettingMock.Repository}/releases/{GitHubResponseHelper.ReleaseId}/assets{{?name,label}}");

            OctoDeployAlias.GitHubApiBaseUrl = GitHubRequestFixture.BaseUrl;
            var httpMock = HttpMockRepository.At(GitHubRequestFixture.BaseUrl);
            responseFixture.SetupHttpMockWithValidResponseForReleaseCreation(httpMock, fixture.OctoSettingMock);
            responseFixture.SetupHttpMockWithValidResponseForReleaseRetrieval(httpMock, fixture.OctoSettingMock);
            responseFixture.SetupHttpMockWithValidAssetUploadResponse(httpMock, fixture.OctoSettingMock);

            var act = new Action(() =>
            {
                fixture.GetCakeContext.PublishReleaseWithArtifact(GitHubRequestFixture.Tag, GitHubRequestFixture.Title, GitHubRequestFixture.ReleaseNotesFilePath,
                GitHubRequestFixture.IsDraft, GitHubRequestFixture.IsPreRelease, GitHubRequestFixture.Artifact1FilePath,
                GitHubRequestFixture.Artifact1Name, GitHubRequestFixture.MimeType, fixture.OctoSettingMock.GetSettings);
            });

            // act
            // assert
            act.ShouldNotThrow<CakeException>();
        }

        [Test]
        public void PublishArtifactWithReleaseNotesWorksCorrectly()
        {
            // arrange
            var fixture = new CakeOctoDeployAliasFixture();
            var responseFixture = new GitHubResponseHelper($"{GitHubRequestFixture.BaseUrl}/repos/{fixture.OctoSettingMock.Owner}/{fixture.OctoSettingMock.Repository}/releases/{GitHubResponseHelper.ReleaseId}/assets{{?name,label}}");

            OctoDeployAlias.GitHubApiBaseUrl = GitHubRequestFixture.BaseUrl;
            var httpMock = HttpMockRepository.At(GitHubRequestFixture.BaseUrl);
            responseFixture.SetupHttpMockWithValidResponseForReleaseCreation(httpMock, fixture.OctoSettingMock);
            responseFixture.SetupHttpMockWithValidResponseForReleaseRetrieval(httpMock, fixture.OctoSettingMock);
            responseFixture.SetupHttpMockWithValidAssetUploadResponse(httpMock, fixture.OctoSettingMock);

            var act = new Action(() =>
            {
                fixture.GetCakeContext.PublishReleaseWithArtifact(GitHubRequestFixture.Tag, GitHubRequestFixture.Title, GitHubRequestFixture.ReleaseNotes,
                GitHubRequestFixture.IsDraft, GitHubRequestFixture.IsPreRelease, GitHubRequestFixture.Artifact1FilePath,
                GitHubRequestFixture.Artifact1Name, GitHubRequestFixture.MimeType, fixture.OctoSettingMock.GetSettings);
            });

            // act
            // assert
            act.ShouldNotThrow<CakeException>();
        }

        [Test]
        public void PublishReleaseWithArtifactsThrowsAnExceptionIfAllArraysAreNotTheSameSize()
        {
            // arrange
            var fixture = new CakeOctoDeployAliasFixture();
            var responseFixture = new GitHubResponseHelper($"{GitHubRequestFixture.BaseUrl}/repos/{fixture.OctoSettingMock.Owner}/{fixture.OctoSettingMock.Repository}/releases/{GitHubResponseHelper.ReleaseId}/assets{{?name,label}}");

            var act = new Action(() =>
            {
                fixture.GetCakeContext.PublishReleaseWithArtifacts(GitHubRequestFixture.Tag, GitHubRequestFixture.Title, GitHubRequestFixture.ReleaseNotes,
                GitHubRequestFixture.IsDraft, GitHubRequestFixture.IsPreRelease, new[] { new FilePath("blah")},new[] {"art1", "art2"}, new[] {"mime1"}, 
                fixture.OctoSettingMock.GetSettings);
            });

            // act
            // assert
            act.ShouldThrow<CakeException>(
                "ArtifactPaths, ArtifactNames and ArtifactMimeTypes all need to be the same length");
        }

        [Test]
        public void PublishArtifactsWithReleaseNotesWorksCorrectly()
        {
            // arrange
            var fixture = new CakeOctoDeployAliasFixture();
            var responseFixture = new GitHubResponseHelper($"{GitHubRequestFixture.BaseUrl}/repos/{fixture.OctoSettingMock.Owner}/{fixture.OctoSettingMock.Repository}/releases/{GitHubResponseHelper.ReleaseId}/assets{{?name,label}}");

            OctoDeployAlias.GitHubApiBaseUrl = GitHubRequestFixture.BaseUrl;
            var httpMock = HttpMockRepository.At(GitHubRequestFixture.BaseUrl);
            responseFixture.SetupHttpMockWithValidResponseForReleaseCreation(httpMock, fixture.OctoSettingMock);
            responseFixture.SetupHttpMockWithValidResponseForReleaseRetrieval(httpMock, fixture.OctoSettingMock);
            responseFixture.SetupHttpMockWithValidAssetUploadResponse(httpMock, fixture.OctoSettingMock);

            var act = new Action(() =>
            {
                fixture.GetCakeContext.PublishReleaseWithArtifacts(GitHubRequestFixture.Tag, GitHubRequestFixture.Title, GitHubRequestFixture.ReleaseNotes,
                GitHubRequestFixture.IsDraft, GitHubRequestFixture.IsPreRelease, new[] { GitHubRequestFixture.Artifact1FilePath, GitHubRequestFixture.Artifact2FilePath},
                new [] { GitHubRequestFixture.Artifact1Name, GitHubRequestFixture.Artifact2Name}, new [] { GitHubRequestFixture.MimeType, GitHubRequestFixture.MimeType}, fixture.OctoSettingMock.GetSettings);
            });

            // act
            // assert
            act.ShouldNotThrow<CakeException>();
        }

        [Test]
        public void PublishArtifactsWithReleaseNotesFileWorksCorrectly()
        {
            // arrange
            var fixture = new CakeOctoDeployAliasFixture();
            var responseFixture = new GitHubResponseHelper($"{GitHubRequestFixture.BaseUrl}/repos/{fixture.OctoSettingMock.Owner}/{fixture.OctoSettingMock.Repository}/releases/{GitHubResponseHelper.ReleaseId}/assets{{?name,label}}");

            OctoDeployAlias.GitHubApiBaseUrl = GitHubRequestFixture.BaseUrl;
            var httpMock = HttpMockRepository.At(GitHubRequestFixture.BaseUrl);
            responseFixture.SetupHttpMockWithValidResponseForReleaseCreation(httpMock, fixture.OctoSettingMock);
            responseFixture.SetupHttpMockWithValidResponseForReleaseRetrieval(httpMock, fixture.OctoSettingMock);
            responseFixture.SetupHttpMockWithValidAssetUploadResponse(httpMock, fixture.OctoSettingMock);

            var act = new Action(() =>
            {
                fixture.GetCakeContext.PublishReleaseWithArtifacts(GitHubRequestFixture.Tag, GitHubRequestFixture.Title, GitHubRequestFixture.ReleaseNotesFilePath,
                GitHubRequestFixture.IsDraft, GitHubRequestFixture.IsPreRelease, new[] { GitHubRequestFixture.Artifact1FilePath, GitHubRequestFixture.Artifact2FilePath },
                new[] { GitHubRequestFixture.Artifact1Name, GitHubRequestFixture.Artifact2Name }, new[] { GitHubRequestFixture.MimeType, GitHubRequestFixture.MimeType }, fixture.OctoSettingMock.GetSettings);
            });

            // act
            // assert
            act.ShouldNotThrow<CakeException>();
        }

        #endregion

        private class CakeOctoDeployAliasFixture
        {
            #region Constructor

            public CakeOctoDeployAliasFixture(int exitCode)
            {
                GetDirectoryPath = Guid.NewGuid().ToString();

                CakeContextMock.Setup(t => t.ProcessRunner).Returns(ProcessRunnerMock.Object);
                CakeContextMock.Setup(t => t.FileSystem).Returns(FileSysteMock.Object);
                CakeContextMock.Setup(t => t.Log).Returns(GetCakeLog);

                ProcessRunnerMock.Setup(t => t.Start(It.IsAny<FilePath>(), It.IsAny<ProcessSettings>()))
                    .Returns(ProcessMock.Object);
                ProcessMock.Setup(t => t.GetExitCode()).Returns(exitCode);
            }

            public CakeOctoDeployAliasFixture()
                : this(0)
            {
            }

            #endregion

            #region Properties

            public Mock<ICakeContext> CakeContextMock { get; } = new Mock<ICakeContext>();

            public Mock<IFileSystem> FileSysteMock { get; } = new Mock<IFileSystem>();

            public ICakeContext GetCakeContext => CakeContextMock.Object;

            public CakeLogFixture GetCakeLog { get; } = new CakeLogFixture();
            public DirectoryPath GetDirectoryPath { get; }

            public OctoSettingFixture OctoSettingMock { get; } = new OctoSettingFixture();

            public Mock<IProcess> ProcessMock { get; } = new Mock<IProcess>();

            public Mock<IProcessRunner> ProcessRunnerMock { get; } = new Mock<IProcessRunner>();

            #endregion
        }
    }
}