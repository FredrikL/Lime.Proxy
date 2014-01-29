using LimeProxy.Proxy;
using Nancy;

namespace LimeProxy.Modules
{
    public class ProxyModule : NancyModule
    {
        private readonly ILimeWebServiceProxy _limeWebServiceProxy;

        public ProxyModule(ILimeWebServiceProxy limeWebServiceProxy) : base("/v1")
        {
            _limeWebServiceProxy = limeWebServiceProxy;
            Post["/sp/{name}"] = x => _limeWebServiceProxy.ExecuteStoredProcedure(x.name, null);

            Post["/table/{name}"] = x => _limeWebServiceProxy.QueryTable(x.name, null);

            After += ctx => ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}
