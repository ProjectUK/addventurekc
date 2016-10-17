using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class FollowWindowManager : MonoBehaviour {

	private static FollowWindowManager s_Instance = null;
	public static FollowWindowManager Instance {
		get{
			if (s_Instance == null) {
				s_Instance = GameObject.FindObjectOfType(typeof(FollowWindowManager)) as FollowWindowManager;

			}

			return s_Instance;
		}
	}


	public GameObject Container;
	public Button CloseButton;
	public Text HeaderText;

	[Header("Facebook Button")]
	public Button FacebookButton;
	public Text FacebookButtonText;
	[Header("Twitter Button")]
	public Button TwitterButton;
	public Text TwitterButtonText;
	[Header("Instagram Button")]
	public Button InstagramButton;
	public Text InstagramButtonText;

	// Use this for initialization
	void Start () {
		CloseButton.onClick.AddListener (()=>{
			Container.SetActive(false);
		});

		Container.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Show(string headerText, string fbButtonText, string twButtonText, string igButtonText, Action fbAction, Action twAction, Action igAction) {
		HeaderText.text 	= headerText;

		if (fbButtonText != "") {
			FacebookButtonText.text = fbButtonText;
			FacebookButton.gameObject.SetActive (true);
			FacebookButton.onClick.AddListener (() => {
				if (fbAction != null) {
					fbAction();
				}
			});
		}

		if (twButtonText != "") {
			TwitterButtonText.text = fbButtonText;
			TwitterButton.gameObject.SetActive (true);
			TwitterButton.onClick.AddListener (() => {
				if (twAction != null) {
					twAction();
				}
			});
		}

		if (igButtonText != "") {
			InstagramButtonText.text = fbButtonText;
			InstagramButton.gameObject.SetActive (true);
			InstagramButton.onClick.AddListener (() => {
				if (igAction != null) {
					igAction();
				}
			});
		}

		Container.SetActive (true);
	}

	public void Show(string headerText, string infoText) {
		HeaderText.text 	= headerText;

//		ConfirmationButton.gameObject.SetActive (false);

		Container.SetActive (true);
	}
}
