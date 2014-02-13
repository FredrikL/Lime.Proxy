using LimeProxy.LimeWs;

namespace LimeProxy.Proxy
{
    public class LimeWebServiceClientInvoker : ILimeWebSerivceClientInvoker
    {
        private readonly IEndpointAddressProvider _endpointAddressProvider;

        public LimeWebServiceClientInvoker(IEndpointAddressProvider endpointAddressProvider)
        {
            _endpointAddressProvider = endpointAddressProvider;
        }

        public string ExecuteProcedure(string db, string xml)
        {
            var client = GetDataServiceClient(db);
            return client.ExecuteProcedure(ref xml, false);
        }

        private DataServiceClient GetDataServiceClient(string db)
        {
            var client = new DataServiceClient();
            client.Endpoint.Address = _endpointAddressProvider.GetUrlForDataBase(db);
            return client;
        }

        public string QueryTable(string db, string xml)
        {
            var client = GetDataServiceClient(db);
            return client.GetXmlQueryData(xml);
        }
    }
}