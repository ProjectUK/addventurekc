using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BackgroundBoundFinder : MonoBehaviour {

	public SpriteRenderer BackgroundSprite;

	[Header("Boundings")]
	public float MinX;
	public float MinY;
	public float MaxX;
	public float MaxY;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (BackgroundSprite != null) {
			MinX = ((BackgroundSprite.sprite.bounds.min.x + BackgroundSprite.transform.localPosition.x )* BackgroundSprite.transform.localScale.x) + this.transform.position.x;
			MaxX = ((BackgroundSprite.sprite.bounds.max.x + BackgroundSprite.transform.localPosition.x )* BackgroundSprite.transform.localScale.x) + this.transform.position.x;
			MinY = ((BackgroundSprite.sprite.bounds.min.y + BackgroundSprite.transform.localPosition.y )* BackgroundSprite.transform.localScale.y) + this.transform.position.y;
			MaxY = ((BackgroundSprite.sprite.bounds.max.y + BackgroundSprite.transform.localPosition.y )* BackgroundSprite.transform.localScale.y) + this.transform.position.y;
		}
	}
}
