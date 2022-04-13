// Brad Lima - 8/2020
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System;
using aSlot;

[RequireComponent(typeof(SlotCredits))]
[RequireComponent(typeof(SlotCompute))]
[RequireComponent(typeof(SlotComputeEditor))]
[RequireComponent(typeof(SlotWins))]
[RequireComponent(typeof(SlotLines))]

/// <summary>
/// The Slot Class is the brain of Slots Creator Pro.
/// Although there's many public properies and variables,
/// you can easily edit all aspects of your slot
/// directly in the inspector.
/// </summary>
public class Slot : MonoBehaviour {

	#region Actions
	/// <summary>
	/// Callback when a state is changed TO
	/// </summary>
	/// <param name="state">returns state changing to</param>
	public static event Action<SlotState> OnSlotStateChangeTo;
	/// <summary>
	/// Calllback when a state is changed FROM	/// 
	/// </summary>
	/// <param name="state">returns state changing from</param>
	public static event Action<SlotState> OnSlotStateChangeFrom;
	/// <summary>
	/// Callback fires when a spin begins.
	/// </summary>
	public static event Action<SlotWinData> OnSpinBegin;
	/// <summary>
	/// Callback occurs when a player attempts to spin but doesn't have enough credits
	/// </summary>
	public static event Action OnSpinInsufficentCredits;
	/// <summary>
	/// Fires each time a reel stops spinnings
	/// </summary>
	/// <param name="reel">returns reel number</param>
	public static event Action<int> OnReelLand;
	/// <summary>
	/// Callback fires when player snaps a spin
	/// </summary>
	public static event Action OnSpinSnap;
	/// <summary>
	/// Called when spin is completely done
	/// </summary>
	/// <param name="totalWon">returns total won</param>
	/// <param name="totalBet">returns total bet</param>
	public static event Action<int, int>OnSpinDone;
	/// <summary>
	/// called when a spin is done, and there were no wins
	/// </summary>
	public static event Action OnSpinDoneNoWins;
	/// <summary>
	/// Callback at the start of a pay count off
	/// </summary>
	/// <param name="amountWon">amount being counted off</param> 
	public static event Action<int>OnBeginCreditWinCountOff;
	/// <summary>
	/// Callback at the start of a bonus count off
	/// </summary>
	/// <param name="amountWon">amount being counted off</param> 
	public static event Action<int>OnBeginCreditBonusCountOff;
	/// <summary>
	/// Callback at the end of a pay count off
	/// </summary>
	/// <param name="amountWon">amount counted off</param> 
	public static event Action<int>OnCompletedCreditCountOff;
	/// <summary>
	/// Callback at the end of a bonus count off
	/// </summary>
	/// <param name="amountWon">amount counted off</param> 
	public static event Action OnCompletedBonusCreditCountOff;
	/// <summary>
	/// callback occurs after each line win is computed
	/// </summary>
	/// <param name="winData">returns a SlotWinData class</param> 
	public static event Action<SlotWinData> OnLineWinComputed;
	/// <summary>
	/// Occurs when on line window displayed.
	/// </summary>
	/// <param name="winData">returns a SlotWinData class</param> 
	public static event Action<SlotWinData, bool> OnLineWinDisplayed;
	/// <summary>
	/// Occurs when on begin delay between line window displayed.
	/// </summary>
	/// <param name="winData">returns a SlotWinData class</param> 
	public static event Action<SlotWinData> OnBeginDelayBetweenLineWinDisplayed;
	/// <summary>
	/// Occurs when on all wins computed.
	/// </summary>
	/// <param name="slotWins">returns a SlotWinSpin class</param> 
	public static event Action<SlotWinSpin, int> OnAllWinsComputed;
	/// <summary>
	/// Occurs when a full set of linked symbols stops on screen
	/// </summary>
	/// <param name="slotWins">returns a SlotWinSpin class</param> 
	public static event Action<int, string> OnLinkedSymbolLanded;
	/// <summary>
	/// Occurs when on scatter symbol hit.
	/// </summary>
	/// <param name="scatterData">returns a SlotScatterHitData class</param> 
	public static event Action<SlotScatterHitData>OnScatterSymbolHit;
	public static event Action<GameObject, int>OnScatterSymbolLanded;
	/// <summary>
	/// Occurs when on anticipation scatter begin.
	/// </summary>
	/// <param name="scatterData">returns a SlotScatterHitData class</param> 
	public static event Action<SlotScatterHitData>OnAnticipationScatterBegin;

