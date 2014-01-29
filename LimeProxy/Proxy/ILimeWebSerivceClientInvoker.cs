namespace LimeProxy.Proxy
{
    public interface ILimeWebSerivceClientInvoker
    {
        string ExecuteProcedure(string xml);
        string QueryTable(string xml);
    }
}