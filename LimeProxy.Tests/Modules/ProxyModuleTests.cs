using System;
using System.Linq;
using FakeItEasy;
using LimeProxy.Models;
using LimeProxy.Modules;
using LimeProxy.Proxy;
using Nancy;
using Nancy.Testing;
using NUnit.Framework;

namespace LimeProxy.Tests.Modules
{
    [TestFixture]
    public class ProxyModuleTests
    {
        private ILimeWebServiceProxy limeWebServiceProxy;

        [SetUp]
        public void Setup()
        {
            limeWebServiceProxy = A.Fake<ILimeWebServiceProxy>();
        }

        [TestCase("/v1/bar/sp/foo")]
        [TestCase("/v1/foo/table/bar")]
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
        public void ShouldPickDbAndSpToExecuteFromUrl()
        {
            var browser = new Browser(w => w.Module(new ProxyModule(limeWebServiceProxy)));

            var result = browser.Post("/v1/some_db/sp/some_sp", with =>
            {
                with.HttpRequest();
                with.Header("accept", "application/json");
                with.Header("Content-Type", "application/json");
            });

            A.CallTo(() => limeWebServiceProxy.ExecuteStoredProcedure("some_db","some_sp", A<ProcedureParameters>._)).MustHaveHappened();
        }

        [Test]
        public void ShouldBindProcedureParameters()
        {
            var browser = new Browser(w => w.Module(new ProxyModule(limeWebServiceProxy)));

            var result = browser.Post("/v1/some_db/sp/some_sp", with =>
            {
                with.HttpRequest();
                with.Header("accept", "application/json");
                with.Header("Content-Type", "application/json");
                with.Body(@"{
    Parameters: [{'Name': 'foo','Value':12}]
}");
            });

            A.CallTo(() => limeWebServiceProxy.ExecuteStoredProcedure(A<string>._, A<string>._, 
                A<ProcedureParameters>.That.Matches(p => p.Parameters.First().Name == "foo"))).MustHaveHappened();
        }

        [Test]
        public void ShouldPickDbAndTableToQueryFromUrl()
        {
            var browser = new Browser(w => w.Module(new ProxyModule(limeWebServiceProxy)));

            var result = browser.Post("/v1/foo/table/customer", with =>
            {
                with.HttpRequest();
                with.Header("accept", "application/json");
                with.Header("Content-Type", "application/json");
            });

            A.CallTo(() => limeWebServiceProxy.QueryTable("foo","customer", A<TableQuery>._)).MustHaveHappened();
        }

        [Test]
        public void ShouldBindTableQueryParameters()
        {
            var browser = new Browser(w => w.Module(new ProxyModule(limeWebServiceProxy)));

            var result = browser.Post("/v1/foo/table/customer", with =>
            {
                with.HttpRequest();
                with.Header("accept", "application/json");
                with.Header("Content-Type", "application/json");
                with.Body(@"{
    'Fields': ['foo','bar'],
    'Conditions': [{'Field': 'idfoo', 'Operator':'>=', 'Value':1001 }]
}");
            });

            Func<TableQuery, bool> matcher = query =>
            {
                Assert.That(query.Fields.Count(), Is.EqualTo(2));
                Assert.That(query.Conditions.Count(), Is.EqualTo(1));
                return true;
            };

            A.CallTo(() => limeWebServiceProxy.QueryTable(A<string>._, A<string>._, A<TableQuery>.That.Matches(matcher, ""))).MustHaveHappened();
        }

        [TestCase("/v1/db/table/customer")]
        [TestCase("/v1/db/sp/csp_some_sp")]
        public void ShouldReturnsNeededCorsHeadersForOptionsMethod(string url)
        {
            var browser = new Browser(w => w.Module(new ProxyModule(limeWebServiceProxy)));

            var result = browser.Options(url, with =>
            {
                with.HttpRequest();
                with.Header("accept", "application/json");
                with.Header("Content-Type", "application/json");
            });

            Assert.That(result.Headers["Access-Control-Allow-Origin"], Is.EqualTo("*"));
            Assert.That(result.Headers["Access-Control-Allow-Methods"], Is.EqualTo("POST, OPTIONS, GET"));
            Assert.That(result.Headers["Access-Control-Allow-Headers"], Is.EqualTo("Accept, Origin, Content-type"));
        }

        [Test]
        public void ShouldSetStatusCodeTo500IfErrorOccuredForStoredProcedure()
        {
            var browser = new Browser(w => w.Module(new ProxyModule(limeWebServiceProxy)));
            A.CallTo(() => limeWebServiceProxy.ExecuteStoredProcedure("db","some_sp", A<ProcedureParameters>._))
                .Returns(new Result() { Success = false });
            var result = browser.Post("/v1/db/sp/some_sp", with =>
            {
                with.HttpRequest();
                with.Header("accept", "application/json");
                with.Header("Content-Type", "application/json");
            });

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        }

        [Test]
        public void ShouldSetStatusCodeTo500IfErrorOccuredForTable()
        {
            var browser = new Browser(w => w.Module(new ProxyModule(limeWebServiceProxy)));
            A.CallTo(() => limeWebServiceProxy.QueryTable("bar","foo", A<TableQuery>._))
                .Returns(new Result() { Success = false });
            var result = browser.Post("/v1/bar/table/foo", with =>
            {
                with.HttpRequest();
                with.Header("accept", "application/json");
                with.Header("Content-Type", "application/json");
            });

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        }

    }
}
