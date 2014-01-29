using System;
using LimeProxy.Proxy;
using NUnit.Framework;

namespace LimeProxy.Tests.Proxy
{
    [TestFixture]
    public class ValueTypeProviderTests
    {
        private ValueTypeProvider valueTypeProvider = new ValueTypeProvider();

        [Test]
        public void ShouldReturn3ForInt()
        {
            int i = 0;

            var result = valueTypeProvider.Get(i);

            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void ShouldReturn3ForLong()
        {
            long l = 0;

            var result = valueTypeProvider.Get(l);

            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void ShouldReturn4ForFloat()
        {
            float f = 0;

            var result = valueTypeProvider.Get(f);

            Assert.That(result, Is.EqualTo(4));
        }

        [Test]
        public void ShouldReturn5ForDouble()
        {
            double d = 0;

            var result = valueTypeProvider.Get(d);

            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void ShouldReturn14ForDecimal()
        {
            decimal d = 0;

            var result = valueTypeProvider.Get(d);

            Assert.That(result, Is.EqualTo(14));
        }

        [Test]
        public void ShouldReturn7ForDateTime()
        {
            DateTime d = DateTime.MinValue;

            var result = valueTypeProvider.Get(d);

            Assert.That(result, Is.EqualTo(7));
        }

        [Test]
        public void ShouldReturn11ForBool()
        {
            bool b = true;

            var result = valueTypeProvider.Get(b);

            Assert.That(result, Is.EqualTo(11));
        }


        [Test]
        public void ShouldReturn17ForByte()
        {
            byte b = 0;

            var result = valueTypeProvider.Get(b);

            Assert.That(result, Is.EqualTo(17));
        }


        [Test]
        public void ShouldReturn8ForString()
        {
            string s = string.Empty;

            var result = valueTypeProvider.Get(s);

            Assert.That(result, Is.EqualTo(8));
        }
    }
}