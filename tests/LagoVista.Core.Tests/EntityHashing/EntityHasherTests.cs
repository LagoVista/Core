using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.EntityHashing
{
    [TestFixture]
    public class EntityHasherTests
    {

        [Test]
        public void ShouldCalculateSameHash()
        {
            var devJson = System.IO.File.ReadAllText("EntityHashing/JsonEntity.Dev.json");


            var prodJson = System.IO.File.ReadAllText("EntityHashing/JsonEntity.Live.json");
            var cosmosJson = System.IO.File.ReadAllText("EntityHashing/JsonEntity.Cosmos.JObject.json");

            var hash1 = LagoVista.EntityHasher.CalculateHash(Newtonsoft.Json.Linq.JToken.Parse(devJson));
            var hash2 = LagoVista.EntityHasher.CalculateHash(Newtonsoft.Json.Linq.JToken.Parse(prodJson));

            var hash3 = LagoVista.EntityHasher.CalculateHash(Newtonsoft.Json.Linq.JToken.Parse(cosmosJson));

            Console.WriteLine(hash1 + "\r\n" + hash2 + "\r\n" + hash3);

            Assert.That(hash1, Is.EqualTo(hash2));
        }


        [Test]
        public void MatchACP_Dev_Prod()
        {
            var acpProdJson = System.IO.File.ReadAllText("EntityHashing/ACP.Dev.json");
            var hashAcpProd = LagoVista.EntityHasher.CalculateHash(Newtonsoft.Json.Linq.JToken.Parse(acpProdJson));

            var acpDevJSON = System.IO.File.ReadAllText("EntityHashing/ACP.Prod.json");
            var hashAcpDev = LagoVista.EntityHasher.CalculateHash(Newtonsoft.Json.Linq.JToken.Parse(acpDevJSON));

            Console.WriteLine(hashAcpDev + "\r\n" + hashAcpProd);
            Assert.That(hashAcpDev, Is.EqualTo(hashAcpProd));
        }

        private static JsonSerializerSettings DefaultSettings()
        {
            return new JsonSerializerSettings
            {
                // Stable, minimal representation
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,

                // Avoid surprises
                ReferenceLoopHandling = ReferenceLoopHandling.Error
            };
        }


        [Test]
        public void MatchACP_Entity_Dev_Prod()
        {
            var acpProdJson = System.IO.File.ReadAllText("EntityHashing/ACP.Dev.Entity.json");
            var hashAcpProd = LagoVista.EntityHasher.CalculateHash(Newtonsoft.Json.Linq.JToken.Parse(acpProdJson));

            var acpDevJSON = System.IO.File.ReadAllText("EntityHashing/ACP.Prod.json");
            var hashAcpDev = LagoVista.EntityHasher.CalculateHash(Newtonsoft.Json.Linq.JToken.Parse(acpDevJSON));

            Console.WriteLine(hashAcpDev + "\r\n" + hashAcpProd);
            Assert.That(hashAcpDev, Is.EqualTo(hashAcpProd));

        }

        [Test]
        public void MatchACP_Cosmos_Prod()
        {
            var acpProdJson = System.IO.File.ReadAllText("EntityHashing/ACP.Cosmos.Prod.json");
            var hashAcpProd = LagoVista.EntityHasher.CalculateHash(Newtonsoft.Json.Linq.JToken.Parse(acpProdJson));

            var acpDevJSON = System.IO.File.ReadAllText("EntityHashing/ACP.Prod.json");
            var hashAcpDev = LagoVista.EntityHasher.CalculateHash(Newtonsoft.Json.Linq.JToken.Parse(acpDevJSON));

            Console.WriteLine(hashAcpDev + "\r\n" + hashAcpProd);
            Assert.That(hashAcpDev, Is.EqualTo(hashAcpProd));

        }

        [Test]
        public void MatchACP_Cosmos_Dev()
        {

            var acpProdJson = System.IO.File.ReadAllText("EntityHashing/ACP.Cosmos.Prod.json");
            var art = JsonConvert.DeserializeObject<LagoVista.AI.Models.ArtifactSpecification>(acpProdJson);

            art.SetHash();

            var acpDevJSON = System.IO.File.ReadAllText("EntityHashing/ACP.Dev.json");
            var hashAcpDev = LagoVista.EntityHasher.CalculateHash(Newtonsoft.Json.Linq.JToken.Parse(acpDevJSON));
            Console.WriteLine(hashAcpDev + "\r\n" + art.Sha256Hex);
            Assert.That(hashAcpDev, Is.EqualTo(art.Sha256Hex));
        }
    }
}
