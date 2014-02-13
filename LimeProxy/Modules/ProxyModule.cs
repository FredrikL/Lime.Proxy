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

            Options["/{db}/table/{name}"] = AddCorsHeadersForOptionsRequest;
            Options["/{db}/sp/{name}"] = AddCorsHeadersForOptionsRequest;
            
            Post["/{db}/sp/{name}"] = x =>
            {
                var param = this.Bind<ProcedureParameters>();
                Result result = limeWebServiceProxy.ExecuteStoredProcedure(x.db, x.name, param);
                
                if(result.Success)
                    return Response.AsJson(result.Data);

                return Response.AsJson(result.Data, HttpStatusCode.InternalServerError);
            };

            Post["/{db}/table/{name}"] = x =>
            {
                var param = this.Bind<TableQuery>();
                Result result = limeWebServiceProxy.QueryTable(x.db, x.name, param);

                if (result.Success)
                    return Response.AsJson(result.Data);

                return Response.AsJson(result.Data, HttpStatusCode.InternalServerError);
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
