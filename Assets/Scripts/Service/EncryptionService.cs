using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface EncryptionService {
    bool useEncyption { get; set; }
    
    void Initialize(string key);
    string Encrypt(string ToEncrypt);
    string Decrypt(string cypherString);
}