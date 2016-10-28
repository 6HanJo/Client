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


[CustomEditor(typeof(IgnoreTagModifier)), CanEditMultipleObjects]
public class IgnoreTagModifierInspector : Editor
{
	protected SerializedProperty debugLevel;
	protected SerializedProperty ignoreList;


    protected void OnEnable()
	{
		this.debugLevel = this.serializedObject.FindProperty("debugLevel");
		this.ignoreList = this.serializedObject.FindProperty("ignoreList");
    }

	public override void OnInspectorGUI()
    {
		this.serializedObject.Update();
		Object[] targetObjs = this.serializedObject.targetObjects;

        IgnoreTagModifier curEditTarget;

		GUIContent content;

		EditorGUIUtility.labelWidth = PGEditorUtils.CONTROLS_DEFAULT_LABEL_WIDTH;

		EditorGUI.indentLevel = 1;

        GUILayout.Space(6);
		
		
		if (this.serializedObject.isEditingMultipleObjects)
		{
			content = new GUIContent("Tags to Ignore", "List of tags to ignore.");
			EditorGUILayout.PropertyField(this.ignoreList, content, true);
		}
		else
		{
			curEditTarget = (IgnoreTagModifier)targetObjs[0];
			
			// Display and get user changes
	        PGEditorUtils.FoldOutTextList
	        (
	            "Tags to ignore",
	            curEditTarget.ignoreList,
				true  // Force the fold-out state
	        );
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
