﻿using UnityEngine;
using System.Collections;

public class LaserPower : PowerUp {

	public float BoostTime;

	void OnTriggerEnter2D(Collider2D coll) {
		// check hit player
		if (coll.tag == "Player") {
			AudioManager.Instance.Play ("rapid_bullet", false, 0.5f, 0f);
			EventManager.Instance.TriggerEvent (new LaserPowerEvent (BoostTime));
			EventManager.Instance.TriggerEvent (new PowerUpNotification ("laser"));
			this.gameObject.SetActive (false);
		}
	}

	public override void Init ()
	{
		base.Init ();
	}
}
