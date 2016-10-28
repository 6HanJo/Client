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


[CustomEditor(typeof(SetConstraintTarget)), CanEditMultipleObjects]
public class SetConstraintTargetInspector : Editor
{
	protected SerializedProperty unityConstraint;
	protected SerializedProperty targetSource;
	
    protected void OnEnable()
	{
		this.unityConstraint = this.serializedObject.FindProperty("unityConstraint");
		this.targetSource = this.serializedObject.FindProperty("_targetSource");
	}

	public override void OnInspectorGUI()
    {
		this.serializedObject.Update();
		Object[] targetObjs = this.serializedObject.targetObjects;

        SetConstraintTarget curEditTarget;

		GUIContent content;

		EditorGUIUtility.labelWidth = PGEditorUtils.CONTROLS_DEFAULT_LABEL_WIDTH;

		EditorGUI.indentLevel = 0;
		
		// Try and init the unityConstrant field. If still null after this. That is OK.
		for (int i = 0; i < targetObjs.Length; i++)
		{
			curEditTarget = (SetConstraintTarget)targetObjs[i];
			if (curEditTarget.unityConstraint == null)
				curEditTarget.unityConstraint = curEditTarget.GetComponent<ConstraintBaseClass>();
		}		
		
		// Try and init the targetSource field. If still null after this. That is OK.
		for (int i = 0; i < targetObjs.Length; i++)
		{
			curEditTarget = (SetConstraintTarget)targetObjs[i];
			if (curEditTarget.targetSource == null)
				curEditTarget.targetSource = curEditTarget.GetComponent<EventFireController>();
			
			if (curEditTarget.targetSource == null)
				curEditTarget.targetSource = curEditTarget.GetComponent<EventTrigger>();
		}		
		
		
		content = new GUIContent
		(
			"Unity Constraint", 
			"A UnityConstraint to set targets for."
		);
		EditorGUILayout.PropertyField(this.unityConstraint, content);
		

		EditorGUI.BeginChangeCheck();
		content = new GUIContent
		(
			"Target Source", 
			"A FireController whose events will set the target of the attached UnityConstraint."
		);
		EditorGUILayout.PropertyField(this.targetSource, content);
		
		serializedObject.ApplyModifiedProperties();
		
		// If changed, trigger the property setter for all objects being edited
		if (EditorGUI.EndChangeCheck())
		{
			for (int i = 0; i < targetObjs.Length; i++)
			{
				curEditTarget = (SetConstraintTarget)targetObjs[i];

				Undo.RecordObject(curEditTarget, targetObjs[0].GetType() + " targetSource");
				
				var obj = (Component)this.targetSource.objectReferenceValue;
				if (obj != null)
				{
					var fireCtrl = obj.GetComponent<EventFireController>();
					if (fireCtrl != null)
					{
						curEditTarget.targetSource = fireCtrl;
					}
					else
					{		
						var eventTrigger = obj.GetComponent<EventTrigger>();
						if (eventTrigger != null)
						{
							curEditTarget.targetSource = eventTrigger;
						}
						else
						{
							curEditTarget.targetSource = null;
							
							Debug.LogError
							(
								"FireController or EventTrigger not found on dropped GameObject."
							);
						}
					}					
				}
				else
				{
					curEditTarget.targetSource = null;
				}
			}
		}
							
        serializedObject.ApplyModifiedProperties();
			
        // Flag Unity to save the changes to to the prefab to disk
		// 	 This is needed to make the gizmos update immediatly.
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
