using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Octokit;
using System;
using System.IO;
using System.Linq;

namespace Cake.OctoDeploy
{
    /// <summary>
    /// Simple GitHub release publishing
    /// </summary>
    [CakeAliasCategory("OctoDeploy")]
    public static class OctoDeployAlias
    {
        #region Properties

        /// <summary>
        /// GitHub Api base URL
        /// </summary>
        public static string GitHubApiBaseUrl { private get; set; } = "https://api.github.com";

        #endregion

        #region Public Methods

        /// <summary>
        /// Publish a release on GitHub
        /// </summary>
        /// <param name="context">Cake context</param>
        /// <param name="tag">Tag for the release</param>
        /// <param name="releaseTitle">Title of the release</param>
        /// <param name="releaseNotesFilePath">Path to file containing Release notes</param>
        /// <param name="draftRelease">Should the release be published as a draft</param>
        /// <param name="preRelease">Should the release be published as a pre-release</param>
        /// <param name="octoDeploySettings">OctoDeploy settings</param>
        /// <returns>Id of the Release</returns>
        [CakeMethodAlias]
        public static int PublishRelease(this ICakeContext context, string tag, string releaseTitle, FilePath releaseNotesFilePath,
            bool draftRelease, bool preRelease, OctoDeploySettings octoDeploySettings)
        {
            var releaseNotes = File.ReadAllText(releaseNotesFilePath.FullPath);
            return context.PublishRelease(tag, releaseTitle, releaseNotes, draftRelease, preRelease, octoDeploySettings);
        }

        /// <summary>
        /// Publish a release on GitHub
        /// </summary>
        /// <param name="context">Cake context</param>
        /// <param name="tag">Tag for the release</param>
        /// <param name="releaseTitle">Title of the release</param>
        /// <param name="releaseNotes">Release notes</param>
        /// <param name="draftRelease">Should the release be published as a draft</param>
        /// <param name="preRelease">Should the release be published as a pre-release</param>
        /// <param name="octoDeploySettings">OctoDeploy settings</param>
        /// <returns>Id of the Release</returns>
        [CakeMethodAlias]
        public static int PublishRelease(this ICakeContext context, string tag, string releaseTitle, string releaseNotes, bool draftRelease, bool preRelease, OctoDeploySettings octoDeploySettings)
        {
            var client = new GitHubClient(new ProductHeaderValue("Cake.OctoDeploy"), new Uri(GitHubApiBaseUrl))
            {
                Credentials = new Credentials(octoDeploySettings.AccessToken)
            };

            var newRelease = new NewRelease(tag)
            {
                Name = releaseTitle,
                Body = releaseNotes,
                Draft = draftRelease,
                Prerelease = preRelease
            };

            try
            {
                var result = client.Repository
                    .Release
                    .Create(octoDeploySettings.Owner, octoDeploySettings.Repository, newRelease)
                    .Result;

                context.Log.Information($"Created GitHub release with Id {result.Id}");

                return result.Id;
            }
            catch (AggregateException exception)
            {
                var innerException = exception.InnerException;
                if (innerException is ApiException validationException)
                {
                    foreach (var error in validationException.ApiError.Errors ?? Enumerable.Empty<ApiErrorDetail>())
                    {
                        context.Log.Error($"Resource: {error.Resource}, Field: {error.Field}, Code: {error.Code}");
                    }
                }

                throw new CakeException(string.IsNullOrEmpty(innerException?.Message) ? "Unknown error occured while creating release" : innerException.Message);
            }
        }

        /// <summary>
        /// Publish a release with an artifact
        /// </summary>
        /// <param name="context">Cake context</param>
        /// <param name="tag">Tag for the release</param>
        /// <param name="releaseTitle">Title of the release</param>
        /// <param name="releaseNotesFilePath">Path to file containing Release notes</param>
        /// <param name="draftRelease">Should the release be published as a draft</param>
        /// <param name="preRelease">Should the release be published as a pre-release</param>
        /// <param name="artifactPath">Path to the artifact to upload</param>
        /// <param name="artifactName">Name of the artifact to use on the release</param>
        /// <param name="artifactMimeType">The MIME type of the artifact that is being uploaded</param>
        /// <param name="octoDeploySettings">OctoDeploy Settings</param>
        [CakeMethodAlias]
        public static void PublishReleaseWithArtifact(this ICakeContext context, string tag, string releaseTitle,
            FilePath releaseNotesFilePath, bool draftRelease, bool preRelease, FilePath artifactPath, string artifactName,
            string artifactMimeType, OctoDeploySettings octoDeploySettings)
        {
            var id = context.PublishRelease(tag, releaseTitle, releaseNotesFilePath, draftRelease, preRelease,
                octoDeploySettings);
            context.UploadArtifact(id, artifactPath, artifactName, artifactMimeType, octoDeploySettings);
        }

