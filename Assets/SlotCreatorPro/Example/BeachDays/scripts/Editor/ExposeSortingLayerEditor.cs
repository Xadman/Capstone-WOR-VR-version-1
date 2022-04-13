using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Reflection;
using System;

[CustomEditor(typeof(ExposeSortingLayer))]
[Serializable]
public class ExposeSortingLayerEditor : Editor 
{
	
	ExposeSortingLayer psl;
	
	void OnEnable() {
		psl = (ExposeSortingLayer)target;
	}
	public string[] GetSortingLayerNames() {
		Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
		return (string[])sortingLayersProperty.GetValue(null, new object[0]);
	}
	
	public override void OnInspectorGUI() { 
		string[] layers = GetSortingLayerNames();
		for(int i = 0; i < layers.Length; i++)
		{
			if (psl.sortingLayerName == layers[i]) psl.sortingLayerNameIndex = i;
		}
		psl.realTime = EditorGUILayout.Toggle("Update RealTime", psl.realTime);
		psl.sortingLayerNameIndex = EditorGUILayout.Popup("Sorting Layer", psl.sortingLayerNameIndex,GetSortingLayerNames());
		psl.sortingLayerName = layers[psl.sortingLayerNameIndex];
		psl.sortingLayerOrder = EditorGUILayout.IntField("Sorting Order", psl.sortingLayerOrder);

		psl.setSorting();
	}
}