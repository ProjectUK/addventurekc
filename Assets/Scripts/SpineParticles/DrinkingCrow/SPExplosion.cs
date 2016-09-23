using UnityEngine;
using System.Collections;

public class SPExplosion : SpineParticle {

	public override void Start ()
	{
		base.Start ();

		EventManager.Instance.AddListener<BombPowerEvent> (OnBombPowerEvent);
	}

	void OnBombPowerEvent(BombPowerEvent eve) {

		this.transform.position = eve.Position;
		ShowAnimation ();
	}
}
