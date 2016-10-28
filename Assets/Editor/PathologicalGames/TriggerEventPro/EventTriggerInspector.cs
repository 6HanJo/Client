/// <Licensing>
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


[CustomEditor(typeof(EventTrigger)), CanEditMultipleObjects]
public class EventTriggerInspector : AreaTargetTrackerInspector
{
	protected SerializedProperty areaHit;
	protected SerializedProperty duration;
	protected SerializedProperty eventInfoList;
	protected SerializedProperty eventTriggerPrefab;
	protected SerializedProperty eventTriggerPoolName;
	protected SerializedProperty fireOnRigidBodySleep;
	protected SerializedProperty fireOnSpawn;
	protected SerializedProperty hitMode;
	protected SerializedProperty listenTimeout;
	protected SerializedProperty notifyTargets;
	protected SerializedProperty overridePoolName;
	protected SerializedProperty overrideGizmoVisibility;
	protected SerializedProperty poolName;
	protected SerializedProperty startRange;
	protected SerializedProperty usePooling;
	
	public bool expandEventInfo = true;

    protected override void OnEnable()
	{
		this.areaHit                 = this.serializedObject.FindProperty("areaHit");
		this.areaLayer          	 = this.serializedObject.FindProperty("_areaLayer");
		this.areaPositionOffset 	 = this.serializedObject.FindProperty("_areaPositionOffset");
		this.areaRotationOffset 	 = this.serializedObject.FindProperty("_areaRotationOffset");
		this.areaShape          	 = this.serializedObject.FindProperty("_areaShape");
		this.debugLevel              = this.serializedObject.FindProperty("debugLevel");
		this.drawGizmo               = this.serializedObject.FindProperty("drawGizmo");
		this.duration                = this.serializedObject.FindProperty("duration");
		this.eventInfoList           = this.serializedObject.FindProperty("_eventInfoList");
		this.eventTriggerPoolName    = this.serializedObject.FindProperty("eventTriggerPoolName");
		this.eventTriggerPrefab      = this.serializedObject.FindProperty("eventTriggerPrefab");
		this.fireOnRigidBodySleep    = this.serializedObject.FindProperty("fireOnRigidBodySleep");
		this.fireOnSpawn             = this.serializedObject.FindProperty("fireOnSpawn");
		this.hitMode                 = this.serializedObject.FindProperty("hitMode");
		this.listenTimeout           = this.serializedObject.FindProperty("listenTimeout");
		this.gizmoColor              = this.serializedObject.FindProperty("gizmoColor");
		this.notifyTargets           = this.serializedObject.FindProperty("notifyTargets");
		this.numberOfTargets         = this.serializedObject.FindProperty("numberOfTargets");
		this.overridePoolName        = this.serializedObject.FindProperty("overridePoolName");
		this.overrideGizmoVisibility = this.serializedObject.FindProperty("overrideGizmoVisibility");
		this.poolName                = this.serializedObject.FindProperty("poolName");
        this.range                   = this.serializedObject.FindProperty("_range");
        this.sortingStyle            = this.serializedObject.FindProperty("_sortingStyle");
		this.updateInterval          = this.serializedObject.FindProperty("updateInterval");
		this.startRange              = this.serializedObject.FindProperty("startRange");
		this.targetLayers            = this.serializedObject.FindProperty("targetLayers");
		this.usePooling              = this.serializedObject.FindProperty("usePooling");
		
	}

