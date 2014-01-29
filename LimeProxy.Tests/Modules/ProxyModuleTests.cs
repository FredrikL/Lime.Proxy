using FakeItEasy;
using LimeProxy.Modules;
using LimeProxy.Proxy;
using Nancy.Testing;
using NUnit.Framework;

namespace LimeProxy.Tests.Modules
{
    public class ProxyModuleTests
    {
        private ILimeWebServiceProxy limeWebServiceProxy;

        [SetUp]
        public void Setup()
        {
            limeWebServiceProxy = A.Fake<ILimeWebServiceProxy>();
        }

        [TestCase("/v1/sp/foo")]
        [TestCase("/v1/table/bar")]
        public void ShouldSetCORSHeaderInResponse(string url)
        {
            var browser = new Browser(w => w.Module(new ProxyModule(limeWebServiceProxy)));

            var result = browser.Post(url, with =>
            {
                with.HttpRequest();
                with.Header("accept", "application/json");
                with.Header("Content-Type", "application/json");
            });

            Assert.That(result.Headers.ContainsKey("Access-Control-Allow-Origin"), Is.True);
            Assert.That(result.Headers["Access-Control-Allow-Origin"], Is.EqualTo("*"));
        }

        [Test]
        public void ShouldPickSpToExecuteFromUrl()
        {
            var browser = new Browser(w => w.Module(new ProxyModule(limeWebServiceProxy)));

            var result = browser.Post("/v1/sp/some_sp", with =>
            {
                with.HttpRequest();
                with.Header("accept", "application/json");
                with.Header("Content-Type", "application/json");
            });

            A.CallTo(() => limeWebServiceProxy.ExecuteStoredProcedure("some_sp", A<object>._)).MustHaveHappened();
        }

        [Test]
        public void ShouldPickTableToQueryFromUrl()
        {
            var browser = new Browser(w => w.Module(new ProxyModule(limeWebServiceProxy)));

            var result = browser.Post("/v1/table/customer", with =>
            {
                with.HttpRequest();
                with.Header("accept", "application/json");
                with.Header("Content-Type", "application/json");
            });

            A.CallTo(() => limeWebServiceProxy.QueryTable("customer", A<object>._)).MustHaveHappened();
        }
    }
}
