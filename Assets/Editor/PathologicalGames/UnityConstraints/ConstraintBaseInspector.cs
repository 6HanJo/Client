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
	[CustomEditor(typeof(ConstraintBaseClass)), CanEditMultipleObjects]
	public class ConstraintBaseInspector : ConstraintFrameworkBaseInspector
	{
		protected SerializedProperty constraintTarget;
		protected SerializedProperty mode;
		protected SerializedProperty noTargetMode;
	
	    protected override void OnEnable()
		{
			base.OnEnable();
			
			this.constraintTarget = this.serializedObject.FindProperty("_target");
			this.mode 			  = this.serializedObject.FindProperty("_mode");
			this.noTargetMode 	  = this.serializedObject.FindProperty("_noTargetMode");		
	    }
		
	    protected override void OnInspectorGUIHeader()
	    {
	        base.OnInspectorGUIHeader();
			
	        var content = new GUIContent
			(
				"Target", 
				"The constraining object for this constraint."
			);
			EditorGUILayout.PropertyField(this.constraintTarget, content);		
	
	    }
	
	    protected override void OnInspectorGUIFooter()
	    {
	
	        GUIContent content;
	
			content = new GUIContent
			(
				"Mode", 
				"The current mode of the constraint. Setting the mode will start or stop the " +
					"constraint coroutine, so if 'Align' is chosen, the constraint will align " +
					"once then go to sleep."
			);
			EditorGUILayout.PropertyField(this.mode, content);	
			
			content = new GUIContent
			(
				"No-Target Mode", 
				"Determines the behavior when no target is available."
			);
			EditorGUILayout.PropertyField(this.noTargetMode, content);	
	
	        base.OnInspectorGUIFooter();
	    }
	
	}
}