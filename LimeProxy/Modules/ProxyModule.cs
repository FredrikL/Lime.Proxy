using Nancy;

namespace LimeProxy.Modules
{
    public class ProxyModule : NancyModule
    {
        public ProxyModule()
        {
            Get["/Api/Version"] = x => "1";
        }
    }
}
