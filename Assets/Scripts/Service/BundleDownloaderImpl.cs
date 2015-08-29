#define BOOT_BUNDLE_EDIT 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using LitJson;

public class BundleDownloaderImpl : Singleton<BundleDownloaderImpl>, BundleDownloader {
    public static int worker = 5;

    Logger logger = new Logger("[BundleDownloader]");
    Dictionary<string, uint> remoteScenes = new Dictionary<string, uint>();
    Dictionary<string, uint> remoteResources = new Dictionary<string, uint>();
    Dictionary<string, uint> localScenes = new Dictionary<string, uint>();
    Dictionary<string, uint> localResources = new Dictionary<string, uint>();
    HashSet<string> nowLoading = new HashSet<string>();
    WeakReference<WWW>[] wwwRef = new WeakReference<WWW>[worker];
    string remoteScenesMeta = "";
    string remoteResourcesMeta = "";
    string remoteScenesMetaFilePath = Application.persistentDataPath + "/remoteScenesMeta";
    string remoteResourcesMetaFilePath = Application.persistentDataPath + "/remoteResourcesMeta";
    string postfix          { get { return string.Format("{0:D3}", Version.remoteBundleVersion); } }
    string cdnHost          { get; set; }
    string cdnResources     { get { return string.Format(cdnHost + "{0}{1}/resources/", GetPlatform(), postfix); } } 
    string cdnResourcesMeta { get { return string.Format(cdnHost + "{0}{1}/resources/resources.meta", GetPlatform(), postfix); } } 
    string cdnScenes        { get { return string.Format(cdnHost + "{0}{1}/scenes/", GetPlatform(), postfix); } } 
    string cdnScenesMeta    { get { return string.Format(cdnHost + "{0}{1}/scenes/scenes.meta", GetPlatform(), postfix); } } 
    bool ready = false;
    int doneCount = 0;
    int successSceneWorkerCount = 0;
    int failedSceneWorkerCount = 0;
    int successResourceWorkerCount = 0;
    int failedResourceWorkerCount = 0;
    BundleLoadingPresenter presenter = null;

    public IEnumerator Initialize(BundleLoadingPresenter presenter, string cdnHost, NetResult result) {
        this.presenter = presenter;
        this.cdnHost = cdnHost;
        this.ready = false;
        Service.Run(UpdateProgress());

        yield return Service.Run(LoadScenes(result));
        if (result.IsFailed()) {
            yield break;
        }

        yield return Service.Run(LoadResources(result));
        if (result.IsFailed()) {
            yield break;
        }

        result.SetSuccess(true);
        ready = true;
        yield break;
    }

