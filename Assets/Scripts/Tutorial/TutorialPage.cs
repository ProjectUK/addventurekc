using UnityEngine;
using System.Collections;

public class TutorialPage : MonoBehaviour {

	public int CurrentPage = 0;

	public bool IsShowing;

	[SerializeField] PageTurner _PageTurner;

	public delegate void _OnEnd();
	public _OnEnd OnEnd;

	// Use this for initialization
	void Start () {
		_PageTurner.HideAll ();
	}
	
	// Update is called once per frame
	void Update () {
		if (IsShowing) {
			if (Input.GetMouseButtonDown(0)) {
				if (CurrentPage < _PageTurner.Pages.Length) {
					_PageTurner.ShowPage (CurrentPage);
					CurrentPage++;
				} else {
					_PageTurner.HideAll ();
					IsShowing = false;
					if (OnEnd != null) {
						OnEnd ();
					}
				}
			}
		}
	}

	public void Show() {
		CurrentPage = 0;
		IsShowing = true;
		_PageTurner.ShowPage (CurrentPage);
	}

	public void Hide() {
		_PageTurner.HideAll ();
	}
}
