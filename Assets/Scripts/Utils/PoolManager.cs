using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour {
	
	private static PoolManager s_Instance = null;
	public static PoolManager Instance {
		get{
			if (s_Instance == null) {
				s_Instance = GameObject.FindObjectOfType(typeof(PoolManager)) as PoolManager;

			}
			return s_Instance;
		}
	}

	public Dictionary<string, GameObjectPool> Pools = new Dictionary<string, GameObjectPool> ();

	public GameObjectPool GetPool(string PoolName) {
		GameObjectPool returnPool;
		if(Pools.TryGetValue(PoolName, out returnPool))
			return returnPool;
		else
			return null;		
	}

	public GameObjectPool CreatePool(string PoolName, GameObject Prefab, int count, bool isExpanding) {
		if (Application.isPlaying) {
			// check no pool with this name
			GameObjectPool newPool = null;
			if (!Pools.TryGetValue (PoolName, out newPool)) {
				
				// create the new game object to hold
				GameObject go = new GameObject();
				go.name = PoolName;
				go.SetActive (false);
				go.transform.parent = this.transform;
				
				newPool = go.AddComponent<GameObjectPool> ();
				newPool.Count = count;
				newPool.IsExpanding = isExpanding;
				newPool.Prefab = Prefab;
				
				// activate gameobject to start pooling
				go.SetActive (true);
				
				Pools.Add (PoolName, newPool);
			}
			return newPool;
		}
		return null;

	}

}
