using UnityEngine;
using System.Collections;

[System.Serializable]
public class GunModel {
	public string Gun_Id;
	public Transform GunPosition;
	public Vector2 BulletSpeed;
	public float ShootingDelay;

	public float BoostTime;
	public float BoostDelay;

	public bool Active;
}