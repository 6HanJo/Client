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
using System.Collections.Generic;


[CustomEditor(typeof(AreaTargetTracker)), CanEditMultipleObjects]
public class AreaTargetTrackerInspector : Editor
{
	protected SerializedProperty areaLayer;
	protected SerializedProperty areaPositionOffset;
	protected SerializedProperty areaRotationOffset;
	protected SerializedProperty areaShape;
	protected SerializedProperty drawGizmo;
	protected SerializedProperty debugLevel;
	protected SerializedProperty gizmoColor;
    protected SerializedProperty numberOfTargets;
    protected SerializedProperty range;
    protected SerializedProperty sortingStyle;
	protected SerializedProperty targetLayers;
	protected SerializedProperty updateInterval;

	protected bool showArea = true;

    protected virtual void OnEnable()
    {
		this.areaLayer  	 = this.serializedObject.FindProperty("_areaLayer");
		this.areaPositionOffset = this.serializedObject.FindProperty("_areaPositionOffset");
		this.areaRotationOffset = this.serializedObject.FindProperty("_areaRotationOffset");
		this.areaShape  	 = this.serializedObject.FindProperty("_areaShape");
		this.drawGizmo	 	 = this.serializedObject.FindProperty("drawGizmo");		
		this.debugLevel 	 = this.serializedObject.FindProperty("debugLevel");		
		this.gizmoColor 	 = this.serializedObject.FindProperty("gizmoColor");
		this.numberOfTargets = this.serializedObject.FindProperty("numberOfTargets");
        this.range           = this.serializedObject.FindProperty("_range");
        this.sortingStyle    = this.serializedObject.FindProperty("_sortingStyle");
		this.targetLayers 	 = this.serializedObject.FindProperty("targetLayers");
		this.updateInterval  = this.serializedObject.FindProperty("updateInterval");
    }

