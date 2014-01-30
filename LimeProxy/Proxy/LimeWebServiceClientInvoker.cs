using LimeProxy.LimeWs;

namespace LimeProxy.Proxy
{
    public class LimeWebServiceClientInvoker : ILimeWebSerivceClientInvoker
    {
        public string ExecuteProcedure(string xml)
        {
            var client = new DataServiceClient();
            return client.ExecuteProcedure(ref xml, false);
        }

        public string QueryTable(string xml)
        {
            var client = new DataServiceClient();
            return client.GetXmlQueryData(xml);
        }
    }
}