/**
 *	GameObjectPool
 *	---------------------
 *	Class to make pooling systems
 *	
 *	17 May 2016, @igrir
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectPool : MonoBehaviour {
	public GameObject Prefab;
	public int Count = 10;
	public bool IsExpanding = false;

	List<GameObject> _Pool = new List<GameObject>();

	public void Start() {
		FillPool (Count);
	}

	private void FillPool(int count) {
		for (int i = 0; i < count; i++) {
			GameObject newPrefab = Instantiate(Prefab) as GameObject;
			newPrefab.transform.parent = this.transform;
			newPrefab.SetActive(false);
			_Pool.Add(newPrefab);
		}
	}

	// Hide all entities
	public void HideAll() {
		for (int i = 0; i < _Pool.Count; i++) {
			_Pool [i].SetActive (false);
		}
	}

	public GameObject Get() {
		// search inactive gameobject from pool
		bool found = false;
		int i = 0;
		while (!found && i < _Pool.Count) {
			GameObject currentObj = _Pool [i];
			if (!currentObj.activeInHierarchy) {
				found = true;
				return currentObj;
			} else {
				i++;
			}
		}

		// expand the pool if it's expanding
		if (!found) {
			if (IsExpanding) {
				FillPool (1);
				// and return the last added object	
				return _Pool[_Pool.Count-1];
			}
		}

		// not found anything
		return null;
	}

}
