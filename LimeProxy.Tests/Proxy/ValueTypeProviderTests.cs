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

            var result = valueTypeProvider.GetForStoredProcedure(i);

            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void ShouldReturn3ForLong()
        {
            long l = 0;

            var result = valueTypeProvider.GetForStoredProcedure(l);

            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void ShouldReturn4ForFloat()
        {
            float f = 0;

            var result = valueTypeProvider.GetForStoredProcedure(f);

            Assert.That(result, Is.EqualTo(4));
        }

        [Test]
        public void ShouldReturn5ForDouble()
        {
            double d = 0;

            var result = valueTypeProvider.GetForStoredProcedure(d);

            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void ShouldReturn14ForDecimal()
        {
            decimal d = 0;

            var result = valueTypeProvider.GetForStoredProcedure(d);

            Assert.That(result, Is.EqualTo(14));
        }

        [Test]
        public void ShouldReturn7ForDateTime()
        {
            DateTime d = DateTime.MinValue;

            var result = valueTypeProvider.GetForStoredProcedure(d);

            Assert.That(result, Is.EqualTo(7));
        }

        [Test]
        public void ShouldReturn11ForBool()
        {
            bool b = true;

            var result = valueTypeProvider.GetForStoredProcedure(b);

            Assert.That(result, Is.EqualTo(11));
        }


        [Test]
        public void ShouldReturn17ForByte()
        {
            byte b = 0;

            var result = valueTypeProvider.GetForStoredProcedure(b);

            Assert.That(result, Is.EqualTo(17));
        }


        [Test]
        public void ShouldReturn8ForString()
        {
            string s = string.Empty;

            var result = valueTypeProvider.GetForStoredProcedure(s);

            Assert.That(result, Is.EqualTo(8));
        }

        [TestCase(1)]
        [TestCase(1L)]
        [TestCase(1F)]
        [TestCase(1D)]
        public void ShouldReturnNumericForNumbers(object value)
        {
            var result = valueTypeProvider.GetForQuery(value);

            Assert.That(result, Is.EqualTo("numeric"));
        }

        [Test]
        public void ShouldReturnNumericForDecimal()
        {
            var result = valueTypeProvider.GetForQuery(1.0M);

            Assert.That(result, Is.EqualTo("numeric"));
        }

        [Test]
        public void ShouldReturnDateForDateTime()
        {
            var result = valueTypeProvider.GetForQuery(DateTime.MinValue);

            Assert.That(result, Is.EqualTo("date"));
        }

        [Test]
        public void ShouldReturnStringForstring()
        {
            var result = valueTypeProvider.GetForQuery(string.Empty);

            Assert.That(result, Is.EqualTo("string"));
        }
    }
}