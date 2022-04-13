// Brad Lima - 11/2019
//
// This class illustrates all callback usage, and demonstrates how you can easily hook into all Slot events,
// along with some simple mechanics used in the examples.
//
// You can use this as the basis for your own slots
//
using UnityEngine;
using System.Collections;

public class SlotCallbacks : MonoBehaviour {

	[HideInInspector]
	public Slot slot;

	#region Enable/Disable
	void OnEnable() {

		slot = GetComponent<Slot>();

		Slot.OnSlotStateChangeTo += OnSlotStateChangeTo;	
		Slot.OnSlotStateChangeFrom += OnSlotStateChangeFrom;	
		Slot.OnSpinBegin += OnSpinBegin;
		Slot.OnSpinInsufficentCredits += OnSpinInsufficentCredits;
		Slot.OnSpinSnap += OnSpinSnap;
		Slot.OnSpinDone += OnSpinDone;
		Slot.OnSpinDoneNoWins += OnSpinDoneNoWins;

		Slot.OnLineWinComputed += OnLineWinComputed;
		Slot.OnLineWinDisplayed += OnLineWinDisplayed;
		Slot.OnAllWinsComputed	+= OnAllWinsComputed;
		Slot.OnScatterSymbolHit += OnScatterSymbolHit;
		Slot.OnAnticipationScatterBegin += OnAnticipationScatterBegin;

		Slot.OnWinDisplayedCycle += OnWinDisplayedCycle;
		Slot.OnLinkedSymbolLanded += OnLinkedSymbolLanded;
	}

	#endregion

	void OnDisable() {

		Slot.OnSlotStateChangeTo -= OnSlotStateChangeTo;	
		Slot.OnSlotStateChangeFrom -= OnSlotStateChangeFrom;

		Slot.OnSpinBegin -= OnSpinBegin;
		Slot.OnSpinInsufficentCredits -= OnSpinInsufficentCredits;
		Slot.OnSpinSnap -= OnSpinSnap;
		Slot.OnSpinDone -= OnSpinDone;
		Slot.OnSpinDoneNoWins -= OnSpinDoneNoWins;

		Slot.OnLineWinDisplayed -= OnLineWinDisplayed;
		Slot.OnLineWinDisplayed -= OnLineWinDisplayed;
		Slot.OnAllWinsComputed -= OnAllWinsComputed;
		Slot.OnScatterSymbolHit -= OnScatterSymbolHit;
		Slot.OnAnticipationScatterBegin -= OnAnticipationScatterBegin;

		Slot.OnWinDisplayedCycle -= OnWinDisplayedCycle;
		Slot.OnLinkedSymbolLanded -= OnLinkedSymbolLanded;
	}

	#region Update Callback

	private void OnSlotUpdate()
	{
	}
	#endregion


	#region State Callbacks 

	private void OnSlotStateChangeFrom(SlotState state)
	{
		slot.log ("onSlotStateChangeFrom " + state);
		switch (state)
		{
		case SlotState.playingwins:
			break;
		case SlotState.ready:
			break;
		case SlotState.snapping:
			break;
		case SlotState.spinning:
			break;
		}
	}
	private void OnSlotStateChangeTo(SlotState state)
	{
		slot.log ("OnSlotStateChangeTo " + state);
		switch (state)
		{
		case SlotState.playingwins:
			break;
		case SlotState.ready:
			break;
		case SlotState.snapping:
			break;
		case SlotState.spinning:
			break;
		}
	}
	#endregion

	#region Spin Callbacks
	private void OnSpinBegin(SlotWinData data)
	{
		slot.refs.lines.hideLines ();
		slot.log ("OnSpinBegin Callback");
	}

	private void OnSpinInsufficentCredits()
	{
		slot.log ("OnSpinInsufficentCredits Callback");
	}

	private void OnSpinSnap()
	{
		slot.log ("OnSpinSnap Callback");
	}

	private void OnSpinDone(int totalWon, int timesWin)
	{
		slot.log ("OnSpinDone Callback");
	}

	private void OnSpinDoneNoWins()
	{
		slot.log ("OnSpinDoneNoWins Callback");
	}
	#endregion

	#region Win Callbacks
	private void OnLineWinComputed(SlotWinData win)
	{
		slot.log ("OnLineWinComputed Callback");

		slot.log ("win line " + win.lineNumber + " :: set: " + win.setName + " (" + win.setIndex + ") paid: " + win.paid + " matches: " + win.matches);
	}

	private void OnLineWinDisplayed(SlotWinData win, bool isFirstLoop)
	{
		slot.log ("OnLineWinDisplayed Callback");
		slot.log ("win line " + win.lineNumber + " :: set: " + win.setName + " (" + win.setIndex + ") paid: " + win.paid + " matches: " + win.matches);
	}

	private void OnAllWinsComputed(SlotWinSpin win, int timesBet)
	{
		slot.log ("OnAllWinsComputed Callback");
	}

	private void OnScatterSymbolHit(SlotScatterHitData hit)
	{
		slot.log ("OnScatterSymbolHit Callback");
		hit.symbol.transform.eulerAngles = new Vector2(0,0);
	}

	private void OnAnticipationScatterBegin(SlotScatterHitData hit)
	{
		slot.log ("OnAnticipationScatterBegin Callback");
	}

	public void OnLinkedSymbolLanded(int reel, string linkName) {
		slot.log ("OnLinkedSymbolLanded:" + reel + " : " + linkName);
		Debug.Log ("OnLinkedSymbolLanded:" + reel + " : " + linkName);
	}
	void OnWinDisplayedCycle (int count)
	{
		slot.log ("OnWinDisplayedCycle Callback");
	}

	#endregion

}
