using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class AudioManagerClip : MonoBehaviour {


	public AudioSource audioSource;
	public AudioManager.AudioType audioType;

	Coroutine FadeRoutine;

//	public float Volume;

	public void Start() {
		audioSource = GetComponent<AudioSource>();
	}

	public void Update() {
//		if (IsMute) {
//			GetAudioSource ().volume = 0;	
//		} else {
//			GetAudioSource ().volume = Volume;
//		}
	}

	public void SetVolume(float volume) {
//		if (FadeRoutine != null) {
//			StopCoroutine (FadeRoutine);
//			FadeRoutine = null;
//		}
		GetAudioSource().volume = volume;
	}

	public float GetVolume() {
//		return GetComponent<AudioSource> ().volume;
//		return this.Volume;
		return GetAudioSource().volume;
	}

	public void Fade(float endValue, float time) {

		if (gameObject.activeInHierarchy) {
			if (time <= 0) {
				SetVolume (endValue);
				return;
			}
			
			if (FadeRoutine != null) {
				StopCoroutine (FadeRoutine);
				FadeRoutine = null;
			}
			float currentVolume = GetVolume ();
			FadeRoutine = StartCoroutine (IFadeRoutine(currentVolume, endValue, time));
		}

	}

	IEnumerator IFadeRoutine(float startValue, float endValue, float time) {
		float currentValue = startValue;
		float t = 0;
		float currentTime = 0;
		while (t < 1) {
			currentTime += IndieTime.Instance.deltaTime;
			t = currentTime / time;
			currentValue = Mathf.Lerp (startValue, endValue, t);
			SetVolume (currentValue);
			yield return null;
		}

		// disable mute objects
//		if (endValue <= 0) {
//			this.gameObject.SetActive (false);
//		}

		yield return null;
	}

	public AudioSource GetAudioSource() {
		if (this.audioSource == null) {
			this.audioSource = GetComponent<AudioSource> ();
		}
		return this.audioSource;
	}

	public void Mute() {
		this.GetAudioSource ().mute = true;
	}

	public void Unmute() {
		this.GetAudioSource ().mute = false;
	}
	
}

