/* Copyright 2017 Cimpress

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License. */


using System;
using FluentAssertions;
using NUnit.Framework;
using VP.FF.PT.Common.Infrastructure.Database;

namespace VP.FF.PT.Common.Infrastructure.UnitTests
{
    // Serializable is only needed by Clone() extension method, not by Copy()
    [Serializable]
    internal class TestFlatClass
    {
        public string Text { get; set; }
        public int Number { get; set; }
    }

    [Serializable]
    internal class TestComplexClass
    {
        public TestFlatClass Custom { get; set; }
    }

    [Serializable]
    internal class TestDerivedClass : TestFlatClass
    {
        public double FloatNumber { get; set; }
    }

    [TestFixture]
    public class CloneTests
    {
        [Test]
        public void FlatCloneTest()
        {
            var a = new TestFlatClass { Text = "old text", Number = 1 };

            var b = a.Clone();

            a.Text = "new text";
            a.Number = 2;

            b.Text.Should().Be("old text");
            b.Number.Should().Be(1);
        }

        [Test]
        public void ReferenceCloneTest()
        {
            var customA = new TestFlatClass { Text  = "old text", Number = 1 };
            var a = new TestComplexClass { Custom = customA };

            var b = a.Clone();

            a.Custom.Text = "new text";
            a.Custom.Number = 2;

            b.Custom.Should().NotBeSameAs(customA, "deep clone must create a new instance");
            b.Custom.Text.Should().Be("old text");
            b.Custom.Number.Should().Be(1);
        }

        [Test]
        public void DerivedCloneTest()
        {
            var a = new TestDerivedClass {Text = "old text", Number = 1, FloatNumber = 1.1};

            var b = a.Clone();

            a.Text = "new text";
            a.Number = 2;
            a.FloatNumber = 2.2;

            b.Text.Should().Be("old text");
            b.Number.Should().Be(1);
            b.FloatNumber.Should().BeInRange(1.1, 1.1);
        }
    }

    [TestFixture]
    public class CompareObjectTests
    {
        private class ClassWithFieldsTest
        {
            public int a;
            public string b;
            public ulong c;
        }

        private class ClassWithMethodTest
        {
            public void Moo()
            {
            }
        }

        private class ClassWithTwoMethodsTest
        {
            public void Moo()
            {
            }

            public int Meh()
            {
                return 1;
            }
        }

        private class ClassWithPropertiesTest
        {
            public ClassWithPropertiesTest(float initB)
            {
                B = initB;
            }

            public int A { get; set; }
            public float B { get; private set; }
        }

        private class ClassWithArrayTest
        {
            public int x;
            public int[] a;
            public int y;
        }

        private class ClassWithComplexArrayTest
        {
            public ClassWithFieldsTest[] a;
        }

        private class ClassWithComplexFieldTest
        {
            public ClassWithFieldsTest item;
        }

        private class EmptyClassA
        {
        }

        private class EmptyClassB
        {
        }

        [Test]
        public void CompareObjectsOnlyFieldsTest_NotEqual()
        {
            var a = new ClassWithFieldsTest { a = 1, b = "test", c = 2 };
            var b = new ClassWithFieldsTest { a = -1, b = "testttt", c = 3 };

            a.CompareObjectsOnlyFields(b).Should().BeFalse("objects have different content");
        }

        [Test]
        public void CompareObjectsOnlyFieldsTest_NullValue_NotEqual()
        {
            var a = new ClassWithFieldsTest { a = 1, b = null, c = 2 };
            var b = new ClassWithFieldsTest { a = -1, b = "testttt", c = 3 };

            a.CompareObjectsOnlyFields(b).Should().BeFalse("objects have different content");
        }

        [Test]
        public void CompareObjectsOnlyFieldsTest_NullValue_Equal()
        {
            var a = new ClassWithFieldsTest { a = 1, b = null, c = 2 };
            var b = new ClassWithFieldsTest { a = -1, b = null, c = 3 };

            a.CompareObjectsOnlyFields(b).Should().BeFalse("objects have different content");
        }

        [Test]
        public void CompareObjectsOnlyFieldsTest_DifferentTypes()
        {
            var a = new EmptyClassA();
            var b = new EmptyClassB();

            a.CompareObjectsOnlyFields(b).Should().BeFalse();
        }

