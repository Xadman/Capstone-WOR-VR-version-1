// Brad Lima - 11/2019
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System;
using System.Linq;

public class SlotReel : MonoBehaviour {

	public static event Action<int> OnReelDoneSpinning;

	public int reelIndex;
	public float speed;

	public List<int> ReelStrip = new List<int>();
	private int reelStripIndex;

	[HideInInspector]
	public List<GameObject> symbols = new List<GameObject>();
	List<Transform> symbolsParents = new List<Transform>();
	List<Transform> symbolsChildren = new List<Transform>();

	private bool snapped = false;
	private bool stopped = false;

	private List<int> cumulativeFrequencyList = new List<int>();
	private int totalFrequency;

	public float symbolHeight;
	public float symbolWidth;
	private float heightOffset;
	private float widthOffset;
	private float symbolPadding;
	private float reelPadding;

	Slot slot;

	private bool anticipation;

	private int symbolsSpinRemaining;
	#region Config
	void Awake ()
	{
	}
	void Start () 
	{

	}

	void OnEnable()
	{

		slot = transform.parent.gameObject.GetComponent<Slot>();

		if (slot.symbolPrefabs.Count == 0)
		{
			slot.logConfigError(SlotErrors.NO_SYMBOLS);
			return;
		}


		cacheSymbolFrequency();

		if (slot.symbolPrefabs[0] == null) return;
		GameObject symb = (GameObject)Instantiate(slot.symbolPrefabs[0]);

		symb.transform.localScale = Vector3.Scale(symb.transform.localScale, transform.parent.transform.localScale);
		//symb.transform.localRotation = transform.parent.transform.rotation;
		if (symb.GetComponent<SpriteRenderer>())
		{
			Vector3 size = symb.GetComponent<SpriteRenderer>().sprite.bounds.size;
			symbolHeight = size.y * symb.transform.localScale.y;
			symbolWidth = size.x * symb.transform.localScale.x;
		} else
		if (symb.GetComponent<MeshFilter>())
		{
			symbolHeight = symb.GetComponent<MeshFilter>().mesh.bounds.size.y * slot.transform.localScale.y;
			symbolWidth = symb.GetComponent<MeshFilter>().mesh.bounds.size.x * slot.transform.localScale.y;
		} else {
			slot.logConfigError(SlotErrors.MISSING_RENDERER);
			return;
		}

		reelPadding = symbolWidth * (slot.horizontalReelPaddingPercent / 100.0f);
		symbolPadding = symbolHeight * (slot.verticalSymbolPaddingPercent / 100.0f);

		heightOffset = -transform.parent.transform.localPosition.y + (symbolHeight * (slot.reelHeight / 2) + (symbolPadding * (slot.reelHeight / 2)));
		widthOffset = -transform.parent.transform.localPosition.x + (symbolWidth * (slot.numberOfReels / 2) + (reelPadding * (slot.numberOfReels / 2)));
		Destroy (symb);
		
		transform.localPosition = new Vector3((-widthOffset + (symbolWidth * (reelIndex - 1)) + (reelPadding * (reelIndex - 1))),transform.localPosition.x, transform.localPosition.z);
		
		if (!slot.useSuppliedResult)
		{
			createReelSymbols();
		}
		else 
		{
			for (int i = 0; i < slot.reelHeight; i++)
			{
				createReelSymbolStartup(i);
			}
		}
	}
	#endregion

	#region Create Reel
	public void createReelSymbols()
	{
		for (int i = 0; i < slot.reelHeight; i++)
		{
			createSymbol(i);
		}	
	}

	void cacheSymbolFrequency()
	{
		cumulativeFrequencyList.Clear();
		totalFrequency = 0;
		for (int index = 0; index < slot.symbolFrequencies.Count; index++)
		{
			if (slot.symbolPrefabs[index] == null) continue;
			while (slot.symbolInfo.Count <= index) slot.symbolInfo.Add (new SlotSymbolInfo());
			SlotSymbolInfo symbol = slot.symbolInfo[index];
			if (symbol.perReelFrequency)
			{
				totalFrequency += slot.reelFrequencies[index].freq[reelIndex-1];
			} else {
				totalFrequency += slot.symbolFrequencies[index];
			}
			cumulativeFrequencyList.Add ( totalFrequency ); 
		}

	}
	int getSymbolCountCurrentlyOnReel(int index)
	{
		int count = 0;
		foreach(GameObject symbol in symbols)
		{
			if (symbol.GetComponent<SlotSymbol>().symbolIndex == index)
				count++;
		}
		return count;
	}


