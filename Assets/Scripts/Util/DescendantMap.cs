using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DescendantMap {
    Dictionary<string, Transform> map = new Dictionary<string, Transform>();

    public DescendantMap(MonoBehaviour mono) {
        Queue<Transform> list = new Queue<Transform>();

        for (int i=0; i<mono.transform.childCount; i++) {
            list.Enqueue(mono.transform.GetChild(i));
        }
        
        while (list.Count > 0) {
            Transform trans = list.Dequeue();
            string key = trans.gameObject.name;

            if (map.ContainsKey(key) == false) {
                map.Add(key, trans);
            }

            for (int j=0;j<trans.childCount;j++) {
                list.Enqueue(trans.GetChild(j));
            }
        }
    }

    public T Get<T>(string name) where T : Component {
        if (map.ContainsKey(name) == false) {
            return null;
        }

        return map[name].GetComponent<T>();
    }

    public GameObject Get(string name){
        if (map.ContainsKey(name) == false) {
            return null;
        }

        return map[name].gameObject;
    }
}