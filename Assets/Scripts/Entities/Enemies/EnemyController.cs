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
	public int MaxHealth = 1;
	public int CurrentHealth = 0;


	public bool IsMoving;
	// type of enemy to wait till dead
	public bool IsWaitTillDead;

	public delegate void _OnDead (EnemyController ec);
	public _OnDead OnDead;

	BoxCollider2D _Collider;

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

			HitPlayerBullet (coll);
		}
	}

	public void ResetHealth() {
		this.CurrentHealth = this.MaxHealth;
	}

	void HitPlayer(Collider2D coll) {

		AircraftController ac = coll.GetComponent<AircraftController> ();
		if (ac != null) {
			ac.Die ();
		}

		Explode ();
			
	}

	protected void Explode() {
		
		EventManager.Instance.TriggerEvent (new EnemyDeadEvent (EnemyType));

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

	void HitPlayerBullet(Collider2D coll) {

		this.CurrentHealth -= 1;

		if (EnemyType == AircraftType.BOSS) {
			AudioManager.Instance.Play ("hit_boss", false, 0.5f, 0.03f);
		} else {
			AudioManager.Instance.Play ("hit_small_villain", false, 0.5f, 0.03f);
		}

		if (this.CurrentHealth <= 0) {
			Explode ();
		}
	}
}
