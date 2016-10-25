using UnityEngine;
using System.Collections;

public abstract class EnemyController : Entity{

	public enum AircraftType
	{
		NORMAL = 0,
		SHOOTING = 1,
		PATTERN = 2,
		BOSS = 3
	}

	public string EnemyName;

	public float Speed = -10;
	public AircraftType EnemyType;
	public float MaxHealth = 1;
	public float CurrentHealth = 0;


	public bool IsMoving;
	// type of enemy to wait till dead
	public bool IsWaitTillDead;

	public delegate void _OnDead (EnemyController ec);
	public _OnDead OnDead;

	BoxCollider2D _Collider;

	// for delay bullet feel
	bool _BulletFeelActive;
	Coroutine FeelingBulletRoutine;

	// Use this for initialization
	public virtual void Start () {
		_Collider = GetComponent<BoxCollider2D> ();
	}



	void OnTriggerEnter2D(Collider2D coll) {
		// check hit player
		if (coll.tag == "Player") {
			HitPlayer (coll);
		}else if(coll.tag == "Bullet/Player") {
			BulletController bc = coll.GetComponent<BulletController> ();
			bc.Explode ();

			HitPlayerBullet (bc);
		}
	}

	void OnTriggerStay2D(Collider2D coll) {
		if(coll.tag == "Bullet/Player") {

			if (_BulletFeelActive) {
				BulletController bc = coll.GetComponent<BulletController> ();
				bc.Explode ();
				
				HitPlayerBullet (bc);

				_BulletFeelActive = false;
			}
		}
	}

	IEnumerator FeelingBullet() {
		while (true) {
			yield return new WaitForSeconds (0.5f);
			_BulletFeelActive = true;
		}
	}


	public void ResetHealth() {
		float totalMaxHealth = Mathf.Ceil(this.MaxHealth * Blackboard.Instance.EnemyHealthMultiplier);
		this.CurrentHealth = totalMaxHealth;
	}

	void HitPlayer(Collider2D coll) {

		AircraftController ac = coll.GetComponent<AircraftController> ();
		if (ac != null) {
			ac.Die ();
		}

		Explode ();
			
	}

	protected void Explode() {
		
		EventManager.Instance.TriggerEvent (new EnemyDeadEvent (this));

		if (EnemyType == AircraftType.BOSS) {
			EventManager.Instance.TriggerEvent (new BossDeadEvent ());
		}

		if (OnDead != null) {
			OnDead (this);
		}



		// throw particle
		Tinker.ParticleManager.Instance.Spawn("Puff", this.transform.position);

		this.CurrentHealth = 0;
		this.gameObject.SetActive (false);
	}

	void HitPlayerBullet(BulletController bc) {

		this.CurrentHealth -= bc.Damage;

		if (EnemyType == AircraftType.BOSS) {
			AudioManager.Instance.Play ("hit_boss", false, 0.5f, 0.03f);
		} else {
			AudioManager.Instance.Play ("hit_small_villain", false, 0.5f, 0.03f);
		}

		if (this.CurrentHealth <= 0) {
			Explode ();
		}
	}

	protected void StartFeelingBullet() {
		if (FeelingBulletRoutine != null) {
			StopCoroutine (FeelingBulletRoutine);
		}
		FeelingBulletRoutine = StartCoroutine (FeelingBullet ());
	}

}