        [Test]
        public void CompareObjectsOnlyFieldsTest_Equal()
        {
            var a = new ClassWithFieldsTest { a = 1, b = "test", c = 2 };
            var b = new ClassWithFieldsTest { a = 1, b = "test", c = 2 };

            a.CompareObjectsOnlyFields(b).Should().BeTrue("objects have same content");
        }

        [Test]
        public void CompareObjects_Fields_NotEqual()
        {
            var a = new ClassWithFieldsTest { a = 1, b = "test", c = 2 };
            var b = new ClassWithFieldsTest { a = -1, b = "testttt", c = 3 };

            a.CompareObjects(b).Should().BeFalse("objects have different content");
        }

        [Test]
        public void CompareObjects_Fields_Equal()
        {
            var a = new ClassWithFieldsTest { a = 1, b = "test", c = 2 };
            var b = new ClassWithFieldsTest { a = 1, b = "test", c = 2 };

            a.CompareObjects(b).Should().BeTrue("objects have same content");
        }

        [Test]
        public void CompareObjectsTest_Method_Equal()
        {
            var a = new ClassWithMethodTest();
            var b = new ClassWithMethodTest();

            a.CompareObjects(b).Should().BeTrue();
        }

        [Test]
        public void CompareObjectsTest_Method_NotEqual()
        {
            var a = new ClassWithMethodTest();
            var b = new ClassWithTwoMethodsTest();

            a.CompareObjects(b).Should().BeFalse();
        }

        [Test]
        public void CompareObjectsTest_Properties_Equal()
        {
            var a = new ClassWithPropertiesTest(2.2f) { A = 1 };
            var b = new ClassWithPropertiesTest(2.2f) { A = 1 };

            a.CompareObjects(b).Should().BeTrue();
        }

        [Test]
        public void CompareObjectsTest_Properties_NotEqual()
        {
            var a = new ClassWithPropertiesTest(2.2f) { A = 1 };
            var b = new ClassWithPropertiesTest(2.3f) { A = -1 };

            a.CompareObjects(b).Should().BeFalse();
        }

        [Test]
        public void CompareObjectsTest_ArrayAndOtherValuesBefore_Equal()
        {
            var a = new ClassWithArrayTest { a = new[] { 1, 2, 3 }, x = 1 };
            var b = new ClassWithArrayTest { a = new[] { 1, 2, 3 }, x = 1 };

            a.CompareObjectsOnlyFields(b).Should().BeTrue();
        }

        [Test]
        public void CompareObjectsTest_ArrayAndOtherValuesAfter_Equal()
        {
            var a = new ClassWithArrayTest { a = new[] { 1, 2, 3 }, y = 1 };
            var b = new ClassWithArrayTest { a = new[] { 1, 2, 3 }, y = 1 };

            a.CompareObjectsOnlyFields(b).Should().BeTrue();
        }

        [Test]
        public void CompareObjectsTest_ArrayAndOtherValuesAfter_NotEqual()
        {
            var a = new ClassWithArrayTest { a = new[] { 1, 2, 3 }, y = 1 };
            var b = new ClassWithArrayTest { a = new[] { 1, 2, 3 }, y = 2 };

            a.CompareObjectsOnlyFields(b).Should().BeFalse();
        }

        [Test]
        public void CompareObjectsTest_ArrayNullSecond_NotEqual()
        {
            var a = new ClassWithArrayTest { a = new[] { 1, 2, 3 } };
            var b = new ClassWithArrayTest { a = null };

            a.CompareObjectsOnlyFields(b).Should().BeFalse();
        }

        [Test]
        public void CompareObjectsTest_ArrayNullFirst_NotEqual()
        {
            var a = new ClassWithArrayTest { a = null };
            var b = new ClassWithArrayTest { a = new[] { 1, 2, 3 } };

            a.CompareObjectsOnlyFields(b).Should().BeFalse();
        }

        [Test]
        public void CompareObjectsTest_Array_Equal()
        {
            var a = new ClassWithArrayTest { a = new[] { 1, 2, 3 } };
            var b = new ClassWithArrayTest { a = new[] { 1, 2, 3 } };

            a.CompareObjects(b).Should().BeTrue("Arrays have same content");
            a.CompareObjectsOnlyFields(b).Should().BeTrue("Classes provides field Arrays and Arrays have same content");
        }

        [Test]
        public void CompareObjectsTest_Array_NotEqual()
        {
            var a = new ClassWithArrayTest { a = new[] { 1, 2, 3 } };
            var b = new ClassWithArrayTest { a = new[] { 1, 1, 3 } };

            a.CompareObjects(b).Should().BeFalse("Arrays have not same content");
            a.CompareObjectsOnlyFields(b).Should().BeFalse("Classes provides field Arrays but Arrays have not same content");
        }

