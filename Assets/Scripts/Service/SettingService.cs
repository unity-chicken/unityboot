using UnityEngine;
using System.Collections;

public interface SettingService {
	Setting values { get; }

	IEnumerator Initialize();
	IEnumerator Sync();
}
