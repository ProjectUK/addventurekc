using UnityEngine;
using System.Collections;
using Spine.Unity;
using System.Collections.Generic;

public class ShadowAircraft : MonoBehaviour {

	[SerializeField] Animator _AirplaneAnimator;
	[SerializeField] SkeletonAnimator _SkeletonAnimator;
	[SerializeField] AircraftController _ParentController;

	public float Opacity = 0.2f;

	GameObjectPool BulletPool;

	float _BulletDamage = 1;
	float _PurchasedBulletDamageMultiplier = 1;

	[Header("Gun")]
	public GunModel[] Guns;
	List<Coroutine> _ShootingRoutine = new List<Coroutine>();

	// Use this for initialization
	void Start () {
		BulletPool = PoolManager.Instance.CreatePool ("BulletPool", Resources.Load ("Prefabs/Bullets/PlayerBullet") as GameObject, 20, true);
	}
	
	// Update is called once per frame
	void Update () {
		if (_SkeletonAnimator.skeleton.a != Opacity) {
			_SkeletonAnimator.skeleton.a = Opacity;
		}

		UpdateAnimator ();
	}

	void OnEnable() {
		StartShooting ();
	}


	void UpdateAnimator() {
		if (_ParentController._CurrentMovementState == AircraftController.AircraftMovementState.IDLE) {
			_AirplaneAnimator.SetBool ("idle", true);
			_AirplaneAnimator.SetBool ("turn_right", false);
			_AirplaneAnimator.SetBool ("turn_left", false);
		} else if (_ParentController._CurrentMovementState == AircraftController.AircraftMovementState.TILT_RIGHT) {
			_AirplaneAnimator.SetBool ("idle", false);
			_AirplaneAnimator.SetBool ("turn_right", true);
			_AirplaneAnimator.SetBool ("turn_left", false);
		}else if (_ParentController._CurrentMovementState == AircraftController.AircraftMovementState.TILT_LEFT) {
			_AirplaneAnimator.SetBool ("idle", false);
			_AirplaneAnimator.SetBool ("turn_right", false);
			_AirplaneAnimator.SetBool ("turn_left", true);
		}
	}

	#region Guns

	void Shoot(GunModel gunModel) {

		if (!gunModel.Active)
			return;

		GameObject bullet = BulletPool.Get ();

		if (bullet != null) {
			bullet.transform.position = gunModel.GunPosition.position;

			BulletController bc = bullet.GetComponent<BulletController> ();
			if (bc != null) {
				bc.Speed = gunModel.BulletSpeed;
				bc.Damage = _BulletDamage * _PurchasedBulletDamageMultiplier;
			}

			bullet.SetActive (true);
		}
	}

	public void StartShooting () {
		// reset previous routines
		StopShooting();

		for (int i = 0; i < Guns.Length; i++) {

			GunModel currentGun = Guns [i];
			Coroutine shootRoutine = StartCoroutine (UpdateShoot (currentGun));
			_ShootingRoutine.Add (shootRoutine);
		}
	}

	public void StopShooting() {
		for (int i = 0; i < _ShootingRoutine.Count; i++) {			
			StopCoroutine (_ShootingRoutine [i]);
		}
		_ShootingRoutine.Clear ();
	}

	IEnumerator UpdateShoot(GunModel currentGunModel) {
		yield return null;
		while (true) {

			// shooting boost
			if (currentGunModel.BoostTime > 0) {
				float elapsedTime = Time.realtimeSinceStartup;
				while (currentGunModel.BoostTime > 0) {
					elapsedTime = Time.realtimeSinceStartup;
					Shoot (currentGunModel);
					yield return new WaitForSeconds (currentGunModel.BoostDelay);
					currentGunModel.BoostTime -= Time.realtimeSinceStartup - elapsedTime;
				}
			}

			Shoot (currentGunModel);
			yield return new WaitForSeconds (currentGunModel.ShootingDelay);
		}
	}

	void SetGunDelayTime(float delayTime) {
		for (int i = 0; i < Guns.Length; i++) {
			GunModel currentGun = Guns [i];
			currentGun.ShootingDelay = delayTime;
		}
	}

	void SetBulletSpeed(Vector2 bulletSpeed) {
		for (int i = 0; i < Guns.Length; i++) {
			GunModel currentGun = Guns [i];
			currentGun.BulletSpeed = new Vector2 (currentGun.BulletSpeed.x, bulletSpeed.y);
			//				bulletSpeed;
		}
	}

	void SetGunActive(string GunID, bool isActive) {
		for (int i = 0; i < Guns.Length; i++) {
			GunModel currentGun = Guns [i];
			if (Guns[i].Gun_Id == GunID) {
				currentGun.Active = isActive;
				return;
			}
		}
	}

	#endregion
}
