using System;
using System.Linq;
using System.Xml.Linq;
using FakeItEasy;
using LimeProxy.Models;
using LimeProxy.Proxy;
using NUnit.Framework;

namespace LimeProxy.Tests.Proxy
{
    [TestFixture]
    public class LimeWebServiceProxyTests
    {
        private ILimeWebSerivceClientInvoker limeWebSerivceClientInvoker;
        private IValueTypeProvider valueTypeProvider = new ValueTypeProvider();
        private LimeWebServiceProxy proxy;

        [SetUp]
        public void Setup()
        {
            limeWebSerivceClientInvoker = A.Fake<ILimeWebSerivceClientInvoker>();

            proxy = new LimeWebServiceProxy(limeWebSerivceClientInvoker, valueTypeProvider);
        }

        [Test]
        public void ShouldCreateCorrectXmlForAStoredProcedureWithoudParameters()
        {
            var p = new ProcedureParameters(){Parameters = new Parameter[]{}};
            var name = "csp_some_proc";

            var expected = "<procedure name=\"csp_some_proc\" />";

            proxy.ExecuteStoredProcedure(name, p);

            A.CallTo(() => limeWebSerivceClientInvoker.ExecuteProcedure(expected)).MustHaveHappened();
        }

        [Test]
        public void ShouldUseValueTypeProviderToResolveTypeForValue()
        {
            var p = A.Fake<IValueTypeProvider>();
            proxy = new LimeWebServiceProxy(limeWebSerivceClientInvoker, p);
            var pp = new ProcedureParameters()
            {
                Parameters = new[]
            {
                new Parameter()
                {
                    Name = "@@Foo", Value = 12
                }, 
            }
            };

            proxy.ExecuteStoredProcedure(string.Empty, pp);
            
            A.CallTo(() => p.GetForStoredProcedure(pp.Parameters.First().Value)).MustHaveHappened();
        }

        [Test]
        public void ShouldUseResultFromValueTypeProviderWhenBuildingXml()
        {
            var p = A.Fake<IValueTypeProvider>();
            proxy = new LimeWebServiceProxy(limeWebSerivceClientInvoker, p);
            var pp = new ProcedureParameters()
            {
                Parameters = new[]
                {
                    new Parameter()
                    {
                        Name = "@@Foo",
                        Value = 12
                    },
                }
            };
            A.CallTo(() => p.GetForStoredProcedure(A<object>._)).Returns(99);
            var name = "csp_some_proc";
            var expected = new XElement("procedure", new XAttribute("name", name),
               new XElement("parameter",
                   new XAttribute("name", "@@Foo"),
                   new XAttribute("value", 12),
                   new XAttribute("valuetype", 99)));

            proxy.ExecuteStoredProcedure(name, pp);

            A.CallTo(() => limeWebSerivceClientInvoker.ExecuteProcedure(expected.ToString())).MustHaveHappened();
        }

        [Test]
        public void ShouldCreateCorrectXmlForAStoredProcedureWithParameter()
        {
            var p = new ProcedureParameters() { Parameters = new[]
            {
                new Parameter()
                {
                    Name = "@@Foo", Value = 12
                }, 
            } };
            var name = "csp_some_proc";

            var expected = new XElement("procedure", new XAttribute("name", name),
                new XElement("parameter",
                    new XAttribute("name","@@Foo"),
                    new XAttribute("value", 12),
                    new XAttribute("valuetype", 3)));

            proxy.ExecuteStoredProcedure(name, p);

            A.CallTo(() => limeWebSerivceClientInvoker.ExecuteProcedure(expected.ToString())).MustHaveHappened();
        }

