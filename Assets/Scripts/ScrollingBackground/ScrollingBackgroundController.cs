using UnityEngine;
using System.Collections;

public class ScrollingBackgroundController : MonoBehaviour {

	[System.Serializable]
	public class BGSeamModel
	{
		public BackgroundSwapper.BGType BGType;
		public BackgroundSwapper.BGType[] NextArea;
	}

	public enum SwapDirection{
		UP, DOWN, RIGHT, LEFT
	}

	public bool IsChanging;
	public BackgroundSwapper[] Backgrounds;
	public Vector3 MovingDistance;
	public Transform TopPosition;
	public Transform BottomPosition;

	[Header("Runtime variables")]
	public int CurrentTopIndex = 0;
	public int CurrentBottomIndex = 2;

	[Header("BG Seam Data")]
	public BGSeamModel[] Seams;

	[Header("Initial seam data")]
	public BackgroundSwapper.BGType InitialSeamModel;
	private BGSeamModel _CurrentSeamModel;

	// Use this for initialization
	void Start () {
		CurrentBottomIndex = Backgrounds.Length - 1;

		// set seam model data based on selected bgtype
		SetCurrentSeamModel (InitialSeamModel);

	}
	
	// Update is called once per frame
	void Update () {
		MoveBackgrounds ();
	}

	void MoveBackgrounds() {
		for (int i = 0; i < Backgrounds.Length; i++) {
			BackgroundSwapper bs = Backgrounds [i];

			bs.Move (MovingDistance*Time.deltaTime);

			if (MovingDistance.y > 0) {
				// moving up
				if (i == CurrentTopIndex) {
					if (bs.transform.position.y > TopPosition.transform.position.y) {
						MoveTopToBottom ();
					}
				}
			} else {
				// moving down
				if (i == CurrentBottomIndex) {
					if (bs.transform.position.y < BottomPosition.transform.position.y) {
						MoveBottomToTop ();
					}
				}
			}
		}
	}

	void MoveTopToBottom() {
		BackgroundBoundFinder topBound = Backgrounds [CurrentTopIndex].GetComponent<BackgroundBoundFinder> ();
		BackgroundBoundFinder bottomBound = Backgrounds [CurrentBottomIndex].GetComponent<BackgroundBoundFinder> ();

		topBound.transform.position = new Vector3 (
			topBound.transform.position.x,
			bottomBound.MinY + (topBound.transform.position.y - topBound.MaxY),
			topBound.transform.position.z
		);
		topBound.GetComponent<BackgroundController> ().Init ();


		if (CurrentTopIndex < Backgrounds.Length-1) {
			CurrentTopIndex++;
		} else {
			CurrentTopIndex = 0;
		}

		if (CurrentBottomIndex < Backgrounds.Length-1) {
			CurrentBottomIndex++;
		} else {
			CurrentBottomIndex = 0;
		}

		BackgroundSwapper bg = topBound.GetComponent<BackgroundSwapper> ();
		ChangeBG (bg);


	}

	void ChangeBG(BackgroundSwapper bg) {

		if (IsChanging) {
			BGSeamModel seam = _CurrentSeamModel;
			
			// get next seam
			int randomRange = Random.Range(0, seam.NextArea.Length);
			
			BackgroundSwapper.BGType nextSeam = seam.NextArea [randomRange];
			
			// change top bound content
			bg.CurrentType = nextSeam;

			bg.CurrentBackgroundModel.BGSprite.Init ();
			
			SetCurrentSeamModel (nextSeam);
		}

	}

	void SetCurrentSeamModel(BackgroundSwapper.BGType bgType){
		for (int i = 0; i < Seams.Length; i++) {
			BGSeamModel model = Seams [i];
			if (model.BGType == bgType) {
				_CurrentSeamModel = model;
				break;
			}
		}
	}

	void MoveBottomToTop() {
		BackgroundBoundFinder topBound = Backgrounds [CurrentTopIndex].GetComponent<BackgroundBoundFinder> ();
		BackgroundBoundFinder bottomBound = Backgrounds [CurrentBottomIndex].GetComponent<BackgroundBoundFinder> ();

		bottomBound.transform.position = new Vector3 (
			bottomBound.transform.position.x,
			topBound.MaxY + (bottomBound.transform.position.y - bottomBound.MinY),
			bottomBound.transform.position.z
		);

			

		if (CurrentTopIndex > 0) {
			CurrentTopIndex--;
		} else {
			CurrentTopIndex = Backgrounds.Length-1;
		}

		if (CurrentBottomIndex > 0) {
			CurrentBottomIndex--;
		} else {
			CurrentBottomIndex = Backgrounds.Length-1;
		}
			
		BackgroundSwapper bg = bottomBound.GetComponent<BackgroundSwapper> ();
		ChangeBG (bg);
	}
}