	public override void OnInspectorGUI()
    {		
		this.serializedObject.Update();

		GUIContent content;
		
		EditorGUI.indentLevel = 0;
		
		EditorGUIUtility.labelWidth = PGEditorUtils.CONTROLS_DEFAULT_LABEL_WIDTH;

		Object[] targetObjs = this.serializedObject.targetObjects;
		
		EventTrigger curEditTarget;

		if (InstanceManager.POOLING_ENABLED)
		{
	        content = new GUIContent
	        (
	            "PoolName",
	            "The name of a pool to be used with PoolManager or other pooling solution. If " +
		            "not using pooling, this will do nothing and be hidden in the Inspector. "
	        );
			EditorGUILayout.PropertyField(this.poolName, content);
		}

		content = new GUIContent
		(
			"Hit Layers", 
			"The layers in which the Area is allowed to find targets."
		);
		EditorGUILayout.PropertyField(this.targetLayers, content);
		

        content = new GUIContent
        (
            "Hit Mode",
            "Determines what should cause this EventTrigger to fire.\n" + 
            "    TargetOnly\n" + 
            "        Only a direct hit will trigger the OnFire event\n" + 
            "    HitLayers\n" + 
            "        Contact with any colliders in any of the layers in\n" + 
            "        the HitLayers mask  will trigger the OnFire event."
        );
		EditorGUILayout.PropertyField(this.hitMode, content);
		
		
        content = new GUIContent
        (
            "listenTimeout (0 = OFF)",
			"An optional timer to allow this EventTrigger to timeout and self-destruct. When " +
				"set to a value above zero, when this reaches 0, the Fire coroutine will be " +
				"started and anything in range may be hit (depending on settings). This can be " +
				"used to give a projectile a max amount of time it can fly around before it " +
				"dies, or a time-based land mine or pick-up."
        );
		EditorGUILayout.PropertyField(this.listenTimeout, content);

		content = new GUIContent
		(
			"Fire On Spawn",
			"If true, the event will be fired as soon as this EventTrigger is spawned by " +
				"instantiation or pooling."
		);
		EditorGUILayout.PropertyField(this.fireOnSpawn, content);
		
		content = new GUIContent
		(
			"Fire On Sleep",
			"If this EventTrigger has a rigidbody, setting this to true will cause it to " +
				"fire if it falls asleep. See Unity's docs for more information on how " +
				"this happens."
		);
		EditorGUILayout.PropertyField(this.fireOnRigidBodySleep, content);
		
		content = new GUIContent
		(
			"Area Hit", 
			"If true, more than just the primary target will be affected when this EventTrigger " +
				"fires. Use the range options to determine the behavior."
		);
		EditorGUILayout.PropertyField(this.areaHit, content);
		
		// To make the gizmo delay work correctly, update the GUI here.
		serializedObject.ApplyModifiedProperties();

        if (this.areaHit.boolValue)
        {
			this.overrideGizmoVisibility.boolValue = false;

			EditorGUI.indentLevel += 1;
			
			content = new GUIContent(
				"Targets (-1 = all)",
				"The number of targets to return. Set to -1 to return all targets"
			);
			EditorGUILayout.PropertyField(this.numberOfTargets, content);

			
			content = new GUIContent("Sorting Style", "The style of sorting to use");
			EditorGUILayout.PropertyField(this.sortingStyle, content);
			
			var sortingStyle = (EventTrigger.SORTING_STYLES)this.sortingStyle.enumValueIndex;
            if (sortingStyle != EventTrigger.SORTING_STYLES.None)
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
			

			EditorGUI.BeginChangeCheck();
			content = new GUIContent
			(
				"Area Layer",
				"The layer to put the Area in."
			);
			content = EditorGUI.BeginProperty(new Rect(0, 0, 0, 0), content, this.areaLayer);
			
			int layer = EditorGUILayout.LayerField(content, this.areaLayer.intValue);
			
			// If changed, trigger the property setter for all objects being edited
			if (EditorGUI.EndChangeCheck())
			{
				for (int i = 0; i < targetObjs.Length; i++)
				{
					curEditTarget = (EventTrigger)targetObjs[i];
					
					Undo.RecordObject(curEditTarget, targetObjs[0].GetType() + " Area Layer");
					
					curEditTarget.areaLayer = layer;
				}
			}
			EditorGUI.EndProperty();
			
			
			EditorGUILayout.BeginHorizontal();
			
			EditorGUI.BeginChangeCheck();
			content = new GUIContent
			(
				"Area Shape", 
				"The shape of the Area used to detect targets in range"
			);
			EditorGUILayout.PropertyField(this.areaShape, content);
			
			// If changed, trigger the property setter for all objects being edited
			if (EditorGUI.EndChangeCheck())
			{
				var shape = (AreaTargetTracker.AREA_SHAPES)this.areaShape.enumValueIndex;

				for (int i = 0; i < targetObjs.Length; i++)
				{
					curEditTarget = (EventTrigger)targetObjs[i];
					
					Undo.RecordObject(curEditTarget, targetObjs[0].GetType() + " areaShape");
					
					curEditTarget.areaShape = shape;
				}
			}
			
			content = new GUIContent
			(
				"Gizmo ", 
				"Visualize the Area in the Editor by turning this on."
			);
			PGEditorUtils.ToggleButton(this.drawGizmo, content, 50);
				
            EditorGUILayout.EndHorizontal();

            if (this.drawGizmo.boolValue)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.BeginHorizontal();

				content = new GUIContent
				(
					"Gizmo Color", 
					"The color of the gizmo when displayed"
				);
				EditorGUILayout.PropertyField(this.gizmoColor, content);

                // If clicked, reset the color to the default
                GUIStyle style = EditorStyles.miniButton;
                style.alignment = TextAnchor.MiddleCenter;
                style.fixedWidth = 52;
				if (GUILayout.Button("Reset", style))
                {
					for (int i = 0; i < this.serializedObject.targetObjects.Length; i++)
					{
						curEditTarget = (EventTrigger)this.serializedObject.targetObjects[i];
						curEditTarget.gizmoColor = curEditTarget.defaultGizmoColor;
					}
                }

                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel -= 1;

                GUILayout.Space(4);
            }

			content = new GUIContent
			(
				"Duration (<0 = stay)", 
				"An optional duration to control how long this EventTrigger stays active. " +
				"Each target will only be hit once with the event notification unless the " +
				"Target leaves and then re-enters range. Set this to -1 to keep it alive " +
				"forever."
			);
			EditorGUILayout.PropertyField(this.duration, content);
			
			serializedObject.ApplyModifiedProperties();
			
			if (this.duration.floatValue > 0)
			{
				EditorGUI.indentLevel += 1;
				
				content = new GUIContent
				(
					"Start Range", 
					"When duration is greater than 0 this can be used have the range change " +
						"over the course of the duration. This is used for things like a " +
						"chockwave from a large explosion, which grows over time. "
				);
				content = EditorGUI.BeginProperty(new Rect(0, 0, 0, 0), content, this.startRange);
				
				EditorGUI.BeginChangeCheck();
				Vector3 startRange = this.startRange.vector3Value;
				switch ((AreaTargetTracker.AREA_SHAPES)this.areaShape.enumValueIndex)
				{
				case AreaTargetTracker.AREA_SHAPES.Circle2D:  // Fallthrough
				case AreaTargetTracker.AREA_SHAPES.Sphere:
					startRange.x = EditorGUILayout.FloatField(content, startRange.x);
					startRange.y = startRange.x;
					startRange.z = startRange.x;
					break;

				case AreaTargetTracker.AREA_SHAPES.Box2D:
					float oldZ = startRange.z;
					startRange = EditorGUILayout.Vector2Field(content.text, startRange);
					startRange.z = oldZ;  // Nice to maintain if switching between 2D and 3D
					break;
				
				case AreaTargetTracker.AREA_SHAPES.Box:
					startRange = EditorGUILayout.Vector3Field(content.text, startRange);
					break;
					
				case AreaTargetTracker.AREA_SHAPES.Capsule:
					startRange = EditorGUILayout.Vector2Field(content.text, startRange);
					startRange.z = startRange.x;
					break;
				}
				
				// Only assign the value back if it was actually changed by the user.
				// Otherwise a single value will be assigned to all objects when multi-object
				// editing, even when the user didn't touch the control.
				if (EditorGUI.EndChangeCheck())
				{
					this.startRange.vector3Value = startRange;
				}
				
				EditorGUI.EndProperty();
				
				EditorGUI.indentLevel -= 1;
			}
			
			
            this.displayRange<EventTrigger>(targetObjs);


			EditorGUI.BeginChangeCheck();
			content = new GUIContent
			(
				"Position Offset",
				"An optional position offset for the Area. For example, if you have an" +
				"object resting on the ground which has a range of 4, a position offset of" +
				"Vector3(0, 4, 0) will place your Area so it is also sitting on the ground."
			);
			EditorGUILayout.PropertyField(this.areaPositionOffset, content);

			// If changed, trigger the property setter for all objects being edited
			if (EditorGUI.EndChangeCheck())
			{
				string undo_message = targetObjs[0].GetType() + " areaPositionOffset";

				for (int i = 0; i < targetObjs.Length; i++)
				{
					curEditTarget = (EventTrigger)targetObjs[i];
					
					Undo.RecordObject(curEditTarget, undo_message);
					
					curEditTarget.areaPositionOffset = this.areaPositionOffset.vector3Value;
				}
			}


			EditorGUI.BeginChangeCheck();
			content = new GUIContent
			(
				"Rotation Offset",
				"An optional rotational offset for the Area."
			);
			EditorGUILayout.PropertyField(this.areaRotationOffset, content);

			// If changed, trigger the property setter for all objects being edited
			if (EditorGUI.EndChangeCheck())
			{
				string undo_message = targetObjs[0].GetType() + " areaPositionOffset";
				for (int i = 0; i < targetObjs.Length; i++)
				{
					curEditTarget = (EventTrigger)targetObjs[i];
					
					Undo.RecordObject(curEditTarget, undo_message);
					
					curEditTarget.areaRotationOffset = this.areaRotationOffset.vector3Value;
				}
			}				
				
			GUILayout.Space(8);
        }
        else
        {
            this.overrideGizmoVisibility.boolValue = true;
        }

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
			"        For every Target hit, a new EventTrigger will be\n" +
			"        spawned and passed this EventTrigger's \n" +
			"        EventInfo. PassToEventTriggerOnce is more \n" +
			"        commonly used when secondary EventTrigger is\n" +
			"        needed, but this can be used for some creative\n" +
			"        or edge use-cases.\n" +
			"    PassInfoToEventTriggerOnce\n" +
			"        OnFire a new EventTrigger will be spawned and\n" +
			"        passed this EventTrigger's EventInfo. Only 1 will\n" +
			"        be spawned. This is handy for making bombs \n" +
			"        where only the first Target would trigger the \n" +
			"        event and only 1 EventTrigger would be spawned\n" +
			"        to expand over time (using duration and start\n" +
			"        range attributes).\n" + 
			"    UseEventTriggerInfo\n" +
			"        Same as PassInfoToEventTrigger but the new\n" +
			"        EventTrigger will use its own EventInfo (this \n" +
			"        EventTrigger's EventInfo will be ignored).\n" +
			"    UseEventTriggerInfoOnce\n" +
			"        Same as PassInfoToEventTriggerOnce but the new\n" +
			"        EventTrigger will will used its own EventInfo\n" +
			"        (this EventTrigger's EventInfo will be ignored)."				
        );
        EditorGUILayout.PropertyField(this.notifyTargets, content);
        
		// 
		// If using EventTrigger...
		//
		EventTrigger.NOTIFY_TARGET_OPTIONS curOption;
		curOption = (EventTrigger.NOTIFY_TARGET_OPTIONS)this.notifyTargets.enumValueIndex;
		if (curOption > EventTrigger.NOTIFY_TARGET_OPTIONS.Direct)
		{
			EditorGUI.indentLevel += 1;

			content = new GUIContent
			(
				"EventTrigger Prefab",
				"An optional prefab to instance another EventTrigger. This can be handy if you " +
					"want to use a 'one-shot' event trigger to then spawn one that expands over " +
					"time using the duration and startRange to simulate a huge explosion."

			);
			
			EditorGUILayout.PropertyField(this.eventTriggerPrefab, content);
		
			if (InstanceManager.POOLING_ENABLED)
			{
				EditorGUI.indentLevel += 1;
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
						content = new GUIContent
						(
							"Override Pool Name",
							"If an eventTriggerPrefab is spawned, setting this to true will " +
								"override the EventTrigger's poolName and use this instead. The " +
								"instance will also be passed this EventTrigger's " +
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
									"WARNING: If poolname is set to '', Pooling will be " +
									"disabled and Unity's Instantiate will be used."
							);
							
							EditorGUILayout.PropertyField(this.eventTriggerPoolName, content);
						}
					}
					
					EditorGUI.indentLevel -= 1;
				}
				else
				{
					this.overridePoolName.boolValue = false;  // Reset
				}
			}

			EditorGUI.indentLevel -= 1;
        }
		
		if (curOption < EventTrigger.NOTIFY_TARGET_OPTIONS.UseEventTriggerInfo)
		{
	        if (this.serializedObject.isEditingMultipleObjects)
			{
				content = new GUIContent
				(
					"Event Info List",
					"A list of EventInfo structs which hold one or more event descriptions of how " +
						"this EventTrigger can affect a Target."
				);
				EditorGUILayout.PropertyField(this.eventInfoList, content, true);
			}
			else
			{
	            EditorGUI.indentLevel += 2;
				
				curEditTarget = (EventTrigger)targetObjs[0];
	            this.expandEventInfo = PGEditorUtils.SerializedObjFoldOutList<EventInfoListGUIBacker>
	            (
					"Event Info List",
					curEditTarget._eventInfoList,
	                this.expandEventInfo,
	                ref curEditTarget._editorListItemStates,
	                true
	            );
				
	            EditorGUI.indentLevel -= 2;
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