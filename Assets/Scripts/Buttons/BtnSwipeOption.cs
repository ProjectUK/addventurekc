using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BtnSwipeOption : MonoBehaviour {

	public Image TiltImage;

	Button MyButton;


	// Use this for initialization
	void Start () {

		// get current method
//		CheckSavedControlType ();


		MyButton = GetComponent<Button> ();

		MyButton.onClick.AddListener (delegate {
			AudioManager.Instance.Play("button_click", false, 1f, 0f);
//			ToggleControl();
//			CheckSavedControlType();	
		});
	}

//	void ToggleControl() {
//		if (GameSaveManager.Instance.IsControlSwipe ()) {
//			// currently swipe
//			// change it to tilt
//			GameSaveManager.Instance.SetControlSwipe (false);
//			EventManager.Instance.TriggerEvent (new AircraftSetControlSwipeEvent (false));
//		}else{
//			// currently tilt
//			// change it to swipe
//			GameSaveManager.Instance.SetControlSwipe (true);
//			EventManager.Instance.TriggerEvent (new AircraftSetControlSwipeEvent (true));
//		}
//	}
//
//	void CheckSavedControlType() {
//		if (GameSaveManager.Instance.IsControlSwipe ()) {
//			TiltImage.enabled = false;
//		} else {
//			TiltImage.enabled = true;
//		}
//	}
		

}
