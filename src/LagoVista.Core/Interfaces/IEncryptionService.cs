using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IEncryptionServices
    {
        string Encrypt(string id, decimal rate);
        string Encrypt(string id, int rate);
        string Encrypt(string id, double rate);
        decimal Decrypt(string id, string rate);
        string DecryptString(string id, string value);

        Task<InvokeResult> SetAccountEncryptionString();
    }
}
