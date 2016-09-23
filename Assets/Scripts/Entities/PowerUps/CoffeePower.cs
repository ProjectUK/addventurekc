using UnityEngine;
using System.Collections;

public class CoffeePower : PowerUp {

	public GameObject[] Images;
	public float WakeValue;

	void OnTriggerEnter2D(Collider2D coll) {
		// check hit player
		if (coll.tag == "Player") {
			EventManager.Instance.TriggerEvent (new ReceiveWakeEvent(WakeValue));
			EventManager.Instance.TriggerEvent (new CoffeePowerEvent());
			this.gameObject.SetActive (false);
		}
	}

	public override void Init ()
	{
		base.Init ();
		SetImage (Random.Range (0, Images.Length));
	}


	private void SetImage(int index) {
		if (index < Images.Length) {
			for (int i = 0; i < Images.Length; i++) {
				if (index == i) {
					Images [i].SetActive (true);
				} else {
					Images [i].SetActive (false);
				}
			}
		}
	}
}
