using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Service : SingletonGameObject<Service> {
	public static bool ready { get; set; }

	// Services
	//-------------------------------------------------------------------------
	public static SceneController sceneController { get; set; }
	public static BundleDownloader bundle { get { return BundleDownloaderImpl.instance; } }
	public static GoPooler goPooler { get { return GoPoolerImpl.instance; } }
	public static StringBundleService sb { get { return StringBundleServiceImpl.instance; } }
	public static EncryptionService encryption { get { return EncryptionServiceImpl.instance; } }
	public static SoundService sound { get { return SoundServiceImpl.instance; } }
	public static SettingService setting { get { return SettingServiceImpl.instance; } }
	public static TimeService time { get { return TimeServiceImpl.instance; } }

	// Callbacks
	//-------------------------------------------------------------------------
	void Awake() {
		DontDestroyOnLoad(gameObject);
		Logger.onSendLog += OnSendLog;
	}

	void OnSendLog(string msg, LogLevel logLevel) {
		Run(SendLogToServer(msg, logLevel));
	}

	// APIs
	//-------------------------------------------------------------------------
	public static Coroutine Run(IEnumerator iterationResult) {
		return Service.instance.StartCoroutine(iterationResult);
	}

	public IEnumerator SendLogToServer(string msg, LogLevel logLevel) {
		yield break;
	}
}