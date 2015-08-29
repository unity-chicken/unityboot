using UnityEngine;
using System.Collections;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections.Generic;


public class StringBundleServiceImpl : Singleton<StringBundleServiceImpl>, StringBundleService {
	private string stringBundlefilePath = "StringBundles/StringBundle";
	private Dictionary<string, Dictionary<string, string>> bundles = new Dictionary<string, Dictionary<string, string>>();
	private Dictionary<string, HashSet<string>> questions = new Dictionary<string, HashSet<string>>();
    private HashSet<string> supportLangs = new HashSet<string>();
    private string defaultLang = "en";
	
	// properties
	//-------------------------------------------------------------------------
	public string language { 
		get { 
			string lang = GetSystemLanguage(Application.systemLanguage); 
            if (supportLangs.Contains(lang) == true) {
				return lang;
			}

			return defaultLang;
		}
	}

	// apis
	//-------------------------------------------------------------------------
	public void Initialize(string defaultLang, string[] supportLangs) {
        this.defaultLang = defaultLang;
        foreach (string lang in supportLangs) {
            this.supportLangs.Add(lang);
        }

		LoadFromFile();
#if UNITY_EDITOR		
		ReportUntranslated();
		ReportQuestions();
#endif		
	}

	public string Get(string key) {
		if (bundles.ContainsKey(key) == false ){
			return key;
		}

		var container = bundles[key];
		if (container.ContainsKey(language) == true) {
			return container[language];
		}

		// default language1
		if (container.ContainsKey("en") == true) {
			return container["en"];
		}

		// default language2
		if (container.ContainsKey("ko") == true) {
#if UNITY_EDITOR			
			return "T@" + container["ko"];
#else
			return container["ko"];
#endif
		}
		
		return key;
	}

	public string GetWithEeGa(string key) {
		return GetWithEeGaValue(Get(key));
	}

	public string GetWithEeGaValue(string value) {
		if (language != "ko") {
			return value;
		}

		if (HasJong(value)) {
			return value + Get("korean.ee");
		}

		return value + Get("korean.ga");
	}


	public string GetWithEulReulValue(string value) {
		if (language != "ko") {
			return value;
		}

		if (HasJong(value)) {
			return value + Get("korean.eul");
		}

		return value + Get("korean.reul");
	}

	public string GetWithRoEuroValue(string value) {
		if (language != "ko") {
			return value;
		}

		int jong = GetJong(value);
		if (jong <= 0 || jong == 8) {
			return value + Get("korean.ro");
		}

		return value + Get("korean.euro");
	}

	public string GetWithEulReul(string key) {
		return GetWithEulReulValue(Get(key));
	}

	public string GetWithRoEuro(string key) {
		return GetWithRoEuroValue(Get(key));
	}

	public bool HasJong(string text) {
		return GetJong(text) > 0;
	}

	private int GetJong(string text) {
		if (text.Length > 0) {
			char last = text[text.Length - 1];
			int hangul = last - 0xAC00;
			return (hangul % 0x001C);
		}

		return 0;
	}


	// private methods
	//-------------------------------------------------------------------------
	private void AddString(string key, string lang, string value) {
		if (bundles.ContainsKey(key) == false) {
			bundles.Add(key, new Dictionary<string, string>());
		}

		var container = bundles[key];
		if (container.ContainsKey(lang) == false) {
			container.Add(lang, value);
		}
		else {
			container[lang] = value;
			//Debug.LogError(key + ", " + value);
		}
	}

	private void AddQuestion(string key, string lang) {
		if (questions.ContainsKey(key) == false) {
			questions.Add(key, new HashSet<string>());
		}

		var container = questions[key];
		if (container.Contains(lang) == false) {
			container.Add(lang);
		}
	}


	private void LoadFromFile() {
		string path = stringBundlefilePath;
		TextAsset textAsset = (TextAsset) Resources.Load(path, typeof(TextAsset));
		if (textAsset == null) {
			Debug.LogError("Can not load string bundle.");
			return;
		}
		
		string key = "";
		var txtArr = textAsset.text.Split('\n');
		foreach (string line in txtArr) {
			if (line.Length == 0 || line[0] == '#') {
				continue;
			}

			string trimedLine = line;
			trimedLine = line.Trim();
			trimedLine = trimedLine.Replace("\r","");
			if (trimedLine.Length == 0) {
				continue;
			}

			bool question = false;
			int keyOffset = 0;
			if (trimedLine[0] == '?') {
				question = true;
				keyOffset = 1;
			}

			if (trimedLine.Length >= 5 && trimedLine[0+keyOffset] == '|' && trimedLine[3+keyOffset] == '|') {
				string lang = trimedLine.Substring(1+keyOffset, 2);
				string value = trimedLine.Substring(4+keyOffset);

				value = value.Trim();
				value = value.Replace("\\n","\n");
				if (value.Length > 0) {
					AddString(key, lang, value);

					if (question == true) {
						AddQuestion(key, lang);
					}
				}
			}
			else {
				key = trimedLine;
			}
		}
	}

