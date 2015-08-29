using UnityEngine;
using System.Collections;


public interface StringBundleService {
    string language { get; }

    IEnumerator Initialize(params string[] langs);
    string Get(string key);
    string GetWithEeGa(string key);
    string GetWithEulReul(string key);
    string GetWithRoEuro(string key);
    string GetWithEeGaValue(string key);
    string GetWithEulReulValue(string key);
    string GetWithRoEuroValue(string value);
    bool HasJong(string text);
}
