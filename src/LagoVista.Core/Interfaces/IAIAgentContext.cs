using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.Core.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public enum LlmProviders
    {
        [EnumLabel("openai", LagoVistaCommonStrings.Names.LLMProvider_OpenAI, typeof(LagoVistaCommonStrings))]
        OpenAI
    }

    public interface IAIAgentContext
    {
        string AzureAccountId { get; set; }
        string AzureApiToken { get; set; }
        string AzureApiTokenSecretId { get; set; }
        string BlobContainerName { get; set; }
        List<IConversationContext> ConversationContexts { get; set; }
        EntityHeader DefaultConversationContext { get; set; }
        string Icon { get; set; }
        string LlmApiKey { get; set; }
        string LlmApiKeySecretId { get; set; }
        EntityHeader<LlmProviders> LlmProvider { get; set; }
        string VectorDatabaseApiKey { get; set; }
        string VectorDatabaseApiKeySecretId { get; set; }
        string VectorDatabaseCollectionName { get; set; }
        string VectorDatabaseUri { get; set; }
        string EmbeddingModel { get; set; }
    }

    public interface IConversationContext
    {
        string Id { get; set; }
        string ModelName { get; set; }
        string Name { get; set; }
        string System { get; set; }
        float Temperature { get; set; }

        List<string> GetFormFields();
    }

}
