using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemRemainings : MonoBehaviour {

	public int Value;
	public GameObject[] ObjectRemainings;

	// Update is called once per frame
	void Update () {
		for (int i = 0 ; i < ObjectRemainings.Length; i++) {
			if (i <= Value - 1) {
				ObjectRemainings [i].SetActive (true);
			} else {
				ObjectRemainings [i].SetActive (false);
			}
		}
	}
}
