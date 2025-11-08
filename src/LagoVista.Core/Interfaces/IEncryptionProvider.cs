// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b3ed3639a46bb8f34e916850cf8e13f068be6dfd2c29f703a393e78d610f7307
// IndexVersion: 1
// --- END CODE INDEX META ---
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
