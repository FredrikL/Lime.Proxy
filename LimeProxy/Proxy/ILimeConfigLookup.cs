using System.Xml.Linq;
using System.Xml.XPath;

namespace LimeProxy.Proxy
{
    public interface ILimeConfigLookup
    {
        string GetUrlForDatabase(string database);
    }

    public class LimeConfigLookup : ILimeConfigLookup
    {
        private readonly IConfigurationManagerWrapper _configurationManagerWrapper;

        public LimeConfigLookup(IConfigurationManagerWrapper configurationManagerWrapper)
        {
            _configurationManagerWrapper = configurationManagerWrapper;
        }

        public string GetUrlForDatabase(string database)
        {
            var path = _configurationManagerWrapper.AppSettings("webserviceconfpath");
            var doc = XDocument.Load(path);
            var node = doc.XPathSelectElement("/configuration/lundalogik.tangelo/accounts/account[@databaseName='"+database+"']");
            var name = node.Attribute("serviceName").Value;
            var endpointNode =
                doc.XPathSelectElement("/configuration/system.serviceModel/services/service[@name='" + name +
                                       "']/endpoint");
            return endpointNode.Attribute("address").Value;
        }
    }
}