using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface BundleDownloader {
	IEnumerator Initialize(BundleLoadingPresenter presenter, string cdnHost, NetResult result);
	IEnumerator LoadLevel(string name);
	IEnumerator LoadLevelAdditiveAsync(string name);
	IEnumerator Instantiate(string path, OutResult<GameObject> result);
	IEnumerator Instantiate(string path, int count, OutResult<List<GameObject>> result);
	IEnumerator LoadAudioClip(string path, string fileType, OutResult<AudioClip> result);
	IEnumerator LoadMaterial(string path, OutResult<Material> result);
	bool IsRemoteResource(string path);
	bool IsRemoteScene(string scene);
}