        [Test]
        public void ShouldCreateCorrectXmlForAStoredProcedureWithParameters()
        {
            var p = new ProcedureParameters()
            {
                Parameters = new[]
            {
                new Parameter()
                {
                    Name = "@@Foo", Value = 12
                }, new Parameter()
                {
                    Name = "@@Bar", Value = "troll lol"
                }, 
            }
            };
            var name = "csp_some_proc";

            var expected = new XElement("procedure", new XAttribute("name", name),
                new XElement("parameter",
                    new XAttribute("name", "@@Foo"),
                    new XAttribute("value", 12),
                    new XAttribute("valuetype", 3)),
                new XElement("parameter",
                    new XAttribute("name", "@@Bar"),
                    new XAttribute("value", "troll lol"),
                    new XAttribute("valuetype", 8)));

            proxy.ExecuteStoredProcedure(name, p);

            A.CallTo(() => limeWebSerivceClientInvoker.ExecuteProcedure(expected.ToString())).MustHaveHappened();
        }

        [Test]
        public void ShouldSetSuccessAsTrueIfNoError()
        {
            var p = new ProcedureParameters() { Parameters = new Parameter[] { } };
            var name = "csp_some_proc";
            A.CallTo(() => limeWebSerivceClientInvoker.ExecuteProcedure(A<string>._)).
               Returns(@"<?xml version=""1.0"" encoding=""UTF-16"" ?><data />");

            var result = proxy.ExecuteStoredProcedure(name, p);

            Assert.That(result.Success, Is.True);
        }

        [Test]
        public void ShouldSetSuccessAsFalseIfErrorIsThrown()
        {
            var p = new ProcedureParameters() { Parameters = new Parameter[] { } };
            var name = "csp_some_proc";

            A.CallTo(() => limeWebSerivceClientInvoker.ExecuteProcedure(A<string>._)).Throws<Exception>();

            var result = proxy.ExecuteStoredProcedure(name, p);

            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void ShouldConvertResultFromWebServiceToJson()
        {
            var p = new ProcedureParameters() { Parameters = new Parameter[] { } };
            var name = "csp_some_proc";
            A.CallTo(() => limeWebSerivceClientInvoker.ExecuteProcedure(A<string>._)).
                Returns(@"<?xml version=""1.0"" encoding=""UTF-16"" ?>
<data>
	<person name=""Karl Kula"" email=""karl.kula@fakeorg.se"" phone=""08-123 123""/>
</data>");

            var result = proxy.ExecuteStoredProcedure(name, p);

            Assert.That(result.Data, Is.StringStarting("{\"data\":{"));
        }

        [Test]
        public void ShouldCreateSimpleQuery()
        {
            var t = new TableQuery() {};
            var name = "customer";

            var expected = new XElement("query", 
                new XElement("tables", new XElement("table", name)
                    )).ToString();

            proxy.QueryTable(name, t);

            A.CallTo(() =>limeWebSerivceClientInvoker.QueryTable(expected)).MustHaveHappened();
        }

        [Test]
        public void ShouldAddFieldsToQuery()
        {
            var t = new TableQuery() {Fields = new[] {"A", "B"}};
            var name = "company";

            var expected = new XElement("query",
                new XElement("tables", new XElement("table", name)),
                new XElement("fields",
                    new XElement("field", "A"),
                    new XElement("field", "B"))
                ).ToString();

            proxy.QueryTable(name, t);

            A.CallTo(() => limeWebSerivceClientInvoker.QueryTable(expected)).MustHaveHappened();
        }

        [Test]
        public void ShouldAddConditionsToQuery()
        {
            var t = new TableQuery()
            {
                Conditions = new[]
                {
                    new Condition() {Field = "id", Operator = "=", Value = 123},
                }
            };
            var name = "X";

            var expected = new XElement("query",
                new XElement("tables", new XElement("table", name)),
                new XElement("conditions",
                    new XElement("condition", new XAttribute("operator", "="),
                        new XElement("exp", new XAttribute("type", "field"), "id"),
                        new XElement("exp", new XAttribute("type", "numeric"), 123)))
                ).ToString();

            proxy.QueryTable(name, t);

            A.CallTo(() => limeWebSerivceClientInvoker.QueryTable(expected)).MustHaveHappened();
        }

        
    }
}
