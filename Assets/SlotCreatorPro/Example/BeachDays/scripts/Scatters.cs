using UnityEngine;
using System.Collections;

public class Scatters : MonoBehaviour {

	int[] scatters = new int[3];
	int[] scatterResets = new int[3];
	// Use this for initialization
	Slot slot;
	// Use this for initialization
	void Start () 
	{

		slot = GameObject.Find ("BeachDays").GetComponent<Slot>();
		if (slot.refs.credits.persistant) {
			scatters[0] = PlayerPrefs.GetInt("Scatter5", 1000);
			scatters[1] = PlayerPrefs.GetInt("Scatter6", 10000);
			scatters[2] = PlayerPrefs.GetInt("Scatter7", 100000);
		} else {
			scatters[0] = 1000;
			scatters[1] = 10000;
			scatters[2] = 100000;
		}
		scatterResets[0] = 1000;
		scatterResets[1] = 10000;
		scatterResets[2] = 100000;

	}
	
	// Update is called once per frame
	void Update () {
		int r = Random.Range(1,10000);

		if (r > 9950) {

			int s = Random.Range(0,3);
			scatters[s]++;
		}
		SaveScatters();
	}

	void SaveScatters() {
		PlayerPrefs.SetInt("Scatter5", scatters[0]);
		PlayerPrefs.SetInt("Scatter6", scatters[1]);
		PlayerPrefs.SetInt("Scatter7", scatters[2]);
	}
	public int GetScatter(int scatter) {
		return scatters[scatter];
	}
	public void ResetScatter(int scatter) {
		scatters[scatter] = scatterResets[scatter];
	}
}
