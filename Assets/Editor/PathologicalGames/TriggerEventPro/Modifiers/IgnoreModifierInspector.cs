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


[CustomEditor(typeof(IgnoreModifier)), CanEditMultipleObjects]
public class IgnoreModifierInspector : Editor
{
	protected SerializedProperty debugLevel;
	protected SerializedProperty ignoreList;
	
    protected void OnEnable()
	{
		this.debugLevel = this.serializedObject.FindProperty("debugLevel");
		this.ignoreList = this.serializedObject.FindProperty("_ignoreList");
    }

	public override void OnInspectorGUI()
    {
		this.serializedObject.Update();
		Object[] targetObjs = this.serializedObject.targetObjects;

		GUIContent content;

		EditorGUIUtility.labelWidth = PGEditorUtils.CONTROLS_DEFAULT_LABEL_WIDTH;

		EditorGUI.indentLevel = 1;
		
		// Build a cache dict to use after the control is displayed for user input
		var ignoreCache = new Dictionary<IgnoreModifier, List<Targetable>>();
		IgnoreModifier curEditTarget;
		List<Targetable> curIgnoreCacheList;		
		for (int i = 0; i < targetObjs.Length; i++)
		{
			curEditTarget = (IgnoreModifier)targetObjs[i];
			curIgnoreCacheList = new List<Targetable>();
			
			// Update the cache to compare later
			curIgnoreCacheList.AddRange(curEditTarget._ignoreList);
			
			// Store to cache dict
			ignoreCache[curEditTarget] = curIgnoreCacheList;
		}
			
		// If multiple objects are being edited display the default control. Otherwise 
		//   The pretty control is displayed below when there is only 1 object in the targetObjs
		if (this.serializedObject.isEditingMultipleObjects)
		{
			content = new GUIContent
			(
				"Targetables to ignore",
				"Drag and drop Targetables to make the TargetTracker ignore them."
			);
			EditorGUILayout.PropertyField(this.ignoreList, content, true);
			
			// Update the backing list or the cache handling won't do anything below.
			this.serializedObject.ApplyModifiedProperties();
		}
	
	
		foreach (KeyValuePair<IgnoreModifier, List<Targetable>> kvp in ignoreCache)
		{
			curEditTarget = kvp.Key;
			curIgnoreCacheList = kvp.Value;

			// See the note above on editing multi objects and control visibility
			//   Note: If this is true then there will only be 1 item I the cache dict here.
			if (!this.serializedObject.isEditingMultipleObjects)
			{
				GUILayout.Space(4);
		        PGEditorUtils.FoldOutObjList<Targetable>
		        (
		            "Targetables to ignore",
		            curEditTarget._ignoreList,
					true  // Force the fold-out state
		        );
			}
			
			// Detect a change to trigger ignore refresh logic if the game is playing
			//	 Note: Don't use counts since new item can be null then later
			//	       drag&dropped, skipping logic.
			if (Application.isPlaying)
			{
				// Sync newly added
				foreach (Targetable targetable in new List<Targetable>(curEditTarget._ignoreList))
				{
					if (targetable == null) continue;
					
					if (!curIgnoreCacheList.Contains(targetable))
						curEditTarget.Add(targetable);
				}
				
				// Sync newly removed
				foreach (Targetable targetable in new List<Targetable>(curIgnoreCacheList))
				{
					if (targetable == null) continue;
	
					if (!curEditTarget._ignoreList.Contains(targetable))
						curEditTarget.Remove(targetable);
				}
			}
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


class IgnoreModifierGizmo
{
	
    [DrawGizmo(GizmoType.Selected | GizmoType.NotInSelectionHierarchy)]
    static void RenderGizmo(IgnoreModifier mod, GizmoType gizmoType)
    {
        if (mod.debugLevel == DEBUG_LEVELS.Off || !mod.enabled) return;
		
		Color color = Color.red;
		color.a = 0.2f;
		foreach (Targetable targetable in mod._ignoreList)
		{
			if (targetable == null) continue;
			
            Gizmos.matrix = targetable.transform.localToWorldMatrix;

            color.a = 0.5f;
            Gizmos.color = color;
            Gizmos.DrawWireSphere(Vector3.zero, 1);

            color.a = 0.2f;
            Gizmos.color = color;
			Gizmos.DrawSphere(Vector3.zero, 1);
		}
		            
		Gizmos.matrix = Matrix4x4.zero;  // Just to be clean

    }
}
	
