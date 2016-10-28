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


[CustomEditor(typeof(EventFireController)), CanEditMultipleObjects]
public class EventFireControllerInspector : Editor
{
	protected SerializedProperty debugLevel;
	protected SerializedProperty eventTriggerPoolName;
	protected SerializedProperty eventInfoList;
	protected SerializedProperty eventTriggerPrefab;
	protected SerializedProperty spawnEventTriggerAtTransform;
	protected SerializedProperty initIntervalCountdownAtZero;
	protected SerializedProperty interval;
	protected SerializedProperty notifyTargets;
	protected SerializedProperty overridePoolName;
	protected SerializedProperty targetTracker;
	protected SerializedProperty usePooling;

	public bool expandEventInfoList = true;

    protected void OnEnable()
	{
		this.debugLevel 	        = this.serializedObject.FindProperty("debugLevel");
		this.eventTriggerPoolName   = this.serializedObject.FindProperty("eventTriggerPoolName");
		this.eventInfoList          = this.serializedObject.FindProperty("_eventInfoList");
		this.eventTriggerPrefab     = this.serializedObject.FindProperty("eventTriggerPrefab");
		this.spawnEventTriggerAtTransform = this.serializedObject.FindProperty("_spawnEventTriggerAtTransform");
		this.initIntervalCountdownAtZero  = this.serializedObject.FindProperty("initIntervalCountdownAtZero");
		this.interval 	            = this.serializedObject.FindProperty("interval");
		this.overridePoolName       = this.serializedObject.FindProperty("overridePoolName");
		this.notifyTargets 	        = this.serializedObject.FindProperty("notifyTargets");
		this.targetTracker 	        = this.serializedObject.FindProperty("targetTracker");
		this.usePooling 	        = this.serializedObject.FindProperty("usePooling");
    }

