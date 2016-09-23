using UnityEngine;
using System.Collections;

public class BulletboostPower : PowerUp {

	public float BoostTime;
	public float DelayEffect;

	void OnTriggerEnter2D(Collider2D coll) {
		// check hit player
		if (coll.tag == "Player") {
			AudioManager.Instance.Play ("rapid_bullet", false, 0.5f, 0f);
			EventManager.Instance.TriggerEvent (new BulletBoostEvent (BoostTime, DelayEffect));
			EventManager.Instance.TriggerEvent (new PowerUpNotification ("fasterbullet"));
			this.gameObject.SetActive (false);
		}
	}

	public override void Init ()
	{
		base.Init ();
	}
}
