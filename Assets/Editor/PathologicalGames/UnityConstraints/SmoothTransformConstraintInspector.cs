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

    [CustomEditor(typeof(SmoothTransformConstraint)), CanEditMultipleObjects]
    public class SmoothTransformConstraintInspector : ConstraintBaseInspector
    {
		protected SerializedProperty position;
		protected SerializedProperty outputPosX;
		protected SerializedProperty outputPosY;
		protected SerializedProperty outputPosZ;
		protected SerializedProperty pos_interp;
		protected SerializedProperty positionSpeed;
		protected SerializedProperty positionMaxSpeed;
		
		protected SerializedProperty rotation;
		protected SerializedProperty output;
		protected SerializedProperty interpolation;
		protected SerializedProperty rotationSpeed;
		
		protected SerializedProperty scale;
		protected SerializedProperty scaleSpeed;
		

	    protected override void OnEnable()
		{
			base.OnEnable();
	
			this.position   = this.serializedObject.FindProperty("constrainPosition");
			this.outputPosX = this.serializedObject.FindProperty("outputPosX");
			this.outputPosY = this.serializedObject.FindProperty("outputPosY");
			this.outputPosZ = this.serializedObject.FindProperty("outputPosZ");
			this.pos_interp = this.serializedObject.FindProperty("position_interpolation");
			this.positionSpeed = this.serializedObject.FindProperty("positionSpeed");
			this.positionMaxSpeed = this.serializedObject.FindProperty("positionMaxSpeed");
			
			this.rotation = this.serializedObject.FindProperty("constrainRotation");
			this.output   = this.serializedObject.FindProperty("output");
			this.interpolation = this.serializedObject.FindProperty("interpolation");
			this.rotationSpeed = this.serializedObject.FindProperty("rotationSpeed");
			
			this.scale = this.serializedObject.FindProperty("constrainScale");
			this.scaleSpeed = this.serializedObject.FindProperty("scaleSpeed");
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
	            GUILayout.EndHorizontal();

			    EditorGUI.indentLevel += 1;

				content = new GUIContent("interpolation Mode", "Option to match the target's position.");
				EditorGUILayout.PropertyField(this.pos_interp, content);
				
				var posInterp = (SmoothTransformConstraint.INTERP_OPTIONS_POS)this.pos_interp.enumValueIndex;
				switch (posInterp)
                {
                    case SmoothTransformConstraint.INTERP_OPTIONS_POS.DampLimited:
						content = new GUIContent("Time", "roughly how long it takes to match the target (lag).");
						EditorGUILayout.PropertyField(this.positionSpeed, content);
					
						content = new GUIContent("Max Speed", "The speed limit. Causes more constant motion.");
						EditorGUILayout.PropertyField(this.positionMaxSpeed, content);
                        break;

                    case SmoothTransformConstraint.INTERP_OPTIONS_POS.Damp:
						content = new GUIContent("Time", "roughly how long it takes to match the target (lag).");
						EditorGUILayout.PropertyField(this.positionSpeed, content);
                        break;

                    default:
						content = new GUIContent("Percent", "Controls lag.");
						EditorGUILayout.PropertyField(this.positionSpeed, content);
                        break;
                }
				
			    EditorGUI.indentLevel -= 1;
			}
			else
			{
            	GUILayout.EndHorizontal();
			}
			
			content = new GUIContent("Rotation", "Option to match the target's rotation.");
			EditorGUILayout.PropertyField(this.rotation, content);

            if (this.rotation.boolValue)
			{
			    EditorGUI.indentLevel += 1;

				content = new GUIContent("Output", "Used to alter the way the rotations are set.");
				EditorGUILayout.PropertyField(this.output, content);
				
				content = new GUIContent
				(
					"Interpolation Mode", 
					"The type of rotation calculation to use. Linear is faster but spherical is more stable when " +
						"rotations make large adjustments.."
				);
				EditorGUILayout.PropertyField(this.interpolation, content);
				
				content = new GUIContent("Speed", "How fast to rotate. Appears different depending on Mode.");
				EditorGUILayout.PropertyField(this.rotationSpeed, content);
				
				EditorGUI.indentLevel -= 1;
			}
			
			content = new GUIContent("Scale", "Option to match the target's scale.");
			EditorGUILayout.PropertyField(this.scale, content);
			
            if (this.scale.boolValue)
			{
			    EditorGUI.indentLevel += 1;

				content = new GUIContent("Percent", "Controls lag.");
				EditorGUILayout.PropertyField(this.scaleSpeed, content);
								
				EditorGUI.indentLevel -= 1;
			}
		}		
		
