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
        private LimeWebServiceProxy proxy;

        [SetUp]
        public void Setup()
        {
            limeWebSerivceClientInvoker = A.Fake<ILimeWebSerivceClientInvoker>();

            proxy = new LimeWebServiceProxy(limeWebSerivceClientInvoker);
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
        public void ShouldCreateCorrectXmlForAStoredProcedureWithParameters()
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
                    new XAttribute("valuetype", 2)));

            proxy.ExecuteStoredProcedure(name, p);

            A.CallTo(() => limeWebSerivceClientInvoker.ExecuteProcedure(expected.ToString())).MustHaveHappened();
        }
    }
}
