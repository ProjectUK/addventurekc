using UnityEngine;
using System.Collections;

public class SoundMuteHandler : MonoBehaviour {

	public enum SoundType{
		BGM = 0,
		SFX = 1
	}

//	public SoundType mySoundType;

//	public AudioSource myAudioSource;

	// Use this for initialization
	void Start () {

		if (GameSaveManager.Instance.IsBGMPlay ()) {
//			myAudioSource.mute = false;
			AudioManager.Instance.Unmute (AudioManager.AudioType.BGM);
		} else {
//			myAudioSource.mute = true;
			AudioManager.Instance.Mute (AudioManager.AudioType.BGM);
		}

		if (GameSaveManager.Instance.IsSFXPlay ()) {
			AudioManager.Instance.Unmute (AudioManager.AudioType.SFX);
		} else {
			AudioManager.Instance.Mute (AudioManager.AudioType.SFX);
		}


//		myAudioSource = GetComponent<AudioSource> ();
//
//		switch (mySoundType) {
//		case SoundType.BGM:
//
//
//			break;
//		case SoundType.SFX:
//
//			if (GameSaveManager.Instance.IsSFXPlay ()) {
//				myAudioSource.mute = false;
//			} else {
//				myAudioSource.mute = true;
//			}
//			break;
//		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
