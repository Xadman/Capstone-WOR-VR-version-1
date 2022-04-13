using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleShare : MonoBehaviour {

	//public GameObject emitter;
	public int emitAmount;
	public float emitSkip;
	public float emitSkipTimeout;
	public List<Transform>emitTransforms;
	public List<GameObject>emitters;

	private int emitIndex;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		//emitSkipTimeout += Time.deltaTime;

		//if (emitSkipTimeout > emitSkip)
		//{
		emitters[emitIndex].transform.position = emitTransforms[emitIndex].position;
		emitters[emitIndex].transform.localScale = emitTransforms[emitIndex].localScale;
		emitters[emitIndex].transform.rotation = emitTransforms[emitIndex].rotation;
		emitters[emitIndex].GetComponent<ParticleSystem>().Emit(emitAmount);
		emitIndex++;
		if (emitIndex == emitTransforms.Count) emitIndex = 0;
		//	emitSkipTimeout = 0;
		//}
	}

	void OnDisable() {
		for (int i = 0; i < emitters.Count; i++)
		{
			emitters[i].GetComponent<ParticleSystem>().Clear();
		}
	}
}
