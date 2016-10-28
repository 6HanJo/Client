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


[CustomEditor(typeof(TriggerEventProMessenger)), CanEditMultipleObjects]
public class TriggerEventProMessengerInspector : Editor
{
	protected SerializedProperty debugLevel;
	protected SerializedProperty otherTarget;
	protected SerializedProperty messageMode;
	protected SerializedProperty forComponent;
	
	protected List<SerializedProperty> fireControllerProps = new List<SerializedProperty>();
	protected List<SerializedProperty> eventTriggerProps = new List<SerializedProperty>();
	protected List<SerializedProperty> targetableProps = new List<SerializedProperty>();
	protected List<SerializedProperty> targetTrackerProps = new List<SerializedProperty>();

	protected void OnEnable()
	{
		this.debugLevel   = this.serializedObject.FindProperty("debugLevel");
		this.forComponent = this.serializedObject.FindProperty("forComponent");
		this.messageMode  = this.serializedObject.FindProperty("messageMode");
		this.otherTarget  = this.serializedObject.FindProperty("otherTarget");

		fireControllerProps.Add(this.serializedObject.FindProperty("fireController_OnStart"));
		fireControllerProps.Add(this.serializedObject.FindProperty("fireController_OnUpdate"));
		fireControllerProps.Add(this.serializedObject.FindProperty("fireController_OnTargetUpdate"));
		fireControllerProps.Add(this.serializedObject.FindProperty("fireController_OnIdleUpdate"));
		fireControllerProps.Add(this.serializedObject.FindProperty("fireController_OnFire"));
		fireControllerProps.Add(this.serializedObject.FindProperty("fireController_OnStop"));
		
		eventTriggerProps.Add(this.serializedObject.FindProperty("eventTrigger_OnListenStart"));
		eventTriggerProps.Add(this.serializedObject.FindProperty("eventTrigger_OnListenUpdate"));
		eventTriggerProps.Add(this.serializedObject.FindProperty("eventTrigger_OnFire"));
		eventTriggerProps.Add(this.serializedObject.FindProperty("eventTrigger_OnFireUpdate"));
		eventTriggerProps.Add(this.serializedObject.FindProperty("eventTrigger_OnTargetHit"));
		
		targetableProps.Add(this.serializedObject.FindProperty("targetable_OnHit"));
		targetableProps.Add(this.serializedObject.FindProperty("targetable_OnHitCollider"));
		targetableProps.Add(this.serializedObject.FindProperty("targetable_OnDetected"));
		targetableProps.Add(this.serializedObject.FindProperty("targetable_OnNotDetected"));
		
		targetTrackerProps.Add(this.serializedObject.FindProperty("targetTracker_OnPostSort"));
		targetTrackerProps.Add(this.serializedObject.FindProperty("targetTracker_OnNewDetected"));
		targetTrackerProps.Add(this.serializedObject.FindProperty("targetTracker_OnTargetsChanged"));
    }

	public override void OnInspectorGUI()
    {
		this.serializedObject.Update();
		//Object[] targetObjs = this.serializedObject.targetObjects;

		GUIContent content;

		EditorGUIUtility.labelWidth = PGEditorUtils.CONTROLS_DEFAULT_LABEL_WIDTH;

		EditorGUI.indentLevel = 0;
		
		content = new GUIContent
		(
			"Other Message Target (Optional)", 
			"An optional GameObject to message instead of this component's GameObject"
		);
		EditorGUILayout.PropertyField(this.otherTarget, content);		

		content = new GUIContent("Message Mode", "SendMessage will only send to this GameObject");
		EditorGUILayout.PropertyField(this.messageMode, content);		
		
		
		EditorGUI.BeginChangeCheck();
		content = new GUIContent("For Component", "Choose which component's events to use");
		content = EditorGUI.BeginProperty(new Rect(0, 0, 0, 0), content, this.forComponent);
				
		// EnumMaskField returns an Enum that has to be cast back to our type before casting to int
		var forCompFlags = (TriggerEventProMessenger.COMPONENTS)this.forComponent.intValue;
        System.Enum result = EditorGUILayout.EnumMaskField(content, forCompFlags);
		int forCompInt = (int)(TriggerEventProMessenger.COMPONENTS)result;
		
		// Only set the property if there was a change to avoid instantly setting all selected.
		if (EditorGUI.EndChangeCheck())
		{
		    this.forComponent.intValue = forCompInt;
		}
		EditorGUI.EndProperty();		
		

		
        // Change the label spacing
		EditorGUIUtility.labelWidth = 240;
		
        EditorGUI.indentLevel += 1;

		List<SerializedProperty> props = new List<SerializedProperty>();
		
		forCompFlags = (TriggerEventProMessenger.COMPONENTS)this.forComponent.intValue;
				
		if ((forCompFlags & TriggerEventProMessenger.COMPONENTS.FireController) == 
							TriggerEventProMessenger.COMPONENTS.FireController)
        {			
			props.AddRange(this.fireControllerProps);
        }
		
		if ((forCompFlags & TriggerEventProMessenger.COMPONENTS.EventTrigger) == 
		    				TriggerEventProMessenger.COMPONENTS.EventTrigger)
        {
			props.AddRange(this.eventTriggerProps);
        }
		
		if ((forCompFlags & TriggerEventProMessenger.COMPONENTS.Targetable) == 
							TriggerEventProMessenger.COMPONENTS.Targetable)
        {
			props.AddRange(this.targetableProps);
        }
		
		if ((forCompFlags & TriggerEventProMessenger.COMPONENTS.TargetTracker) == 
							TriggerEventProMessenger.COMPONENTS.TargetTracker)
        {
			props.AddRange(this.targetTrackerProps);
        }
		
		foreach (SerializedProperty prop in props)
		{
			// Help in debugging
			if (prop == null)
				throw new System.NullReferenceException("Property is null. Inspector typo?");
			
			EditorGUILayout.PropertyField(prop, new GUIContent(prop.name.Capitalize()));
		}
		
        EditorGUI.indentLevel -= 1;
		
		EditorGUIUtility.labelWidth = PGEditorUtils.CONTROLS_DEFAULT_LABEL_WIDTH;
		
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
