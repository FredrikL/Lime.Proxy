using LimeProxy.Models;

namespace LimeProxy.Proxy
{
    public interface ILimeWebServiceProxy
    {
        Result ExecuteStoredProcedure(string name, object parameters);
        Result QueryTable(string name, object parameters);
    }

    class LimeWebServiceProxy : ILimeWebServiceProxy
    {
        public Result ExecuteStoredProcedure(string name, object parameters)
        {
            throw new System.NotImplementedException();
        }

        public Result QueryTable(string name, object parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}
