using NUnit.Framework;

namespace DotLiquid.Tests.Ns1
{
    public class TestClass
    {
        public string TestClassProp1 { get; set; }
    }
}

namespace DotLiquid.Tests.Ns2
{
    public class TestClass
    {
        public string TestClassProp2 { get; set; }
    }
}


namespace DotLiquid.Tests
{
    [TestFixture]
    public class HashTests
    {
        public class TestBaseClass
        {
            public string TestBaseClassProp { get; set; }
        }

        public class TestChildClass1 : TestBaseClass
        {
            public string TestChildClass1Prop { get; set; }
        }

        public class TestChildClass2 : TestChildClass1
        {
            public string TestChildClass2Prop { get; set; }
        }

        #region Mapper Cache Tests
        /// <summary>
        /// "mapperCache" should consider namespace.
        /// Types with same name (but different namespace) should be cached separately
        /// </summary>
        [Test]
        public void TestMapperCacheShouldCacheSeperateNamespaces()
        {
            var testClass1 = new DotLiquid.Tests.Ns1.TestClass()
            {
                TestClassProp1 = "TestClassProp1Value"
            };

            var value1 = Hash.FromAnonymousObject(testClass1);

            Assert.AreEqual(
                testClass1.TestClassProp1,
                value1[nameof(DotLiquid.Tests.Ns1.TestClass.TestClassProp1)]);

            //Same type name but different namespace
            var testClass2 = new DotLiquid.Tests.Ns2.TestClass()
            {
                TestClassProp2 = "TestClassProp2Value"
            };
            var value2 = Hash.FromAnonymousObject(testClass2);

            Assert.AreEqual(
                testClass2.TestClassProp2,
                value2[nameof(DotLiquid.Tests.Ns2.TestClass.TestClassProp2)]);
        }

        #endregion

        #region Including Base Class Properties Tests

        private void IncludeBaseClassPropertiesOrNot(bool includeBaseClassProperties)
        {
            var TestChildClass2Value = "TestChildClass2Prop";
            var TestChildClass1Value = "TestChildClass1Prop";
            var TestBaseClassPropValue = "TestBaseClassPropValue";

            var value = Hash.FromAnonymousObject(new TestChildClass2()
            {
                TestChildClass2Prop = TestChildClass2Value,
                TestChildClass1Prop = TestChildClass1Value,
                TestBaseClassProp = TestBaseClassPropValue
            }, includeBaseClassProperties);

            Assert.AreEqual(
                TestChildClass2Value,
                value[nameof(TestChildClass2.TestChildClass2Prop)]);

            Assert.AreEqual(
                includeBaseClassProperties ? TestChildClass1Value : null,
                value[nameof(TestChildClass1.TestChildClass1Prop)]);

            Assert.AreEqual(
                includeBaseClassProperties ? TestBaseClassPropValue : null,
                value[nameof(TestBaseClass.TestBaseClassProp)]);
        }

        /// <summary>
        /// Mapping without properties from base class 
        /// </summary>
        [Test]
        public void TestShouldNotMapPropertiesFromBaseClass()
        {
            IncludeBaseClassPropertiesOrNot(includeBaseClassProperties: false);
        }

        /// <summary>
        /// Mapping with properties from base class 
        /// </summary>
        [Test]
        public void TestShouldMapPropertiesFromBaseClass()
        {
            IncludeBaseClassPropertiesOrNot(includeBaseClassProperties: true);
        }

        /// <summary>
        /// Mapping/Not mapping properties from base class should work for same class. 
        /// "mapperCache" should consider base class property mapping option ("includeBaseClassProperties").
        /// </summary>
        [Test]
        public void TestUpperTwoScenarioWithSameClass()
        {
            //These two need to be called together to be sure same cache is being used for two  
            IncludeBaseClassPropertiesOrNot(false);
            IncludeBaseClassPropertiesOrNot(true);
        }
        #endregion
    }
}
