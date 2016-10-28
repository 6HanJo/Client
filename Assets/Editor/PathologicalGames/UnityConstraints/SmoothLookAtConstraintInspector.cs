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
	[CustomEditor(typeof(SmoothLookAtConstraint)), CanEditMultipleObjects]
	public class SmoothLookAtConstraintInspector : LookAtConstraintInspector
	{
		protected SerializedProperty interpolation;
		protected SerializedProperty output;
		protected SerializedProperty speed;
	
	    protected override void OnEnable()
		{
			base.OnEnable();
	
			this.interpolation = this.serializedObject.FindProperty("interpolation");
			this.output = this.serializedObject.FindProperty("output");
			this.speed  = this.serializedObject.FindProperty("speed");
	    }
		
	    protected override void OnInspectorGUIUpdate()
	    {
	        base.OnInspectorGUIUpdate();	
			
			GUIContent content;
	
			content = new GUIContent
			(
				"Interpolation", 
				"The rotation interpolation solution to use."
			);
			EditorGUILayout.PropertyField(this.interpolation, content);	
			
	        
			content = new GUIContent
			(
				"Speed", 
				"How fast the constrant can rotate. The result depends on the interpolation chosen."
			);
			EditorGUILayout.PropertyField(this.speed, content);	
			
			
			content = new GUIContent
			(
				"Output", 
				"What axes and space to output the result to."
			);
			EditorGUILayout.PropertyField(this.output, content);			
	    }
	}
}