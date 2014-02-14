using FakeItEasy;
using LimeProxy.Proxy;
using NUnit.Framework;

namespace LimeProxy.Tests.Proxy
{
    [TestFixture]
    public class LimeConfigLookupTests
    {
        [UnderTest] private LimeConfigLookup limeConfigLookup;
        [Fake] private IConfigurationManagerWrapper configurationManagerWrapper;

        [SetUp]
        public void Setup()
        {
            Fake.InitializeFixture(this);
        }

        [Test]
        public void ShouldResolveDataBaseWebSerivceUrl()
        {
            A.CallTo(() => configurationManagerWrapper.AppSettings("webserviceconfpath")).
                Returns(@"C:\Program Files (x86)\Lundalogik\Lime Web Service\TangeloWindowsServiceHost.exe.config");

            var result = limeConfigLookup.GetUrlForDatabase("ith");

            Assert.That(result, Is.EqualTo("http://localhost:23183/"));
        }
    }
}