using UnityEngine;
using System.Collections;

public class PitstopController : Entity {

	BoxCollider2D _Collider;

	// Use this for initialization
	void Start () {
		_Collider = GetComponent<BoxCollider2D> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D coll) {
		// check hit player
		if (coll.tag == "Player") {
			EventManager.Instance.TriggerEvent (new CafeActionStartedEvent ());
			_Collider.enabled = false;
		}
	}

	public override void Init ()
	{
		_Collider = GetComponent<BoxCollider2D> ();
		_Collider.enabled = true;
	}
}