        /// <summary>
        /// Publish a release with an artifact
        /// </summary>
        /// <param name="context">Cake context</param>
        /// <param name="tag">Tag for the release</param>
        /// <param name="releaseTitle">Title of the release</param>
        /// <param name="releaseNotes">Release notes</param>
        /// <param name="draftRelease">Should the release be published as a draft</param>
        /// <param name="preRelease">Should the release be published as a pre-release</param>
        /// <param name="artifactPath">Path to the artifact to upload</param>
        /// <param name="artifactName">Name of the artifact to use on the release</param>
        /// <param name="artifactMimeType">The MIME type of the artifact that is being uploaded</param>
        /// <param name="octoDeploySettings">OctoDeploy Settings</param>
        [CakeMethodAlias]
        public static void PublishReleaseWithArtifact(this ICakeContext context, string tag, string releaseTitle,
            string releaseNotes, bool draftRelease, bool preRelease, FilePath artifactPath, string artifactName,
            string artifactMimeType, OctoDeploySettings octoDeploySettings)
        {
            var id = context.PublishRelease(tag, releaseTitle, releaseNotes, draftRelease, preRelease,
                octoDeploySettings);
            context.UploadArtifact(id, artifactPath, artifactName, artifactMimeType, octoDeploySettings);
        }

        /// <summary>
        /// Publish a release with multiple artifacts.
        /// Note that the artifact paths and their respective names and MIME types need to be in the same order in their respective arrays otherwise they will end up incorrectly named
        /// </summary>
        /// <param name="context">Cake context</param>
        /// <param name="tag">Tag for the release</param>
        /// <param name="releaseTitle">Title of the release</param>
        /// <param name="releaseNotesFilePath">Path to file containing Release notes</param>
        /// <param name="draftRelease">Should the release be published as a draft</param>
        /// <param name="preRelease">Should the release be published as a pre-release</param>
        /// <param name="artifactPaths">Paths to the artifacts to upload</param>
        /// <param name="artifactNames">Names of the artifacts to use on the release</param>
        /// <param name="artifactMimeTypes">The MIME type of the artifact that is being uploaded</param>
        /// <param name="octoDeploySettings">OctoDeploy Settings</param>
        [CakeMethodAlias]
        public static void PublishReleaseWithArtifacts(this ICakeContext context, string tag, string releaseTitle,
            FilePath releaseNotesFilePath, bool draftRelease, bool preRelease, FilePath[] artifactPaths, string[] artifactNames,
            string[] artifactMimeTypes, OctoDeploySettings octoDeploySettings)
        {
            var releaseNotes = File.ReadAllText(releaseNotesFilePath.FullPath);
            context.PublishReleaseWithArtifacts(tag, releaseTitle, releaseNotes, draftRelease, preRelease, artifactPaths, artifactNames, artifactMimeTypes, octoDeploySettings);
        }

        /// <summary>
        /// Publish a release with an artifact.
        /// Note that the artifact paths and their respective names need to be in the same order in their respective arrays otherwise they will end up incorrectly named
        /// </summary>
        /// <param name="context">Cake context</param>
        /// <param name="tag">Tag for the release</param>
        /// <param name="releaseTitle">Title of the release</param>
        /// <param name="releaseNotes">Release notes</param>
        /// <param name="draftRelease">Should the release be published as a draft</param>
        /// <param name="preRelease">Should the release be published as a pre-release</param>
        /// <param name="artifactPaths">Paths to the artifacts to upload</param>
        /// <param name="artifactNames">Names of the artifacts to use on the release</param>
        /// <param name="artifactMimeTypes">The MIME type of the artifact that is being uploaded</param>
        /// <param name="octoDeploySettings">OctoDeploy Settings</param>
        [CakeMethodAlias]
        public static void PublishReleaseWithArtifacts(this ICakeContext context, string tag, string releaseTitle,
            string releaseNotes, bool draftRelease, bool preRelease, FilePath[] artifactPaths, string[] artifactNames,
            string[] artifactMimeTypes, OctoDeploySettings octoDeploySettings)
        {
            if (artifactPaths.Length != artifactMimeTypes.Length || artifactPaths.Length != artifactNames.Length)
            {
                throw new CakeException("ArtifactPaths, ArtifactNames and ArtifactMimeTypes all need to be the same length");
            }

            var id = context.PublishRelease(tag, releaseTitle, releaseNotes, draftRelease, preRelease,
                octoDeploySettings);
            for (var r = 0; r < artifactPaths.Length; r++)
            {
                context.UploadArtifact(id, artifactPaths[r], artifactNames[r], artifactMimeTypes[r], octoDeploySettings);
            }
        }

