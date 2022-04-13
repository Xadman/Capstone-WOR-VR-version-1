// Brad Lima - 11/2019
//
// This is the customized Callbacks script for Beach Days Slot
//
using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
public class BeachDaysCallbacks : MonoBehaviour {

	[HideInInspector]
	public Slot slot;
	public BeachDaysGUI deck;
	public Leveling leveling;
	public Scatters scatters;

	void Awake()
	{
		// Example of setting the board result on startup
		//
		//slot = GetComponent<Slot>();
		//slot.suppliedResult = new int[5,4] { { 1,1,1,1 }, { 1,1,1,1 }, { 1,1,1,1 }, { 1,1,1,1 }, { 1,1,1,1 }};
		//slot.useSuppliedResult = true;
	}

	void Start() 
	{
	}

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
		Slot.OnReelLand += OnReelLand;

		Slot.OnLineWinComputed += OnLineWinComputed;
		Slot.OnLineWinDisplayed += OnLineWinDisplayed;
		Slot.OnAllWinsComputed	+= OnAllWinsComputed;
		Slot.OnScatterSymbolHit += OnScatterSymbolHit;
		Slot.OnAnticipationScatterBegin += OnAnticipationScatterBegin;

		Slot.OnScatterSymbolLanded += OnScatterSymbolLanded;
		Slot.OnWinDisplayedCycle += OnWinDisplayedCycle;
		Slot.OnLinkedSymbolLanded += OnLinkedSymbolLanded;

		Slot.OnBeginCreditWinCountOff += OnBeginCreditWinCountOff;
		Slot.OnCompletedCreditCountOff += OnCompletedCreditCountOff;

		Slot.OnSymbolReturningToPool += OnSymbolReturningToPool;
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
		Slot.OnAllWinsComputed -= OnAllWinsComputed;

		Slot.OnScatterSymbolHit -= OnScatterSymbolHit;
		Slot.OnAnticipationScatterBegin -= OnAnticipationScatterBegin;

		Slot.OnScatterSymbolLanded -= OnScatterSymbolLanded;
		Slot.OnWinDisplayedCycle -= OnWinDisplayedCycle;
		Slot.OnLinkedSymbolLanded -= OnLinkedSymbolLanded;

		Slot.OnBeginCreditWinCountOff -= OnBeginCreditWinCountOff;
		Slot.OnCompletedCreditCountOff -= OnCompletedCreditCountOff;

		Slot.OnSymbolReturningToPool -= OnSymbolReturningToPool;

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
	public void OnSlotStateChangeTo(SlotState state)
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
		leveling.AwardXp(slot.refs.credits.totalBet());
		slot.log ("OnSpinBegin Callback");
	}

	private void OnSpinInsufficentCredits()
	{
		slot.log ("OnSpinInsufficentCredits Callback");
	}

	void OnReelLand (int obj)
	{

	}

	private void OnSpinSnap()
	{
		slot.log ("OnSpinSnap Callback");
	}

	int freespins = 0;
	private void OnSpinDone(int totalWon, int timesWin)
	{
		// Demonstrates frozen positions functionality
		/*
		if (freespins < 10)
		{
			DOTween.Sequence().AppendInterval(1).AppendCallback(()=> {

				int[,] result = slot.getResultArray();

				for (int i = 0; i < result.GetLength(0); i++)
				{
					for (int ii = 0; ii < result.GetLength(1); ii++)
					{
						if (result[i,ii] == 1)
							slot.frozenPositions[i, ii] = 1;
					}
				}
				
				slot.spin(true);
				freespins++;
			});
		} else {
			slot.resetFrozenPositions();
		}
		*/

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
	
	// Illustrates an example of parsing a specifically ordered win
	/*
		int extraWin = 10;
		
		SlotWinData winLine = slot.refs.compute.lineResultData.Find(item => item.setName == "Any Sevens");
		if (winLine != null)
		{
			List<SlotSymbol> symbols = new List<SlotSymbol>();
			winLine.symbols.ForEach(item => symbols.Add(item.GetComponent<SlotSymbol>()));

			if (symbols.Find(item => item.reelIndex == 0 && item.symbolIndex == INDEX_OF_RED_SEVEN_SYMBOL) &&
				symbols.Find(item => item.reelIndex == 1 && item.symbolIndex == INDEX_OF_WHITE_SEVEN_SYMBOL) &&
				symbols.Find(item => item.reelIndex == 2 && item.symbolIndex == INDEX_OF_BLUE_SEVEN_SYMBOL))
				{
					slot.refs.credits.awardWin(extraWin);
				}
		}
	*/

		
		int matches = 5;
		while (matches < 8) {
			// search line results for the set name and the matches
			int jackPot = slot.refs.compute.lineResultData.FindIndex(item => item.setName == "Jackpot" && item.matches == matches);

			if (jackPot > -1) {
				win.setPaid(scatters.GetScatter(matches-5));
				slot.refs.compute.lineResultData[jackPot] = win;
				scatters.ResetScatter(matches-5);
				matches=8;
			}
			matches++;
		}

		slot.log ("win line " + win.lineNumber + " :: set: " + win.setName + " (" + win.setIndex + ") paid: " + win.paid + " matches: " + win.matches);
	}

	private void OnLineWinDisplayed(SlotWinData win, bool isFirstLoop)
	{
		// Itterate through symbols that make up the win
		foreach(GameObject symbol in win.symbols)
		{
			// if there is an animator component, play the win animation
			if (symbol.GetComponent<Animator>())
				symbol.GetComponent<Animator>().SetTrigger("playwin");
		}
		slot.log ("OnLineWinDisplayed Callback");
		slot.log ("win line " + win.lineNumber + " :: set: " + win.setName + " (" + win.setIndex + ") paid: " + win.paid + " matches: " + win.matches);
	}

	private void OnAllWinsComputed(SlotWinSpin win, int timesBet)
	{
		slot.log ("OnAllWinsComputed Callback");
	}

	private void OnScatterSymbolLanded(GameObject symbol, int count) {

		// we don't want to play the scatter animations if the reels are snapping
		if (slot.state == SlotState.snapping) return;

		// trigger animation when scatter symbol lands if there is one
		Animator anim = symbol.GetComponent<Animator>();
		if (anim)
		{
			anim.SetTrigger("playwin");
		}

	}

	private void OnScatterSymbolHit(SlotScatterHitData hit)
	{
		slot.log ("OnScatterSymbolHit Callback");
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

	void OnBeginCreditWinCountOff (int obj)
	{

	}

	void OnCompletedCreditCountOff (int obj)
	{

	}
	
	void OnSymbolReturningToPool (GameObject obj)
	{

	}

	#endregion
}
