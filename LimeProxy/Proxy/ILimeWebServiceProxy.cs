using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using LimeProxy.Models;
using Nancy.Json;
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
                    Success = false,
                    Data = JsonConvert.SerializeObject(ex)
                };
            }
        }

        public Result QueryTable(string name, TableQuery parameters)
        {
            try
            {
                var xml = BuildQueryXml(name, parameters);
                var result = _limeWebSerivceClientInvoker.QueryTable(xml);
                var json = ConvertFromXmlStringToJsonString(result);
                return new Result()
                {
                    Data = json,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new Result()
                {
                    Success = false,
                    Data = JsonConvert.SerializeObject(ex)
                };
            }
        }

        private string BuildProcedureXml(string name, ProcedureParameters parameters)
        {
            var x = new XElement("procedure", new XAttribute("name", name));

            if (parameters.Parameters != null)
                foreach (var param in parameters.Parameters)
                    x.Add(new XElement("parameter",
                        new XAttribute("name", param.Name),
                        new XAttribute("value", param.Value),
                        new XAttribute("valuetype", _valueTypeProvider.GetForStoredProcedure(param.Value))));

            return x.ToString();
        }

        private string BuildQueryXml(string name, TableQuery parameters)
        {
            var q = new XElement("query",
                new XElement("tables", new XElement("table", name)
                    ));

            if (parameters.Fields != null && parameters.Fields.Any())
            {
                q.Add(new XElement("fields",
                    (parameters.Fields.Select(f => new XElement("field", f)))));
            }

            if (parameters.Conditions != null && parameters.Conditions.Any())
            {
                q.Add(new XElement("conditions",
                    (parameters.Conditions.Select(c => new XElement("condition",
                        new XAttribute("operator", c.Operator),
                        new XElement("exp", new XAttribute("type", "field"), c.Field),
                        new XElement("exp", new XAttribute("type", _valueTypeProvider.GetForQuery(c.Value)),
                            c.Value))))));
            }

            return q.ToString();
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
