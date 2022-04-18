// Brad Lima - 11/2019
//
// This is a simple gui class that is used in slot machine examples
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlotSimpleGUI : MonoBehaviour {

	public GUISkin skin;

	[HideInInspector]
	Slot slot;
	bool togglePays;


	void Awake() {
		slot = GetComponent<Slot>();
	}
	void Start() {
	}
	void Update() {

		//Cursor.visible = false;
		//if (Input.GetMouseButtonUp(0))
		//{
		//	slot.spin();
		//}
	}

	void showBetAndLinesButtons()
	{
		if (GUI.Button(new Rect(Screen.width-250, Screen.height-50, 100, 50), "Lines Played: " +  slot.refs.credits.linesPlayed.ToString ()))
		{
			slot.refs.credits.incLinesPlayed();
		}
		if (GUI.Button(new Rect(Screen.width-150, Screen.height-50, 100, 50), "Bet Per Line: " +  slot.refs.credits.betPerLine.ToString ()))
		{
			slot.refs.credits.incBetPerLine();
		}

		if (GUI.Button(new Rect(Screen.width-150, 25, 100, 50), "Get Credits"))
		{
			slot.refs.credits.depositCredits(100);
		}
		string payslabel = (togglePays)?"Hide Pays":"Show Pays";
		if (GUI.Button(new Rect(Screen.width-250, 25, 100, 50), payslabel))
		{
			togglePays = !togglePays;
		}

	}

	void showPays(int windowID)
	{
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		GUI.contentColor = Color.black;
		for (int setIndex = 0; setIndex < slot.symbolSets.Count; setIndex++)
		{
			GUI.Label(new Rect(25,60+(30*setIndex),200,50), slot.symbolSetNames[setIndex]);
			for (int payIndex = 0; payIndex < slot.setPays[setIndex].pays.Count; payIndex++)
			{
				if (setIndex == 0)
				{
					GUI.Label(new Rect(250+(50 * payIndex),30,50,50), (payIndex+1).ToString());
				}
				string pay = slot.setPays[setIndex].pays[payIndex].ToString();
				if (pay == "0") pay = "-";
				GUI.Label(new Rect(250+(50 * payIndex),60+(30*setIndex),50,50), pay);
			}
		}
	}
	void OnGUI () {

		GUI.skin = skin;

		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.skin.label.fontSize = 18;

		if (togglePays)
		{
			Rect windowRect = new Rect(Screen.width*0.1f, Screen.height*0.1f, Screen.width*0.8f, Screen.height*0.8f);
			windowRect = GUI.Window(0, windowRect, showPays, "Pays");
		}

		string spinText = "Spin";
		if(slot != null)
        {
			switch (slot.state)
			{

				case SlotState.playingwins:
					if (slot.refs.wins.currentWin == null) return;
					GUI.Label(new Rect(0, Screen.height - 125, Screen.width, 50), slot.refs.wins.currentWin.readout.ToString());
					GUI.Label(new Rect(0, Screen.height - 75, Screen.width, 50), "Total Won: " + slot.refs.credits.lastWin.ToString());
					showBetAndLinesButtons();
					break;
				case SlotState.ready:
					showBetAndLinesButtons();
					break;
				case SlotState.snapping:
					break;
				case SlotState.spinning:
					spinText = "Stop";
					break;
			}
			if (GUI.Button(new Rect(50, Screen.height - 100, 100, 50), spinText))
			{
				// Simply spin and get a random result
				slot.spin();
			}

			if (GUI.Button(new Rect(50, Screen.height - 50, 100, 50), "Spin & Wait"))
			{
				// SpinWaitForResult
				// This demonstrates the functionality to be able to load an explicit result into the slot
				slot.spinWaitForResult(false);
			}
			if (GUI.Button(new Rect(175, Screen.height - 50, 100, 50), "Supply Result"))
			{
				// SupplyResultToSpin
				// Once you call this, the reels will stop and display the result you specify. 
				// The array of integers must match the structure of your slot machine.
				int[,] result = new int[,] { { 0, 2, 3 }, { 1, 2, 3 }, { 1, 2, 3 }, { 1, 2, 3 }, { 1, 2, 3 } };
				slot.supplyResultToSpin(result);
			}
			GUI.skin.label.fontSize = 22;
			GUI.Label(new Rect(0, 0, Screen.width, 50), "Credits: " + slot.refs.credits.totalCreditsReadout().ToString(), GUI.skin.label);
			GUI.Label(new Rect(0, Screen.height - 50, Screen.width, 50), "Total Bet: " + slot.refs.credits.totalBet().ToString());
		}
		
	}
}
