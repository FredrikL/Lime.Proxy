namespace LimeProxy.Proxy
{
    public interface ILimeWebSerivceClientInvoker
    {
        string ExecuteProcedure(string db, string xml);
        string QueryTable(string db, string xml);
    }
}