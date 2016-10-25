using UnityEngine;
using System.Collections;

public class BasicEnemyController : EnemyController{

	public virtual void Start() {
		EventManager.Instance.AddListenerOnce<BombPowerEvent> (OnBombPowerEvent);
	}

	// Update is called once per frame
	public virtual void Update () {
		if (IsMoving)
			Move ();
	}

	public override void Init ()
	{
		StartFeelingBullet ();
		if (IsMoving)
			Move ();
		ResetHealth ();
	}

	public virtual void Move() {
		transform.Translate (0, Speed * Time.deltaTime *Blackboard.Instance.EntitiesSpeedMultiplier, 0);

		// TODO: set hardcoded to automatic out of area?
		if (this.transform.position.y < -4.5) {
			this.gameObject.SetActive (false);
		}
	}

	public virtual void OnBombPowerEvent(BombPowerEvent eve) {
		if (gameObject.activeInHierarchy && CurrentHealth > 0) {
			Debug.Log ("Explode ah");
			Explode ();
		}
	}
}
