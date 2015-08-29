using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class PersistenceUtil {

    public static string LoadTextFile(string path) {
        try {
            StreamReader sr = new StreamReader(path);
            string text = sr.ReadToEnd();
            sr.Close();
            
            Debug.Log("File load complete : " + path);
            return text;
        }
        catch (Exception e) {
            Debug.Log(e.Message);
            return "";
        }
    }

    public static string LoadDecryptTextFile(string path) {
        string cypherString = LoadTextFile(path);
        if (string.IsNullOrEmpty(cypherString) == true) {
            return "";
        }

        return Service.encryption.Decrypt(cypherString);
    }


    public static string LoadTextResource(string path) {
        TextAsset textAsset = (TextAsset) Resources.Load(path, typeof(TextAsset));
        
        if (textAsset == null) {
            Debug.Log("Can not load text resource : " + path);
            return "";
        }
        
        return textAsset.text;
    }

    public static bool SaveTextFile(string path, string text) {
        try {
            StreamWriter sw = new StreamWriter(path);
            sw.Write(text);
            sw.Close();
            
            Debug.Log("File save complete: "  + path);
            return true;
        }
        catch (Exception e) {
            Debug.LogError(e.Message);
            return false;
        }
    }

    public static bool SaveEncryptTextFile(string path, string text) {
        string cypherString = Service.encryption.Encrypt(text);
        return SaveTextFile(path, cypherString);
    }


    public static bool DeleteFile(string path) {
        try {
            File.Delete(path);
            return true;
        } catch (Exception e) {
            Debug.LogError(e.Message);
            return false;
        }

    }

    public static byte[] ReadBinaryFile(string path) {
        try {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            if (fs == null) {
                return null;
            }

            BinaryReader reader = new BinaryReader(fs);
            byte[] result = reader.ReadBytes((int)reader.BaseStream.Length);
            reader.Close();
            fs.Close();

            return result;
        }
        catch (Exception e) {
            Debug.Log(e.Message);
            return null;
        }

        return null;
    }

    public static bool WriteBinaryFile(string path, byte[] data) {
        try {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            if (fs == null) {
                return false;
            }

            BinaryWriter writer = new BinaryWriter(fs);
            writer.Write(data);
            writer.Close();
            fs.Close();
            return true;
        }
        catch (Exception e) {
            Debug.LogError(e.Message);
            return false;
        }

        return false;
    }

    public static void CreateFolder(string path) {
        DirectoryInfo di = new DirectoryInfo(path);
        if (di.Exists == false) {
            di.Create();
        }
    }

    public static void DeleteFolder(string path) {
        DirectoryInfo di = new DirectoryInfo(path);
        try {
            if (di.Exists == true) {
                di.Delete(true);
            }
        }
        catch (Exception e) {
            Debug.LogError(e.ToString());
        }
    }
    
    public static List<string> GetFileList(string folderPath, string extName, bool includeExt) {
        List<string> fileList = new List<string>();
        DirectoryInfo di = new DirectoryInfo(folderPath);
        try {
            if (di.Exists == true) {
                var list = di.GetFiles();
                for (int i=0; i < list.Length; i++) {
                    var fileInfo = list[i];
                    var fileName = fileInfo.Name;
                    if (fileInfo.Extension.Equals(extName)) {
                        if (!includeExt) {
                            fileName = fileName.Replace(fileInfo.Extension, "");
                        }
                        
                        fileList.Add(fileName);
                    }
                }
            }
        }
        catch (Exception e) {
            Debug.LogError(e.ToString());
        }
        
        return fileList;
    }
}