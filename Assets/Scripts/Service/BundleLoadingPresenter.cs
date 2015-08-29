using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface BundleLoadingPresenter {
	void SetDescription(string description);
	void SetProgress(float progress);
}