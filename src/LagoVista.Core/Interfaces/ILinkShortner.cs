﻿using LagoVista.Core.Validation;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface ILinkShortener
    {
        Task<InvokeResult<string>> ShortenLinkAsync(string url);
        Task<InvokeResult<string>> RestoreLinkAsync(string partition, string link);
    }
}
