using UnityEngine;
using System.Collections;

// Resets symbol sprite to original on Disable
public class ResetAnimationSprite : MonoBehaviour {

	public Sprite original = null;
	// Use this for initialization
	void Start () {
		original = transform.GetComponent<SpriteRenderer>().sprite;
	}

	void OnDisable() {
		if (original != null)
		{
			transform.GetComponent<SpriteRenderer>().sprite = original;
		}
	}
}
