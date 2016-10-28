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
	[CustomEditor(typeof(ConstraintFrameworkBaseClass)), CanEditMultipleObjects]
	public class ConstraintFrameworkBaseInspector : Editor
	{
	    protected virtual void OnEnable()
		{
			
		}
			
	    // This is Unity's. Block from sub-classes - Use header, update and footer callbacks callbacks
		public override void OnInspectorGUI()
	    {		
	        // Used like a header to set a global label width
	        EditorGUI.indentLevel = 0;
			EditorGUIUtility.labelWidth = PGEditorUtils.CONTROLS_DEFAULT_LABEL_WIDTH;

			this.serializedObject.Update();
	
	        this.OnInspectorGUIHeader();
	        this.OnInspectorGUIUpdate();
	
	        EditorGUILayout.Space();
	
	        this.OnInspectorGUIFooter();
	
	        // Flag Unity to save the changes to to the prefab to disk
			serializedObject.ApplyModifiedProperties();
	        if (GUI.changed)
	            EditorUtility.SetDirty(target);
	    }
	
	    // Three functions to inherit instead of one for greater flexibility. 
	    protected virtual void OnInspectorGUIHeader()
	    {
	
		}
	
	    protected virtual void OnInspectorGUIUpdate()
	    {
	
		}
	
	    protected virtual void OnInspectorGUIFooter()
	    {
	
		}
	
	}
}