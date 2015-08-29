using UnityEngine;
using System.Collections;

public interface SceneController {
	string startingScene { get; set; }

	void SwitchScene(string nextSceneName);
	void PreviousScene();
	void FadeIn();
}
