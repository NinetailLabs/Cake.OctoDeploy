using System.Globalization;
using Newtonsoft.Json.Serialization;

namespace Cake.OctoDeploy.Tests.Helpers
{
    public class LowercaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower(CultureInfo.InvariantCulture);
        }
    }
}