using UnityEngine;
using System.Collections;

[System.Serializable]
public class GunModel {
	public Transform GunPosition;
	public Vector2 BulletSpeed;
	public float ShootingDelay;

	public float BoostTime;
	public float BoostDelay;
}