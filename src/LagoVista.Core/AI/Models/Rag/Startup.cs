using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models.Rag
{
    internal class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            RagPayloadIndexRegistry.Register<RagAIVectorPayload>();
            RagPayloadIndexRegistry.Register<RagCodeVectorPayload>();
            RagPayloadIndexRegistry.Register<RagVectorPayload>();
            RagPayloadIndexRegistry.Register<RagEntityVectorPayload>();
            RagPayloadIndexRegistry.Register<RagArtifactVectorPayload>();
        }
    }
}