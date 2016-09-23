using UnityEngine;
using System.Collections;
using UnityEditor;

public class EnemyCreatorMenu:MonoBehaviour {

	[MenuItem("GameObject/Crow Coffee/EnemyPattern", false, 0)]
	static void CreateEnemyPattern () {
		GameObject go = Instantiate (Resources.Load ("Templates/EnemyPattern") as GameObject);
		go.transform.name = "EnemyPattern";
	}
}
