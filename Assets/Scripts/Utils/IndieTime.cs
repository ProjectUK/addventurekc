using UnityEngine;
using System.Collections;

public class IndieTime : Singleton<IndieTime> {

	/* 
	 * IndieTime is an independent time
	 * which use deltaTime without affected by Time.deltaTime
	 * 
	 */

	float previoustimeSinceStartup;
	public float deltaTime {
		get;
		private set;
	}

	// Use this for initialization
	void Start () {
		previoustimeSinceStartup = Time.realtimeSinceStartup;
	}
	
	// Update is called once per frame
	void Update () {
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		deltaTime = realtimeSinceStartup - previoustimeSinceStartup;
		previoustimeSinceStartup = realtimeSinceStartup;
	}

	public IEnumerator WaitForSeconds(float time) {
		float elapsedTime = 0;
		while (elapsedTime < time) {
			yield return null;
			elapsedTime += deltaTime;
		}
		yield return null;
	}
}
