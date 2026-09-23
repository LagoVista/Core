using Newtonsoft.Json;
using NUnit.Framework;

namespace LagoVista.Core.Tests.Models
{
    [TestFixture]
    public class LagoVistaIconJsonConverterTests
    {
        private sealed class IconContainer
        {
            public LagoVistaIcon Icon { get; set; }
        }

        [Test]
        public void Should_Serialize_As_String()
        {
            var json = JsonConvert.SerializeObject(new IconContainer
            {
                Icon = LagoVistaIcon.Parse("icon-ae-document")
            });

            Assert.That(json, Is.EqualTo("{\"Icon\":\"icon-ae-document\"}"));
        }

        [Test]
        public void Should_Deserialize_String_Shape()
        {
            var result = JsonConvert.DeserializeObject<IconContainer>("{\"Icon\":\"icon-ae-document\"}");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Icon.Value, Is.EqualTo("icon-ae-document"));
        }

        [Test]
        public void Should_Deserialize_Legacy_Object_Shape()
        {
            var result = JsonConvert.DeserializeObject<IconContainer>("{\"Icon\":{\"Value\":\"icon-ae-document\"}}");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Icon.Value, Is.EqualTo("icon-ae-document"));
        }
    }
}
