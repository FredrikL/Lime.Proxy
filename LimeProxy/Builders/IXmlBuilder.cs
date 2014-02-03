using System.Linq;
using System.Xml.Linq;
using LimeProxy.Models;
using LimeProxy.Proxy;

namespace LimeProxy.Builders
{
    public interface IXmlBuilder
    {
        string BuildProcedureXml(string name, ProcedureParameters parameters);
        string BuildQueryXml(string name, TableQuery parameters);
    }

    public class XmlBuilder : IXmlBuilder
    {
        private readonly IValueTypeProvider _valueTypeProvider;


        public XmlBuilder(IValueTypeProvider valueTypeProvider)
        {
            _valueTypeProvider = valueTypeProvider;
        }

        public string BuildProcedureXml(string name, ProcedureParameters parameters)
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

        public string BuildQueryXml(string name, TableQuery parameters)
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
    }
}