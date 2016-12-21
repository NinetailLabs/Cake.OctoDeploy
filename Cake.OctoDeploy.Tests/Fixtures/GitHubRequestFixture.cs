using System;
using Cake.Core.IO;
using Path = System.IO.Path;

namespace Cake.OctoDeploy.Tests.Fixtures
{
    public static class GitHubRequestFixture
    {
        public static string BaseUrl => "http://localhost:9966";
        public static string Tag => "Test Tag";
        public static string Title => "Test Title";
        public static string ReleaseNotes => "Hello world!";
        public static bool IsDraft => false;
        public static bool IsPreRelease => false;
        public static string Artifact1Name => "artifact1.zip";
        public static string Artifact2Name => "artifact2.zip";
        public static string MimeType => "application/zip";
        public static string ReleaseNotesFile => "ReleaseNotes.txt";
        public static string Artifact1Path => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", Artifact1Name);
        public static string Artifact2Path => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", Artifact2Name);
        public static FilePath Artifact1FilePath => new FilePath(Artifact1Path);
        public static FilePath Artifact2FilePath => new FilePath(Artifact2Path);
        public static FilePath ReleaseNotesFilePath => new FilePath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", ReleaseNotesFile));
    }
}