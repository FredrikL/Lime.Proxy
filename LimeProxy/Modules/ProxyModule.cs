using LimeProxy.Models;
using LimeProxy.Proxy;
using Nancy;
using Nancy.ModelBinding;

namespace LimeProxy.Modules
{
    public class ProxyModule : NancyModule
    {
        public ProxyModule(ILimeWebServiceProxy limeWebServiceProxy) : base("/v1")
        {
            Get["/Echo/{data}"] = x => Response.AsText((string)x.data);

            Options["/table/{name}"] = AddCorsHeadersForOptionsRequest;
            Options["/sp/{name}"] = AddCorsHeadersForOptionsRequest;
            
            Post["/sp/{name}"] = x =>
            {
                var param = this.Bind<ProcedureParameters>();
                return limeWebServiceProxy.ExecuteStoredProcedure(x.name, param);
            };

            Post["/table/{name}"] = x =>
            {
                var param = this.Bind<TableQuery>();
                return limeWebServiceProxy.QueryTable(x.name, param);
            };

            After += ctx => ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        private Response AddCorsHeadersForOptionsRequest(dynamic x)
        {
            return this.Response.AsJson(Request)
                .WithHeader("Access-Control-Allow-Methods", "POST, OPTIONS, GET")
                .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type");
        }
    }
}
