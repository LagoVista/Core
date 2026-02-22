using LagoVista.Core.AutoMapper;
using LagoVista.Core.AutoMapper.Converters;
using LagoVista.Core.AutoMapper.LagoVista.Core.AutoMapper;
using LagoVista.Core.Interfaces.AutoMapper;
using Microsoft.Win32;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LagoVista.Core.Tests.AutoMapper.TestModels.PlannerModels;

namespace LagoVista.Core.Tests.AutoMapper
{
    [TestFixture]
    public class PlannerTests
    {
        private IAtomicPlanBuilder _atomicBuilder;
        private IMapValueConverterRegistry _registry;

        [SetUp]
        public void TestInitialize()
        {
            _registry = ConvertersRegistration.DefaultConverterRegistery;
            _atomicBuilder = new ReflectionAtomicPlanBuilder(_registry);
        }


        [Test]
        public void Should_Use_Encryption_With_Encryption_Attribute_ToDTO()
        {
            var plan = _atomicBuilder.BuildAtomicSteps(typeof(SimpleEncrypted), typeof(SimpleEncryptedDTO));
            var encryptedStep = plan.Result.FirstOrDefault(s => s.SourceProperty.Name == nameof(SimpleEncrypted.EncryptedValue));
            foreach (var step in plan.Result)
            {
                Console.WriteLine($"Source: {step.SourceProperty.Name}, Destination: {step.TargetProperty.Name}, Kind: {step.Kind}, Converter: {step.ConverterType?.Name}");
            }

            Assert.That(encryptedStep.Kind, Is.EqualTo(AtomicMapStepKind.Crypto));
        }

        [Test]
        public void Should_Use_Encryption_With_Encryption_Attribute_FromDTO()
        {
            var plan = _atomicBuilder.BuildAtomicSteps(typeof(SimpleEncryptedDTO), typeof(SimpleEncrypted));
           var encryptedStep = plan.Result.FirstOrDefault(s => s.SourceProperty.Name == nameof(SimpleEncrypted.EncryptedValue));
            foreach (var step in plan.Result)
            {
                Console.WriteLine($"Source: {step.SourceProperty?.Name ?? "NULL"}, Destination: {step.TargetProperty?.Name ?? "NULL"}, Kind: {step.Kind}, Converter: {step.ConverterType?.Name}");
            }

           Assert.That(encryptedStep.Kind, Is.EqualTo(AtomicMapStepKind.Crypto));
        }

    }
}
