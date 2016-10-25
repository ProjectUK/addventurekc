using UnityEngine;
using System.Collections;

public class ShadowPower : PowerUp {

	public float BoostTime;

	void OnTriggerEnter2D(Collider2D coll) {
		// check hit player
		if (coll.tag == "Player") {
			AudioManager.Instance.Play ("rapid_bullet", false, 0.5f, 0f);
			EventManager.Instance.TriggerEvent (new ShadowAircraftPowerEvent (BoostTime));
			EventManager.Instance.TriggerEvent (new PowerUpNotification ("shadow"));
			this.gameObject.SetActive (false);
		}
	}

	public override void Init ()
	{
		base.Init ();
	}
}