        /// <summary>
        /// Upload an artifact to an existing release
        /// </summary>
        /// <param name="context">Cake context</param>
        /// <param name="releaseId">Id of the Release to which the asset should be attached</param>
        /// <param name="artifactPath">Path to the artifact to upload</param>
        /// <param name="artifactName">Name of the artifact to use on the release</param>
        /// <param name="artifactMimeType">The MIME type of the artifact that is being uploaded</param>
        /// <param name="octoDeploySettings">OctoDeploy Settings</param>
        [CakeMethodAlias]
        public static void UploadArtifact(this ICakeContext context, int releaseId, FilePath artifactPath, string artifactName,
            string artifactMimeType, OctoDeploySettings octoDeploySettings)
        {
            var client = new GitHubClient(new ProductHeaderValue("Cake.OctoDeploy"), new Uri(GitHubApiBaseUrl))
            {
                Credentials = new Credentials(octoDeploySettings.AccessToken)
            };

            context.Log.Information("Uploading Artifacts...");
            using (var archiveContents = File.OpenRead(artifactPath.FullPath))
            {
                var assetUpload = new ReleaseAssetUpload
                {
                    FileName = artifactName,
                    ContentType = artifactMimeType,
                    RawData = archiveContents
                };

                try
                {
                    var release = client.Repository
                        .Release
                        .Get(octoDeploySettings.Owner, octoDeploySettings.Repository, releaseId)
                        .Result;

                    var asset = client.Repository
                    .Release
                    .UploadAsset(release, assetUpload)
                    .Result;

                    context.Log.Information($"Uploaded artifact {artifactPath.FullPath} to GitHub. Id {asset.Id}");
                }
                catch (Exception exception)
                {
                    var innerException = exception.InnerException;
                    
                    throw new CakeException(string.IsNullOrEmpty(innerException?.Message) ? "Unknown error occured while creating release" : innerException.Message);
                }
            }
        }

        /// <summary>
        /// Upload an artifact to an existing release
        /// </summary>
        /// <param name="context">Cake context</param>
        /// <param name="tagName">Git tag of the Release to which the asset should be attached</param>
        /// <param name="artifactPath">Path to the artifact to upload</param>
        /// <param name="artifactName">Name of the artifact to use on the release</param>
        /// <param name="artifactMimeType">The MIME type of the artifact that is being uploaded</param>
        /// <param name="octoDeploySettings">OctoDeploy Settings</param>
        [CakeMethodAlias]
        public static void UploadArtifact(this ICakeContext context, string tagName, FilePath artifactPath,
            string artifactName, string artifactMimeType, OctoDeploySettings octoDeploySettings)
        {
            var client = new GitHubClient(new ProductHeaderValue("Cake.OctoDeploy"), new Uri(GitHubApiBaseUrl))
            {
                Credentials = new Credentials(octoDeploySettings.AccessToken)
            };

            try
            {
                var releases = client.Repository
                    .Release
                    .GetAll(octoDeploySettings.Owner, octoDeploySettings.Repository)
                    .Result;

                var namedReleases = releases.Where(x => x.TagName == tagName).ToArray();

                if (!namedReleases.Any())
                {
                    throw new CakeException($"No releases with tag '{tagName}' found");
                }

                var releaseIds = namedReleases.Select(x => x.Id).Distinct();

                foreach (var releaseId in releaseIds)
                {
                    UploadArtifact(context, releaseId, artifactPath, artifactName, artifactMimeType, octoDeploySettings);
                }
            }
            catch (Exception exception)
            {
                var innerException = exception.InnerException;

                throw new CakeException(string.IsNullOrEmpty(innerException?.Message) ? "Unknown error occured while creating release" : innerException.Message);
            }
        }

        #endregion
    }
}