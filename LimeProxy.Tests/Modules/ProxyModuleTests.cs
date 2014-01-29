using LimeProxy.Modules;
using Nancy.Testing;
using NUnit.Framework;

namespace LimeProxy.Tests
{
    public class ProxyModuleTests
    {
        [TestCase("/v1/sp/foo")]
        [TestCase("/v1/table/bar")]
        public void ShouldSetCORSHeaderInResponse(string url)
        {
            var browser = new Browser(w => w.Module<ProxyModule>());

            var result = browser.Post(url, with => with.HttpRequest());

            Assert.That(result.Headers.ContainsKey("Access-Control-Allow-Origin"), Is.True);
            Assert.That(result.Headers["Access-Control-Allow-Origin"], Is.EqualTo("*"));
        }
    }
}
