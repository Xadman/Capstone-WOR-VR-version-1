// Fix for particles in 2D system not appearing due to hidden sorting layer property
//
using UnityEngine;
using System.Collections;

public class ExposeSortingLayer : MonoBehaviour {

	public string sortingLayerName;
	public int sortingLayerNameIndex;
	public int sortingLayerOrder;
	public bool realTime;
	// Use this for initialization
	void Start () {
		setSorting();
	}

	void Update() {
		if (!realTime) return;
		setSorting();
	}

	public void setSorting()
	{
		if (GetComponent<ParticleSystem>() != null)
		{
			GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = sortingLayerName;
			GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = sortingLayerOrder;
		}
		if (GetComponent<TextMesh>() != null)
		{
			GetComponent<TextMesh>().GetComponent<Renderer>().sortingLayerName = sortingLayerName;
			GetComponent<TextMesh>().GetComponent<Renderer>().sortingOrder = sortingLayerOrder;
		}
	}
}
