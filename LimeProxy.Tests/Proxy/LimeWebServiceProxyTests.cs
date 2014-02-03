using System;
using FakeItEasy;
using LimeProxy.Builders;
using LimeProxy.Models;
using LimeProxy.Proxy;
using NUnit.Framework;

namespace LimeProxy.Tests.Proxy
{
    [TestFixture]
    public class LimeWebServiceProxyTests
    {
        private ILimeWebSerivceClientInvoker limeWebSerivceClientInvoker;
        private IXmlBuilder xmlBuilder;
        private LimeWebServiceProxy proxy;

        [SetUp]
        public void Setup()
        {
            xmlBuilder = A.Fake<IXmlBuilder>();
            limeWebSerivceClientInvoker = A.Fake<ILimeWebSerivceClientInvoker>();

            proxy = new LimeWebServiceProxy(limeWebSerivceClientInvoker, xmlBuilder);
        }

        [Test]
        public void ShouldUseXmlBuilderWhenBuildingProcedureQuery()
        {
            var p = new ProcedureParameters() { Parameters = new Parameter[] { } };
            var name = "csp_some_proc";

            proxy.ExecuteStoredProcedure(name, p);

            A.CallTo(() => xmlBuilder.BuildProcedureXml(name,p)).MustHaveHappened();
        }

        [Test]
        public void ShouldSendResultFromXmlBuilderToExecuteStoredProcedure()
        {
            var p = new ProcedureParameters() { Parameters = new Parameter[] { } };
            var name = "csp_some_proc";
            var xml = "not xml";
            A.CallTo(() => xmlBuilder.BuildProcedureXml(name, p)).Returns(xml);

            proxy.ExecuteStoredProcedure(name, p);

            A.CallTo(() => limeWebSerivceClientInvoker.ExecuteProcedure(xml)).MustHaveHappened();
        }

        [Test]
        public void ShouldSetSuccessAsTrueIfNoErrorWhenCallingAProcedure()
        {
            var p = new ProcedureParameters() { Parameters = new Parameter[] { } };
            var name = "csp_some_proc";
            A.CallTo(() => limeWebSerivceClientInvoker.ExecuteProcedure(A<string>._)).
               Returns(@"<?xml version=""1.0"" encoding=""UTF-16"" ?><data />");

            var result = proxy.ExecuteStoredProcedure(name, p);

            Assert.That(result.Success, Is.True);
        }

        [Test]
        public void ShouldSetSuccessAsFalseIfErrorIsThrownWhenCallingAProcedure()
        {
            var p = new ProcedureParameters() { Parameters = new Parameter[] { } };
            var name = "csp_some_proc";

            A.CallTo(() => limeWebSerivceClientInvoker.ExecuteProcedure(A<string>._)).Throws<Exception>();

            var result = proxy.ExecuteStoredProcedure(name, p);

            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void ShouldSetDataIfErrorIsThrownWhenCallingAProcedure()
        {
            var p = new ProcedureParameters() { Parameters = new Parameter[] { } };
            var name = "csp_some_proc";

            A.CallTo(() => limeWebSerivceClientInvoker.ExecuteProcedure(A<string>._)).Throws<Exception>();

            var result = proxy.ExecuteStoredProcedure(name, p);

            Assert.That(result.Data, Is.Not.Empty);
        }

        [Test]
        public void ShouldConvertResultFromWebServiceToJsonWhenCallingAProcedure()
        {
            var p = new ProcedureParameters() { Parameters = new Parameter[] { } };
            var name = "csp_some_proc";
            A.CallTo(() => limeWebSerivceClientInvoker.ExecuteProcedure(A<string>._)).
                Returns(@"<?xml version=""1.0"" encoding=""UTF-16"" ?>
<data>
	<person name=""Karl Kula"" email=""karl.kula@fakeorg.se"" phone=""08-123 123""/>
</data>");

            var result = proxy.ExecuteStoredProcedure(name, p);

            Assert.That(result.Data, Is.StringStarting("{\"person\":"));
        }

        [Test]
        public void ShouldUseXmlBuilderWhenBuildingTableQuery()
        {
            var t = new TableQuery() { };
            var name = "foo";

            proxy.QueryTable(name, t);

            A.CallTo(() => xmlBuilder.BuildQueryXml(name, t)).MustHaveHappened();
        }

        [Test]
        public void ShouldSendResultFromXmlBuilderToExecuteQueryTable()
        {
            var t = new TableQuery() { };
            var name = "foo";
            var xml = "not xml";
            A.CallTo(() => xmlBuilder.BuildQueryXml(name, t)).Returns(xml);

            proxy.QueryTable(name, t);

            A.CallTo(() => limeWebSerivceClientInvoker.QueryTable(xml)).MustHaveHappened();
        }

        [Test]
        public void ShouldSetSuccessAsTrueIfNoErrorWhenCallingQueryTable()
        {
            var t = new TableQuery() { };
            var name = "foo";
            A.CallTo(() => limeWebSerivceClientInvoker.QueryTable(A<string>._)).
               Returns(@"<?xml version=""1.0"" encoding=""UTF-16"" ?><data />");

            var result = proxy.QueryTable(name, t);

            Assert.That(result.Success, Is.True);
        }

        [Test]
        public void ShouldSetSuccessAsFalseIfErrorIsThrownWhenCallingQueryTable()
        {
            var t = new TableQuery() { };
            var name = "foo";

            A.CallTo(() => limeWebSerivceClientInvoker.QueryTable(A<string>._)).Throws<Exception>();

            var result = proxy.QueryTable(name, t);

            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void ShouldSetDataIfErrorIsThrownWhenCallingQueryTable()
        {
            var t = new TableQuery() { };
            var name = "foo";

            A.CallTo(() => limeWebSerivceClientInvoker.QueryTable(A<string>._)).Throws<Exception>();

            var result = proxy.QueryTable(name, t);

            Assert.That(result.Data, Is.Not.Empty);
        }

        [Test]
        public void ShouldConvertResultFromWebServiceToJsonWhenCallingQueryTable()
        {
            var t = new TableQuery() { };
            var name = "foo";
            A.CallTo(() => limeWebSerivceClientInvoker.QueryTable(A<string>._)).
                Returns(@"<?xml version=""1.0"" encoding=""UTF-16"" ?>
<data>
	<person name=""Karl Kula"" email=""karl.kula@fakeorg.se"" phone=""08-123 123""/>
</data>");

            var result = proxy.QueryTable(name, t);

            Assert.That(result.Data, Is.StringStarting("{\"person\":"));
        }
    }
}
