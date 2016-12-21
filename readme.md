## Cake.OctoDeploy

[![Build status](https://ci.appveyor.com/api/projects/status/4kpfk44nw4n8duso/branch/master?svg=true)](https://ci.appveyor.com/project/DeadlyEmbrace/cake-octodeploy/branch/master)
[![NuGet](https://img.shields.io/nuget/v/Cake.OctoDeploy.svg)](https://www.nuget.org/packages/Cake.OctoDeploy/)
[![Coverage Status](https://coveralls.io/repos/github/NinetailLabs/Cake.OctoDeploy/badge.svg?branch=master)](https://coveralls.io/github/NinetailLabs/Cake.OctoDeploy?branch=master)

An addin for [Cake](http://cakebuild.net/) that allows for the easy creation of GitHub releases with or without assets

```csharp
#addin nuget:?package=Cake.OctoDeploy
```

### Methods
The following methods are provided:

- PublishRelease(this ICakeContext context, string tag, string releaseTitle, string releaseNotes, bool draftRelease, bool preRelease, OctoDeploySettings octoDeploySettings)
- PublishRelease(this ICakeContext context, string tag, string releaseTitle, FilePath releaseNotesFilePath, bool draftRelease, bool preRelease, OctoDeploySettings octoDeploySettings)

*Publish a release on GitHub*

- PublishReleaseWithArtifact(this ICakeContext context, string tag, string releaseTitle, string releaseNotes, bool draftRelease, bool preRelease, FilePath artifactPath, string artifactName, string artifactMimeType, OctoDeploySettings octoDeploySettings)
- PublishReleaseWithArtifact(this ICakeContext context, string tag, string releaseTitle, FilePath releaseNotesFilePath, bool draftRelease, bool preRelease, FilePath artifactPath, string artifactName, string artifactMimeType, OctoDeploySettings octoDeploySettings)

*Publish a release with an artifact*

- PublishReleaseWithArtifacts(this ICakeContext context, string tag, string releaseTitle, string releaseNotes, bool draftRelease, bool preRelease, FilePath[] artifactPaths, string[] artifactNames, string[] artifactMimeTypes, OctoDeploySettings octoDeploySettings)
- PublishReleaseWithArtifacts(this ICakeContext context, string tag, string releaseTitle, FilePath releaseNotesFilePath, bool draftRelease, bool preRelease, FilePath[] artifactPaths, string[] artifactNames, string[] artifactMimeTypes, OctoDeploySettings octoDeploySettings)

*Publish a release with an artifact. Note that the artifact paths and their respective names need to be in the same order in their respective arrays otherwise they will end up incorrectly named*

- UploadArtifact(this ICakeContext context, int releaseId, FilePath artifactPath, string artifactName, string artifactMimeType, OctoDeploySettings octoDeploySettings)

*Upload an artifact to an existing release*

### Icon
[Github](https://thenounproject.com/search/?q=git&i=543561) by [Doejo](https://thenounproject.com/doejo/) from [The Noun Project](https://thenounproject.com/), remixed with the Cake icon.