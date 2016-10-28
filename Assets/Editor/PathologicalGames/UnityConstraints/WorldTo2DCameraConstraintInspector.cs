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

    [CustomEditor(typeof(WorldTo2DCameraConstraint)), CanEditMultipleObjects]
    public class WorldTo2DCameraConstraintInspector : TransformConstraintInspector
    {
        // Singleton cache to set some defaults on inspection
        Camera[] cameras;

        protected override void OnInspectorGUIUpdate()
        {
            var script = (WorldTo2DCameraConstraint)target;

            script.targetCamera = PGEditorUtils.ObjectField<Camera>("Target Camera", script.targetCamera);
            script.orthoCamera = PGEditorUtils.ObjectField<Camera>("Output Camera", script.orthoCamera);

            EditorGUILayout.Space();

            script.offsetMode = PGEditorUtils.EnumPopup<WorldTo2DCameraConstraint.OFFSET_MODE>
            (
                "Offset Mode",
                script.offsetMode
            );

            script.offset = EditorGUILayout.Vector3Field("Offset", script.offset);



            EditorGUILayout.Space();

            script.offScreenMode = PGEditorUtils.EnumPopup<WorldTo2DCameraConstraint.OFFSCREEN_MODE>
            (
                "Offscreen Mode",
                script.offScreenMode
            );

            EditorGUI.indentLevel += 2;
            if (script.offScreenMode != WorldTo2DCameraConstraint.OFFSCREEN_MODE.Constrain)
            {
                script.offscreenThreasholdH =
                    EditorGUILayout.Vector2Field("Height Threashold", script.offscreenThreasholdH);

                script.offscreenThreasholdW =
                    EditorGUILayout.Vector2Field("Width Threashold", script.offscreenThreasholdW);
            }

            EditorGUI.indentLevel -= 2;

            EditorGUILayout.Space();

            base.OnInspectorGUIUpdate();



            // Set some singletone defaults (will only run once)..

            // This will actually run when the inspector changes, but still better than
            //   running every update
            if (this.cameras == null)
                this.cameras = FindObjectsOfType(typeof(Camera)) as Camera[];

            // Default to the first ortho camera that is set to render this object
            if (script.orthoCamera == null)
            {
                foreach (Camera cam in cameras)
                {
                    if (!cam.orthographic)
                        continue;

                    if ((cam.cullingMask & 1 << script.gameObject.layer) > 0)
                    {
                        script.orthoCamera = cam;
                        break;
                    }
                }
            }

            // Default to the first camera that is set to render the target
            if (script.target != null && script.targetCamera == null)
            {
                foreach (Camera cam in cameras)
                {
                    if ((cam.cullingMask & 1 << script.target.gameObject.layer) > 0)
                    {
                        script.targetCamera = cam;
                        break;
                    }
                }
            }
        }
    }
}