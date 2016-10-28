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


[CustomEditor(typeof(EventInfoListStandalone)), CanEditMultipleObjects]
public class EventInfoListStandaloneInspector : Editor
{
	protected SerializedProperty eventInfoList;

	public bool expandInfo = true;

    protected void OnEnable()
	{
		this.eventInfoList = this.serializedObject.FindProperty("_eventInfoListInspectorBacker");
    }

	public override void OnInspectorGUI()
    {
		this.serializedObject.Update(); 

		GUIContent content;
		EditorGUI.indentLevel = 0;
		EditorGUIUtility.labelWidth = PGEditorUtils.CONTROLS_DEFAULT_LABEL_WIDTH;
		Object[] targetObjs = this.serializedObject.targetObjects;
		EventInfoListStandalone curEditTarget;
				
		content = new GUIContent
		(
			"Event Info List",
			"A list of event descriptions to be passed by struct to Targets"
		);

		if (this.serializedObject.isEditingMultipleObjects)
		{
			EditorGUILayout.PropertyField(this.eventInfoList, content, true);
		}
		else
		{
            GUILayout.Space(6);

            EditorGUI.indentLevel += 2;
			
			curEditTarget = (EventInfoListStandalone)targetObjs[0];
            this.expandInfo = PGEditorUtils.SerializedObjFoldOutList<EventInfoListGUIBacker>
            (
                content.text,
                curEditTarget._eventInfoListGUIBacker,
                this.expandInfo,
                ref curEditTarget._inspectorListItemStates,
                true
            );
			
            EditorGUI.indentLevel -= 2;
		
            GUILayout.Space(4);
		}
		
        serializedObject.ApplyModifiedProperties();
			
        // Flag Unity to save the changes to to the prefab to disk
		// 	 This is needed to make the gizmos update immediatly.
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

}
