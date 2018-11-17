using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface IEncryptionProvider
    {
        string Encrypt(string plainTextString, string encryptionKeyModifier = null);
        string Decrypt(string encryptedString, string encryptionKeyModifier = null); 
    }
}
