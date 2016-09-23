using UnityEngine;
using System.Collections;

public class PowerUp : Entity {

	BoxCollider2D _Collider;

	// Use this for initialization
	void Start () {
		_Collider = GetComponent<BoxCollider2D> ();
	}

	public override void Init ()
	{
//		throw new System.NotImplementedException ();
	}
}
