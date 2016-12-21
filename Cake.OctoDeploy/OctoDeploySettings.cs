namespace Cake.OctoDeploy
{
    /// <summary>
    /// OctoDeploy settings
    /// </summary>
    public class OctoDeploySettings
    {
        #region Properties

        /// <summary>
        /// Github Personal Access token.
        /// TODO - Add which permissions are required
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Owner of the GitHub repository
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Name of the repository
        /// </summary>
        public string Repository { get; set; }

        #endregion
    }
}