using UnityEngine;
using System.Collections;

public class PageTurner : MonoBehaviour {

	public GameObject[] Pages;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void HideAll () {
		for (int i = 0; i <  Pages.Length; i++) {
			Pages [i].SetActive (false);
		}
	}

	public void ShowPage(int page) {
		// show specific page
		for (int i = 0; i <  Pages.Length; i++) {
			if (i == page) {
				Pages [i].SetActive (true);
			} else {
				Pages [i].SetActive (false);
			}
		}
	}
}