	private void ReportUntranslated() {
		StringBuilder sb = new StringBuilder();

		int count = 0;
		foreach (string key in bundles.Keys) {
			var container = bundles[key];
			if (container.ContainsKey("en") == false) {
				sb.Append(key);
				sb.Append("\n");

				foreach (string lang in container.Keys) {
					sb.Append("	|");
					sb.Append(lang);
					sb.Append("| ");
					sb.Append(container[lang].Replace("\n", "\\n"));
					sb.Append("\n");
				}

				sb.Append("	|en| \n");
				count++;
			}
		}

		string path = Application.persistentDataPath + "/UntranslatedStringBundle.txt";
		PersistenceUtil.SaveTextFile(path, sb.ToString());

		if (count > 0) {
			Debug.LogError("Untranslated strings: " + count);
		}
	}

	private void ReportQuestions() {
		StringBuilder sb = new StringBuilder();

		int count = 0;
		foreach (string key in bundles.Keys) {
			var container = bundles[key];
			if (container.ContainsKey("en") == true && container.ContainsKey("ko") == true) {
				if (questions.ContainsKey(key) == false) {
					continue;
				}

				var question = questions[key];
				if (question.Contains("en") == false) {
					continue;
				}

				sb.Append(key);
				sb.Append("\t");

				sb.Append("|ko| ");
				sb.Append(container["ko"].Replace("\n", "\\n"));
				sb.Append("\t");
				sb.Append("|en| ");
				sb.Append(container["en"].Replace("\n", "\\n"));
				sb.Append("\n");

				count++;
			}
		}

		string path = Application.persistentDataPath + "/QuestionStringBundle.txt";
		PersistenceUtil.SaveTextFile(path, sb.ToString());

		if (count > 0) {
			Debug.LogError("Question strings: " + count);
		}
	}

    public static string GetSystemLanguage() {
        return GetSystemLanguage(Application.systemLanguage);
    }

    public static string GetSystemLanguage(SystemLanguage language) {
        switch (language) {
            case SystemLanguage.Afrikaans: return "af";
            case SystemLanguage.Arabic: return "ar";
            case SystemLanguage.Basque: return "eu";
            case SystemLanguage.Belarusian: return "be";
            case SystemLanguage.Bulgarian: return "bg";
            case SystemLanguage.Catalan: return "ca";
            case SystemLanguage.Chinese: return "zh";
            case SystemLanguage.Czech: return "cs";
            case SystemLanguage.Danish: return "da";
            case SystemLanguage.Dutch: return "nl";
            case SystemLanguage.English: return "en";
            case SystemLanguage.Estonian: return "et";
            case SystemLanguage.Faroese: return "fo";
            case SystemLanguage.Finnish: return "fi";
            case SystemLanguage.French: return "fr";
            case SystemLanguage.German: return "de";
            case SystemLanguage.Greek: return "el";
            case SystemLanguage.Hebrew: return "he";
            case SystemLanguage.Icelandic: return "is";
            case SystemLanguage.Indonesian: return "id";
            case SystemLanguage.Japanese: return "ja";
            case SystemLanguage.Korean: return "ko";
            case SystemLanguage.Latvian: return "lv";
            case SystemLanguage.Lithuanian: return "lt";
            case SystemLanguage.Norwegian: return "no";
            case SystemLanguage.Polish: return "pl";
            case SystemLanguage.Portuguese: return "pt";
            case SystemLanguage.Romanian: return "ro";
            case SystemLanguage.Russian: return "ru";
            case SystemLanguage.SerboCroatian: return "hr";
            case SystemLanguage.Slovak: return "sk";
            case SystemLanguage.Slovenian: return "sl";
            case SystemLanguage.Spanish: return "es";
            case SystemLanguage.Swedish: return "sv";
            case SystemLanguage.Thai: return "th";
            case SystemLanguage.Turkish: return "tr";
            case SystemLanguage.Ukrainian: return "uk";
            case SystemLanguage.Vietnamese: return "vi";
            case SystemLanguage.Hungarian: return "hu";
            case SystemLanguage.Unknown: return "en";
        }

        return "en";
    }
}