//        protected override void OnInspectorGUIUpdate()
//        {
//            base.OnInspectorGUIUpdate();
//
//            var script = (SmoothTransformConstraint)target;
//
//            GUILayout.BeginHorizontal();
//
//            script.constrainPosition = EditorGUILayout.Toggle("Position", script.constrainPosition);
//
//            if (script.constrainPosition)
//            {
//                GUIStyle style = EditorStyles.toolbarButton;
//                style.alignment = TextAnchor.MiddleCenter;
//                style.stretchWidth = true;
//
//                script.outputPosX = GUILayout.Toggle(script.outputPosX, "X", style);
//                script.outputPosY = GUILayout.Toggle(script.outputPosY, "Y", style);
//                script.outputPosZ = GUILayout.Toggle(script.outputPosZ, "Z", style);
//            }
//            GUILayout.EndHorizontal();
//
//            if (script.constrainPosition)
//            {
//                EditorGUI.indentLevel = 2;
//
//                script.position_interpolation = PGEditorUtils.EnumPopup<SmoothTransformConstraint.INTERP_OPTIONS_POS>
//                (
//                    "Interpolation Mode",
//                    script.position_interpolation
//                );
//
//                switch (script.position_interpolation)
//                {
//                    case SmoothTransformConstraint.INTERP_OPTIONS_POS.DampLimited:
//                        script.positionSpeed = EditorGUILayout.FloatField("Time", script.positionSpeed);
//                        script.positionMaxSpeed = EditorGUILayout.FloatField("Max Speed", script.positionMaxSpeed);
//                        break;
//
//                    case SmoothTransformConstraint.INTERP_OPTIONS_POS.Damp:
//                        script.positionSpeed = EditorGUILayout.FloatField("Time", script.positionSpeed);
//                        break;
//
//                    default:
//                        script.positionSpeed = EditorGUILayout.FloatField("Percent", script.positionSpeed);
//                        break;
//                }
//
//
//                EditorGUI.indentLevel = 0;
//                EditorGUILayout.Space();
//            }
//
//            GUILayout.BeginHorizontal();
//            script.constrainRotation = EditorGUILayout.Toggle("Rotation", script.constrainRotation);
//            if (script.constrainRotation)
//                script.output = PGEditorUtils.EnumPopup<UnityConstraints.OUTPUT_ROT_OPTIONS>(script.output);
//            GUILayout.EndHorizontal();
//
//            if (script.constrainRotation)
//            {
//                EditorGUI.indentLevel = 2;
//                script.interpolation = PGEditorUtils.EnumPopup<UnityConstraints.INTERP_OPTIONS>
//                (
//                    "Interpolation Mode",
//                    script.interpolation
//                );
//                script.rotationSpeed = EditorGUILayout.FloatField("Speed", script.rotationSpeed);
//
//                EditorGUI.indentLevel = 0;
//                EditorGUILayout.Space();
//            }
//
//            script.constrainScale = EditorGUILayout.Toggle("Scale", script.constrainScale);
//            if (script.constrainScale)
//            {
//                EditorGUI.indentLevel = 2;
//                script.scaleSpeed = EditorGUILayout.FloatField("Percent", script.scaleSpeed);
//                EditorGUI.indentLevel = 0;
//                EditorGUILayout.Space();
//            }
//
//        }
    }
}