	/// <summary>
	/// Occurs when a full cycle of wins has played
	/// </summary>
	/// <param name="winCount">returns total wins</param> 
	public static event Action<int> OnWinDisplayedCycle;
	/// <summary>
	/// Occurs when on increment lines played.
	/// </summary>
	/// <param name="lines">number of lines played</param> 
	public static event Action<int> OnIncrementLinesPlayed;
	/// <summary>
	/// Occurs when on decrement lines played.
	/// </summary>
	/// <param name="lines">number of lines played</param> 
	public static event Action<int> OnDecrementLinesPlayed;
	/// <summary>
	/// Occurs when bet per line level is increased
	/// </summary>
	/// <param name="bet">returns current bet</param> 
	public static event Action<int> OnIncrementBetPerLine;
	/// <summary>
	/// Occurs when bet per line level is decremented
	/// </summary>
	/// <param name="bet">returns current bet</param> 
	public static event Action<int> OnDecrementBetPerLine;

	/// <summary>
	/// called when a reel symbol object is returned to the prefab pool
	/// </summary>
	/// <param name="symbol">returns symbol gameobject</param> 
	public static event Action<GameObject> OnSymbolReturningToPool;


	#endregion


	/// <summary>
	/// Setting this to true will enable some log messages generated by KCP
	/// </summary>
	public bool debugMode = true;
	/// <summary>
	/// Active Random Number Generator
	/// </summary>
	public pRNGs activeRNG;

	/// <summary>
	/// The height of each reel in symbols
	/// </summary>
	public int reelHeight = 5;				// how many symbols tall are the reels to be on this slot machine
	/// <summary>
	/// The number of "edge" symbols on the top and bottom of each reel.
	/// This will not be considered part of any line, but will spin.
	/// </summary>
	public int reelIndent = 1;
	/// <summary>
	/// The number of reels.
	/// </summary>
	public int numberOfReels = 3;
	/// <summary>
	/// The horizontal reel padding.
	/// </summary>
	public float horizontalReelPaddingPercent = 0;
	public float verticalSymbolPaddingPercent = 0;
	/// <summary>
	/// The total spin time of the first reel
	/// </summary>
	public float spinTime = 1.0f;			// how long should the total spin time be?
	/// <summary>
	/// The spin time increment each additional reel.
	/// </summary>
	public float spinTimeIncPerReel = 0.5f;	// how much longer should each extra reel spin (spinTime + spinTimeIncPerReel)
	/// <summary>
	/// spin time increase for scatter anticipations
	/// </summary>
	public float spinTimeScatterAnticipation = 0.5f;
	/// <summary>
	/// the speed in which reels spin
	/// </summary>
	public float spinningSpeed = 0.033f;
	/// <summary>
	///  the ease out time for the stopping tween
	/// </summary>
	public float easeOutTime = 0.5f; 
	/// <summary>
	/// the effect for the stopping tween
	/// </summary>
	public Ease reelEase = Ease.OutElastic;
	/// <summary>
	///  the transform that the reel cage will be centered on if supplied
	/// </summary>
	public Transform reelCenter;
	/// <summary>
	/// If true, use a prefab pool for the slot's symbols
	/// </summary>
	public bool usePool = true;

	/// <summary>
	/// Gameobject that represents the background of each symbol on each reel, if specified. (optional)
	/// </summary>
	public List<GameObject> symbolBgPrefabs = new List<GameObject>();
	  
