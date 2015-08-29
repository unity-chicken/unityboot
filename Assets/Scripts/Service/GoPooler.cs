using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public interface GoPooler {
	float prepareTime { get; }

	void Prepare(string path, int count);
	void Return(GoItem item);
	void Return(GoItem item, float delay);
	T Get<T>(string path, GameObject parent) where T : GoItem;
    IEnumerator GetRemote<T>(string path, GameObject parent, OutResult<T> result) where T : GoItem;
}