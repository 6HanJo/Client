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


[CustomEditor(typeof(FireDistanceModifier)), CanEditMultipleObjects]
public class FireDistanceModifierInspector : Editor
{
	protected SerializedProperty debugLevel;
	protected SerializedProperty drawGizmo;
	protected SerializedProperty gizmoColor;
	protected SerializedProperty maxDistance;
	protected SerializedProperty minDistance;
	protected SerializedProperty originMode;

    protected void OnEnable()
	{
		this.debugLevel  = this.serializedObject.FindProperty("debugLevel");
		this.drawGizmo   = this.serializedObject.FindProperty("drawGizmo");
		this.gizmoColor  = this.serializedObject.FindProperty("gizmoColor");
		this.maxDistance = this.serializedObject.FindProperty("maxDistance");
		this.minDistance = this.serializedObject.FindProperty("minDistance");
		this.originMode  = this.serializedObject.FindProperty("originMode");
    }

	public override void OnInspectorGUI()
    {
		this.serializedObject.Update();
		Object[] targetObjs = this.serializedObject.targetObjects;

        FireDistanceModifier curEditTarget;

		GUIContent content;
		GUIStyle style;

		EditorGUIUtility.labelWidth = PGEditorUtils.CONTROLS_DEFAULT_LABEL_WIDTH;

		EditorGUI.indentLevel = 1;


		content = new GUIContent
		(
			"Origin Mode", 
			"The origin is the point from which the distance is measuered."
		);
		EditorGUILayout.PropertyField(this.originMode, content);		
		
		var noLabel = new GUIContent("");
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Distance:", GUILayout.MaxWidth(70));
		
		EditorGUILayout.LabelField("Min", GUILayout.MaxWidth(40));
		EditorGUILayout.PropertyField(this.minDistance, noLabel, GUILayout.MinWidth(40));
		
		EditorGUILayout.LabelField("Max", GUILayout.MaxWidth(40));
		EditorGUILayout.PropertyField(this.maxDistance, noLabel, GUILayout.MinWidth(40));
		EditorGUILayout.EndHorizontal();
		
		
		// GIZMO...
        EditorGUILayout.BeginHorizontal();

		GUILayout.Space(12);  // For some reason the button won't indent. So fake it.

		content = new GUIContent
		(
			"Gizmo", 
			"Visualize the distance in the Editor by turning this on."
		);
		PGEditorUtils.ToggleButton(this.drawGizmo, content, 50);
		
        if (this.drawGizmo.boolValue)
        {			
			EditorGUILayout.PropertyField(this.gizmoColor, noLabel, GUILayout.MinWidth(60));
            style = EditorStyles.miniButton;
            style.alignment = TextAnchor.MiddleCenter;
            style.fixedWidth = 52;
			if (GUILayout.Button("Reset", style))
            {
				for (int i = 0; i < targetObjs.Length; i++)
				{
					curEditTarget = (FireDistanceModifier)targetObjs[i];
					curEditTarget.gizmoColor = curEditTarget.defaultGizmoColor;
				}
            }

        }
		EditorGUILayout.EndHorizontal();
		
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

class FireDistanceModifierGizmo
{
	static GameObject spaceCalculator;
	
    [DrawGizmo(GizmoType.Selected | GizmoType.NotInSelectionHierarchy)]
    static void RenderGizmo(FireDistanceModifier mod, GizmoType gizmoType)
    {
        if (!mod.drawGizmo || !mod.enabled || mod.overrideGizmoVisibility) return;
		
		// In the Editor, this may not be cached yet. Required component, so WILL exist
		if (mod.fireCtrl == null)
			mod.fireCtrl = mod.GetComponent<EventFireController>();

		TargetTracker tt = mod.fireCtrl.targetTracker;
		if (tt == null)
			tt = mod.fireCtrl.GetComponent<TargetTracker>();

		Transform xform = mod.transform;
		
		switch (mod.originMode)
		{
			case FireDistanceModifier.ORIGIN_MODE.TargetTracker:
				xform = tt.transform;
				break;
			
			case FireDistanceModifier.ORIGIN_MODE.TargetTrackerArea:
				var perimterTT = tt as AreaTargetTracker;
				if (perimterTT == null)
					throw new System.Exception
					(
						"TargetTracker not a AreaTargetTracker. Use a different origin " +
						"mode than 'TargetTrackerArea'"
					);
	            
				// Set the space everything is drawn in.
	            if (FireDistanceModifierGizmo.spaceCalculator == null)
	            {
	                FireDistanceModifierGizmo.spaceCalculator = new GameObject();
	                FireDistanceModifierGizmo.spaceCalculator.hideFlags = HideFlags.HideAndDontSave;
	            }
	
	            xform = FireDistanceModifierGizmo.spaceCalculator.transform;
	            xform.position = 
				(
					(perimterTT.transform.rotation * perimterTT.areaPositionOffset) + 
					perimterTT.transform.position
				);

				break;
				
			case FireDistanceModifier.ORIGIN_MODE.FireController:
				// Already defaulted above.
				break;
				
			case FireDistanceModifier.ORIGIN_MODE.FireControllerEmitter:
	            if (mod.fireCtrl.spawnEventTriggerAtTransform != null)
	                xform = mod.fireCtrl.spawnEventTriggerAtTransform;
	            //else   // Already defaulted above.
	            //    xform = mod.transform;
				break;
		}
		
        Gizmos.matrix = xform.localToWorldMatrix;

        Vector3 pos = Vector3.zero;  // We set the sapce relative above
       			
		Color color = mod.gizmoColor;
        color.a = 0.2f;
        Gizmos.color = color;
        Gizmos.DrawWireSphere(pos, mod.maxDistance);

        color.a = 0.05f;
        Gizmos.color = color;
		Gizmos.DrawSphere(pos, mod.maxDistance);

        Gizmos.matrix = Matrix4x4.zero;  // Just to be clean
    }
}
	
