using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Shared.Common.Interfaces
{
    public interface ICertificateStore : IDisposable
    {
        string GetPasswordForSigningCertificate(int key);
        string GetPasswordForEncryptionCertificate(int key);
        X509Certificate2 GetCertificateForEncryption(int key);
        X509Certificate2 GetCertificateForSigning(int key);
        //X509Certificate2 GetDummyCertificateForEncryption();
        void AddCertificate(int herId, string name, string password, X509Certificate2 forSigning, X509Certificate2 forCrypto);
        List<int> GetAllValidHerIds();
        string GetArNameFromHerId(int herId);
        void Clone(int herId, string arName, int cloneFromHerId);
    }
}