    IEnumerator UpdateProgress() {
        while (ready == false) {
            float progress = 0;
            bool hasProgress = false;

            for (int index = 0; index < wwwRef.Length; index++) {
                if (wwwRef[index] != null && wwwRef[index].Target != null) {
                    progress = Mathf.Max(progress, wwwRef[index].Target.progress);
                    hasProgress = true;
                }
            }

            if (hasProgress && presenter != null) {
                presenter.SetProgress(progress);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator LoadScenes(NetResult result) {
        yield return Service.Run(DownloadSceneMeta(result));
        if (result.IsFailed()) {
            logger.LogError("Can not load scene meta.");
            yield break;
        }

#if UNITY_EDITOR && BOOT_BUNDLE_EDIT
        yield break;
#endif

        // init count
        doneCount = 0;
        string text = Service.sb.Get("loading.status.bundle.load.scene");
        if (presenter != null) {
            presenter.SetDescription(string.Format(text, 0));
        }
    
        successSceneWorkerCount = 0;
        failedSceneWorkerCount = 0;
        for (int index = 0; index<worker; index++) {
            Service.Run(LoadSceneWorker(index));
        }

        // waiting load
        while (true) {
            yield return new WaitForSeconds(0.1f);
            if (successSceneWorkerCount + failedSceneWorkerCount < worker) {
                continue;
            }
            break;
        }

        // result check
        if (failedSceneWorkerCount > 0) {
            result.SetSuccess(false);
        }
        else {
            result.SetSuccess(true);
            PersistenceUtil.SaveTextFile(remoteScenesMetaFilePath, remoteScenesMeta);
        }
    }

    IEnumerator LoadSceneWorker(int offset) {
        int index = 0;

        foreach(string name in remoteScenes.Keys) {
            if ((index % worker) != offset) {
                index++;
                continue;
            }

            uint crc = remoteScenes[name];
            string url = cdnScenes + name + ".unity3d";

            // local cache check
            if (localScenes.ContainsKey(name) == false || crc != localScenes[name]) {
                using (WWW www = WWW.LoadFromCacheOrDownload(url, Version.remoteBundleVersion, crc)) {
                    wwwRef[offset] = new WeakReference<WWW>(www);
                    yield return www;
                    wwwRef[offset] = null;
                    if (presenter != null) {
                        presenter.SetProgress(1);
                    }

                    if (www.error != null && www.assetBundle == null) {
                        failedSceneWorkerCount++;
                        logger.LogError(www.error);
                        yield break;
                    }
                }
            }

            doneCount++;
            index++;

            string text = Service.sb.Get("loading.status.bundle.load.scene");
            int percent = Mathf.RoundToInt((doneCount / (float)remoteScenes.Count) * 100);
            percent = Mathf.Min(100, percent);
            if (presenter != null) {
                presenter.SetDescription(string.Format(text, percent));
                presenter.SetProgress(1);
            }
        }

        successSceneWorkerCount++;
    }

    IEnumerator LoadResources(NetResult result) {
        yield return Service.Run(DownloadResourceMeta(result));
        if (result.IsFailed()) {
            logger.LogError("Can not load resources meta.");
            yield break;
        }

#if UNITY_EDITOR && BOOT_BUNDLE_EDIT
        yield break;
#endif

        // init count
        doneCount = 0;
        string text = Service.sb.Get("loading.status.bundle.load.resource");
        if (presenter != null) {
            presenter.SetDescription(string.Format(text, 0));
        }

        successResourceWorkerCount = 0;
        failedResourceWorkerCount = 0;
        for (int index = 0; index<worker; index++) {
            Service.Run(LoadResourceWorker(index));
        }

        // waiting load
        while (true) {
            yield return new WaitForSeconds(0.1f);
            if (successResourceWorkerCount + failedResourceWorkerCount < worker) {
                continue;
            }
            break;
        }

        // result check
        if (failedSceneWorkerCount > 0) {
            result.SetSuccess(false);
        }
        else {
            result.SetSuccess(true);
            PersistenceUtil.SaveTextFile(remoteResourcesMetaFilePath, remoteResourcesMeta);
        }
    }

    IEnumerator LoadResourceWorker(int offset) {
        int index = 0;
        foreach(string name in remoteResources.Keys) {
            if ((index % worker) != offset) {
                index++;
                continue;
            }

            uint crc = remoteResources[name];
            string url = cdnResources + name + ".unity3d";

            // local cache check
            if (localResources.ContainsKey(name) == false || crc != localResources[name]) {
                using (WWW www = WWW.LoadFromCacheOrDownload(url, Version.remoteBundleVersion, crc)) {
                    wwwRef[offset] = new WeakReference<WWW>(www);
                    yield return www;
                    wwwRef[offset] = null;
                    if (presenter != null) {
                        presenter.SetProgress(1);
                    }

                    if (www.error != null && www.assetBundle == null) {
                        failedResourceWorkerCount++;
                        logger.LogError(name + ":" + www.error);
                        yield break;        
                    }
                }
            }

            doneCount++;
            index++;

            string text = Service.sb.Get("loading.status.bundle.load.resource");
            int percent = Mathf.RoundToInt((doneCount / (float)remoteResources.Count) * 100);
            percent = Mathf.Min(100, percent);
            if (presenter != null) {
                presenter.SetDescription(string.Format(text, percent));
            }
        }

        successResourceWorkerCount++;
    }


    IEnumerator DownloadResourceMeta(NetResult result) {
        // get local
        string localMeta = PersistenceUtil.LoadTextFile(remoteResourcesMetaFilePath);
        if (localMeta.Length > 0) {
            try {
                localResources = JsonMapper.ToObject<Dictionary<string, uint>>(localMeta);
            }
            catch (Exception e) {
                logger.LogWarning(e.ToString());
            }
        }

        // get remote
        using (WWW www = new WWW(cdnResourcesMeta)) {
            yield return www;
            if (www.error != null) {
                result.SetSuccess(false);
                yield break;
            }

            try {
                remoteResources = JsonMapper.ToObject<Dictionary<string, uint>>(www.text);
                remoteResourcesMeta = www.text;
                result.SetSuccess(true);
            }
            catch (Exception e) {
                result.SetSuccess(false);
                logger.LogError(e.ToString());
            }
        } 
    }

    IEnumerator DownloadSceneMeta(NetResult result) {
        // get local
        string localMeta = PersistenceUtil.LoadTextFile(remoteScenesMetaFilePath);
        if (localMeta.Length > 0) {
            try {
                localScenes = JsonMapper.ToObject<Dictionary<string, uint>>(localMeta);
            }
            catch (Exception e) {
                logger.LogWarning(e.ToString());
            }
        }

        // get remote
        using (WWW www = new WWW(cdnScenesMeta)) {
            yield return www;
            if (www.error != null) {
                result.SetSuccess(false);
                yield break;
            }

            try {
                remoteScenes = JsonMapper.ToObject<Dictionary<string, uint>>(www.text);
                remoteScenesMeta = www.text;
                result.SetSuccess(true);
            }
            catch (Exception e) {
                result.SetSuccess(false);
                logger.LogError(e.ToString());
            }
        } 
    }

    public IEnumerator LoadAudioClip(string path, string fileType, OutResult<AudioClip> result) {
        string name = path;
        int index = name.LastIndexOf("/");
        if (index > 0) {
            name = name.Substring(index + 1);
        }

#if UNITY_EDITOR && BOOT_BUNDLE_EDIT
        string realPath = "Assets/Resources2Pack/" + path + "." + fileType;
        result.value = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(realPath);
        yield break;
#endif

        if (remoteResources.ContainsKey(name) == false) {
            logger.LogError("It's not a remote resource: " + path);
            yield break;
        }

        float startTime = Time.realtimeSinceStartup;
        while (nowLoading.Contains(path) == true) {
            yield return new WaitForEndOfFrame();
            float pastTime = Time.realtimeSinceStartup - startTime;
            if (pastTime > 3f) {
                break;
            }
        }

        if (nowLoading.Contains(path) == false) {
            nowLoading.Add(path);
        }

        uint crc = remoteResources[name];
        string url = cdnResources + name + ".unity3d";
        using (WWW www = WWW.LoadFromCacheOrDownload(url, Version.remoteBundleVersion, crc) ) {
            yield return www;
            if (www.error != null) {
                logger.LogError(www.error);
                nowLoading.Remove(path);
                yield break;
            }

            AssetBundle bundle = www.assetBundle;
            bundle.LoadAllAssets();
            AudioClip clip = bundle.LoadAsset(name, typeof(AudioClip)) as AudioClip;
            if (clip == null) {
                nowLoading.Remove(path);
                yield break;
            }
            result.value = clip;
            bundle.Unload(false);
        }
        nowLoading.Remove(path);
    }

    public IEnumerator LoadMaterial(string path, OutResult<Material> result) {
        string name = path;
        int index = name.LastIndexOf("/");
        if (index > 0) {
            name = name.Substring(index + 1);
        }

#if UNITY_EDITOR && BOOT_BUNDLE_EDIT
        string realPath = "Assets/Resources2Pack/" + path + ".mat";
        result.value = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(realPath);
        yield break;
#endif
        if (remoteResources.ContainsKey(name) == false) {
            logger.LogError("It's not a remote resource: " + path);
            yield break;
        }

        float startTime = Time.realtimeSinceStartup;
        while (nowLoading.Contains(path) == true) {
            yield return new WaitForEndOfFrame();
            float pastTime = Time.realtimeSinceStartup - startTime;
            if (pastTime > 3f) {
                break;
            }
        }

        if (nowLoading.Contains(path) == false) {
            nowLoading.Add(path);
        }

        uint crc = remoteResources[name];
        string url = cdnResources + name + ".unity3d";
        using (WWW www = WWW.LoadFromCacheOrDownload(url, Version.remoteBundleVersion, crc) ) {
            yield return www;
            if (www.error != null) {
                logger.LogError(www.error);
                nowLoading.Remove(path);
                yield break;
            }

            AssetBundle bundle = www.assetBundle;
            bundle.LoadAllAssets();
            Material mat = bundle.LoadAsset(name, typeof(Material)) as Material;
            if (mat == null) {
                nowLoading.Remove(path);
                yield break;
            }
            result.value = mat;
            bundle.Unload(false);
        }
        nowLoading.Remove(path);
    }

    public IEnumerator Instantiate(string path, OutResult<GameObject> result) {
        string name = path;
        int index = name.LastIndexOf("/");
        if (index > 0) {
            name = name.Substring(index + 1);
        }

#if UNITY_EDITOR && BOOT_BUNDLE_EDIT
        string realPath = "Assets/Resources2Pack/" + path + ".prefab";
        result.value = GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(realPath)) as GameObject;
        yield break;
#endif
        if (remoteResources.ContainsKey(name) == false) {
            logger.LogError("It's not a remote resource: " + path);
            yield break;
        }

        float startTime = Time.realtimeSinceStartup;
        while (nowLoading.Contains(path) == true) {
            yield return new WaitForEndOfFrame();
            float pastTime = Time.realtimeSinceStartup - startTime;
            if (pastTime > 3f) {
                break;
            }
        }

        if (nowLoading.Contains(path) == false) {
            nowLoading.Add(path);
        }

        uint crc = remoteResources[name];
        string url = cdnResources + name + ".unity3d";
        using (WWW www = WWW.LoadFromCacheOrDownload(url, Version.remoteBundleVersion, crc) ) {
            yield return www;
            if (www.error != null) {
                logger.LogError(www.error + ": " + name);
                nowLoading.Remove(path);
                yield break;
            }

            AssetBundle bundle = www.assetBundle;
            bundle.LoadAllAssets();
            GameObject obj = bundle.LoadAsset(name, typeof(GameObject)) as GameObject;
            if (obj == null) {
                nowLoading.Remove(path);
                yield break;
            }
            result.value = GameObject.Instantiate(obj) as GameObject;
            bundle.Unload(false);
        }

        nowLoading.Remove(path);
    }

    public IEnumerator Instantiate(string path, int count, OutResult<List<GameObject>> result) {
        string name = path;
        int index = name.LastIndexOf("/");
        if (index > 0) {
            name = name.Substring(index + 1);
        }

#if UNITY_EDITOR && BOOT_BUNDLE_EDIT
        string realPath = "Assets/Resources2Pack/" + path + ".prefab";
        GameObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(realPath);
        if (obj == null) {
            logger.LogError("Can not load:" + realPath);
            yield break;
        }

        result.value = new List<GameObject>();
        for (int i=0; i<count; i++) {
            GameObject item = GameObject.Instantiate(obj) as GameObject;
            result.value.Add(item);
        }
        yield break;
#endif

        if (remoteResources.ContainsKey(name) == false) {
            logger.LogError("It's not a remote resource: " + path);
            yield break;
        }

        while (nowLoading.Contains(path) == true) {
            yield return new WaitForEndOfFrame();
        }

        nowLoading.Add(path);
        uint crc = remoteResources[name];
        string url = cdnResources + name + ".unity3d";
        using (WWW www = WWW.LoadFromCacheOrDownload(url, Version.remoteBundleVersion, crc) ) {
            yield return www;
            if (www.error != null) {
                logger.LogError(www.error);
                nowLoading.Remove(path);
                yield break;
            }

            AssetBundle bundle = www.assetBundle;
            bundle.LoadAllAssets();

            GameObject res = bundle.LoadAsset(name, typeof(GameObject)) as GameObject;
            if (res == null) {
                nowLoading.Remove(path);
                yield break;
            }

            result.value = new List<GameObject>();
            for (int i=0; i<count; i++) {
                GameObject item = GameObject.Instantiate(res) as GameObject;
                result.value.Add(item);
            }

            bundle.Unload(false);
        }        
        nowLoading.Remove(path);
    }

    public bool IsRemoteResource(string path) {
        string name = path;
        int index = name.LastIndexOf("/");
        if (index > 0) {
            name = name.Substring(index + 1);
        }

        return remoteResources.ContainsKey(name);
    }

    public IEnumerator LoadLevel(string name) {
#if (BOOT_BUNDLE_EDIT && UNITY_EDITOR)
        Application.LoadLevel(name);
        yield break;
#endif

        if (IsRemoteScene(name) == false) {
            Application.LoadLevel(name);
            yield break;
        }

        uint crc = remoteScenes[name];
        string url = cdnScenes + name + ".unity3d";
        
        using (WWW www = WWW.LoadFromCacheOrDownload(url, Version.remoteBundleVersion, crc)) {
            yield return www;
            if (www.error != null) {
                yield break;        
            }

            AssetBundle bundle = www.assetBundle;
            bundle.LoadAllAssets();
            logger.Log("LoadLevel remote scene:" + name);

            Application.LoadLevel(name);
            yield return new WaitForEndOfFrame();
            bundle.Unload(false);
        }
    }

    public IEnumerator LoadLevelAdditiveAsync(string name) {
        string asset = name;

#if (BOOT_BUNDLE_EDIT && UNITY_EDITOR)
        {
            AsyncOperation async = Application.LoadLevelAdditiveAsync(name);
            yield return async;
            yield break;
        }
#endif
        if (IsRemoteScene(asset) == false) {
            AsyncOperation async = Application.LoadLevelAdditiveAsync(name);
            yield return async;
            yield break;
        }

        uint crc = remoteScenes[asset];
        string url = cdnScenes + asset + ".unity3d";

        using (WWW www = WWW.LoadFromCacheOrDownload(url, Version.remoteBundleVersion, crc)) {
            yield return www;
            if (www.error != null) {
                logger.LogError("Loading Scene failed:" + asset + " with:" + www.error.ToString());
                yield break;        
            }

            AssetBundle bundle = www.assetBundle;
            bundle.LoadAllAssets();
            logger.Log("LoadLevelAdditiveAsync remote scene:" + name);
            AsyncOperation async = Application.LoadLevelAdditiveAsync(name);
            yield return async;
            bundle.Unload(false);
        }
    }

    public bool IsRemoteScene(string scene) {
        return remoteScenes.ContainsKey(scene);
    }

    string GetPlatform() {
    #if UNITY_ANDROID
        return "AOS";
    #elif UNITY_IPHONE
        return "iOS";
    #endif
        return "unknown";
    }
}
