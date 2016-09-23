using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyShootingController : BasicEnemyController{

	[Header("Gun")]
	public GunModel[] Guns;


	GameObjectPool BulletPool;

	List<Coroutine> _ShootingRoutine = new List<Coroutine>();

	public override void Init ()
	{
		StartShooting ();
		ResetHealth ();
	}

	public void StartShooting() {
		BulletPool = PoolManager.Instance.GetPool ("EnemyBulletPool");

		// reset previous routines
		for (int i = 0; i < _ShootingRoutine.Count; i++) {
			StopCoroutine (_ShootingRoutine [i]);
		}

		for (int i = 0; i < Guns.Length; i++) {

			GunModel currentGun = Guns [i];

			Coroutine shootRoutine = StartCoroutine (UpdateShoot (currentGun));
			_ShootingRoutine.Add (shootRoutine);
		}

	}

	IEnumerator UpdateShoot(GunModel currentGunModel) {
		yield return null;
		while (true) {

			Shoot (currentGunModel);

			float shootingDelay = currentGunModel.ShootingDelay;

			if (Blackboard.Instance.BulletSpawnSpeedMultiplier > 0) {
				shootingDelay = currentGunModel.ShootingDelay / Blackboard.Instance.BulletSpawnSpeedMultiplier;
				yield return new WaitForSeconds (shootingDelay);
			}
			yield return null;

		}
	}

	void Shoot(GunModel gunModel) {
		GameObject bullet = BulletPool.Get ();

		if (bullet != null) {
			bullet.transform.position = gunModel.GunPosition.position;

			BulletController bc = bullet.GetComponent<BulletController> ();
			if (bc != null)
				bc.Speed = gunModel.BulletSpeed;

			bullet.SetActive (true);
		}
	}
}
