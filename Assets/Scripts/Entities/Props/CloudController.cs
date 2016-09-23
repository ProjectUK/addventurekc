using UnityEngine;
using System.Collections;

public class CloudController : Entity {

	public Vector2 Speed;

	public GameObject[] Clouds;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (Speed * Time.deltaTime);
	}

	// Set the active cloud by index
	private void SetCloud(int index) {
		if (index < Clouds.Length) {
			for (int i = 0; i < Clouds.Length; i++) {
				if (index == i) {
					Clouds [i].SetActive (true);
				} else {
					Clouds [i].SetActive (false);
				}
			}
		}
	}

	// set faster cloud is always on top
	private void SetCloudLayerOrder() {
		for (int i = 0; i < Clouds.Length; i++) {
			SpriteRenderer sr = Clouds [i].GetComponent<SpriteRenderer> ();
			if (sr != null) {
				// +2? Because the range is from -1 to -5, so we make it 2 to -2
				sr.sortingOrder = ((int)(Speed.y + 3)) * 10;
			}
		}
	}
		
	#region implemented abstract members of Entity
	public override void Init ()
	{
		SetCloud (Random.Range (0, Clouds.Length));
		Speed = new Vector2 (0, Random.Range (-1f, -5f));
		SetCloudLayerOrder ();

	}
	#endregion

	public void OnTriggerEnter2D(Collider2D coll) {
		if (coll.tag == "CloudBoundary") {
			this.gameObject.SetActive (false);
		}
	}
}
