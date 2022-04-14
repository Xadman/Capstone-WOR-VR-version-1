using UnityEngine;
using System.Collections;

public class Leveling : MonoBehaviour {

	public int xpBase = 50;

	public int currentXP;

	int xpToNext = 0;
	int xpToCurrent = 0;

	public float percentToNext;

	public GameObject xpMeter;

	Slot slot;
	// Use this for initialization
	void Start () 
	{
		slot = GameObject.Find ("BeachDays").GetComponent<Slot>();
		if (slot.refs.credits.persistant) {
			// initialize the level
			computeLevel();
			// award any xp saved in player prefs
			AwardXp(PlayerPrefs.GetInt("CurrentXP", 0));
			// hack for unity bug
			xpMeter.SetActive(false);
			xpMeter.SetActive(true);
		}
	}

	// Compute and update leveling info for example
	int computeLevel() {
		float lvl = Mathf.Sqrt(currentXP/xpBase);
		int clevel = Mathf.FloorToInt(lvl) + 1;
		
		if (clevel == 0) clevel = 1;
		
		xpToNext = ((clevel)*(clevel))*xpBase;
		xpToCurrent = ((clevel - 1)*(clevel - 1))*xpBase;

		percentToNext = ((float)currentXP-(float)xpToCurrent) / ((float)xpToNext - (float)xpToCurrent);

		return clevel;
	}

	// Recompute and return current level for GUI readout
	public int GetLevel() {
		int clevel = 0;
		if (xpBase == 0) return clevel;

		clevel = computeLevel();

		return clevel;
	}

	// Award an xp amount, and also update the visual sprite meter size, and save the new XP to player prefs
	public void AwardXp(int amount) {
		currentXP += amount;
		computeLevel();
		xpMeter.transform.localScale = new Vector3(xpMeter.GetComponent<Leveling>().percentToNext, 1, 1);
		PlayerPrefs.SetInt("CurrentXP", currentXP);
	}
}