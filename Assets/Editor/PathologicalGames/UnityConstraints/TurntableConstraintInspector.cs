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
	[CustomEditor(typeof(TurntableConstraint)), CanEditMultipleObjects]
	public class TurntableConstraintInspector : ConstraintFrameworkBaseInspector
	{
		protected SerializedProperty randomStart;
		protected SerializedProperty speed;
	
	    protected override void OnEnable()
		{
			base.OnEnable();
	
			this.randomStart = this.serializedObject.FindProperty("randomStart");
			this.speed  = this.serializedObject.FindProperty("speed");
	    }
		
	    protected override void OnInspectorGUIUpdate()
	    {
	        base.OnInspectorGUIUpdate();	
	
	        GUIContent content;
	
			content = new GUIContent
			(
				"Speed", 
				"How fast the constrant can rotate."
			);
			EditorGUILayout.PropertyField(this.speed, content);	
			
			
			content = new GUIContent
			(
				"Random Start", 
				"If true, each time the constraint is enabled it will start from a new and random " +
					"angle of rotation. This is helpful when several objects start rotating on the " +
					"same frame but you do not want them to all rotate insync with each other " +
					"visually." 
			);
			EditorGUILayout.PropertyField(this.randomStart, content);			
			
	    }
	}
}