	public override void OnInspectorGUI()
    {
		this.serializedObject.Update();

		GUIContent content;
		
		EditorGUI.indentLevel = 0;
		
		EditorGUIUtility.labelWidth = PGEditorUtils.CONTROLS_DEFAULT_LABEL_WIDTH;

		Object[] targetObjs = this.serializedObject.targetObjects;
		
		EventFireController curEditTarget;

				
		// Try and init the TargetTracker field
		for (int i = 0; i < targetObjs.Length; i++)
		{
			curEditTarget = (EventFireController)targetObjs[i];
			if (curEditTarget.targetTracker == null)
				curEditTarget.targetTracker = curEditTarget.GetComponent<TargetTracker>();// null OK
		}	
		
		
        content = new GUIContent("Interval", "Fire every X seconds");
        EditorGUILayout.PropertyField(this.interval, content);

        content = new GUIContent
        (
            "Init Countdown at 0",
            "Able to fire immediatly when first spawned, before interval count begins"
        );
        EditorGUILayout.PropertyField(this.initIntervalCountdownAtZero, content);

        content = new GUIContent
        (
            "Notify Targets",
            "Sets the target notification behavior. Telling targets they are hit is optional " +
            "for situations where a delayed response is required, such as launching a , " +
            "projectile or for custom handling.\n" +
            "\n" +
            "MODES\n" +
				"    Off\n" +
				"        Do not notify anything. delegates can still be used\n" +
				"        for custom handling\n" +
				"    Direct\n" +
				"        OnFire targets will be notified immediately\n" +
				"    PassInfoToEventTrigger\n" +
				"        OnFire, for each Target hit, a new EventTrigger will\n" +
				"        be spawned and passed this EventFireController's \n" +
				"        EventInfo.\n" +
				"    UseEventTriggerInfo\n" +
				"        Same as PassInfoToEventTrigger but the new\n" +
				"        EventTrigger will use its own EventInfo (this \n" +
				"        EventTrigger's EventInfo will be ignored). "
				);
		EditorGUILayout.PropertyField(this.notifyTargets, content);
        
		// 
		// If using an EventTrigger Prefab...
		//
		EventFireController.NOTIFY_TARGET_OPTIONS curOption;
		curOption = (EventFireController.NOTIFY_TARGET_OPTIONS)this.notifyTargets.enumValueIndex;
		if (curOption > EventFireController.NOTIFY_TARGET_OPTIONS.Direct)
		{
			EditorGUI.indentLevel += 1;
						
			content = new GUIContent
			(
				"Spawn At Transform",
				"This transform is optionally used as the position at which an EventTrigger " +
					"prefab is spawned from. Some Utility components may also use this as a " +
					"position reference if chosen."
			);
			
			EditorGUILayout.PropertyField(this.spawnEventTriggerAtTransform, content);

			if (curOption == EventFireController.NOTIFY_TARGET_OPTIONS.PassInfoToEventTrigger ||
				curOption == EventFireController.NOTIFY_TARGET_OPTIONS.UseEventTriggerInfo)
			{
				content = new GUIContent
				(
					"EventTrigger Prefab",
					"An optional EventTrigger to spawn OnFire depending on notifyTarget's " + 
						"NOTIFY_TARGET_OPTIONS."
				);
				
				EditorGUILayout.PropertyField(this.eventTriggerPrefab, content);
			}
			
			if (InstanceManager.POOLING_ENABLED)
			{

				if (this.eventTriggerPrefab.objectReferenceValue != null)
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
							"Override Pool",
							"If an eventTriggerPrefab is spawned, setting this to true will " +
								"override the EventTrigger's poolName and use this instead. The " +
								"instance will also be passed this EventFireController's " +
								"eventTriggerPoolName to be used when the EventTrigger is " +
								"desapwned."
						);
						
						EditorGUILayout.PropertyField(this.overridePoolName, content);
						
						if (this.overridePoolName.boolValue)
						{							
							content = new GUIContent
							(
								"Pool Name",
								"The name of a pool to be used with PoolManager or other " +
									"pooling solution. If not using pooling, this will do " +
									"nothing and be hidden in the Inspector.\n" +
									"WARNING: If poolname is set to '', Pooling will be disabled " +
									"and Unity's Instantiate will be used."
							);
							
							EditorGUILayout.PropertyField(this.eventTriggerPoolName, content);
						}
						
						EditorGUI.indentLevel -= 1;
					}
				}
				else
				{
					this.overridePoolName.boolValue = false;  // Reset
				}
			}
			
			EditorGUI.indentLevel -= 1;
        }
				
		if (curOption != EventFireController.NOTIFY_TARGET_OPTIONS.UseEventTriggerInfo)
		{
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
                EditorGUI.indentLevel += 2;
				
				curEditTarget = (EventFireController)targetObjs[0];
	            this.expandEventInfoList = PGEditorUtils.SerializedObjFoldOutList<EventInfoListGUIBacker>
	            (
	                content.text,
	                curEditTarget._eventInfoList,
	                this.expandEventInfoList,
	                ref curEditTarget._editorListItemStates,
	                true
	            );
				
                EditorGUI.indentLevel -= 2;
			}
		}
		

		// Init with the TargetTracker found on the same GameObject if possible
		for (int i = 0; i < targetObjs.Length; i++)
		{
			curEditTarget = (EventFireController)targetObjs[i];
			if (curEditTarget.targetTracker == null)
				curEditTarget.targetTracker = curEditTarget.GetComponent<AreaTargetTracker>();
		}
		
		GUILayout.Space(6);
		
        content = new GUIContent
        (
            "TargetTracker",
            "This FireController's TargetTracker. Defaults to one on the same GameObject."
        );
		EditorGUILayout.PropertyField(this.targetTracker, content);
		
		GUILayout.Space(8);
			
		content = new GUIContent("Debug Level", "Set it higher to see more verbose information.");
		EditorGUILayout.PropertyField(this.debugLevel, content);
			
        serializedObject.ApplyModifiedProperties();
			
        // Flag Unity to save the changes to to the prefab to disk
		// 	 This is needed to make the gizmos update immediatly.
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

}
