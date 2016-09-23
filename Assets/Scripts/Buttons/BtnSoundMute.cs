using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BtnSoundMute : MonoBehaviour {

	public Image MuteImage;

	Button MyButton;

	public SoundMuteHandler.SoundType SoundType;

	// Use this for initialization
	void Start () {

		MyButton = GetComponent<Button> ();

		MyButton.onClick.AddListener (delegate {
			AudioManager.Instance.Play("button_click", false, 1f, 0f);
			ToggleMute();
			CheckMute();	
		});

		CheckMute ();
	}
	
	// Update is called once per frame
	void Update () {
		CheckMute ();
	}

	void ToggleMute() {
		switch (SoundType) {
		case SoundMuteHandler.SoundType.BGM:

			if (GameSaveManager.Instance.IsBGMPlay ()) {
//				SetAudioManagerVolume (SoundMuteHandler.SoundType.BGM, 0, 0.5f);
				GameSaveManager.Instance.SetBGM (false);

				AudioManager.Instance.Mute(AudioManager.AudioType.BGM);
				AudioManager.Instance.SetVolume (AudioManager.AudioType.BGM, 0, 0.1f);

				AudioManager.Instance.IsBgmPlay = false;

			} else {

//				SetAudioManagerVolume (SoundMuteHandler.SoundType.BGM, 0.5f, 0.5f);
				GameSaveManager.Instance.SetBGM (true);

				AudioManager.Instance.Unmute(AudioManager.AudioType.BGM);
				Debug.Log("PLAY!!!!" + BGMManager.Instance.CurrentBGM);
				AudioManager.Instance.Play (BGMManager.Instance.CurrentBGM, true, 1, 0f);
				AudioManager.Instance.SetVolume (BGMManager.Instance.CurrentBGM, 1, 0.2f);

				AudioManager.Instance.IsBgmPlay = true;

//				AudioManager.Instance.Play ("BGM", true, 1, 0.3f);
			}

			break;
		case SoundMuteHandler.SoundType.SFX:

			if (GameSaveManager.Instance.IsSFXPlay ()) {
				GameSaveManager.Instance.SetSFX (false);
//				SetAudioManagerVolume (SoundMuteHandler.SoundType.SFX, 0, 0.5f);
				AudioManager.Instance.Mute(AudioManager.AudioType.SFX);
				AudioManager.Instance.IsSfxPlay = false;
			} else {
				GameSaveManager.Instance.SetSFX (true);
//				SetAudioManagerVolume (SoundMuteHandler.SoundType.SFX, 0.5f, 0.5f);
				AudioManager.Instance.Unmute(AudioManager.AudioType.SFX);
				AudioManager.Instance.IsSfxPlay = true;
			}
			break;
		}
	}

	void CheckMute() {

		switch (SoundType) {
		case SoundMuteHandler.SoundType.BGM:

			if (GameSaveManager.Instance.IsBGMPlay ()) {
				MuteImage.enabled = false;
			} else {
				MuteImage.enabled = true;
			}

			break;
		case SoundMuteHandler.SoundType.SFX:

			if (GameSaveManager.Instance.IsSFXPlay ()) {
				MuteImage.enabled = false;
			} else {
				MuteImage.enabled = true;
			}
			break;
		}

	}

//	private void SetAudioManagerVolume(SoundMuteHandler.SoundType soundType, float volume, float fadeTime) {
//
//		switch (SoundType) {
//		case SoundMuteHandler.SoundType.BGM:
//			AudioManager.Instance.SetVolume (AudioManager.AudioType.BGM, volume, fadeTime);
//			break;
//		case SoundMuteHandler.SoundType.SFX:
//			AudioManager.Instance.SetVolume (AudioManager.AudioType.SFX, volume, fadeTime);
//			break;
//		}
//	
//
//	}

}
