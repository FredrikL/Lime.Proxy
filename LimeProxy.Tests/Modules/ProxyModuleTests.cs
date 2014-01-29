﻿using System.Linq;
using FakeItEasy;
using LimeProxy.Models;
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

            A.CallTo(() => limeWebServiceProxy.ExecuteStoredProcedure("some_sp", A<ProcedureParameters>._)).MustHaveHappened();
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

            A.CallTo(() => limeWebServiceProxy.QueryTable("customer", A<TableQuery>._)).MustHaveHappened();
        }

        [Test]
        public void ShouldBindProcedureParameters()
        {
            var browser = new Browser(w => w.Module(new ProxyModule(limeWebServiceProxy)));

            var result = browser.Post("/v1/sp/some_sp", with =>
            {
                with.HttpRequest();
                with.Header("accept", "application/json");
                with.Header("Content-Type", "application/json");
                with.Body(@"{
    Parameters: [{'Name': 'foo','Value':12}]
}");
            });

            A.CallTo(() => limeWebServiceProxy.ExecuteStoredProcedure(A<string>._, 
                A<ProcedureParameters>.That.Matches(p => p.Parameters.First().Name == "foo"))).MustHaveHappened();
        }
    }
}