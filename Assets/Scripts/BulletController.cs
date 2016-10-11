using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {
	public Vector2 Speed = new Vector2(0,10);

	public bool AffectedByLevelSpeed;

	// Use this for initialization
	void Start () {
		EventManager.Instance.AddListener<HideBulletEvent> (OnBulletHideEvent);
	}

	// Update is called once per frame
	void Update () {

		if (AffectedByLevelSpeed) {
			transform.Translate (
				Speed.x * Time.deltaTime * Blackboard.Instance.BulletSpeedMultiplier,
				Speed.y * Time.deltaTime * Blackboard.Instance.BulletSpeedMultiplier, 0);
		} else {
			transform.Translate (
				Speed.x * Time.deltaTime,
				Speed.y * Time.deltaTime, 0);
		}


		// HARDCODED
		// TODO: Specify automatic out of screen?
		if (this.transform.position.y > 7 || this.transform.position.y < -7) {
			this.gameObject.SetActive (false);
		}
	}

	public void Explode() {
		Tinker.ParticleManager.Instance.Spawn ("SpikyExplosion", this.transform.position);
		this.gameObject.SetActive (false);
	}

	public void OnTriggerEnter2D(Collider2D coll) {
		if (coll.tag == "AreaBoundary") {
			this.gameObject.SetActive (false);
		}
	}

	public void OnBulletHideEvent(HideBulletEvent eve) {
		this.gameObject.SetActive (false);
	}
}
