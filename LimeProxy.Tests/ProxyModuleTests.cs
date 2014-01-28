using LimeProxy.Modules;
using Nancy.Testing;
using NUnit.Framework;

namespace LimeProxy.Tests
{
    public class ProxyModuleTests
    {
        [Test]
        public void VersionTests()
        {
            var browser = new Browser(w => w.Module<ProxyModule>());

            var result = browser.Get("/Api/Version", with => with.HttpRequest());
            
            Assert.That(result.Body.AsString(), Is.EqualTo("1"));
        }
    }
}
