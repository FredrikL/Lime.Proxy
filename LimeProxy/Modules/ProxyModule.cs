using Nancy;

namespace LimeProxy.Modules
{
    public class ProxyModule : NancyModule
    {
        public ProxyModule() : base("/v1")
        {
            Post["/sp/{name}"] = x => "1";

            Post["/table/{name}"] = x => "2";

            After += ctx => ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}
