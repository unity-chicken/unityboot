using UnityEngine;
using System.Collections;

public interface SceneController {
    IEnumerator LoadLevel(string scene);
}