	[SerializeField]
	/// <summary>
	/// Symbol prefabs specified in the inspector
	/// </summary>
	public List<GameObject> symbolPrefabs = new List<GameObject>();	// A list of symbol s (attach SlotSymbol script to each symbol)
	/// <summary>
	/// SymbolInfo specified in the inspector
	/// </summary>
	public List<SlotSymbolInfo> symbolInfo = new List<SlotSymbolInfo>();
	[SerializeField]
	/// <summary>
	/// Toggle display line and win boxes per reel
	/// </summary>
	public int displayLineBoxes = 0;
	/// <summary>
	/// The GameObject used for non-contributing symbols on a line (oppisite of winBox)
	/// </summary>
	public GameObject lineboxPrefab;
	/// <summary>
	/// The GameObject used for contributing symbols on a line for each symbol, specified in the inspector
	/// </summary>
	public List<GameObject> winboxPrefabs = new List<GameObject>();	// A list of symbol prefabs (attach SlotSymbol script to each symbol)
	/// <summary>
	/// Per-reel symbol frequencies of each symbol, specified in the inspector
	/// </summary>
	public List<FrequencyWrapper> reelFrequencies = new List<FrequencyWrapper>();
	/// <summary>
	/// Prequencies of each symbol, specified in the inspector
	/// </summary>
	public List<int> symbolFrequencies = new List<int>();		// A list of frequencies of each symbol on this slot machine

	/// <summary>
	/// Symbol set names, defined in the inspector
	/// </summary>
	public List<string> symbolSetNames = new List<string>();		// A list of names describing each symbol set (i.e. Triple Sevens)
	[SerializeField]
	/// <summary>
	/// internal
	/// </summary>
	public SaveEditor edsave;
	/// <summary>
	/// List of bet levels, defined in the inspector
	/// </summary>
	[SerializeField]
	public List<BetsWrapper> betsPerLine = new List<BetsWrapper>();
	/// <summary>
	/// List symbol sets, defined in the inspector
	/// </summary>
	[SerializeField]
	public List<SetsWrapper> symbolSets = new List<SetsWrapper>();  	// A list of strings describing each valid symbol set (i.e. ,0,1,2,) must start and end with commas
	/// <summary>
	/// List of set pays, defined in the inspector
	/// </summary>
	[SerializeField]
	public List<PaysWrapper> setPays = new List<PaysWrapper>();
	/// <summary>
	/// List of lines, defined in the inspector
	/// </summary>
	[SerializeField]
	public List<LinesWrapper> lines = new List<LinesWrapper>();		// a list of strings that describe each valid line on the slot machine using symbol indexes

	/// <summary>
	/// current machine state
	/// </summary>
	public SlotState state = SlotState.ready;
	/// <summary>
	/// array of reels by 0-index
	/// </summary>
	public Dictionary<int, SlotReel> reels = new Dictionary<int, SlotReel>();

	List<GameObject>[] pool;

	List<GameObject>[] poolWinbox;
	[HideInInspector]
	GameObject poolContainer;
	GameObject poolWinboxContainer;

	/// <summary>
	/// the global percision for symbol frequencies
	/// </summary>
	public int percision;
	/// <summary>
	/// refs is a reference variable which allows you to access
	/// some internal information.
	/// </summary>
	public SlotComponents refs;
	/// <summary>
	/// temporary holder for a supplied slot result
	/// </summary>
	public int[,] suppliedResult;
	
	/// <summary>
	/// Use this array to freeze positions on the board by specifying symbol indexes for positions to freeze
	/// Call reset frozen positions to clear them (aka set them to all -1).
	////// </summary>
	public int[,] frozenPositions;
	/// <summary>
	/// set to true when a spinWithResult is executed
	/// </summary>
	public bool useSuppliedResult;
	/// <summary>
	/// The waiting for result.
	/// </summary>
	public bool waitingForResult;

