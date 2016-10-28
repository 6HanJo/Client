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


[CustomEditor(typeof(LineOfSightModifier)), CanEditMultipleObjects]
public class LineOfSightModifierInspector : Editor
{
	protected SerializedProperty debugLevel;
	protected SerializedProperty fireControllerLayerMask;
	protected SerializedProperty targetTrackerLayerMask;
	protected SerializedProperty testMode;

    protected void OnEnable()
	{
		this.debugLevel = this.serializedObject.FindProperty("debugLevel");
		this.fireControllerLayerMask = this.serializedObject.FindProperty("fireControllerLayerMask");
		this.targetTrackerLayerMask = this.serializedObject.FindProperty("targetTrackerLayerMask");
		this.testMode = this.serializedObject.FindProperty("testMode");
	}

	public override void OnInspectorGUI()
    {
		this.serializedObject.Update();
		Object[] targetObjs = this.serializedObject.targetObjects;

        LineOfSightModifier curEditTarget;

		GUIContent content;

		EditorGUIUtility.labelWidth = PGEditorUtils.CONTROLS_DEFAULT_LABEL_WIDTH;

		EditorGUI.indentLevel = 1;

        // Display some information
        GUILayout.Space(6);
		
		EditorGUILayout.HelpBox
		(
            "Add layers of obsticles to activate LOS filtering.\n" +
                "  - Target Tracker to ignore targets\n" +
                "  - Fire Controller to hold fire\n" + 
				"If any options don't appear it is because this GameObject doesn't have " +
				"one or both components.", 
			MessageType.None
		);            
		
		// Try to do some setup automation and track some info for display below
		bool anyHaveTracker = false;
		bool anyHaveFireCtrl = false;
		bool allHaveTracker = true;
		bool allHaveFireCtrl = true;
		for (int i = 0; i < targetObjs.Length; i++)
		{
			curEditTarget = (LineOfSightModifier)targetObjs[i];
			
			// Attempt init...
			if (curEditTarget.tracker == null)
				curEditTarget.tracker = curEditTarget.GetComponent<AreaTargetTracker>();
			
			if (curEditTarget.fireCtrl == null)
				curEditTarget.fireCtrl = curEditTarget.GetComponent<EventFireController>();
			
			// Track what exists...
			if (curEditTarget.tracker != null)
				anyHaveTracker = true;
			else
				allHaveTracker = false;
			
			if (curEditTarget.fireCtrl != null)
				anyHaveFireCtrl = true;
			else
				allHaveFireCtrl = false;
		}
				
		// Leave a blank GUI if BOTH are still missing
		if (!anyHaveTracker && !anyHaveFireCtrl)
		{
			EditorGUILayout.HelpBox
			(
	            "Add a FireController or TargetTracker to see options.", 
				MessageType.Warning
			);
			
			return;
		}
		
		if (anyHaveTracker)
		{
			content = new GUIContent
			(
				"Target Tracker Mask", 
				"Layers of obsticales to block line-of-sight."
			);
			EditorGUILayout.PropertyField(this.targetTrackerLayerMask, content);		
		
			if (!allHaveTracker)
			{
	            EditorGUI.indentLevel += 1;
				
				EditorGUILayout.HelpBox
				(
		            "Multi-Edit Note: 1 or more selected GameObjects do not have a TargetTracker.\n" +
		            "This is just a note, not an error. The option above will still apply to " +
		            "GameObjects which do have a TargetTracker", 
					MessageType.Info
				);
	
				EditorGUI.indentLevel -= 1;
			}
		}	
		
        if (anyHaveFireCtrl)
		{
			content = new GUIContent
			(
				"Fire Controller Mask", 
				"Layers of obsticales to block line-of-sight."
			);
			EditorGUILayout.PropertyField(this.fireControllerLayerMask, content);		

			if (!allHaveFireCtrl)
			{
	            EditorGUI.indentLevel += 1;
	
				EditorGUILayout.HelpBox
				(
		            "Multi-Edit Note: 1 or more selected GameObjects do not have a FireController.\n" +
		            "This is just a note, not an error. The option above will still apply to " +
		            "GameObjects which do have a FireController", 
					MessageType.Info
				);
	
				EditorGUI.indentLevel -= 1;
			}
		}

        GUILayout.Space(6);

		content = new GUIContent
			(
				"LOS Test Mode", 
				"Choose a test mode."
				);
		EditorGUILayout.PropertyField(this.testMode, content);

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