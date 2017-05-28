using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassFade : MonoBehaviour {

	private Color startColor;
	private float startAlpha, fadeDuration, fadeSpeed;
	private Renderer rend;

	void Start() {
		rend = GetComponent<Renderer>();
		startColor = rend.material.color;
		startAlpha = startColor.a;
		fadeDuration = 3;
		fadeSpeed = .5f;
	}
	void Update() {
		if (fadeDuration <= 0) {
			Destroy(gameObject);
		}
		rend.material.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Clamp(fadeDuration, 0, startAlpha));
		fadeDuration -= fadeSpeed * Time.deltaTime;
	}
}