	#region Startup
	void Awake () 
	{
		// added 12/8/2020 to fix long time bug of first spin not stopping properly
		waitingForResult = false;
		if (suppliedResult == null)
			useSuppliedResult = false;

		if (reelCenter == null) reelCenter = transform;

		if (usePool) createGlobalSymbolPool ();
		if (usePool) createGlobalWinboxPool ();

		createReelGameObjects();
		resetFrozenPositions();

		refs.credits = GetComponent<SlotCredits>();
		refs.wins = GetComponent<SlotWins>();
		refs.compute = GetComponent<SlotCompute>();
		refs.lines = GetComponent<SlotLines>();

		if (betsPerLine.Count == 0)
		{
			betsPerLine.Add (new BetsWrapper());
			betsPerLine[0].value = 1;
			betsPerLine[0].canBet = true;
		}

		if (displayLineBoxes == 0)	displayLineBoxes = numberOfReels;

		DOTween.Init();
	}
	void Start() {
		state = SlotState.ready;
	}

	void createReelGameObjects()
	{
		for (int reelNumber = 0; reelNumber < numberOfReels; reelNumber++)
		{
			GameObject reel = new GameObject("Reel" + (reelNumber + 1));
			reel.SetActive(false);
			reel.AddComponent(typeof(SlotReel));
			reel.GetComponent<SlotReel>().reelIndex = (reelNumber + 1);
			reel.transform.parent = this.transform;
			//reel.transform.localRotation = this.transform.rotation;
			reel.SetActive(true);
			reels[reelNumber] = reel.GetComponent<SlotReel>();
		}
		suppliedResult = null;
		useSuppliedResult = false;
	}
	
	void OnEnable() 
	{
		SlotReel.OnReelDoneSpinning += OnReelDoneSpinning;
	}
	void OnDisable()
	{
		SlotReel.OnReelDoneSpinning -= OnReelDoneSpinning;
		
		releaseGlobalSymbolPool();
		releaseGlobalWinboxPool();
	}
	#endregion

	#region Create Pool
	void releaseGlobalSymbolPool()
	{
		// Release Symbol Pool
		if (usePool)
		{
			foreach(List<GameObject> list in pool)
			{
				foreach(GameObject obj in list)
					Destroy (obj);
				list.Clear ();
			}
			Destroy(poolContainer);
		}
	}

	void releaseGlobalWinboxPool()
	{
		// Release Symbol Pool
		if (usePool)
		{
			foreach(List<GameObject> list in poolWinbox)
			{
				foreach(GameObject obj in list)
					Destroy (obj);
				list.Clear ();
			}
			Destroy(poolWinboxContainer);
		}
	}

	void createGlobalSymbolPool()
	{
		poolContainer = new GameObject("_SymbolPool");
		poolContainer.transform.parent = transform;
		pool = new List<GameObject>[symbolPrefabs.Count];
		for(int prefabIndex = 0; prefabIndex < symbolPrefabs.Count; prefabIndex++)
		{
			pool[prefabIndex] = new List<GameObject>();
			for (int prefabCount = 0; prefabCount < (numberOfReels * reelHeight); prefabCount++)
			{
				if (symbolPrefabs[prefabIndex] == null) 
				{
					logConfigError(SlotErrors.MISSING_SYMBOL);
					return;
				}
				GameObject symb = (GameObject)Instantiate(symbolPrefabs[prefabIndex]);
				symb.GetComponent<SlotSymbol>().symbolIndex = prefabIndex;

				symb.SetActive(false);
				pool[prefabIndex].Add (symb);
				symb.transform.parent = poolContainer.transform;
				symb.transform.localScale = Vector3.Scale(symb.transform.localScale, transform.localScale);

				if (symbolBgPrefabs.Count > prefabIndex)
				if (symbolBgPrefabs[prefabIndex] != null)
				{
					GameObject reelbkg = (GameObject)Instantiate(symbolBgPrefabs[prefabIndex]);
					reelbkg.transform.parent = symb.transform;
					reelbkg.transform.localScale = Vector3.Scale(reelbkg.transform.localScale, transform.localScale);
					//reelbkg.transform.localRotation = transform.rotation;
				}
				//Debug.Log ("created " + prefabIndex + " symbol #" + prefabCount);
			}
		}
		
	}

