using UnityEngine;
using System.Collections;

public class BootDemoController : MonoBehaviour, BundleLoadingPresenter {
	Logger logger = new Logger("[IntroController]");

	void Awake() {
		// initialize string bundle
		Service.sb.Initialize("en", new string[] {"ko", "en"});

#if !UNITY_ANDROID && !UNITY_IOS
		Swith Platfrom to "Android" Or "iOS" to Test BundleDownload
#endif
	}

	IEnumerator Start() {
		// initialize setting service
		yield return Service.Run(Service.setting.Initialize());

		// initialize bundle downloader service
		NetResult result = new NetResult();
		yield return Service.Run(Service.bundle.Initialize(this, "http://raindays.net/unity/boot/", result));
		if (result.IsFailed()) {
			logger.LogError(Service.sb.Get("loading.status.bundle.failed"));
			yield break;
		}

		// load remote scene
		yield return Service.Run(Service.bundle.LoadLevelAdditiveAsync("BootStage1"));

		// load remote prefabs
		var cubeOut = new OutResult<BootCubePlayer>();
		yield return Service.Run(Service.goPooler.GetRemote<BootCubePlayer>("Prefabs/Cube", null, cubeOut));
		BootCubePlayer cube = cubeOut.value;
		cube.transform.localPosition = new Vector3(0, 3, 0);
	}

	// BundleLoadingPresenter interface implementation
	//-------------------------------------------------------------------------
	public void SetDescription(string description) {
		logger.Log(description);
	}

	public void SetProgress(float progress) {
		int percent = Mathf.RoundToInt((progress * 100f));
		logger.Log("DOWNLOAD:" + percent.ToString() + "%");
	}
}
