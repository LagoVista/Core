using LagoVista.Core.AutoMapper;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LagoVista.Core.Tests.Mapping
{
    [TestFixture]
    public sealed class AtomicStepExecutorTests
    {
        private sealed class FakeConverterRegistry : IMapValueConverterRegistry
        {
            public bool NextTryConvertResult { get; set; } = true;
            public object NextConvertedValue { get; set; }

            public IEnumerable<IMapValueConverter> Converters => throw new NotImplementedException();

            public int AddRange(params IMapValueConverter[] converters)
            {
                throw new NotImplementedException();
            }

            public bool CanConvert(Type sourceType, Type targetType)
            {
                throw new NotImplementedException();
            }

            public IMapValueConverter GetConverter(Type sourceType, Type targetType)
            {
                throw new NotImplementedException();
            }

            public IMapValueConverter GetConverter(Type converterType)
            {
                throw new NotImplementedException();
            }

            public bool TryConvert(object source, Type targetType, out object converted)
            {
                converted = NextConvertedValue;
                return NextTryConvertResult;
            }

            public bool TryConvert(object source, Type targetType, Type converterType, out object converted)
            {
                converted = NextConvertedValue;
                return NextTryConvertResult;
            }
        }

        private sealed class DummyEh { }

        private sealed class Src
        {
            public string Id { get; set; }
            public string Text { get; set; }

            public int IntVal { get; set; }
            public string StrVal { get; set; }

            public Guid GuidVal { get; set; }
            public GuidString36 Guid36Val { get; set; }

            public LagoVistaKey KeyVal { get; set; }
            public NormalizedId32 Norm32Val { get; set; }
        }

        private sealed class Tgt
        {
            public EntityHeader Eh { get; set; }
            public EntityHeader<DummyEh> EhGeneric { get; set; }
            public string StrVal { get; set; }

            public EntityHeader<TestEnum> EhEnum { get; set; }

            public Guid GuidVal { get; set; }
            public Guid? GuidNullable { get; set; }
            public GuidString36 Guid36Val { get; set; }
            public GuidString36? Guid36Nullable { get; set; }

            public LagoVistaKey KeyVal { get; set; }
            public NormalizedId32 Norm32Val { get; set; }
        }

        private enum TestEnum
        {
            A = 0
        }

        private static PropertyInfo P<T>(string name) => typeof(T).GetProperty(name, BindingFlags.Public | BindingFlags.Instance);

        private static AtomicMapStep Step(
            AtomicMapStepKind kind,
            PropertyInfo src,
            PropertyInfo tgt,
            PropertyInfo src2 = null,
            Type converterType = null,
            TargetTypes targetType = TargetTypes.FromProperty)
        {
            return new AtomicMapStep(
                targetProperty: tgt,
                sourceProperty: src,
                kind: kind,
                sourceProperty2: src2,
                converterType: converterType,
                targetType: targetType);
        }

        [Test]
        public void Execute_NullArgs_Throw()
        {
            var sut = new AtomicStepExecutor(new FakeConverterRegistry());
            var src = new Src();
            var tgt = new Tgt();
            var steps = new List<AtomicMapStep>();

            Assert.Throws<ArgumentNullException>(() => sut.Execute(null, tgt, steps));
            Assert.Throws<ArgumentNullException>(() => sut.Execute(src, null, steps));
            Assert.Throws<ArgumentNullException>(() => sut.Execute(src, tgt, null));
        }

        [Test]
        public void Execute_SkipsNonExecutableKinds()
        {
            var sut = new AtomicStepExecutor(new FakeConverterRegistry());
            var src = new Src { StrVal = "x" };
            var tgt = new Tgt { StrVal = null };

            var steps = new List<AtomicMapStep>
            {
                Step(AtomicMapStepKind.Manual, P<Src>(nameof(Src.StrVal)), P<Tgt>(nameof(Tgt.StrVal))),
                Step(AtomicMapStepKind.Ignored, P<Src>(nameof(Src.StrVal)), P<Tgt>(nameof(Tgt.StrVal))),
                Step(AtomicMapStepKind.Crypto, P<Src>(nameof(Src.StrVal)), P<Tgt>(nameof(Tgt.StrVal))),
                Step(AtomicMapStepKind.ChildLeaf, P<Src>(nameof(Src.StrVal)), P<Tgt>(nameof(Tgt.StrVal))),
            };

            sut.Execute(src, tgt, steps);

            Assert.That(tgt.StrVal, Is.Null);
        }

        [Test]
        public void Execute_EntityHeaderFromIdText_NonGeneric_Works()
        {
            var sut = new AtomicStepExecutor(new FakeConverterRegistry());
            var src = new Src { Id = "id-1", Text = "hello" };
            var tgt = new Tgt();

            var steps = new List<AtomicMapStep>
            {
                Step(AtomicMapStepKind.EntityHeaderFromIdText, P<Src>(nameof(Src.Id)), P<Tgt>(nameof(Tgt.Eh)), src2: P<Src>(nameof(Src.Text)))
            };

            sut.Execute(src, tgt, steps);

            Assert.That(tgt.Eh, Is.Not.Null);
            Assert.That(tgt.Eh.Id, Is.EqualTo("id-1"));
            Assert.That(tgt.Eh.Text, Is.EqualTo("hello"));
        }

        [Test]
        public void Execute_EntityHeaderFromIdText_Generic_Works()
        {
            var sut = new AtomicStepExecutor(new FakeConverterRegistry());
            var src = new Src { Id = "id-2", Text = "world" };
            var tgt = new Tgt();

            var steps = new List<AtomicMapStep>
            {
                Step(AtomicMapStepKind.EntityHeaderFromIdText, P<Src>(nameof(Src.Id)), P<Tgt>(nameof(Tgt.EhGeneric)), src2: P<Src>(nameof(Src.Text)))
            };

            sut.Execute(src, tgt, steps);

            Assert.That(tgt.EhGeneric, Is.Not.Null);
            Assert.That(tgt.EhGeneric.Id, Is.EqualTo("id-2"));
            Assert.That(tgt.EhGeneric.Text, Is.EqualTo("world"));
        }

        [Test]
        public void Execute_EntityHeaderFromIdText_MissingSourceProperty2_Throws()
        {
            var sut = new AtomicStepExecutor(new FakeConverterRegistry());
            var src = new Src { Id = "id-3" };
            var tgt = new Tgt();

            var steps = new List<AtomicMapStep>
            {
                Step(AtomicMapStepKind.EntityHeaderFromIdText, P<Src>(nameof(Src.Id)), P<Tgt>(nameof(Tgt.Eh)))
            };

            Assert.Throws<InvalidOperationException>(() => sut.Execute(src, tgt, steps));
        }

        [Test]
        public void Execute_DirectAssign_Assignable_Works()
        {
            var sut = new AtomicStepExecutor(new FakeConverterRegistry());
            var src = new Src { StrVal = "abc" };
            var tgt = new Tgt();

            var steps = new List<AtomicMapStep>
            {
                Step(AtomicMapStepKind.DirectAssign, P<Src>(nameof(Src.StrVal)), P<Tgt>(nameof(Tgt.StrVal)))
            };

            sut.Execute(src, tgt, steps);

            Assert.That(tgt.StrVal, Is.EqualTo("abc"));
        }

        [Test]
        public void Execute_DirectAssign_NullValue_SetsNull()
        {
            var sut = new AtomicStepExecutor(new FakeConverterRegistry());
            var src = new Src { StrVal = null };
            var tgt = new Tgt { StrVal = "was" };

            var steps = new List<AtomicMapStep>
            {
                Step(AtomicMapStepKind.DirectAssign, P<Src>(nameof(Src.StrVal)), P<Tgt>(nameof(Tgt.StrVal)))
            };

            sut.Execute(src, tgt, steps);

            Assert.That(tgt.StrVal, Is.Null);
        }

        [Test]
        public void Execute_DirectAssign_GuidString36_ToGuid_AndGuidNullable_Works()
        {
            var sut = new AtomicStepExecutor(new FakeConverterRegistry());
            var g36 = new GuidString36("6f9619ff-8b86-d011-b42d-00c04fc964ff");
            var src = new Src { Guid36Val = g36 };
            var tgt = new Tgt();

            var steps = new List<AtomicMapStep>
            {
                Step(AtomicMapStepKind.DirectAssign, P<Src>(nameof(Src.Guid36Val)), P<Tgt>(nameof(Tgt.GuidVal))),
                Step(AtomicMapStepKind.DirectAssign, P<Src>(nameof(Src.Guid36Val)), P<Tgt>(nameof(Tgt.GuidNullable))),
            };

            sut.Execute(src, tgt, steps);

            Assert.That(tgt.GuidVal, Is.EqualTo(g36.ToGuid()));
            Assert.That(tgt.GuidNullable, Is.EqualTo((Guid?)g36.ToGuid()));
        }

        [Test]
        public void Execute_DirectAssign_Guid_ToGuidString36_Works()
        {
            var sut = new AtomicStepExecutor(new FakeConverterRegistry());
            var g = Guid.Parse("3f2504e0-4f89-11d3-9a0c-0305e82c3301");
            var src = new Src { GuidVal = g };
            var tgt = new Tgt();

            var steps = new List<AtomicMapStep>
            {
                Step(AtomicMapStepKind.DirectAssign, P<Src>(nameof(Src.GuidVal)), P<Tgt>(nameof(Tgt.Guid36Val))),
            };

            sut.Execute(src, tgt, steps);

            Assert.That(tgt.Guid36Val.Value, Is.EqualTo(g.ToString("D").ToLowerInvariant()));
        }

        [Test]
        public void Execute_DirectAssign_StringLike_ToString_Works()
        {
            var sut = new AtomicStepExecutor(new FakeConverterRegistry());
            var src = new Src { KeyVal = new LagoVistaKey("mykey") };
            var tgt = new Tgt();

            var steps = new List<AtomicMapStep>
            {
                Step(AtomicMapStepKind.DirectAssign, P<Src>(nameof(Src.KeyVal)), P<Tgt>(nameof(Tgt.StrVal))),
            };

            sut.Execute(src, tgt, steps);

            Assert.That(tgt.StrVal, Is.EqualTo("mykey"));
        }

        [Test]
        public void Execute_EntityHeaderFromIdText_EnumEntityHeader_Throws()
        {
            var sut = new AtomicStepExecutor(new FakeConverterRegistry());

            var src = new Src
            {
                Id = "not-a-valid-enum",
                Text = "text"
            };

            var tgt = new Tgt();

            var steps = new List<AtomicMapStep>
            {
                Step(
                    AtomicMapStepKind.EntityHeaderFromIdText,
                    P<Src>(nameof(Src.Id)),
                    P<Tgt>(nameof(Tgt.EhEnum)),
                    src2: P<Src>(nameof(Src.Text)))
            };

            Assert.Throws<InvalidCastException>(() => sut.Execute(src, tgt, steps));
        }

        [Test]
        public void Execute_DirectAssign_String_ToStringLike_Works()
        {
            var sut = new AtomicStepExecutor(new FakeConverterRegistry());
            var src = new Src { StrVal = "F47AC10B58CC4372A5670E02B2C3D479" };
            var tgt = new Tgt();

            var steps = new List<AtomicMapStep>
            {
                Step(AtomicMapStepKind.DirectAssign, P<Src>(nameof(Src.StrVal)), P<Tgt>(nameof(Tgt.Norm32Val))),
            };

            sut.Execute(src, tgt, steps);

            Assert.That(tgt.Norm32Val.Value, Is.EqualTo("F47AC10B58CC4372A5670E02B2C3D479"));
        }

        [Test]
        public void Execute_DirectAssign_UnsupportedConversion_Throws()
        {
            var sut = new AtomicStepExecutor(new FakeConverterRegistry());
            var src = new Src { IntVal = 5 };
            var tgt = new Tgt();

            var steps = new List<AtomicMapStep>
            {
                Step(AtomicMapStepKind.DirectAssign, P<Src>(nameof(Src.IntVal)), P<Tgt>(nameof(Tgt.StrVal))),
            };

            Assert.Throws<ArgumentException>(() => sut.Execute(src, tgt, steps));
        }

        [Test]
        public void Execute_AssignWithPlannedConverter_Success_SetsValue()
        {
            var reg = new FakeConverterRegistry { NextTryConvertResult = true, NextConvertedValue = "converted" };
            var sut = new AtomicStepExecutor(reg);

            var src = new Src { IntVal = 42 };
            var tgt = new Tgt();

            var steps = new List<AtomicMapStep>
            {
                Step(AtomicMapStepKind.DirectAssign, P<Src>(nameof(Src.IntVal)), P<Tgt>(nameof(Tgt.StrVal)), converterType: typeof(object))
            };

            sut.Execute(src, tgt, steps);

            Assert.That(tgt.StrVal, Is.EqualTo("converted"));
        }

        [Test]
        public void Execute_AssignWithPlannedConverter_Failure_Throws()
        {
            var reg = new FakeConverterRegistry { NextTryConvertResult = false };
            var sut = new AtomicStepExecutor(reg);

            var src = new Src { IntVal = 42 };
            var tgt = new Tgt();

            var steps = new List<AtomicMapStep>
            {
                Step(AtomicMapStepKind.DirectAssign, P<Src>(nameof(Src.IntVal)), P<Tgt>(nameof(Tgt.StrVal)), converterType: typeof(object))
            };

            Assert.Throws<InvalidOperationException>(() => sut.Execute(src, tgt, steps));
        }

        [Test]
        public void Execute_ConverterAssign_RuntimeFallback_Success_SetsValue()
        {
            var reg = new FakeConverterRegistry { NextTryConvertResult = true, NextConvertedValue = "fallback" };
            var sut = new AtomicStepExecutor(reg);

            var src = new Src { IntVal = 9 };
            var tgt = new Tgt();

            var steps = new List<AtomicMapStep>
            {
                Step(AtomicMapStepKind.ConverterAssign, P<Src>(nameof(Src.IntVal)), P<Tgt>(nameof(Tgt.StrVal)))
            };

            sut.Execute(src, tgt, steps);

            Assert.That(tgt.StrVal, Is.EqualTo("fallback"));
        }

        [Test]
        public void Execute_ConverterAssign_RuntimeFallback_Failure_Throws()
        {
            var reg = new FakeConverterRegistry { NextTryConvertResult = false };
            var sut = new AtomicStepExecutor(reg);

            var src = new Src { IntVal = 9 };
            var tgt = new Tgt();

            var steps = new List<AtomicMapStep>
            {
                Step(AtomicMapStepKind.ConverterAssign, P<Src>(nameof(Src.IntVal)), P<Tgt>(nameof(Tgt.StrVal)))
            };

            Assert.Throws<InvalidOperationException>(() => sut.Execute(src, tgt, steps));
        }

        [Test]
        public void Execute_UnsupportedKind_Throws()
        {
            var sut = new AtomicStepExecutor(new FakeConverterRegistry());
            var src = new Src { StrVal = "x" };
            var tgt = new Tgt();

            var steps = new List<AtomicMapStep>
            {
                Step((AtomicMapStepKind)9999, P<Src>(nameof(Src.StrVal)), P<Tgt>(nameof(Tgt.StrVal)))
            };

            Assert.Throws<NotSupportedException>(() => sut.Execute(src, tgt, steps));
        }
    }
}