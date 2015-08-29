using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography;

public class EncryptionServiceImpl : Singleton<EncryptionServiceImpl>, EncryptionService {
    private string key = "EncRypt!0nKey";
    public bool useEncyption { get; set; }

    public EncryptionServiceImpl() {
        useEncyption = false;
    }

    public void Initialize(string key) {
        this.key = key;
    }

    public string Encrypt(string plainText) {
        if (useEncyption == false) {
            return plainText;
        }

        return Encrypt(key, plainText, true);
    }

    public string Decrypt(string cypherString) {
        if (useEncyption == false) {
            return cypherString;
        }

        return Decrypt(key, cypherString, true);
    }

    private string Encrypt(string key, string ToEncrypt, bool useHasing) {
        byte[] keyArray;
        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(ToEncrypt);
        
        if (useHasing) {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();  
        }
        else {
            keyArray = UTF8Encoding.UTF8.GetBytes(key);
        }
        TripleDESCryptoServiceProvider tDes = new TripleDESCryptoServiceProvider();
        tDes.Key = keyArray;
        tDes.Mode = CipherMode.ECB;
        tDes.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = tDes.CreateEncryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        tDes.Clear();
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    private string Decrypt(string key, string cypherString, bool useHasing) {
        byte[] keyArray;
        
        if (useHasing) {
            MD5CryptoServiceProvider hashmd = new MD5CryptoServiceProvider();
            keyArray = hashmd.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd.Clear();
        }
        else {
            keyArray = UTF8Encoding.UTF8.GetBytes(key);
        }
        TripleDESCryptoServiceProvider tDes = new TripleDESCryptoServiceProvider();
        tDes.Key = keyArray;
        tDes.Mode = CipherMode.ECB;
        tDes.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = tDes.CreateDecryptor();
        try {
            byte[] toDecryptArray = Convert.FromBase64String(cypherString);
            byte[] resultArray = cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);
            tDes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray,0,resultArray.Length);
        }
        catch (Exception ex) {
        }

        return "";
    }
}