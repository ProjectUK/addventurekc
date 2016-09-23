using UnityEngine;
using System.Collections;

public class BGMManager : MonoBehaviour {

	public string CurrentBGM;

	public static BGMManager _Instance;

	public static BGMManager Instance{
		get{
			if (_Instance == null) {
				_Instance = GameObject.FindObjectOfType<BGMManager> ();
			}
			return _Instance;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
