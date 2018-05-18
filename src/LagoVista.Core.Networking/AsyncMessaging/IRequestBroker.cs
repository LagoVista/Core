﻿using LagoVista.Core.Validation;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IRequestBroker
    {
        /// <summary>
        /// T must be an interface and must include the AsyncMessagingAttribute attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subject"></param>
        void RegisterSubject<T>(T subject) where T : class;

        Task<IAsyncResponse> HandleMessageAsync(IAsyncRequest request);
    }
}
