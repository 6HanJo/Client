/// <Licensing>
/// © 2011 (Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEditor;
using UnityEngine;
using System.Collections;


namespace PathologicalGames
{

    [CustomEditor(typeof(TransformConstraint)), CanEditMultipleObjects]
    public class TransformConstraintInspector : ConstraintBaseInspector
    {
		protected SerializedProperty position;
		protected SerializedProperty outputPosX;
		protected SerializedProperty outputPosY;
		protected SerializedProperty outputPosZ;
		
		protected SerializedProperty rotation;
		protected SerializedProperty output;
		
		protected SerializedProperty scale;
		

	    protected override void OnEnable()
		{
			base.OnEnable();
	
			this.position   = this.serializedObject.FindProperty("constrainPosition");
			this.outputPosX = this.serializedObject.FindProperty("outputPosX");
			this.outputPosY = this.serializedObject.FindProperty("outputPosY");
			this.outputPosZ = this.serializedObject.FindProperty("outputPosZ");
			
			this.rotation = this.serializedObject.FindProperty("constrainRotation");
			this.output   = this.serializedObject.FindProperty("output");
			
			this.scale = this.serializedObject.FindProperty("constrainScale");
	    }
	
		protected override void OnInspectorGUIUpdate()
	    {
	        base.OnInspectorGUIUpdate();
			
			GUIContent content;
			
            GUILayout.BeginHorizontal();

			content = new GUIContent("Position", "Option to match the target's position.");
			EditorGUILayout.PropertyField(this.position, content);
			
            if (this.position.boolValue)
			{
				PGEditorUtils.ToggleButton(this.outputPosX, new GUIContent("X", "Toggle Costraint for this axis."), 24);
				PGEditorUtils.ToggleButton(this.outputPosY, new GUIContent("Y", "Toggle Costraint for this axis."), 24);
				PGEditorUtils.ToggleButton(this.outputPosZ, new GUIContent("Z", "Toggle Costraint for this axis."), 24);
			}
			
            GUILayout.EndHorizontal();
			
			content = new GUIContent("Rotation", "Option to match the target's rotation.");
			EditorGUILayout.PropertyField(this.rotation, content);

            if (this.rotation.boolValue)
			{
			    EditorGUI.indentLevel += 1;

				content = new GUIContent("Output", "Used to alter the way the rotations are set.");
				EditorGUILayout.PropertyField(this.output, content);
				
				EditorGUI.indentLevel -= 1;
			}
			
			content = new GUIContent("Scale", "Option to match the target's scale.");
			EditorGUILayout.PropertyField(this.scale, content);
			
		}
		
    }
}