	int lastSelected;

	public int getSymbol()
	{
		// Linked symbol
		int chosen = -1;
		// if the symbol is linked to another, and not the bottom, automatically draw the next link
		if ((slot.symbolInfo[lastSelected].linked) && (slot.symbolInfo[lastSelected].linkPosition != LinkPosition.Top))
		{
			if (slot.symbolInfo[lastSelected].link > 0)
			chosen = slot.symbolInfo[lastSelected].link;
			lastSelected = chosen;
			return chosen;
		}

		while (chosen == -1)
		{
			if (ReelStrip.Count > 0)
			{
				chosen = ReelStrip[reelStripIndex];
				reelStripIndex++;
				if (reelStripIndex > ReelStrip.Count - 1)
					reelStripIndex = 0;
			} else {
				uint selectedFrequency = RNGManager.getRandomRange(slot.activeRNG, 1, totalFrequency+1);
				for (int index = 0; index < cumulativeFrequencyList.Count; index++)
				{
					if (selectedFrequency <= cumulativeFrequencyList[index]) { chosen = index; break; }
				}
				if (!slot.symbolInfo[chosen].active) { chosen = -1; continue; }

				int maxPerReel = slot.symbolInfo[chosen].clampPerReel;
				if (maxPerReel > 0)
				{
					if (getSymbolCountCurrentlyOnReel(chosen) >= maxPerReel) { chosen = -1; continue; }
					int maxTotal = slot.symbolInfo[chosen].clampTotal;
					if (maxTotal > 0)
						if (slot.getSymbolCountCurrentlyTotal(chosen) >= maxTotal) chosen = -1;
				}
				// Linked symbols cannot be organically drawn
				if (slot.symbolInfo[chosen].linked)
				{
					if (slot.symbolInfo[chosen].linkPosition != LinkPosition.Bottom)
						chosen = -1;
				}
			}
		}
		

		lastSelected = chosen;
		return chosen;
	}

	void createSymbol(int slotIndex)
	{
		int symbolIndex;
		if (slot.useSuppliedResult)
		{
			if ((symbolsSpinRemaining >= slot.reelIndent) && (symbolsSpinRemaining < (slot.reelHeight - slot.reelIndent)))
			{
				if (slot.suppliedResult[reelIndex-1,symbolsSpinRemaining - slot.reelIndent] > -1)
				{
					symbolIndex = slot.suppliedResult[reelIndex-1,symbolsSpinRemaining - slot.reelIndent];
				} else {
					symbolIndex = getSymbol();
				}
			} else {
				symbolIndex = getSymbol();
			}
		} else {
			if ((symbolsSpinRemaining >= slot.reelIndent) && (symbolsSpinRemaining < (slot.reelHeight - slot.reelIndent)))
			{
				if (slot.frozenPositions[reelIndex-1,symbolsSpinRemaining - slot.reelIndent] > -1)
				{
					symbolIndex = slot.frozenPositions[reelIndex-1,symbolsSpinRemaining - slot.reelIndent];
				} else {
					symbolIndex = getSymbol();
				}
			} else {
				symbolIndex = getSymbol();
			}
		}

		GameObject symb;
		if (slot.usePool) 
		{
			symb = getFromPool(symbolIndex);
		} else {
			symb = (GameObject)Instantiate(slot.symbolPrefabs[symbolIndex]);
			symb.GetComponent<SlotSymbol>().symbolIndex = symbolIndex;
			symb.transform.localScale = Vector3.Scale(symb.transform.localScale, transform.parent.transform.localScale);
			//symb.transform.localRotation = transform.parent.transform.rotation;
		}


		symb.transform.parent = transform;
		if (symb.GetComponent<SpriteRenderer>())
			symb.transform.localPosition = new Vector3(slot.reelCenter.position.x + 0,slot.reelCenter.position.y + (-heightOffset + (slotIndex * symbolHeight - transform.localPosition.y)) + (symbolPadding * slotIndex), 0);
		if (symb.GetComponent<MeshFilter>())
			symb.transform.localPosition = new Vector3(slot.reelCenter.position.x + 0,slot.reelCenter.position.y + (-heightOffset + (slotIndex * symbolHeight - transform.localPosition.y)) + (symbolPadding * slotIndex), 0);

		symbols.Insert(0,symb);
	}

