using UnityEngine;
using System.Collections;

public class MovingEntity : MonoBehaviour {

	public bool IsMoving = false;
	public float Speed = 2;

	// Update is called once per frame
	public virtual void Update () {
		if (IsMoving)
			Move ();
	}

	public virtual void Move() {
		transform.Translate (0, (Speed *Blackboard.Instance.EntitiesSpeedMultiplier) * Time.deltaTime, 0);

		// TODO: set hardcoded to automatic out of area?
		if (this.transform.position.y < -4.5) {
			this.gameObject.SetActive (false);
		}
	}
}
