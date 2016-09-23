using UnityEngine;
using System.Collections;

public class IndieTime : MonoBehaviour {


	/* 
	 * IndieTime is an independent time
	 * which use deltaTime without affected by Time.deltaTime
	 * 
	 */

	private static IndieTime _Instance;
	
	public static IndieTime Instance
	{
		get
		{
			if (_Instance == null){
				GameObject go = new GameObject();
				go.name = "IndieTime";
				_Instance = go.AddComponent<IndieTime>();
			}
			
			return _Instance;
		}
	}

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
