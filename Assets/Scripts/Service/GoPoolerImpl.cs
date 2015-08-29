using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GoPoolerImpl : SingletonGameObject<GoPoolerImpl>, GoPooler {
    Dictionary<string, Stack> storedObjects = new Dictionary<string, Stack>();
    Dictionary<string, GameObject> storedPrefabs = new Dictionary<string, GameObject>();
    Logger logger = new Logger("[GoPooler]");
    #if BOOT_NGUI_SUPPORT
    UIRoot uiRoot = null;
    #endif

    public float prepareTime { get { return 0.2f; } }

    public T Get<T>(string path, GameObject parent) where T : GoItem {
        if (storedObjects.ContainsKey(path)) {
            Stack stack = storedObjects[path];
            GameObject stored = PopGameObjectFromStack(stack);
            if (stored != null) {
                SetReady(stored, parent);
                return stored.GetComponent<T>();
            }
        }

        GameObject newly = MakeLocal(path);
        if (newly == null) {
            return null;
        }

        SetReady(newly, parent);
        return newly.GetComponent<T>();
    }

    public IEnumerator GetRemote<T>(string path, GameObject parent, OutResult<T> result) where T : GoItem {
        if (storedObjects.ContainsKey(path)) {
            Stack stack = storedObjects[path];
            GameObject stored = PopGameObjectFromStack(stack);
            if (stored != null) {
                SetReady(stored, parent);
                result.value = stored.GetComponent<T>();
                yield break;
            }
        }

        OutResult<GameObject> res = new OutResult<GameObject>();
        yield return StartCoroutine(MakeRemote(path, res));
        GameObject newly = res.value;
        if (newly == null) {
            yield break;
        }

        SetReady(newly, parent);
        result.value = newly.GetComponent<T>();
    }    

    public void Return(GoItem item) {
        string path = item.resourcePath;
        if (path == null || path.Length == 0) {
            Destroy(item.gameObject);
            return;
        }

        if (item.gameObject.activeSelf == false) {
            return;
        }

        Stack stack = GetStack(path);
        stack.Push(new WeakReference(item.gameObject));
        item.OnGoingIntoPool();
        MatchParent(item);
        item.transform.localPosition = Vector3.up * 9999;
        item.gameObject.SetActive(false);
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void Return(GoItem item, float delay) {
        StartCoroutine(ReturnWithDelay(item, delay));
    }

    IEnumerator ReturnWithDelay(GoItem item, float delay) {
        yield return new WaitForSeconds(delay);
        Return(item);
    }

    public void Prepare(string path, int count) { 
        if (Service.bundle.IsRemoteResource(path)) {
            StartCoroutine(PrepareRemote(path, count));
        }
        else {
            PrepareLocal(path, count);
        }
    }
   
    void Awake() {
    #if BOOT_NGUI_SUPPORT
        uiRoot = GameObject.FindObjectOfType(typeof(UIRoot)) as UIRoot;
    #endif
    }

    GameObject MakeLocal(string path) {
        GameObject itemObj = null;

        GameObject obj;
        if (!storedPrefabs.TryGetValue(path, out obj) || obj == null)
        {
            obj = Resources.Load<GameObject>(path);
            storedPrefabs[path] = obj;
        }

        if (obj == null) {
            logger.LogError("Can not load resource:" + path);
            return null;
        }

        itemObj = Instantiate(obj) as GameObject;
        if (itemObj == null) {
            logger.LogError("Can not load GameObject:" + path);
            return null;
        }

        GoItem item = itemObj.GetComponent<GoItem>();
        if (item == null) {
            logger.LogError("Doesn't have GoItem:" + path);
        }

        item.resourcePath = path;
        MatchParent(item);
        item.transform.localScale = Vector3.one;
        return itemObj;
    }

    IEnumerator MakeRemote(string path, OutResult<GameObject> result) {
#if !UNITY_EDITOR
        if (Service.bundle.IsRemoteResource(path) == false) {
            logger.LogError("It is not a remote resource:" + path);
            yield break;
        }
#endif

        yield return StartCoroutine(Service.bundle.Instantiate(path, result));
        if (result.value == null) {
            yield break;
        }

        GameObject itemObj = result.value;
        GoItem item = itemObj.GetComponent<GoItem>();
        if (item == null) {
            logger.LogError("Doesn't have GoItem:" + path);
            yield break;
        }
        item.resourcePath = path;
    }

    Stack GetStack(string path) {
        Stack stack = null; 
        if (storedObjects.ContainsKey(path)) {
            stack = storedObjects[path];
        }
        else {
            stack = new Stack();
            storedObjects[path] = stack;
        }

        return stack;
    }

    void SetReady(GameObject itemObj, GameObject parent) {
        if (parent != null) {
            itemObj.transform.parent = parent.transform;
        }

        itemObj.transform.localPosition = Vector3.up * 9999;
        itemObj.SetActive(true);

        GoItem item = itemObj.GetComponent<GoItem>();
        if (item != null) {
            item.OnGettingOutPool();
        }
    }

    void PrepareLocal(string path, int count) { 
        for (int i=0; i<count; i++) {
            GameObject itemObj = MakeLocal(path);
            if (itemObj == null) {
                continue;
            }

            GoItem item = itemObj.GetComponent<GoItem>();
            if (item == null) {
                logger.LogError("Doesn't have GoItem:" + path);
            }

            MatchParent(item);
            item.transform.localPosition = Vector3.zero;
            itemObj.SetActive(true);
            item.resourcePath = path;
            item.Prepare();
            Return(item, prepareTime);
        }
    }

    IEnumerator PrepareRemote(string path, int count) { 
        OutResult<List<GameObject>> result = new OutResult<List<GameObject>>();
        yield return StartCoroutine(Service.bundle.Instantiate(path, count, result));

        for (int i=0; i<result.value.Count; i++) {
            GameObject itemObj = result.value[i];
            if (itemObj == null) {
                continue;
            }

            GoItem item = itemObj.GetComponent<GoItem>();
            if (item == null) {
                logger.LogError("Doesn't have GoItem:" + path);
            }

            MatchParent(item);
            item.transform.localPosition = Vector3.up * 9999;
            itemObj.SetActive(true);
            item.resourcePath = path;
            item.Prepare();
            Return(item, prepareTime);
        }
    }    

#if BOOT_NGUI_SUPPORT
    bool IsNGUIWidget(GoItem item) {
        return item.GetComponentInChildren<UIWidget>() != null;
    }

    void MatchParent(GoItem item) {
        if (IsNGUIWidget(item)) {
            item.transform.parent = uiRoot.transform;
        }
        else {
            item.transform.parent = transform;
        }
    }
#else
    void MatchParent(GoItem item) {
        item.transform.parent = transform;
    }
#endif

    GameObject PopGameObjectFromStack(Stack stack) {
        while (stack.Count > 0) {
            WeakReference item = (WeakReference)stack.Pop();

            if (item.Target != null) {
                return item.Target as GameObject;
            }
        }

        return null;
    }
}
