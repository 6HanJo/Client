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


[CustomEditor(typeof(SpawnOnFire)), CanEditMultipleObjects]
public class SpawnOnFireInspector : Editor
{
	protected SerializedProperty altSource;
	protected SerializedProperty poolName;
	protected SerializedProperty prefab;
	protected SerializedProperty spawnAtMode;
	protected SerializedProperty spawnAtTransform;
	protected SerializedProperty usePooling;

    protected void OnEnable()
	{
		this.altSource        = this.serializedObject.FindProperty("altSource");
		this.poolName         = this.serializedObject.FindProperty("poolName");
		this.prefab           = this.serializedObject.FindProperty("prefab");
		this.spawnAtMode      = this.serializedObject.FindProperty("spawnAtMode");
		this.spawnAtTransform = this.serializedObject.FindProperty("spawnAtTransform");
		this.usePooling       = this.serializedObject.FindProperty("usePooling");
    }

	public override void OnInspectorGUI()
    {
		this.serializedObject.Update();
		//Object[] targetObjs = this.serializedObject.targetObjects;
        //SpawnOnFire curEditTarget;

		GUIContent content;

		EditorGUIUtility.labelWidth = PGEditorUtils.CONTROLS_DEFAULT_LABEL_WIDTH;

		EditorGUI.indentLevel = 0;
		
		content = new GUIContent("Prefab", "The prefab to spawn Instances from.");
		EditorGUILayout.PropertyField(this.prefab, content);

		content = new GUIContent
		(
			"Spawn At", 
			"The origin is the point from which the distance is measuered."
		);
		EditorGUILayout.PropertyField(this.spawnAtMode, content);		
				
		var curSpawnAtMode = (SpawnOnFire.SPAWN_AT_MODE)this.spawnAtMode.enumValueIndex;
		if (curSpawnAtMode == SpawnOnFire.SPAWN_AT_MODE.OtherTransform)
		{
			EditorGUI.indentLevel += 1;
			
			content = new GUIContent
			(
				"Spawn At", 
				"This Transform's position and rotation will be used to spawn the instance if the" +
				"origin mode is set to 'OtherTransform'."
			);
			EditorGUILayout.PropertyField(this.spawnAtTransform, content);		

			EditorGUI.indentLevel -= 1;
		}
		
		
		if (InstanceManager.POOLING_ENABLED)
		{
			content = new GUIContent
			(
				"Use Pooling",
				"If false, do not add the new instance to a pool. Use Unity's " + 
					"Instantiate/Destroy"	
			);
			
			EditorGUILayout.PropertyField(this.usePooling, content);

			if (this.usePooling.boolValue)
			{
				EditorGUI.indentLevel += 1;
				
				content = new GUIContent
		        (
		            "PoolName",
		            "The name of a pool to be used with PoolManager or other pooling solution. If " +
			            "not using pooling, this will do nothing and be hidden in the Inspector. "
		        );
				EditorGUILayout.PropertyField(this.poolName, content);
				
				EditorGUI.indentLevel -= 1;
			}
		}
				
		content = new GUIContent
		(
			"Alternate Event Source", 
			"Use this when the FireController or EventTrigger with OnFire event is on another GameObject"
		);
		EditorGUILayout.PropertyField(this.altSource, content);
		
        serializedObject.ApplyModifiedProperties();
			
        // Flag Unity to save the changes to to the prefab to disk
		// 	 This is needed to make the gizmos update immediatly.
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
