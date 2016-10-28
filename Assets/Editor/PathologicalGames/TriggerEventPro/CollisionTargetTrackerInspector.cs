/// <Licensing>
/// ©2011-2014 (Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEditor;
using UnityEngine;
using System.Collections;
using PathologicalGames;


[CustomEditor(typeof(CollisionTargetTracker)), CanEditMultipleObjects]
public class CollisionTargetTrackerInspector : Editor
{
	protected SerializedProperty debugLevel;
	protected SerializedProperty numberOfTargets;
	protected SerializedProperty targetLayers;


    protected void OnEnable()
	{
		this.debugLevel = this.serializedObject.FindProperty("debugLevel");
		this.numberOfTargets = this.serializedObject.FindProperty("numberOfTargets");
		this.targetLayers = this.serializedObject.FindProperty("targetLayers");
    }

	public override void OnInspectorGUI()
    {
		this.serializedObject.Update();
		//Object[] targetObjs = this.serializedObject.targetObjects;

		GUIContent content;

		EditorGUIUtility.labelWidth = PGEditorUtils.CONTROLS_DEFAULT_LABEL_WIDTH;

		EditorGUI.indentLevel = 0;
		
		
		content = new GUIContent(
			"Targets (-1 for all)",
			"The number of targets to return. Set to -1 to return all targets"
		);
		EditorGUILayout.PropertyField(this.numberOfTargets, content);
			
		
		content = new GUIContent
		(
			"Target Layers", 
			"The layers in which the Area is allowed to find targets."
		);
		EditorGUILayout.PropertyField(this.targetLayers, content);

		
		GUILayout.Space(4);
			
		content = new GUIContent
		(
			"Debug Level", "Set it higher to see more verbose information."
		);
		EditorGUILayout.PropertyField(this.debugLevel, content);
			
        serializedObject.ApplyModifiedProperties();
			
        // Flag Unity to save the changes to to the prefab to disk
		// 	 This is needed to make the gizmos update immediatly.
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}