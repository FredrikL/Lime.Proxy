using LimeProxy.Validation;
using NUnit.Framework;

namespace LimeProxy.Tests.Validation
{
    [TestFixture]
    class OperatorValidatorTests
    {
        private OperatorValidator _validator = new OperatorValidator();

        [TestCase("=")]
        [TestCase(">")]
        [TestCase("<")]
        [TestCase(">=")]
        [TestCase("<=")]
        [TestCase("!=")]
        [TestCase("LIKE")]
        [TestCase("IN")]
        [TestCase("ANY")]
        [TestCase("LIKE%")]
        [TestCase("%LIKE")]
        [TestCase("NOT LIKE")]
        [TestCase("NOT LIKE%")]
        [TestCase("NOT %LIKE")]
        [TestCase("NOT IN")]
        [TestCase("IS")]
        [TestCase("IS NOT")]
        public void ShouldReturnTrueForValidOperators(string op)
        {
            var result = _validator.IsValid(op);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ShouldReturnFalseForUnknowOperators()
        {
            var result = _validator.IsValid("<=>");

            Assert.That(result,Is.False);
        }

        [Test]
        public void ShouldHandleLowerCaseVersionOfOperator()
        {
            var result = _validator.IsValid("is");

            Assert.That(result, Is.True);
        }
    }
}
