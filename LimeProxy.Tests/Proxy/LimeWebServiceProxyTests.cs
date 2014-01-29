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
            
            A.CallTo(() => p.Get(pp.Parameters.First().Value)).MustHaveHappened();
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
            A.CallTo(() => p.Get(A<object>._)).Returns(99);
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
    }
}
