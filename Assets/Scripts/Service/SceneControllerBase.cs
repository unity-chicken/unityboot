using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SceneControllerBase : MonoBehaviour, SceneController {
    protected void InitController() {
        Service.sceneController = this;
        if (Application.loadedLevel != 0 && Service.ready == false) {
            Application.LoadLevel(0);
        }
    }

    public IEnumerator LoadLevel(string scene) {
        yield return StartCoroutine(CameraFade.instance.FadeOut());
        Time.timeScale = 1;
        Application.LoadLevel(scene);
    }
}
