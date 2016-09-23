using UnityEngine;
using System.Collections;

public class PitstopSpawner : MonoBehaviour {

	PitstopController _PitstopController;


	// Use this for initialization
	void Start () {
		EventManager.Instance.AddListenerOnce<StartGameEvent> (OnStartGameEvent);
		Spawn ();	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Spawn() {
		if (_PitstopController == null) {
			GameObject pitstopObject = Instantiate(Resources.Load ("Prefabs/Buildings/Pitstop") as GameObject) ;
			_PitstopController = pitstopObject.GetComponent<PitstopController> ();
			_PitstopController.transform.position = this.transform.position;
			_PitstopController.transform.parent = this.transform;
		}

		int randomSeen = Random.Range (0, 5);
		if (randomSeen < 3) {
			_PitstopController.Init ();
			_PitstopController.gameObject.SetActive (true);
		} else {
			_PitstopController.gameObject.SetActive (false);
		}
	}

	void OnStartGameEvent(StartGameEvent eve) {
		_PitstopController.Init ();
	}

	void OnDestroy() {
		if (EventManager.Instance != null) 
			EventManager.Instance.RemoveListener<StartGameEvent> (OnStartGameEvent);
	}
}
