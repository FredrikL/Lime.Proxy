using System;
using System.Xml;
using System.Xml.Linq;
using LimeProxy.Models;
using Newtonsoft.Json;

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
        private readonly IValueTypeProvider _valueTypeProvider;

        public LimeWebServiceProxy(ILimeWebSerivceClientInvoker limeWebSerivceClientInvoker,
            IValueTypeProvider valueTypeProvider)
        {
            _limeWebSerivceClientInvoker = limeWebSerivceClientInvoker;
            _valueTypeProvider = valueTypeProvider;
        }

        public Result ExecuteStoredProcedure(string name, ProcedureParameters parameters)
        {
            try
            {
                var xml = BuildProcedureXml(name, parameters);
                var result = _limeWebSerivceClientInvoker.ExecuteProcedure(xml);
                var json = ConvertFromXmlStringToJsonString(result);
                return new Result()
                {
                    Success = true,
                    Data = json
                };
            }
            catch (Exception ex)
            {
                return new Result()
                {
                    Success = false
                };
            }
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
                    new XAttribute("valuetype", _valueTypeProvider.Get(param.Value))));
            }
            return x.ToString();
        }

        private string ConvertFromXmlStringToJsonString(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            string jsonText = JsonConvert.SerializeXmlNode(doc.SelectSingleNode("/data"));
            return jsonText;
        }
    }
}
