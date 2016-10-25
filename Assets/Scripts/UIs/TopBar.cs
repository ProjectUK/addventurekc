using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TopBar : MonoBehaviour {

	[SerializeField] Text NameText;

	// Use this for initialization
	void Start () {
		EventManager.Instance.AddListener<StartGameEvent> (OnStartGameEvent);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnStartGameEvent(StartGameEvent eve) {
		if (eve.PlayerInitial != null && eve.PlayerInitial != "") {
			NameText.text = eve.PlayerInitial;
		}

	}
}
