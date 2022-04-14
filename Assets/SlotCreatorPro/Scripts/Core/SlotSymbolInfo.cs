using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LinkPosition {
	Top,
	Mid,
	Bottom
}

[System.Serializable]
public class SlotSymbolInfo
{
	List<GameObject> linkedSymbol;
	public bool active = true;
	public int link;
	public string linkName;
	public bool linked;
	public LinkPosition linkPosition;

	public bool isWild;
	public int clampPerReel;
	public int clampTotal;
	public int symbolIndex;
	public bool perReelFrequency;
}