        [Test]
        public void CompareObjectsTest_ComplexArray_Equal()
        {
            var subA1 = new ClassWithFieldsTest { a = 1, b = "test", c = 2};
            var subB1 = new ClassWithFieldsTest { a = 1, b = "test", c = 2 }; 
            var subA2 = new ClassWithFieldsTest { a = 2, b = "testt", c = 3 };
            var subB2 = new ClassWithFieldsTest { a = 2, b = "testt", c = 3 };

            var a = new ClassWithComplexArrayTest {a = new[] {subA1, subA2}};
            var b = new ClassWithComplexArrayTest { a = new[] {subB1, subB2}};

            a.CompareObjects(b).Should().BeTrue("Arrays have same content");
            a.CompareObjectsOnlyFields(b).Should().BeTrue("Classes provides field Arrays and Arrays have same content");
        }

        [Test]
        public void CompareObjectsTest_ComplexArray_NotEqual()
        {
            var subA1 = new ClassWithFieldsTest { a = 1, b = "test", c = 2 };
            var subB1 = new ClassWithFieldsTest { a = 1, b = "test", c = 2 };
            var subA2 = new ClassWithFieldsTest { a = 2, b = "testt", c = 3 };
            var subB2 = new ClassWithFieldsTest { a = 2, b = "differencehere", c = 3 };

            var a = new ClassWithComplexArrayTest { a = new[] { subA1, subA2 } };
            var b = new ClassWithComplexArrayTest { a = new[] { subB1, subB2 } };

            a.CompareObjects(b).Should().BeFalse("Arrays have not same content");
            a.CompareObjectsOnlyFields(b).Should().BeFalse("Classes provides field Arrays but Arrays have not same content");
        }

        [Test]
        public void CompareObjectsTest_ComplexField_Equal()
        {
            var a = new ClassWithComplexFieldTest { item = new ClassWithFieldsTest { a = 12 } };
            var b = new ClassWithComplexFieldTest { item = new ClassWithFieldsTest { a = 12 } };

            a.CompareObjectsOnlyFields(b).Should().BeTrue();
        }

        [Test]
        public void CompareObjectsTest_ComplexField_NotEqual()
        {
            var a = new ClassWithComplexFieldTest {item = new ClassWithFieldsTest {a = 12}};
            var b = new ClassWithComplexFieldTest {item = new ClassWithFieldsTest {b = "12"}};

            a.CompareObjectsOnlyFields(b).Should().BeFalse();
        }
    }

    [TestFixture]
    public class CopyTests
    {
        [Test]
        public void FlatCloneTest()
        {
            var a = new TestFlatClass {Text = "old text", Number = 1};

            var b = a.Copy();

            a.Text = "new text";
            a.Number = 2;

            b.Text.Should().Be("old text");
            b.Number.Should().Be(1);
        }

        [Test]
        public void ReferenceCloneTest()
        {
            var customA = new TestFlatClass {Text = "old text", Number = 1};
            var a = new TestComplexClass {Custom = customA};

            var b = a.Copy();

            a.Custom.Text = "new text";
            a.Custom.Number = 2;

            b.Custom.Should().NotBeSameAs(customA, "deep clone must create a new instance");
            b.Custom.Text.Should().Be("old text");
            b.Custom.Number.Should().Be(1);
        }

        [Test]
        public void DerivedCloneTest()
        {
            var a = new TestDerivedClass {Text = "old text", Number = 1, FloatNumber = 1.1};

            var b = a.Copy();

            a.Text = "new text";
            a.Number = 2;
            a.FloatNumber = 2.2;

            b.Text.Should().Be("old text");
            b.Number.Should().Be(1);
            b.FloatNumber.Should().BeInRange(1.1, 1.1);
        }

        [Test]
        public void PartialCopyTest()
        {
            const string textValue = "magictextvalue";
            const int numValue = 574842389;
            const float floatValue = 1.58067453f;
            var a = new TestFlatClass();
            a.Text = textValue;
            a.Number = numValue;
            var b = new TestDerivedClass();
            b.FloatNumber = floatValue;

            b.PartialCopy(a);

            b.Text.Should().Be(textValue);
            b.Number.Should().Be(numValue);
            b.FloatNumber.Should().Be(floatValue);
        }
    }
}
