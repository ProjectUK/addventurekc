using UnityEngine;
using System.Collections;

public class ShieldPower : PowerUp {

	public float ShieldTime;

	void OnTriggerEnter2D(Collider2D coll) {
		// check hit player
		if (coll.tag == "Player") {
			EventManager.Instance.TriggerEvent (new ShieldPowerEvent (ShieldTime));
			EventManager.Instance.TriggerEvent (new PowerUpNotification ("shield"));
			this.gameObject.SetActive (false);
		}
	}

	public override void Init ()
	{
		base.Init ();
	}
}
