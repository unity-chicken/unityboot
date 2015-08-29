using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
 
public class CameraFade : SingletonGameObject<CameraFade> {   
	GUIStyle backgroundStyle = new GUIStyle();
	Texture2D fadeTexture;
	Color fromColor = Color.black;
	Color toColor = Color.black;
	int fadeGUIDepth = -1000;
	float times = 0;
	float duration = 1;
	bool run = false;
 
	public IEnumerator Flash(float duration) {
		Color blank = new Color(1, 1, 1, 0);
		Color white = new Color(1, 1, 1, 1);

		StartFade(blank, white, duration/2);
		while (this.run) {
			yield return new WaitForEndOfFrame();
		}

		StartFade(white, blank, duration/2);
		while (this.run) {
			yield return new WaitForEndOfFrame();
		}

		StopFade();
	}

	public IEnumerator FadeOut() {
		Color blank = new Color(0, 0, 0, 0);
		Color black = new Color(0, 0, 0, 1);

		float duration = 0.4f;
		StartFade(blank, black, duration);
		while (this.run) {
			yield return new WaitForEndOfFrame();
		}

		StopFade();
	}

	public IEnumerator FadeIn() {
		Color blank = new Color(0, 0, 0, 0);
		Color black = new Color(0, 0, 0, 1);

		float duration = 0.4f;
		StartFade(black, blank, duration);
		while (this.run) {
			yield return new WaitForEndOfFrame();
		}

		StopFade();
	}


	void Awake() {
		fadeTexture = new Texture2D(1, 1);		
		backgroundStyle.normal.background = fadeTexture;
	}
 
	void OnGUI() {   
		times += Time.deltaTime;
		times = Mathf.Min(times, duration);
		float rate = times / duration;
		Color color = Color.Lerp(fromColor, toColor, rate);

		SetScreenOverlayColor(color);
		GUI.depth = fadeGUIDepth;
		GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), fadeTexture, backgroundStyle);
		if (rate >= 1) {
			run = false;
		}
	}
 
	void SetScreenOverlayColor(Color color) {
		fadeTexture.SetPixel(0, 0, color);
		fadeTexture.Apply();
	}
 
	void StopFade() {
		this.gameObject.SetActive(false);
	}

	void StartFade(Color fromColor, Color toColor, float fadeDuration) {
		this.duration = fadeDuration;
		this.fromColor = fromColor;
		this.toColor = toColor;
		this.times = 0;
		this.gameObject.SetActive(true);

		if (fadeDuration <= 0.0f) {
			SetScreenOverlayColor(toColor);
			this.run = false;
		}
		else {
			SetScreenOverlayColor(fromColor);
			this.run = true;
		}
	}
}