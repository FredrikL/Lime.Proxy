using LimeProxy.Models;
using LimeProxy.Proxy;
using Nancy;
using Nancy.ModelBinding;

namespace LimeProxy.Modules
{
    public class ProxyModule : NancyModule
    {
        private readonly ILimeWebServiceProxy _limeWebServiceProxy;

        public ProxyModule(ILimeWebServiceProxy limeWebServiceProxy) : base("/v1")
        {
            _limeWebServiceProxy = limeWebServiceProxy;
            Post["/sp/{name}"] = x =>
            {
                var param = this.Bind<ProcedureParameters>();
                return _limeWebServiceProxy.ExecuteStoredProcedure(x.name, param);
            };

            Post["/table/{name}"] = x =>
            {
                var param = this.Bind<TableQuery>();
                return _limeWebServiceProxy.QueryTable(x.name, param);
            };

            After += ctx => ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}
