using UnityEngine;
using System.Collections;

public class RockyEndController : BackgroundController {

	public PitstopSpawner Pitstop;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void Init ()
	{
		base.Init ();
		Pitstop.Spawn ();
	}
}