	void createReelSymbolStartup(int slotIndex)
	{
		int symbolIndex = -1;
		if ((slotIndex >= slot.reelIndent) && (slotIndex < (slot.reelHeight - slot.reelIndent)))
		{
			symbolIndex = slot.suppliedResult[reelIndex-1,slotIndex-slot.reelIndent];
		} else {
			symbolIndex = getSymbol();
		}
		GameObject symb;
		if (slot.usePool) 
		{
			symb = getFromPool(symbolIndex);
		} else {
			symb = (GameObject)Instantiate(slot.symbolPrefabs[symbolIndex]);
			symb.GetComponent<SlotSymbol>().symbolIndex = symbolIndex;
			symb.transform.localScale = Vector3.Scale(symb.transform.localScale, transform.parent.transform.localScale);
		}

		symb.transform.parent = transform;
		if (symb.GetComponent<SpriteRenderer>())
			symb.transform.localPosition = new Vector3(slot.reelCenter.position.x + 0,slot.reelCenter.position.y + (-heightOffset + (slotIndex * symbolHeight - transform.localPosition.y)) + (symbolPadding * slotIndex), 0);
		if (symb.GetComponent<MeshFilter>())
			symb.transform.localPosition = new Vector3(slot.reelCenter.position.x + 0,slot.reelCenter.position.y + (-heightOffset + (slotIndex * symbolHeight - transform.localPosition.y)) + (symbolPadding * slotIndex), 0);

		symbols.Insert(0,symb);
	}
	#endregion

	#region Actions
	public void spinReel(float spinTime)
	{
		retatchBackgrounds();
		cacheSymbolFrequency();

		snapped = false;
		stopped = false;
		speed = slot.spinningSpeed;

		symbolsSpinRemaining = (int)(spinTime / speed);
		//Debug.Log(symbolsSpinRemaining);
		//spinTween = HOTween.To (this,spinTime,new TweenParms().Prop("speed", speed).OnComplete(finishSpinning));
		transform.DOLocalMoveY(-symbolHeight, speed).OnComplete(OnNextSymbol).SetRelative(true).SetEase(Ease.InOutBounce);
		//DOTween.To (()=> transform,x => transform = x, new Vector3(0,-symbolHeight,0), speed).OnComplete(OnNextSymbol).SetEase(Ease.Linear);
		//HOTween.To (transform,speed,new TweenParms().Prop ("position", new Vector3(0,-symbolHeight,0), true).OnComplete(OnNextSymbol).Ease (EaseType.Linear));
	}

	public void snapReel()
	{
		Debug.Log("waiting: " + slot.waitingForResult);
		if (snapped) return;
		if (slot.waitingForResult) return;

		snapped = true;
		symbolsSpinRemaining = (int)(slot.reelHeight - slot.reelIndent);
	}
	#endregion

	#region Symbol Misc
	GameObject getFromPool(int symbolIndex)
	{
		GameObject symbol = slot.getSymbolFromPool(symbolIndex);
		return symbol;
	}
	void returnToPool()
	{
		if (symbols.Count == 0)
		{
			slot.logConfigError(SlotErrors.NO_SYMBOLS);
			return;
		}
		GameObject symb = symbols[symbols.Count-1];

		slot.returnSymbolToPool(symb);
	}

	void detatchBackgrounds()
	{
		foreach(GameObject symbol in symbols)
		{
			
			foreach(Transform child in symbol.transform) {
				symbolsParents.Add (child.parent);
				child.parent = child.parent.transform.parent;
				symbolsChildren.Add(child);
			}
		}
	}

	void retatchBackgrounds()
	{
		for(int i = 0; i < symbolsChildren.Count; i++) symbolsChildren[i].parent = symbolsParents[i];
		symbolsChildren.Clear();
		symbolsParents.Clear() ;
	}

	#endregion

