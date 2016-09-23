using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SpriteRendererAscender : MonoBehaviour {

	public bool Ascend;
	public bool Descend;

	public string TargetSortingLayer;
	public bool ChangeSortingLayer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Ascend) {
			Ascend = false;
			DoAscend ();
		}

		if (Descend) {
			Descend = false;
			DoDescend ();
		}

		if (ChangeSortingLayer) {
			ChangeSortingLayer = false;
			DoChangeSortingLayer ();
		}
	}

	void DoAscend() {
		SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer> (true);
		foreach (SpriteRenderer sr in srs) {
			sr.sortingOrder++;
		}

	}

	void DoDescend() {
		SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer> (true);
		foreach (SpriteRenderer sr in srs) {
			sr.sortingOrder--;
		}
	}

	void DoChangeSortingLayer() {
		SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer> (true);
		foreach (SpriteRenderer sr in srs) {
			sr.sortingLayerName = TargetSortingLayer;
		}
	}
}
