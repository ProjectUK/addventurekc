using UnityEngine;
using System.Collections;
using Spine.Unity;

public class SPDrinkingCrow : SpineParticle {

	public override void Start ()
	{
		base.Start ();
		EventManager.Instance.AddListener<CoffeePowerEvent> (OnCoffeePowerEvent);
	}

	void OnCoffeePowerEvent(CoffeePowerEvent eve) {
		ShowAnimation ();
	}

}