	void createGlobalWinboxPool()
	{
		poolWinboxContainer = new GameObject("_SymbolWinboxPool");
		poolWinboxContainer.transform.parent = transform;
		poolWinbox = new List<GameObject>[winboxPrefabs.Count];

		int numberOfPrefabsToPool = 0;
		foreach(PaysWrapper c in setPays)
		{
			if (c.pays.Count > numberOfPrefabsToPool)
				numberOfPrefabsToPool = c.pays.Count;
		}

		for(int prefabIndex = 0; prefabIndex < winboxPrefabs.Count; prefabIndex++)
		{
			poolWinbox[prefabIndex] = new List<GameObject>();
			//int numberOfPrefabsToPool = numberOfReels;
			//if (prefabIndex > symbolSets.Count - 1) continue;
			//if (symbolSets[prefabIndex].typeofSet == SetsType.scatter)
			//	numberOfPrefabsToPool = setPays[prefabIndex].pays.Count;
			for (int prefabCount = 0; prefabCount < numberOfPrefabsToPool; prefabCount++)
			{
				if (winboxPrefabs[prefabIndex] == null) 
				{
					continue;
					//logConfigError(SlotErrors.MISSING_SYMBOL);
					//return;
				}
				// MEMORY SAVING
				if ((prefabIndex > 0) && (winboxPrefabs[prefabIndex-1] == winboxPrefabs[prefabIndex])) 
				{
					poolWinbox[prefabIndex] = poolWinbox[prefabIndex-1];
					continue;
				}
				GameObject winb = (GameObject)Instantiate(winboxPrefabs[prefabIndex]);
				//winb.GetComponent<SlotSymbol>().symbolIndex = prefabIndex;
				winb.SetActive(false);
				poolWinbox[prefabIndex].Add (winb);
				winb.transform.parent = poolWinboxContainer.transform;
				winb.transform.localScale = Vector3.Scale(winb.transform.localScale, transform.localScale);
			}
		}
		
	}
	internal GameObject getSymbolFromPool(int symbolIndex)
	{
		GameObject symbol = pool[symbolIndex][0];
		pool[symbolIndex].RemoveAt (0);
		symbol.transform.parent = null;
		symbol.SetActive(true);
		return symbol;
	}
	internal void returnSymbolToPool(GameObject symbol)
	{
		if (OnSymbolReturningToPool != null)
			OnSymbolReturningToPool(symbol);

		pool[symbol.GetComponent<SlotSymbol>().symbolIndex].Add (symbol);
		symbol.transform.parent = poolContainer.transform;
		symbol.SetActive(false);
	}

	internal GameObject getWinboxFromPool(int symbolIndex)
	{
		GameObject winbox = poolWinbox[symbolIndex][0];
		poolWinbox[symbolIndex].RemoveAt (0);
		winbox.transform.parent = null;
		winbox.SetActive(true);
		return winbox;
	}
	internal void returnWinboxToPool(GameObject winbox, int symbolIndex)
	{
		//if (OnSymbolReturningToPool != null)
		//	OnSymbolReturningToPool(symbol);
		if (winbox.transform.parent.GetComponent<SlotSymbol>() != null)
		{
			symbolIndex = winbox.transform.parent.GetComponent<SlotSymbol>().symbolIndex;
			poolWinbox[symbolIndex].Add (winbox);
		}
		winbox.transform.parent = poolWinboxContainer.transform;
		winbox.transform.position = new Vector2(1000,1000);
		winbox.SetActive(false);
	}
	#endregion

	#region Debug

	internal void log(string txt)
	{
		if (debugMode)
			Debug.Log(txt);
	}
	internal void logError(string txt)
	{
		if (debugMode)
			Debug.LogError(txt);
	}
	internal void logConfigError(string txt)
	{
		if (debugMode)
			Debug.LogError(txt);
	}

	#endregion
	#region State Function
	/// <summary>
	/// Sets the state of slot machine.
	/// You shouldn't really have to touch this manually most of the time.
	/// </summary>
	/// <param name="newState">supply the new state.</param>
	public void setState(SlotState newState)
	{
		if (OnSlotStateChangeFrom != null)
			OnSlotStateChangeFrom(state);
		state = newState;
		if (OnSlotStateChangeTo != null)
			OnSlotStateChangeTo(newState);
	}

