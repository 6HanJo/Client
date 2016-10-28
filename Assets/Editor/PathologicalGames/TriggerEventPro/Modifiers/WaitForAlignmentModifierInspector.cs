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
using System.Collections.Generic;
using PathologicalGames;


[CustomEditor(typeof(WaitForAlignmentModifier)), CanEditMultipleObjects]
public class WaitForAlignmentModifierInspector : Editor
{
	protected SerializedProperty debugLevel;
	protected SerializedProperty origin;
	protected SerializedProperty angleTolerance;
	protected SerializedProperty flatAngleCompare;
	protected SerializedProperty flatCompareAxis;

    protected void OnEnable()
	{
		this.debugLevel       = this.serializedObject.FindProperty("debugLevel");
		this.origin           = this.serializedObject.FindProperty("_origin");
		this.angleTolerance   = this.serializedObject.FindProperty("angleTolerance");
		this.flatAngleCompare = this.serializedObject.FindProperty("flatAngleCompare");
		this.flatCompareAxis  = this.serializedObject.FindProperty("flatCompareAxis");
    }

	public override void OnInspectorGUI()
    {
		this.serializedObject.Update();
		Object[] targetObjs = this.serializedObject.targetObjects;

        WaitForAlignmentModifier curEditTarget;

		GUIContent content;

		EditorGUIUtility.labelWidth = PGEditorUtils.CONTROLS_DEFAULT_LABEL_WIDTH;

		EditorGUI.indentLevel = 0;
		
		
		EditorGUI.BeginChangeCheck();
		content = new GUIContent
		(
			"Origin", 
			"The transform used to determine alignment. If not used, this will be the transform " +
			"of the GameObject this component is attached to."
		);
		EditorGUILayout.PropertyField(this.origin, content);		
		
		// If changed, trigger the property setter for all objects being edited
		if (EditorGUI.EndChangeCheck())
		{
			for (int i = 0; i < targetObjs.Length; i++)
			{
				curEditTarget = (WaitForAlignmentModifier)targetObjs[i];

				Undo.RecordObject(curEditTarget, targetObjs[0].GetType() + " origin");
				
				curEditTarget.origin = (Transform)this.origin.objectReferenceValue;
			}
		}
		
        EditorGUILayout.BeginHorizontal();
		EditorGUIUtility.labelWidth = 106;

		content = new GUIContent
		(
			"Angle Tolerance",
			"If waitForAlignment is true: If the emitter is pointing towards " +
				"the target within this angle in degrees, the target can be fired on."
		);
		EditorGUILayout.PropertyField(this.angleTolerance, content);		
		EditorGUIUtility.labelWidth = PGEditorUtils.CONTROLS_DEFAULT_LABEL_WIDTH;

		
		content = new GUIContent
		(
			"Flat Comparison",
			"If false the actual angles will be compared for alignment. " +
				"(More precise. Emitter must point at target.)\n" +
				"If true, only the direction matters. " +
				"(Good when turning in a direction but perfect alignment isn't needed.)"
		);
		PGEditorUtils.ToggleButton(this.flatAngleCompare, content, 88);
			
        EditorGUILayout.EndHorizontal();
		
		if (this.flatAngleCompare.boolValue)
		{
			EditorGUI.indentLevel += 1;
        	EditorGUILayout.BeginHorizontal();
			
			GUILayout.FlexibleSpace();  // To make the rest of the row right-justified
			
			content = new GUIContent
			(
				"Flatten Axis",
				"The axis to flatten when comparing angles to see if alignment has occurred. " +
					"For example, for a 2D game, this should be Z. For a 3D game that wants to " +
					"ignore altitude, set this to Y."
			);
	
			EditorGUILayout.PropertyField(this.flatCompareAxis, content, GUILayout.Width(228));	

        	EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel -= 1;
		}

		
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