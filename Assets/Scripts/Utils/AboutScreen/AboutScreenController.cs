using UnityEngine;
using System.Collections;

public class AboutScreenController : MonoBehaviour {

	// Use this for initialization
	void Start () {

		// hide at start
		gameObject.SetActive (false);

	}
	
	public void Show() {
		AudioManager.Instance.Play("button_click", false, 1f, 0f);
		this.gameObject.SetActive (true);
	}

	public void Hide() {
		AudioManager.Instance.Play("button_click", false, 1f, 0f);
		this.gameObject.SetActive(false);
	}
}