	public override void OnInspectorGUI()
    {
		this.serializedObject.Update();

		GUIContent content;
		EditorGUI.indentLevel = 0;
		EditorGUIUtility.labelWidth = PGEditorUtils.CONTROLS_DEFAULT_LABEL_WIDTH;

		Object[] targetObjs = this.serializedObject.targetObjects;
		
		AreaTargetTracker curEditTarget;
		
		
		content = new GUIContent(
			"Targets (-1 for all)",
			"The number of targets to return. Set to -1 to return all targets"
		);
		EditorGUILayout.PropertyField(this.numberOfTargets, content);
			
		content = new GUIContent
		(
			"Target Layers", 
			"The layers in which the area is allowed to find targets."
		);
		EditorGUILayout.PropertyField(this.targetLayers, content);
			
		EditorGUI.BeginChangeCheck();
		content = new GUIContent("Sorting Style", "The style of sorting to use");
		EditorGUILayout.PropertyField(this.sortingStyle, content);
		
		var sortingStyle = (TargetTracker.SORTING_STYLES)this.sortingStyle.enumValueIndex;
			
		// If changed, trigger the property setter for all objects being edited
		if (EditorGUI.EndChangeCheck())
		{
			for (int i = 0; i < targetObjs.Length; i++)
			{
				curEditTarget = (AreaTargetTracker)targetObjs[i];

				Undo.RecordObject(curEditTarget, targetObjs[0].GetType() + " sortingStyle");
				
				curEditTarget.sortingStyle = sortingStyle;
			}
		}
				
        if (sortingStyle != TargetTracker.SORTING_STYLES.None)
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
		
        this.showArea = EditorGUILayout.Foldout(this.showArea, "Area Settings");
		
        if (this.showArea)
        {
            EditorGUI.indentLevel += 1;
			
			EditorGUI.BeginChangeCheck();
			content = new GUIContent
			(
				"Area Layer",
				"The layer to put the area in."
			);
			content = EditorGUI.BeginProperty(new Rect(0, 0, 0, 0), content, this.areaLayer);
			
			int layer = EditorGUILayout.LayerField(content, this.areaLayer.intValue);
			
			// If changed, trigger the property setter for all objects being edited
			if (EditorGUI.EndChangeCheck())
			{				
				for (int i = 0; i < targetObjs.Length; i++)
				{
					curEditTarget = (AreaTargetTracker)targetObjs[i];
					
					Undo.RecordObject(curEditTarget, targetObjs[0].GetType() + " areaLayer");
					
					curEditTarget.areaLayer = layer;
				}
			}
			EditorGUI.EndProperty();


			EditorGUILayout.BeginHorizontal();
			
			EditorGUI.BeginChangeCheck();
			content = new GUIContent
			(
				"Area Shape", 
				"The shape of the area used to detect targets in range"
			);
			EditorGUILayout.PropertyField(this.areaShape, content);
			
			// If changed, trigger the property setter for all objects being edited
			if (EditorGUI.EndChangeCheck())
			{
				var shape = (AreaTargetTracker.AREA_SHAPES)this.areaShape.enumValueIndex;
				
				string undo_message = targetObjs[0].GetType() + " areaShape";
				
				for (int i = 0; i < targetObjs.Length; i++)
				{
					curEditTarget = (AreaTargetTracker)targetObjs[i];
					
					if (curEditTarget.area != null)  // Only works at runtime.
					{
//						// This would have happened anyway, but this way is undoable
//						if (curEditTarget.area.coll != null)
//						{
//							Undo.DestroyObjectImmediate(curEditTarget.area.coll);
//						}
//						else if (curEditTarget.area.coll2D != null)
//						{
//							Undo.DestroyObjectImmediate(curEditTarget.area.coll2D);
//						}
						
						Undo.RecordObject(curEditTarget.area.transform, undo_message);						
					}
					
					Undo.RecordObject(curEditTarget, undo_message);

					
					// Property. Handles collider changes at runtime.
					curEditTarget.areaShape = shape;
				}
			}


			Area area;
			bool ok;
			for (int i = 0; i < targetObjs.Length; i++)
			{
				curEditTarget = (AreaTargetTracker)targetObjs[i];
				area = curEditTarget.area;
				ok = false;
				
				if (area == null)
					continue;
				
	
				if (area.coll != null)
				{
					switch (curEditTarget.areaShape)
					{
						case AreaTargetTracker.AREA_SHAPES.Sphere:
							if (area.coll is SphereCollider)
								ok = true;
							
							break;
							
						case AreaTargetTracker.AREA_SHAPES.Box:
							if (area.coll is BoxCollider)
								ok = true;
							
							break;	
						
						case AreaTargetTracker.AREA_SHAPES.Capsule:
							if (area.coll is CapsuleCollider)
								ok = true;
							
							break;	
						
					}
				}
				else if (area.coll2D != null)
				{
					switch (curEditTarget.areaShape)
					{
						case AreaTargetTracker.AREA_SHAPES.Box2D:
							if (area.coll2D is BoxCollider2D)
								ok = true;
							
							break;
							
						case AreaTargetTracker.AREA_SHAPES.Circle2D:
							if (area.coll2D is CircleCollider2D)
								ok = true;
							
							break;	
					}
					
				}					

				if (!ok)
				{
					var shape = (AreaTargetTracker.AREA_SHAPES)this.areaShape.enumValueIndex;
					curEditTarget.areaShape = shape;
				}
			}
				
			content = new GUIContent
			(
				"Gizmo ", 
				"Visualize the area in the Editor by turning this on."
			);
			PGEditorUtils.ToggleButton(this.drawGizmo, content, 50);
			
			//script.drawGizmo = EditorGUILayout.Toggle(script.drawGizmo, GUILayout.MaxWidth(47));
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
					for (int i = 0; i < targetObjs.Length; i++)
					{
						curEditTarget = (AreaTargetTracker)targetObjs[i];
						curEditTarget.gizmoColor = curEditTarget.defaultGizmoColor;
					}
                }

                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel -=1;

                GUILayout.Space(4);
            }


			this.displayRange<AreaTargetTracker>(targetObjs);
			

			EditorGUI.BeginChangeCheck();
			content = new GUIContent
			(
				"Position Offset",
				"An optional position offset for the area. For example, if you have an" +
				"object resting on the ground which has a range of 4, a position offset of" +
				"Vector3(0, 4, 0) will place your area so it is also sitting on the ground."
			);
			EditorGUILayout.PropertyField(this.areaPositionOffset, content);

