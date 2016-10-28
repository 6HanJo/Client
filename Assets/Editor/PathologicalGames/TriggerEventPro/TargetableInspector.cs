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


[CustomEditor(typeof(Targetable)), CanEditMultipleObjects]
public class TargetableInspector : Editor
{
	protected SerializedProperty debugLevel;
	protected SerializedProperty isTargetable;


    protected void OnEnable()
	{
		this.debugLevel   = this.serializedObject.FindProperty("debugLevel");
		this.isTargetable = this.serializedObject.FindProperty("_isTargetable");
    }

	public override void OnInspectorGUI()
    {
		this.serializedObject.Update();

		GUIContent content;
		EditorGUI.indentLevel = 0;
		EditorGUIUtility.labelWidth = PGEditorUtils.CONTROLS_DEFAULT_LABEL_WIDTH;

		Object[] targetObjs = this.serializedObject.targetObjects;
		Targetable curEditTarget;
		
		
		EditorGUI.BeginChangeCheck();
		content = new GUIContent
		(
			"Is Targetable",
			"Indicates whether this object is targetable. If the Targetable is being tracked " +
				"when this is set to false, it will be removed from all Areas. When set to " +
				"true, it will be added to any Perimieters it is inside of, if applicable."
		);
		EditorGUILayout.PropertyField(this.isTargetable, content);

		// If changed, trigger the property setter for all objects being edited
		// Only trigger logic if the game is playing. Otherwise, the backing field setting is enough
		if (EditorGUI.EndChangeCheck() & Application.isPlaying)
		{
			string undo_message = targetObjs[0].GetType() + " isTargetable";			
			for (int i = 0; i < targetObjs.Length; i++)
			{
				curEditTarget = (Targetable)targetObjs[i];

				Undo.RecordObject(curEditTarget, undo_message);
				
				curEditTarget.isTargetable = this.isTargetable.boolValue;
			}
		}
		

		//GUILayout.Space(4);  // Use if more controls are added
			
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