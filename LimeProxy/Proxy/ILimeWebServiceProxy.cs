using System;
using System.Xml;
using LimeProxy.Builders;
using LimeProxy.Models;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace LimeProxy.Proxy
{
    public interface ILimeWebServiceProxy
    {
        Result ExecuteStoredProcedure(string db, string name, ProcedureParameters parameters);
        Result QueryTable(string db, string name, TableQuery parameters);
    }

    public class LimeWebServiceProxy : ILimeWebServiceProxy
    {
        private readonly ILimeWebSerivceClientInvoker _limeWebSerivceClientInvoker;
        private readonly IXmlBuilder _xmlBuilder;

        public LimeWebServiceProxy(ILimeWebSerivceClientInvoker limeWebSerivceClientInvoker,
            IXmlBuilder xmlBuilder)
        {
            _limeWebSerivceClientInvoker = limeWebSerivceClientInvoker;
            _xmlBuilder = xmlBuilder;
        }

        public Result ExecuteStoredProcedure(string db, string name, ProcedureParameters parameters)
        {
            try
            {
                var xml = _xmlBuilder.BuildProcedureXml(name, parameters);
                var result = _limeWebSerivceClientInvoker.ExecuteProcedure(db, xml);
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

        public Result QueryTable(string db, string name, TableQuery parameters)
        {
            try
            {
                var xml = _xmlBuilder.BuildQueryXml(name, parameters);
                var result = _limeWebSerivceClientInvoker.QueryTable(db, xml);
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

        private string ConvertFromXmlStringToJsonString(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            string jsonText = JsonConvert.SerializeXmlNode(doc.SelectSingleNode("/data"), Formatting.None, true);
            return jsonText;
        }
    }
}