			// If changed, trigger the property setter for all objects being edited
			if (EditorGUI.EndChangeCheck())
			{
				string undo_message = targetObjs[0].GetType() + " areaPositionOffset";				
				for (int i = 0; i < targetObjs.Length; i++)
				{
					curEditTarget = (AreaTargetTracker)targetObjs[i];
					
					Undo.RecordObject(curEditTarget, undo_message);

					if (curEditTarget.area != null)  // Only works at runtime.
					{
						Undo.RecordObject(curEditTarget.area.transform, undo_message);						
					}
					
					curEditTarget.areaPositionOffset = this.areaPositionOffset.vector3Value;
				}
			}


			EditorGUI.BeginChangeCheck();
			content = new GUIContent
			(
				"Rotation Offset",
				"An optional rotational offset for the area."
			);
			EditorGUILayout.PropertyField(this.areaRotationOffset, content);

			// If changed, trigger the property setter for all objects being edited
			if (EditorGUI.EndChangeCheck())
			{
				string undo_message = targetObjs[0].GetType() + " areaPositionOffset";		
				for (int i = 0; i < targetObjs.Length; i++)
				{
					curEditTarget = (AreaTargetTracker)targetObjs[i];
					
					Undo.RecordObject(curEditTarget, undo_message);
					
					if (curEditTarget.area != null)  // Only works at runtime.
					{
						Undo.RecordObject(curEditTarget.area.transform, undo_message);						
					}
					
					curEditTarget.areaRotationOffset = this.areaRotationOffset.vector3Value;
				}
			}
		}
		
        EditorGUI.indentLevel -= 1;

		GUILayout.Space(8);
			
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

	protected void displayRange<T>(Object[] targetObjs) where T : AreaTargetTracker
	{
		var content = new GUIContent
		(
			"Range",
			"The range of the area from the center to the outer edge."
		);
		content = EditorGUI.BeginProperty(new Rect(0, 0, 0, 0), content, this.range);
		
		EditorGUI.BeginChangeCheck();
		Vector3 range = this.range.vector3Value;
		switch ((AreaTargetTracker.AREA_SHAPES)this.areaShape.enumValueIndex)
		{
			case AreaTargetTracker.AREA_SHAPES.Circle2D:   // Fallthrough
			case AreaTargetTracker.AREA_SHAPES.Sphere:				
				range.x = EditorGUILayout.FloatField(content, range.x);
				range.y = range.x;
				range.z = range.x;
				break;
			case AreaTargetTracker.AREA_SHAPES.Box2D:
				float oldZ = range.z;
				range = EditorGUILayout.Vector2Field(content.text, range);
				range.z = oldZ;  // Nice to maintain if switching between 2D and 3D
				break;
			case AreaTargetTracker.AREA_SHAPES.Box:
				range = EditorGUILayout.Vector3Field(content.text, range);
				break;
			case AreaTargetTracker.AREA_SHAPES.Capsule:
				range = EditorGUILayout.Vector2Field(content.text, range);
				range.z = range.x;
				break;
		}
		
		// Only assign the value back if it was actually changed by the user.
		// Otherwise a single value will be assigned to all objects when multi-object editing,
		// even when the user didn't touch the control.
		if (EditorGUI.EndChangeCheck())
		{
			this.range.vector3Value = range;
			
			string undo_message = targetObjs[0].GetType() + " range";				
			T curEditTarget;
			List<Object> undoObjs = new List<Object>();
			Area curArea;
			for (int i = 0; i < targetObjs.Length; i++)
			{
				curEditTarget = (T)targetObjs[i];
				
				undoObjs.Add(curEditTarget);
				
				// Have to manage collider undo because it is set by the range property
				curArea = curEditTarget.area;
				if (curArea)
				{
					if (curArea.GetComponent<Collider>() != null)
						undoObjs.Add(curArea.GetComponent<Collider>());
					else if (curArea.GetComponent<Collider2D>() != null)
						undoObjs.Add(curArea.GetComponent<Collider2D>());
				}
				
				Undo.RecordObjects(undoObjs.ToArray(), undo_message);
				
				curEditTarget.range = this.range.vector3Value;
			}
		}
		
		EditorGUI.EndProperty();
	}
}


class AreaGizmo
{
    static GameObject spaceCalculator;