	#region DOTween Callbacks
	void OnNextSymbol()
	{
		if (symbols.Count == 0)
		{
			slot.logConfigError(SlotErrors.NO_SYMBOLS);
			return;
		}

		if (slot.usePool) returnToPool();
		else 
			Destroy (symbols[symbols.Count-1]);
		symbols.RemoveAt(symbols.Count-1);

		createSymbol(slot.reelHeight-1);

		symbolsSpinRemaining--;
		if (symbolsSpinRemaining == 0) {
			stopped = true;
		}

		if (slot.waitingForResult) {
			symbolsSpinRemaining = slot.reelHeight;
			stopped = false;
		}

		//if (reelIndex == 3)
		//	Debug.Log("remaining=" + symbolsSpinRemaining);

		if (stopped)
		{
			transform.DOLocalMoveY(-symbolHeight - symbolPadding, slot.easeOutTime).SetRelative(true).OnComplete(OnReelStopped).SetEase(slot.reelEase);
			//HOTween.To (transform,slot.easeOutTime,new TweenParms().Prop ("position", new Vector3(0,-symbolHeight,0), true).OnComplete(OnReelStopped).Ease (slot.reelEase));
			Invoke("checkScatterLanded", slot.easeOutTime / 2.0f);
			slot.reelLanded(reelIndex);

		} else {
			if (anticipation)
			{
				transform.DOLocalMoveY(-symbolHeight - symbolPadding, speed/2.0f).SetRelative(true).OnComplete(OnNextSymbol).SetEase(Ease.Linear);
				//HOTween.To (transform,speed/2.0f,new TweenParms().Prop ("position", new Vector3(0,-symbolHeight,0), true).OnComplete(OnNextSymbol).Ease (EaseType.Linear));
			} else {
				transform.DOLocalMoveY(-symbolHeight - symbolPadding, speed).SetRelative(true).OnComplete(OnNextSymbol).SetEase(Ease.Linear);
				//if ((symbolsSpinRemaining % 10) == 0)
				//	Debug.Log(":: " + reelIndex + " :: " + symbolsSpinRemaining);
				//HOTween.To (transform,speed,new TweenParms().Prop ("position", new Vector3(0,-symbolHeight,0), true).OnComplete(OnNextSymbol).Ease (EaseType.Linear));
			}
		}
	}

	void finishSpinning()
	{
		stopped = true;
	}

	void OnReelStopped()
	{
		if (slot.usePool) returnToPool();
		else 
			Destroy (symbols[symbols.Count-1]);
		symbols.RemoveAt(symbols.Count-1);

		createSymbol(slot.reelHeight-1);

		if (OnReelDoneSpinning != null)
			OnReelDoneSpinning(reelIndex);

		detatchBackgrounds();

		inlineScatterCalc();

		slot.reels[reelIndex-1].anticipation = false;
		
	}
	
	void checkScatterLanded()
	{

		for(int currentSymbolSetIndex = 0; currentSymbolSetIndex < slot.symbolSets.Count; currentSymbolSetIndex++)
		{
			
			SetsWrapper currentSet = slot.symbolSets[currentSymbolSetIndex];
			if (currentSet.typeofSet != SetsType.scatter) continue;
			foreach(int symbol in currentSet.symbols)
			{
				for (int i = slot.reelIndent - 1; i < (slot.reelHeight - slot.reelIndent) - 1; i++)
				{
					int reelSymbolIndex = symbols[i].GetComponent<SlotSymbol>().symbolIndex;
					if (reelSymbolIndex == symbol)
					{
						slot.scatterSymbolLanded(symbols[i], currentSet.scatterCount + 1);
					}
				}
			}
		}
	}

	void inlineScatterCalc()
	{
		for(int currentSymbolSetIndex = 0; currentSymbolSetIndex < slot.symbolSets.Count; currentSymbolSetIndex++)
		{
			
			SetsWrapper currentSet = slot.symbolSets[currentSymbolSetIndex];
			if (currentSet.typeofSet != SetsType.scatter) continue;
			int matches = 0;
			SlotScatterHitData hit = new SlotScatterHitData(reelIndex);

			foreach(int symbol in currentSet.symbols)
			{
				for (int i = slot.reelIndent; i < (slot.reelHeight - slot.reelIndent); i++)
				{
					int reelSymbolIndex = symbols[i].GetComponent<SlotSymbol>().symbolIndex;
					if (reelSymbolIndex == symbol)
					{
						currentSet.scatterCount++;
						matches++;
						hit.hits = currentSet.scatterCount;
						hit.setIndex = currentSymbolSetIndex;
						hit.setType = SetsType.scatter;
						hit.setName = slot.symbolSetNames[hit.setIndex];
						hit.symbol = symbols[i];
						slot.scatterSymbolHit(hit);
					}
				}
			}
			// anticipation
			if (currentSet.scatterCount > 0)
			{

				if ((currentSet.scatterCount < slot.numberOfReels) && (reelIndex < slot.numberOfReels))
				{
					if ((slot.setPays[currentSymbolSetIndex].anticipate[currentSet.scatterCount-1]) == true)
					{
						slot.reels[reelIndex].anticipation = true;
						for (int i = reelIndex; i < slot.numberOfReels; i++)
						{
							//Debug.Log("anticipate reel:" + i);
							slot.reels[i].symbolsSpinRemaining += 60;

							slot.anticipationScatterBegin(hit);
						}
					}
				}
			}
		}
	}
	#endregion

}