	internal int getSymbolCountCurrentlyTotal(int index)
	{
		int count = 0;
		
		for (int i = 0; i < reels.Count; i++)
		{
			foreach(GameObject symbol in reels[i].symbols)
			{
				if (symbol.GetComponent<SlotSymbol>().symbolIndex == index)
					count++;
			}
		}
		return count;
	}

	#endregion

	#region Slot Actions
	/// <summary>
	/// Spins with a supplied result array.
	/// It is a 2 demensional array which represents the reels and stopped positions.
	/// -1 will fall back to a random symbol for that spot
	/// </summary>
	/// <param name="result">The result array</param>
	/// <param name="freeSpin">If set to <c>true</c> the bet will not be subtracted (free spin)</param>
	public void spinWithResult(int[,] result, bool freeSpin = false)
	{
		suppliedResult = result;
		useSuppliedResult = true;
		spin ();
	}
	/// <summary>
	/// Spins the wait for result.
	/// </summary>
	/// <param name="freeSpin">If set to <c>true</c> the bet will not be subtracted.</param>
	public void spinWaitForResult(bool freeSpin = false) 
	{
		waitingForResult = true;
		useSuppliedResult = true;
		spin (freeSpin);
	}

	/// <summary>
	/// This is the second half of spinWaitForResult
	/// </summary>
	/// <param name="result"></param>
	public void supplyResultToSpin(int[,] result) 
	{
		if (!waitingForResult) {
			Debug.LogError("Call spinWaitForResult first");
		}
		suppliedResult = result;
		waitingForResult = false;
	}
	
	/// <summary>
	/// Resets the frozen positions array
	/// </summary>
	public void resetFrozenPositions()
	{
		frozenPositions = new int[,] { { -1, -1, -1, -1 }, { -1, -1, -1, -1 }, { -1, -1, -1, -1 }, { -1, -1, -1, -1 }, { -1, -1, -1, -1 } };
	}

	/// <summary>
	/// Launch a normal spin
	/// </summary>
	/// <param name="freeSpin">If set to <c>true</c> the bet will not be subtracted (free spin).</param>
	public void spin(bool freeSpin = false) 
	{

		switch (state)
		{
		case SlotState.ready:

			bool doSpin = false; 

			if (!freeSpin) doSpin = GetComponent<SlotCredits>().placeBet();


			if (doSpin || freeSpin)
			{
				if (OnSpinBegin != null)
					OnSpinBegin(GetComponent<SlotWins>().currentWin);


				GetComponent<SlotWins>().reset();

				foreach(SetsWrapper set in symbolSets) set.scatterCount = 0;
				 
				for (int reelIndex = 0; reelIndex < reels.Count; reelIndex++) 
				{
					reels[reelIndex].GetComponent<SlotReel>().spinReel(spinTime + (spinTimeIncPerReel * reels[reelIndex].reelIndex - 1));
				}

				setState(SlotState.spinning);
			} else {
				if (OnSpinInsufficentCredits != null)
					OnSpinInsufficentCredits();
			}
		break;
		case SlotState.spinning:
			snap();
			break;
		case SlotState.snapping:
			break;
		case SlotState.playingwins:

			setState(SlotState.ready);
			spin (freeSpin);
			break;
		}
	}

	void snap()
	{
		if (state != SlotState.spinning) return;

		if (OnSpinSnap != null)
			OnSpinSnap();

		setState(SlotState.snapping);

		for (int i = 0; i < reels.Count; i++)
		{
			reels[i].GetComponent<SlotReel>().snapReel();
		}
	}

	public int[,] getResultArray()
	{
		int[,] result = new int[reels.Count, reels[0].symbols.Count - (reelIndent * 2)];

		string resultArray = "";
		
		for(int i = 0; i < reels.Count; i++)
		{
			for (int ii = 0; ii < reels[0].symbols.Count - (reelIndent * 2); ii++)
			{
				result[i,ii] = reels[i].symbols[reelIndent + ii].GetComponent<SlotSymbol>().symbolIndex;
				resultArray += result[i,ii] + ",";
			}
			resultArray += "\r\n";
		}
		return result;
	}

