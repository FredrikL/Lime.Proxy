using LimeProxy.Models;

namespace LimeProxy.Proxy
{
    public interface ILimeWebServiceProxy
    {
        Result ExecuteStoredProcedure(string name, object parameters);
        Result QueryTable(string name, object parameters);
    }
}
