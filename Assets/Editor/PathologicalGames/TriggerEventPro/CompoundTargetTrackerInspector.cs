/// <Licensing>
/// © 2011 (Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;


[CustomEditor(typeof(CompoundTargetTracker)), CanEditMultipleObjects]
public class CompoundTargetTrackerInspector : Editor
{
	protected SerializedProperty debugLevel;
    protected SerializedProperty numberOfTargets;
	protected SerializedProperty sortingStyle;
	protected SerializedProperty updateInterval;
	protected SerializedProperty targetTrackers;
		
    protected void OnEnable()
    {
		this.debugLevel 	 = this.serializedObject.FindProperty("debugLevel");		
	    this.numberOfTargets = this.serializedObject.FindProperty("numberOfTargets");
		this.sortingStyle 	 = this.serializedObject.FindProperty("_sortingStyle");
		this.updateInterval  = this.serializedObject.FindProperty("updateInterval");
		this.targetTrackers  = this.serializedObject.FindProperty("targetTrackers");
    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();

		GUIContent content;
		
        EditorGUI.indentLevel = 0;
        
		EditorGUIUtility.labelWidth = PGEditorUtils.CONTROLS_DEFAULT_LABEL_WIDTH;

		Object[] targetObjs = this.serializedObject.targetObjects;
		
        CompoundTargetTracker curEditTarget;		

		EditorGUILayout.PropertyField(this.numberOfTargets, new GUIContent("Targets (-1 for all)"));
					
		EditorGUI.BeginChangeCheck();
		content = new GUIContent("Sorting Style", "The style of sorting to use");
		EditorGUILayout.PropertyField(this.sortingStyle, content);
			
		var sortingStyle = (TargetTracker.SORTING_STYLES)this.sortingStyle.enumValueIndex;
		
		// If changed, trigger the property setter for all objects being edited
		if (EditorGUI.EndChangeCheck())  
		{
			for (int i = 0; i < targetObjs.Length; i++)
			{
				curEditTarget = (CompoundTargetTracker)targetObjs[i];

				Undo.RecordObject(curEditTarget, targetObjs[0].GetType() + " sortingStyle");
				
				curEditTarget.sortingStyle = sortingStyle;
			}
		}
				
        if (sortingStyle != TargetTracker.SORTING_STYLES.None)
        {
            EditorGUI.indentLevel += 1;

			content = new GUIContent(
				"Minimum Interval", 
				"How often the target list will be sorted. If set to 0, " +
					"sorting will only be triggered when Targets enter or exit range."
			);
			EditorGUILayout.PropertyField(this.updateInterval, content);
			
            EditorGUI.indentLevel -= 1;
        }

        GUILayout.Space(8);
		
		// If multi-selected, use Unity's display. Much easier in this case.
		if (serializedObject.isEditingMultipleObjects)
		{
			content = new GUIContent
			(
				"TargetTrackers", 
				"The TargetTrackers whos targets will be combined by this CompoundTargetTracker."
			);
			EditorGUILayout.PropertyField(this.targetTrackers, content, true);
		}
		else
		{			
			curEditTarget = (CompoundTargetTracker)targetObjs[0];
		
			// Track changes so we only register undo entries when needed.
			EditorGUI.BeginChangeCheck();
			
			var targetTrackersCopy = new List<TargetTracker>(curEditTarget.targetTrackers);
			PGEditorUtils.FoldOutObjList<TargetTracker>
			(
				"TargetTrackers", 
				targetTrackersCopy, 
				true
			);
			
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(curEditTarget, targetObjs[0].GetType() + " targetTrackers");
				
				curEditTarget.targetTrackers.Clear();
				curEditTarget.targetTrackers.AddRange(targetTrackersCopy);
			}
		}

		GUILayout.Space(4);
		
		EditorGUILayout.BeginHorizontal();
					     
		content = new GUIContent
		(
			"Show All Gizmos", 
			"Click to force all TargetTracker gizmos to be visibile to visualize the Area " + 
				"in the Editor."
		);		
		this.GizmoVisibilityButton(targetObjs, content, true);

		content = new GUIContent
		(
			"Hide All Gizmos", 
			"Click to force all TargetTracker gizmos to be hidden."
		);
		this.GizmoVisibilityButton(targetObjs, content, false);

		EditorGUILayout.EndHorizontal();

		GUILayout.Space(8);
			
		content = new GUIContent("Debug Level", "Set it higher to see more verbose information.");
		EditorGUILayout.PropertyField(this.debugLevel, content);
			
        serializedObject.ApplyModifiedProperties();
			
        // Flag Unity to save the changes to to the prefab to disk
		// 	 This is needed to make the gizmos update immediatly.
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

	protected void GizmoVisibilityButton(Object[] targetObjs, GUIContent content, bool show)
	{
		bool pressed = GUILayout.Button(content);
		if (pressed)
		{
			List<TargetTracker> subtrackers = new List<TargetTracker>();
			for (int i = 0; i < targetObjs.Length; i++)
			{
				var tracker = (CompoundTargetTracker)targetObjs[i];
				subtrackers.AddRange(tracker.targetTrackers);
			}

			foreach (TargetTracker subtracker in subtrackers)
			{
				Undo.RecordObject(subtracker, subtracker.name + " drawGizmo");

				subtracker.drawGizmo = show;
			}
		}
	}
}
