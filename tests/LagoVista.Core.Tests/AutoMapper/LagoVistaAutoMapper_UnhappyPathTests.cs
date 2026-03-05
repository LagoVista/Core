using LagoVista.Core.AutoMapper;
using LagoVista.Core.AutoMapper.LagoVista.Core.AutoMapper;
using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.Mapping
{
    [TestFixture]
    public class LagoVistaAutoMapper_UnhappyPathTests
    {
        private sealed class StubEncryptedMapper : IEncryptedMapper
        {
            public Task MapDecryptAsync<TDomain, TDto>(TDomain domain, TDto dto, EntityHeader org, EntityHeader user, CancellationToken ct = default)
                where TDomain : class where TDto : class => Task.CompletedTask;

            public Task MapEncryptAsync<TDomain, TDto>(TDomain domain, TDto dto, EntityHeader org, EntityHeader user, CancellationToken ct = default)
                where TDomain : class where TDto : class => Task.CompletedTask;
        }

        private sealed class FailingAtomicBuilder : IAtomicPlanBuilder
        {
            public InvokeResult<IReadOnlyList<AtomicMapStep>> BuildAtomicSteps(Type sourceType, Type targetType)
                => InvokeResult<IReadOnlyList<AtomicMapStep>>.FromErrors(new ErrorMessage { Message = "boom" });
        }

        private sealed class EmptyAtomicBuilder : IAtomicPlanBuilder
        {
            public InvokeResult<IReadOnlyList<AtomicMapStep>> BuildAtomicSteps(Type sourceType, Type targetType)
                => InvokeResult<IReadOnlyList<AtomicMapStep>>.Create(Array.Empty<AtomicMapStep>());
        }

        private sealed class DummyRegistry : IMapValueConverterRegistry
        {
            public IEnumerable<IMapValueConverter> Converters => Array.Empty<IMapValueConverter>();
            public bool CanConvert(Type sourceType, Type targetType) => false;
            public IMapValueConverter GetConverter(Type converterType) => null;
            public IMapValueConverter GetConverter(Type sourceType, Type targetType) => null;
            public bool TryConvert(object sourceValue, Type targetType, out object convertedValue) { convertedValue = null; return false; }
            public bool TryConvert(object sourceValue, Type targetType, Type converterType, out object convertedValue) { convertedValue = null; return false; }
            public int AddRange(IEnumerable<IMapValueConverter> converters) => 0;
            public int AddRange(params IMapValueConverter[] converters) => 0;
            public void Deduplicate() { }
        }

        private static readonly EntityHeader Org = EntityHeader.Create("org", "Org");
        private static readonly EntityHeader User = EntityHeader.Create("user", "User");

        private sealed class SimpleSrc { public string Name { get; set; } = "x"; }
        private sealed class SimpleDst { public string Name { get; set; } }

        [Test]
        public void MapAsync_Throws_WhenAtomicBuilderReturnsErrors()
        {
            var mapper = new LagoVistaAutoMapper(
                encryptedMapper: new StubEncryptedMapper(),
                atomicBuilder: new FailingAtomicBuilder(),
                converters: new DummyRegistry());

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await mapper.MapAsync(new SimpleSrc(), new SimpleDst(), Org, User));

            Assert.That(ex!.Message, Does.Contain("boom"));
        }

        // ---- Child-step coverage helpers ----

        private sealed class ChildStep : IChildMapStep
        {
            public ChildMapStepKind Kind { get; set; }
            public PropertyInfo SourceProperty { get; set; }
            public PropertyInfo TargetProperty { get; set; }
            public Type ChildSourceType { get; set; }
            public Type ChildTargetType { get; set; }
            public IReadOnlyList<IChildMapStep> Children { get; set; } = Array.Empty<IChildMapStep>();
        }

        private sealed class ParentSrc_List
        {
            public List<ChildSrc_NoHeader> Items { get; set; }
        }

        private sealed class ParentDst_List
        {
            public List<ChildDst> Items { get; set; }
        }

        private sealed class ChildSrc_NoHeader { public string Name { get; set; } = "n"; }
        private sealed class ChildDst { public string Name { get; set; } }

        [Test]
        public void ExecuteChildSteps_Throws_OnUnsupportedKind()
        {
            var mapper = new LagoVistaAutoMapper(new StubEncryptedMapper(), new EmptyAtomicBuilder(), new DummyRegistry());

            // We call MapAsync with a configurePlan that forces childSteps to be used.
            // But easiest is to invoke the private ExecuteChildStepsAsync via reflection.

            var mi = typeof(LagoVistaAutoMapper).GetMethod("ExecuteChildStepsAsync", BindingFlags.Instance | BindingFlags.NonPublic);

            var badStep = new ChildStep
            {
                Kind = (ChildMapStepKind)999
            };

            var task = (Task)mi!.Invoke(mapper, new object[]
            {
                new object(),
                new object(),
                new List<IChildMapStep> { badStep },
                Org,
                User,
                CancellationToken.None
            });

            var ex = Assert.ThrowsAsync<NotSupportedException>(async () => await task);
            Assert.That(ex!.Message, Does.Contain("Unsupported child step kind"));
        }

        [Test]
        public void ExecuteChildCollection_SetsTargetNull_WhenSourceListNull()
        {
            var mapper = new LagoVistaAutoMapper(new StubEncryptedMapper(), new EmptyAtomicBuilder(), new DummyRegistry());
            var mi = typeof(LagoVistaAutoMapper).GetMethod("ExecuteChildCollectionAsync", BindingFlags.Instance | BindingFlags.NonPublic);

            var src = new ParentSrc_List { Items = null };
            var dst = new ParentDst_List { Items = new List<ChildDst>() };

            var step = new ChildStep
            {
                Kind = ChildMapStepKind.Collection,
                SourceProperty = typeof(ParentSrc_List).GetProperty(nameof(ParentSrc_List.Items))!,
                TargetProperty = typeof(ParentDst_List).GetProperty(nameof(ParentDst_List.Items))!,
                ChildSourceType = typeof(ChildSrc_NoHeader),
                ChildTargetType = typeof(ChildDst),
                Children = Array.Empty<IChildMapStep>()
            };

            var task = (Task)mi!.Invoke(mapper, new object[] { src, dst, step, Org, User, CancellationToken.None });
            Assert.DoesNotThrowAsync(async () => await task);
            Assert.That(dst.Items, Is.Null);
        }

        [Test]
        public void ExecuteChildCollection_AddsNullItem_WhenSourceContainsNull()
        {
            var mapper = new LagoVistaAutoMapper(new StubEncryptedMapper(), new EmptyAtomicBuilder(), new DummyRegistry());
            var mi = typeof(LagoVistaAutoMapper).GetMethod("ExecuteChildCollectionAsync", BindingFlags.Instance | BindingFlags.NonPublic);

            var src = new ParentSrc_List { Items = new List<ChildSrc_NoHeader> { null } };
            var dst = new ParentDst_List();

            var step = new ChildStep
            {
                Kind = ChildMapStepKind.Collection,
                SourceProperty = typeof(ParentSrc_List).GetProperty(nameof(ParentSrc_List.Items))!,
                TargetProperty = typeof(ParentDst_List).GetProperty(nameof(ParentDst_List.Items))!,
                ChildSourceType = typeof(ChildSrc_NoHeader),
                ChildTargetType = typeof(ChildDst),
                Children = Array.Empty<IChildMapStep>()
            };

            var task = (Task)mi!.Invoke(mapper, new object[] { src, dst, step, Org, User, CancellationToken.None });
            Assert.DoesNotThrowAsync(async () => await task);

            Assert.That(dst.Items, Is.Not.Null);
            Assert.That(dst.Items.Count, Is.EqualTo(1));
            Assert.That(dst.Items[0], Is.Null);
        }

        private sealed class ParentSrc_EH { public ChildSrc_NoHeader Customer { get; set; } = new(); }
        private sealed class ParentDst_EH { public EntityHeader<ChildDst> Customer { get; set; } }

        [Test]
        public void ExecuteEntityHeaderValue_Throws_WhenToEntityHeaderMissing()
        {
            var mapper = new LagoVistaAutoMapper(new StubEncryptedMapper(), new EmptyAtomicBuilder(), new DummyRegistry());
            var mi = typeof(LagoVistaAutoMapper).GetMethod("ExecuteEntityHeaderValueAsync", BindingFlags.Instance | BindingFlags.NonPublic);

            var src = new ParentSrc_EH();
            var dst = new ParentDst_EH();

            var step = new ChildStep
            {
                Kind = ChildMapStepKind.EntityHeaderValue,
                SourceProperty = typeof(ParentSrc_EH).GetProperty(nameof(ParentSrc_EH.Customer))!,
                TargetProperty = typeof(ParentDst_EH).GetProperty(nameof(ParentDst_EH.Customer))!,
                ChildSourceType = typeof(ChildSrc_NoHeader), // no ToEntityHeader()
                ChildTargetType = typeof(ChildDst),
                Children = Array.Empty<IChildMapStep>()
            };

            var task = (Task)mi!.Invoke(mapper, new object[] { src, dst, step, Org, User, CancellationToken.None });
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await task);
            Assert.That(ex!.Message, Does.Contain("must implement public instance method ToEntityHeader"));
        }

        private sealed class ChildSrc_NullHeader
        {
            public EntityHeader ToEntityHeader() => null;
        }

        private sealed class ParentSrc_EH2 { public ChildSrc_NullHeader Customer { get; set; } = new(); }

        [Test]
        public void ExecuteEntityHeaderValue_Throws_WhenToEntityHeaderReturnsNull()
        {
            var mapper = new LagoVistaAutoMapper(new StubEncryptedMapper(), new EmptyAtomicBuilder(), new DummyRegistry());
            var mi = typeof(LagoVistaAutoMapper).GetMethod("ExecuteEntityHeaderValueAsync", BindingFlags.Instance | BindingFlags.NonPublic);

            var src = new ParentSrc_EH2();
            var dst = new ParentDst_EH();

            var step = new ChildStep
            {
                Kind = ChildMapStepKind.EntityHeaderValue,
                SourceProperty = typeof(ParentSrc_EH2).GetProperty(nameof(ParentSrc_EH2.Customer))!,
                TargetProperty = typeof(ParentDst_EH).GetProperty(nameof(ParentDst_EH.Customer))!,
                ChildSourceType = typeof(ChildSrc_NullHeader),
                ChildTargetType = typeof(ChildDst),
                Children = Array.Empty<IChildMapStep>()
            };

            var task = (Task)mi!.Invoke(mapper, new object[] { src, dst, step, Org, User, CancellationToken.None });
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await task);
            Assert.That(ex!.Message, Does.Contain("ToEntityHeader() returned null"));
        }
    }
}