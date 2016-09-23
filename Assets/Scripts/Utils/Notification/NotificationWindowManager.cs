using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class NotificationWindowManager : MonoBehaviour {

	private static NotificationWindowManager s_Instance = null;
	public static NotificationWindowManager Instance {
		get{
			if (s_Instance == null) {
				s_Instance = GameObject.FindObjectOfType(typeof(NotificationWindowManager)) as NotificationWindowManager;

			}

			return s_Instance;
		}
	}


	public GameObject Container;
	public Button CloseButton;
	public Text HeaderText;
	public Text InfoText;
	public Button ConfirmationButton;
	public Text ConfirmationButtonText;

	// Use this for initialization
	void Start () {
		CloseButton.onClick.AddListener (()=>{
			Container.SetActive(false);
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Show(string headerText, string infoText, string buttonText, Action confirmAction) {
		HeaderText.text 	= headerText;
		InfoText.text 		= infoText;

		ConfirmationButtonText.text = buttonText;
		ConfirmationButton.gameObject.SetActive (true);
		ConfirmationButton.onClick.AddListener (() => {
			if (confirmAction != null) {
				confirmAction();
			}
		});

		Container.SetActive (true);
	}

	public void Show(string headerText, string infoText) {
		HeaderText.text 	= headerText;
		InfoText.text 		= infoText;

		ConfirmationButton.gameObject.SetActive (false);

		Container.SetActive (true);
	}
}