	void calculateWins()
	{
		useSuppliedResult = false;

		SlotWinSpin slotWinSpin = GetComponent<SlotCompute>().calculateAllLinesWins();
		
		if (OnSpinDone != null)
			OnSpinDone(slotWinSpin.totalWonAdjusted, GetComponent<SlotCredits>().totalBet());
		if (slotWinSpin.totalWonAdjusted > 0)
		{
			setState(SlotState.playingwins);
			GetComponent<SlotCredits>().awardWin(slotWinSpin.totalWonAdjusted);
		} else {
			setState(SlotState.ready);
			if (OnSpinDoneNoWins != null)
				OnSpinDoneNoWins();
		}

		if (OnAllWinsComputed != null)
			OnAllWinsComputed(slotWinSpin, slotWinSpin.totalWonAdjusted / refs.credits.totalBet());
	}

	#endregion

	#region Callback Passthrus

	internal void computedWinLine(SlotWinData data)
	{
		if (OnLineWinComputed != null)
			OnLineWinComputed(data);
	}
	internal void displayedWinLine(SlotWinData data, bool isFirstLoop)
	{
		if (OnLineWinDisplayed != null)
			OnLineWinDisplayed(data, isFirstLoop);
	}
	internal void reelLanded(int reelIndex)
	{
		if (OnReelLand != null)
			OnReelLand(reelIndex);
	}
	internal void scatterSymbolLanded(GameObject symbol, int scatterCount)
	{
		if (OnScatterSymbolLanded != null)
			OnScatterSymbolLanded(symbol, scatterCount);
	}
	internal void scatterSymbolHit(SlotScatterHitData hit)
	{
		if (OnScatterSymbolHit != null)
			OnScatterSymbolHit(hit);
	}
	internal void beginCreditWinCountOff(int totalWin)
	{
		if (OnBeginCreditWinCountOff != null)
			OnBeginCreditWinCountOff(totalWin);
	}
	internal void beginCreditBonusCountOff(int totalWin)
	{
		if (OnBeginCreditBonusCountOff != null)
			OnBeginCreditBonusCountOff(totalWin);
	}
	internal void completedBonusCreditCountOff()
	{
		if (OnCompletedBonusCreditCountOff != null)
			OnCompletedBonusCreditCountOff();
	}
	internal void completedCreditCountOff(int totalWin)
	{
		if (OnCompletedCreditCountOff != null)
			OnCompletedCreditCountOff(totalWin);
	}
	internal void beginDelayOnWin(SlotWinData data)
	{
		if (OnBeginDelayBetweenLineWinDisplayed != null)
			OnBeginDelayBetweenLineWinDisplayed(data);
	}
	internal void anticipationScatterBegin(SlotScatterHitData hit)
	{
		if (OnAnticipationScatterBegin != null)
			OnAnticipationScatterBegin(hit);
	}
	internal void completedWinDisplayCycle(int count)
	{
		if (OnWinDisplayedCycle != null)
			OnWinDisplayedCycle(count);
	}
	internal void incrementedBet(int bet)
	{
		if (OnIncrementBetPerLine != null)
			OnIncrementBetPerLine(bet);
		
	}
	internal void decrementedBet(int bet)
	{
		if (OnDecrementBetPerLine != null)
			OnDecrementBetPerLine(bet);
		
	}
	internal void incrementedLinesPlayed(int linesPlayed)
	{
		if (OnIncrementLinesPlayed != null)
			OnIncrementLinesPlayed(linesPlayed);
		
	}
	internal void decrementedLinesPlayed(int linesPlayed)
	{
		if (OnDecrementLinesPlayed != null)
			OnDecrementLinesPlayed(linesPlayed);
		
	}
	internal void linkedSymbolLanded(int reel, string identifier) {
		if (OnLinkedSymbolLanded != null)
			OnLinkedSymbolLanded(reel, identifier);
	}
	#endregion

	#region Internal Callbacks
	void OnReelDoneSpinning(int reelIndex)
	{
		if (reelIndex == reels.Count)
		{
			calculateWins();
		}

	}
	
	#endregion

}
