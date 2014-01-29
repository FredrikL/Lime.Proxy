using System;
using System.Xml;
using System.Xml.Linq;
using LimeProxy.Models;

namespace LimeProxy.Proxy
{
    public interface ILimeWebServiceProxy
    {
        Result ExecuteStoredProcedure(string name, ProcedureParameters parameters);
        Result QueryTable(string name, TableQuery parameters);
    }

    public class LimeWebServiceProxy : ILimeWebServiceProxy
    {
        private readonly ILimeWebSerivceClientInvoker _limeWebSerivceClientInvoker;

        public LimeWebServiceProxy(ILimeWebSerivceClientInvoker limeWebSerivceClientInvoker)
        {
            _limeWebSerivceClientInvoker = limeWebSerivceClientInvoker;
        }

        public Result ExecuteStoredProcedure(string name, ProcedureParameters parameters)
        {
            var xml = BuildProcedureXml(name, parameters);
            var result = _limeWebSerivceClientInvoker.ExecuteProcedure(xml);

            return new Result();
        }

        public Result QueryTable(string name, TableQuery parameters)
        {
            throw new System.NotImplementedException();
        }

        private string BuildProcedureXml(string name, ProcedureParameters parameters)
        {
            var x = new XElement("procedure", new XAttribute("name", name));
            foreach (var param in parameters.Parameters)
            {
                x.Add(new XElement("parameter",
                    new XAttribute("name", param.Name),
                    new XAttribute("value", param.Value),
                    new XAttribute("valuetype", 2)));
            }
            return x.ToString();
        }
    }

    
}
