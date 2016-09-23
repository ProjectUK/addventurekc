using UnityEngine;
using System.Collections;

public class BombPower : PowerUp {

	void OnTriggerEnter2D(Collider2D coll) {
		// check hit player
		if (coll.tag == "Player") {
			EventManager.Instance.TriggerEvent (new BombPowerEvent (this.transform.position));
			EventManager.Instance.TriggerEvent (new CameraShakeEvent (0.5f, 0.7f, 10, 3));
			EventManager.Instance.TriggerEvent (new PowerUpNotification ("bomb"));
			this.gameObject.SetActive (false);
			AudioManager.Instance.Play ("bomb", false, 1.5f, 0.1f);
		}
	}

	public override void Init ()
	{
		base.Init ();
	}
}
