namespace Cake.OctoDeploy.Tests.Fixtures
{
    public class Uploader
    {
        public string Login { get; set; }
        public int Id { get; set; }
        public string AvatarUrl { get; set; }
        public string GravatarId { get; set; }
        public string Url { get; set; }
        public string HtmlUrl { get; set; }
        public string FollowersUrl { get; set; }
        public string FollowingUrl { get; set; }
        public string GistsUrl { get; set; }
        public string StarredUrl { get; set; }
        public string SubscriptionsUrl { get; set; }
        public string OrganizationsUrl { get; set; }
        public string ReposUrl { get; set; }
        public string EventsUrl { get; set; }
        public string ReceivedEventsUrl { get; set; }
        public string Type { get; set; }
        public bool SiteAdmin { get; set; }
    }

    public class GitHubAssetUploadResponseFixture
    {
        public string Url { get; set; }
        public string BrowserDownloadUrl { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string State { get; set; }
        public string ContentType { get; set; }
        public int Size { get; set; }
        public int DownloadCount { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public Uploader Uploader { get; set; }
    }
}