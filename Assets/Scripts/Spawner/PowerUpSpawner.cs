using UnityEngine;
using System.Collections;

public class PowerUpSpawner : EntitySpawner {

	protected override void OnEntitySpawned (Entity e)
	{
		if (e != null) {
			MovingEntity me = e.GetComponent<MovingEntity> ();
			me.IsMoving = true;
		}
	}

}