	[DrawGizmo(GizmoType.Selected | GizmoType.NotInSelectionHierarchy | GizmoType.Active | GizmoType.InSelectionHierarchy)]
    static void RenderAreaGizmo(AreaTargetTracker tt, GizmoType gizmoType)
    {
        if (!tt.drawGizmo || !tt.enabled || tt.overrideGizmoVisibility) 
			return;
		
		// Hide the gizmo of the collider is disabled during run-time
		if (Application.isPlaying && tt.area != null && 
		    ((tt.area.coll != null && !tt.area.coll.enabled) || (tt.area.coll2D != null && !tt.area.coll2D.enabled)))
		    return;
		    
        Color color = tt.gizmoColor;
        Gizmos.color = color;

        // Set the space everything is drawn in.
        if (AreaGizmo.spaceCalculator == null)
        {
            AreaGizmo.spaceCalculator = new GameObject();
            AreaGizmo.spaceCalculator.hideFlags = HideFlags.HideAndDontSave;
        }

        Transform xform = AreaGizmo.spaceCalculator.transform;

        xform.position = (tt.transform.rotation * tt.areaPositionOffset) + 
																tt.transform.position;

        var rotOffset = Quaternion.Euler(tt.areaRotationOffset);
        xform.rotation = tt.transform.rotation * rotOffset;

        Gizmos.matrix = xform.localToWorldMatrix;

        Vector3 range = tt.GetNormalizedRange();
        Vector3 pos = Vector3.zero;  // We set the space relative above
        Vector3 capsuleBottomPos = pos;
        Vector3 capsuleTopPos = pos;
        switch (tt.areaShape)
        {
			case AreaTargetTracker.AREA_SHAPES.Circle2D:	// Fallthrough
        		pos = xform.position;
			
				Handles.color = color;
				Handles.DrawWireDisc(pos, xform.forward, range.x);				
				break;
			case AreaTargetTracker.AREA_SHAPES.Sphere:
                Gizmos.DrawWireSphere(pos, range.x);
                break;
			case AreaTargetTracker.AREA_SHAPES.Box2D:   	// Fallthrough
			case AreaTargetTracker.AREA_SHAPES.Box:
			    Gizmos.DrawWireCube(pos, range);
                break;

            case AreaTargetTracker.AREA_SHAPES.Capsule:
                float delta = (range.y * 0.5f) - range.x;

                capsuleTopPos.y += Mathf.Clamp(delta, 0, delta);
                Gizmos.DrawWireSphere(capsuleTopPos, range.x);

                capsuleBottomPos.y -= Mathf.Clamp(delta, 0, delta);
                Gizmos.DrawWireSphere(capsuleBottomPos, range.x);

                // Draw 4 lines to connect the two spheres to make a capsule
		        Vector3 startLine;
		        Vector3 endLine;
			
                startLine = capsuleTopPos;
                endLine = capsuleBottomPos;
                startLine.x += range.x;
                endLine.x += range.x;
                Gizmos.DrawLine(startLine, endLine);

                startLine = capsuleTopPos;
                endLine = capsuleBottomPos;
                startLine.x -= range.x;
                endLine.x -= range.x;
                Gizmos.DrawLine(startLine, endLine);

                startLine = capsuleTopPos;
                endLine = capsuleBottomPos;
                startLine.z += range.x;
                endLine.z += range.x;
                Gizmos.DrawLine(startLine, endLine);

                startLine = capsuleTopPos;
                endLine = capsuleBottomPos;
                startLine.z -= range.x;
                endLine.z -= range.x;
                Gizmos.DrawLine(startLine, endLine);

                break;
        }

        color.a = color.a * 0.5f;
        Gizmos.color = color;
        switch (tt.areaShape)
        {
			case AreaTargetTracker.AREA_SHAPES.Circle2D:	// Fallthrough
		        color.a = color.a * 0.5f;  // 50% again
        		Handles.color = color;
				Handles.DrawSolidDisc(pos, xform.forward, range.x);
				break;
            case AreaTargetTracker.AREA_SHAPES.Sphere:
                Gizmos.DrawSphere(pos, range.x);
                break;
			case AreaTargetTracker.AREA_SHAPES.Box2D:   	// Fallthrough
            case AreaTargetTracker.AREA_SHAPES.Box:
                Gizmos.DrawCube(pos, range);
                break;

            case AreaTargetTracker.AREA_SHAPES.Capsule:
                Gizmos.DrawSphere(capsuleTopPos, range.x);  // Set above
                Gizmos.DrawSphere(capsuleBottomPos, range.x);  // Set above
                break;
        }

        Gizmos.matrix = Matrix4x4.zero;  // Just to be clean		
    }
}
