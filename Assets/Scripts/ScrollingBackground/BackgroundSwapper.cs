using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BackgroundSwapper : MonoBehaviour {

	public enum BGType{
		DESERT,
		DESERT_END,
		ROCKY_END,
		ROCKY_START,
		SEA,
		SHORE_START
	}

	[System.Serializable]
	public class BackgroundModel{
		public BackgroundController BGSprite;
		public BGType BackgroundType;
	}

	public BackgroundModel[] Backgrounds;

	public BGType CurrentType;
	private BGType _TmpCurrentType;

	SpriteRenderer _SpriteRenderer;
	

	[HideInInspector]
	public BackgroundBoundFinder Bounds;

	[HideInInspector]
	public BackgroundModel CurrentBackgroundModel;

	// Use this for initialization
	void Start () {
		_SpriteRenderer = GetComponent<SpriteRenderer>();
		Bounds = GetComponent<BackgroundBoundFinder> ();

		// initialize
		SetBackground(CurrentType);
		_TmpCurrentType = CurrentType;
	}
	
	// Update is called once per frame
	void Update () {
		SetBackground (CurrentType);
	}

	void SetBackground (BGType background) {

		// when there's changes in type
		if (_TmpCurrentType != CurrentType) {
			CurrentBackgroundModel = FindBackground (background);
			CurrentBackgroundModel.BGSprite.Init ();
			_TmpCurrentType = CurrentType;
		}

	}

	BackgroundModel FindBackground(BGType type) {
		
		BackgroundModel currentBGModel = null;
		BackgroundModel foundedModel = null;
		for (int i = 0; i < Backgrounds.Length; i++) {
			currentBGModel = Backgrounds [i];
			if (currentBGModel.BackgroundType == type) {
				currentBGModel.BGSprite.gameObject.SetActive (true);
				foundedModel = currentBGModel;
			} else {
				currentBGModel.BGSprite.gameObject.SetActive (false);
			}

		}

		return foundedModel;
	}

	public void Move(Vector3 distance) {
		this.gameObject.transform.Translate (distance);
	}
}
