using System;
using System.Linq;
using System.Xml.Linq;
using FakeItEasy;
using LimeProxy.Builders;
using LimeProxy.Models;
using LimeProxy.Proxy;
using NUnit.Framework;

namespace LimeProxy.Tests.Builders
{
    [TestFixture]
    public class XmlBuilderTests
    {
        private XmlBuilder builder;

        [SetUp]
        public void Setup()
        {
            builder =new XmlBuilder(new ValueTypeProvider());
        }

        [Test]
        public void ShouldCreateCorrectXmlForAStoredProcedureWithoudParameters()
        {
            var p = new ProcedureParameters() { Parameters = new Parameter[] { } };
            var name = "csp_some_proc";

            var expected = "<procedure name=\"csp_some_proc\" />";

            var result = builder.BuildProcedureXml(name, p);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldUseValueTypeProviderToResolveTypeForValue()
        {
            var v = A.Fake<IValueTypeProvider>();
            builder = new XmlBuilder(v);
            var pp = new ProcedureParameters()
            {
                Parameters = new[]
            {
                new Parameter()
                {
                    Name = "@@Foo", Value = 12
                }, 
            }
            };

            builder.BuildProcedureXml(A.Dummy<string>(), pp);

            A.CallTo(() => v.GetForStoredProcedure(pp.Parameters.First().Value)).MustHaveHappened();
        }

        [Test]
        public void ShouldUseResultFromValueTypeProviderWhenBuildingXml()
        {
            var v = A.Fake<IValueTypeProvider>();
            builder = new XmlBuilder(v);
            var pp = new ProcedureParameters()
            {
                Parameters = new[]
                {
                    new Parameter()
                    {
                        Name = "@@Foo",
                        Value = 12
                    },
                }
            };
            A.CallTo(() => v.GetForStoredProcedure(A<object>._)).Returns(99);
            var name = "csp_some_proc";
            var expected = new XElement("procedure", new XAttribute("name", name),
               new XElement("parameter",
                   new XAttribute("name", "@@Foo"),
                   new XAttribute("value", 12),
                   new XAttribute("valuetype", 99))).ToString();

            var result = builder.BuildProcedureXml(name, pp);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldCreateCorrectXmlForAStoredProcedureWithParameter()
        {
            var p = new ProcedureParameters()
            {
                Parameters = new[]
            {
                new Parameter()
                {
                    Name = "@@Foo", Value = 12
                }, 
            }
            };
            var name = "csp_some_proc";

            var expected = new XElement("procedure", new XAttribute("name", name),
                new XElement("parameter",
                    new XAttribute("name", "@@Foo"),
                    new XAttribute("value", 12),
                    new XAttribute("valuetype", 3))).ToString();

            var result = builder.BuildProcedureXml(name, p);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldCreateCorrectXmlForAStoredProcedureWithParameters()
        {
            var p = new ProcedureParameters()
            {
                Parameters = new[]
            {
                new Parameter()
                {
                    Name = "@@Foo", Value = 12
                }, new Parameter()
                {
                    Name = "@@Bar", Value = "troll lol"
                }, 
            }
            };
            var name = "csp_some_proc";

            var expected = new XElement("procedure", new XAttribute("name", name),
                new XElement("parameter",
                    new XAttribute("name", "@@Foo"),
                    new XAttribute("value", 12),
                    new XAttribute("valuetype", 3)),
                new XElement("parameter",
                    new XAttribute("name", "@@Bar"),
                    new XAttribute("value", "troll lol"),
                    new XAttribute("valuetype", 8))).ToString();

            var result = builder.BuildProcedureXml(name, p);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldCreateSimpleQuery()
        {
            var t = new TableQuery() { };
            var name = "customer";

            var expected = new XElement("query",
                new XElement("tables", new XElement("table", name)
                    )).ToString();

            var result = builder.BuildQueryXml(name, t);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldAddFieldsToQuery()
        {
            var t = new TableQuery() { Fields = new[] { "A", "B" } };
            var name = "company";

            var expected = new XElement("query",
                new XElement("tables", new XElement("table", name)),
                new XElement("fields",
                    new XElement("field", "A"),
                    new XElement("field", "B"))
                ).ToString();

            var result = builder.BuildQueryXml(name, t);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldAddConditionsToQuery()
        {
            var t = new TableQuery()
            {
                Conditions = new[]
                {
                    new Condition() {Field = "id", Operator = "=", Value = 123},
                }
            };
            var name = "X";

            var expected = new XElement("query",
                new XElement("tables", new XElement("table", name)),
                new XElement("conditions",
                    new XElement("condition", new XAttribute("operator", "="),
                        new XElement("exp", new XAttribute("type", "field"), "id"),
                        new XElement("exp", new XAttribute("type", "numeric"), 123)))
                ).ToString();

            var result = builder.BuildQueryXml(name, t);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldUseValueTypeProviderWhenBuildingQueries()
        {
            var value = 123;
            var t = new TableQuery()
            {
                Conditions = new[]
                {
                    new Condition() {Field = "id", Operator = "=", Value = value},
                }
            };
            var v = A.Fake<IValueTypeProvider>();
            builder = new XmlBuilder(v);

            builder.BuildQueryXml(A.Dummy<string>(), t);

            A.CallTo(() => v.GetForQuery(value)).MustHaveHappened();
        }

        [Test]
        public void ShouldUseResultFromValueTypeProviderInQueryXml()
        {
            var v = A.Fake<IValueTypeProvider>();
            builder = new XmlBuilder(v);
            var value = 123;
            var typeValue = "wizard";

            A.CallTo(() => v.GetForQuery(value)).Returns(typeValue);

            var t = new TableQuery()
            {
                Conditions = new[]
                {
                    new Condition() {Field = "id", Operator = "=", Value = value},
                }
            };
            var name = "X";

            var expected = new XElement("query",
                new XElement("tables", new XElement("table", name)),
                new XElement("conditions",
                    new XElement("condition", new XAttribute("operator", "="),
                        new XElement("exp", new XAttribute("type", "field"), "id"),
                        new XElement("exp", new XAttribute("type", typeValue), value)))
                ).ToString();

            var result = builder.BuildQueryXml(name, t);

            Assert.That(result, Is.EqualTo(expected));
        }

    }
}
