using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Cake.OctoDeploy.Tests.Fixtures;
using HttpMock;
using Newtonsoft.Json;

namespace Cake.OctoDeploy.Tests.Helpers
{
    public class GitHubResponseHelper
    {
        #region Constructor

        public GitHubResponseHelper()
            : this("")
        { }

        public GitHubResponseHelper(string uploadUrl)
        {
            ResponseDummy = new GitHubReleaseResponseFixture
            {
                Assets = new List<object>(),
                Author = new Author(),
                Id = ReleaseId,
                TagName = ReleaseId.ToString(),
                CreatedAt = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                UploadUrl = uploadUrl
            };

            AllResponsesDummy = new[]
            {
                ResponseDummy
            };

            AssetUploadResponseDummy = new GitHubAssetUploadResponseFixture
            {
                Id = 123
            };
        }

        #endregion

        public void SetupHttpMockWithValidResponseForReleaseCreation(IHttpServer httpMock, OctoSettingFixture octoSettingMock)
        {
            httpMock.Stub(x => x.Post($"/api/v3/repos/{octoSettingMock.Owner}/{octoSettingMock.Repository}/releases"))
                .AsContentType("application/json")
                .Return(GetResponseJson)
                .OK();
        }

        public void SetupHttpMockWithValidResponseForReleaseRetrieval(IHttpServer httpMock,
            OctoSettingFixture octoSettingFixture)
        {
            httpMock.Stub(x => x.Get($"/api/v3/repos/{octoSettingFixture.Owner}/{octoSettingFixture.Repository}/releases/{ReleaseId}"))
               .AsContentType("application/json")
               .Return(GetResponseJson)
               .OK();
        }

        public void SetupHttpMockWithValidAssetUploadResponse(IHttpServer httpMock,
            OctoSettingFixture octoSettingFixture)
        {
            httpMock.Stub(x => x.Post($"/repos/{octoSettingFixture.Owner}/{octoSettingFixture.Repository}/releases/{ReleaseId}/assets"))
               .AsContentType("application/json")
               .Return(GetUploadResponseJson)
               .WithStatus(HttpStatusCode.Created);
        }

        public void SetupHttpMockWithInvalidAssetUploadResponse(IHttpServer httpMock,
            OctoSettingFixture octoSettingFixture)
        {
            httpMock.Stub(x => x.Post($"/repos/{octoSettingFixture.Owner}/{octoSettingFixture.Repository}/releases/{ReleaseId}/assets"))
                .WithStatus(HttpStatusCode.BadRequest);
        }

        public void SetupHttpMockWithValidResponseForAllReleasesRetrieval(IHttpServer httpMock,
            OctoSettingFixture octoSettingFixture)
        {
            httpMock.Stub(x => x.Get($"/api/v3/repos/{octoSettingFixture.Owner}/{octoSettingFixture.Repository}/releases"))
                .AsContentType("application/json")
                .Return(GetAllResponsesJson)
                .OK();
        }

        #region Properties

        public string GetResponseJson => JsonConvert.SerializeObject(ResponseDummy, new JsonSerializerSettings
        {
            ContractResolver = new LowercaseContractResolver()
        });

        public string GetAllResponsesJson => JsonConvert.SerializeObject(AllResponsesDummy, new JsonSerializerSettings
        {
            ContractResolver = new LowercaseContractResolver()
        });

        public string GetUploadResponseJson
            => JsonConvert.SerializeObject(AssetUploadResponseDummy, new JsonSerializerSettings
            {
                ContractResolver = new LowercaseContractResolver()
            });

        public GitHubReleaseResponseFixture ResponseDummy { get; }
        public GitHubReleaseResponseFixture[] AllResponsesDummy { get; }
        public GitHubAssetUploadResponseFixture AssetUploadResponseDummy { get; }
        public static int ReleaseId => 1234;
        public static string ReleaseName = "1234";

        #endregion
    }
}