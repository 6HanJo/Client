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
	[CustomEditor(typeof(LookAtBaseClass)), CanEditMultipleObjects]
	public class LookAtBaseClassInspector : ConstraintBaseInspector
	{
		protected SerializedProperty pointAxis;
		protected SerializedProperty upAxis;
	
	    protected override void OnEnable()
		{
			base.OnEnable();
	
			this.pointAxis = this.serializedObject.FindProperty("pointAxis");
			this.upAxis    = this.serializedObject.FindProperty("upAxis");
	    }
	
		protected override void OnInspectorGUIUpdate()
	    {
	        base.OnInspectorGUIUpdate();
			
			GUIContent content;
				
			content = new GUIContent
			(
				"Point Axis", 
				"The axis used to point at the target."
			);
			EditorGUILayout.PropertyField(this.pointAxis, content);
	
			content = new GUIContent
			(
				"Up Axis", 
				"The axis to stabalize the look-at result so it does roll on the point axis. This should never be the " +
				"same as the point axis."
			);
			EditorGUILayout.PropertyField(this.upAxis, content);		
	
	    }
	}
}