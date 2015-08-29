using UnityEngine;
using System;
using System.Collections;
using LitJson;

public class SettingServiceImpl : Singleton<SettingServiceImpl>, SettingService {
    private Logger logger = new Logger("SettingService");
    public Setting values { get; internal set; }

    public IEnumerator Initialize() {
        if (Load() == false) {
            values = new Setting();
        }
        yield break;
    }

    public IEnumerator Sync() {
        Save();
        yield break;
    }

    bool Load() {
        string text = PersistenceUtil.LoadDecryptTextFile("setting");
        if (text.Length == 0) {
            return false;
        }

        try {
            values = JsonMapper.ToObject<Setting>(text);
        }
        catch (Exception e) {
            logger.LogError(e.ToString());
            return false;
        }

        return values != null;        
    }

    bool Save() {
        string text = JsonMapper.ToJson(values);
        return PersistenceUtil.SaveEncryptTextFile("setting", text);
    }
}
