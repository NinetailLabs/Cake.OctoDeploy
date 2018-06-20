using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cake.OctoDeploy.Tests.Fixtures
{
    public class Author
    {
        #region Properties

        public string AvatarUrl { get; set; }
        public string EventsUrl { get; set; }
        public string FollowersUrl { get; set; }
        public string FollowingUrl { get; set; }
        public string GistsUrl { get; set; }
        public string GravatarId { get; set; }
        public string HtmlUrl { get; set; }
        public int Id { get; set; }
        public string Login { get; set; }
        public string OrganizationsUrl { get; set; }
        public string ReceivedEventsUrl { get; set; }
        public string ReposUrl { get; set; }
        public bool SiteAdmin { get; set; }
        public string StarredUrl { get; set; }
        public string SubscriptionsUrl { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }

        #endregion
    }

    public class GitHubReleaseResponseFixture
    {
        #region Properties

        public List<object> Assets { get; set; }
        [JsonProperty("assets_url")]
        public string AssetsUrl { get; set; }
        public Author Author { get; set; }
        public string Body { get; set; }
        public string CreatedAt { get; set; }
        public bool Draft { get; set; }
        public string HtmlUrl { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Prerelease { get; set; }
        [JsonProperty("published_at")]
        public string PublishedAt { get; set; }
        [JsonProperty("tag_name")]
        public string TagName { get; set; }
        public string TarballUrl { get; set; }
        public string TargetCommitish { get; set; }
        [JsonProperty("upload_url")]
        public string UploadUrl { get; set; }
        public string Url { get; set; }
        public string ZipballUrl { get; set; }

        #endregion
    }
}