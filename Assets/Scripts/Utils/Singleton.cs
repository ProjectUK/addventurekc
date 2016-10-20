// from http://answers.unity3d.com/questions/408518/dontdestroyonload-duplicate-object-in-a-singleton.html
// by Sotirosn

using UnityEngine;
using System.Collections;

public class Singleton<m_Instance> : MonoBehaviour where m_Instance : Singleton<m_Instance> {
	public static m_Instance Instance;
	public bool isPersistant;

	public virtual void Awake() {
		if(isPersistant) {
			if(!Instance) {
				Instance = this as m_Instance;
			}
			else {
				DestroyObject(gameObject);
			}
			DontDestroyOnLoad(gameObject);
		}
		else {
			Instance = this as m_Instance;
		}
	}
}