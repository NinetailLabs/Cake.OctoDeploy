namespace Cake.OctoDeploy.Tests.Fixtures
{
    public class OctoSettingFixture
    {
        #region Constructor

        public OctoSettingFixture()
        {
            GetSettings = new OctoDeploySettings
            {
                Owner = Owner,
                Repository = Repository,
                AccessToken = Token
            };
        }

        #endregion

        #region Properties

        public OctoDeploySettings GetSettings { get; }
        public string Owner => "NineTailLabs";
        public string Repository => "Cake.OctoDeploy";
        public string Token => "sometoken";

        #endregion
    }
}