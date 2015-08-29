using UnityEngine;
using System.Collections;


public interface StringBundleService {
    string language { get; }

    void Initialize(string defaultLang, string[] supportLangs);
    string Get(string key);
    string GetWithEeGa(string key);
    string GetWithEulReul(string key);
    string GetWithRoEuro(string key);
    string GetWithEeGaValue(string key);
    string GetWithEulReulValue(string key);
    string GetWithRoEuroValue(string value);
    bool HasJong(string text);
}
