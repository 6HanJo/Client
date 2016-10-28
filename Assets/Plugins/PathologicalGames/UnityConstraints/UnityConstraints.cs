/// <Licensing>
/// © 2011 (Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEngine;

namespace PathologicalGames
{
	/// <description>
	///	Holds system-wide members for UnityConstraints
	/// </description>
	[AddComponentMenu("")] // Hides from Unity4 Inspector menu
	public static class UnityConstraints
	{
	    /// <summary>
	    /// Constraint Mode options.
	    /// </summary>
	    public enum MODE_OPTIONS { Align, Constrain }
	
	    /// <summary>
	    /// Constraint Mode options. 
	    /// </summary>
	    public enum NO_TARGET_OPTIONS { Error, DoNothing, ReturnToDefault, SetByScript }
	
	    /// <summary>
	    /// Rotation interpolation options
	    /// </summary>
	    public enum INTERP_OPTIONS { Linear, Spherical, SphericalLimited }
	
	    /// <summary>
	    /// Rotations can't output to a combination of axis reliably so this is an enum.
	    /// </summary>
	    public enum OUTPUT_ROT_OPTIONS
	    {
	        WorldAll,
	        WorldX, WorldY, WorldZ,
	        LocalX, LocalY, LocalZ
	    }
	
	    private static float lastRealtimeSinceStartup = 0; // Used for Editor work
	
	    /// <summary>
	    /// Gets a rotation interpolated toward a target rotation based on the given interpolation 
	    /// method.
	    /// </summary>
	    /// <param name='currentRot'>
	    /// The starting rotation.
	    /// </param>
	    /// <param name='targetRot'>
	    /// Target rot to rotate toward.
	    /// </param>
	    /// <param name='interpolation'>
	    /// Interpolation mode determined by the INTERP_OPTIONS enum.
	    /// </param>
	    /// <param name='speed'>
	    /// Speed to rotate. This may be used differently by each interpolation mode.
	    /// </param>    
	    public static Quaternion GetInterpolateRotationTo(Quaternion currentRot, Quaternion targetRot,
			                                             INTERP_OPTIONS interpolation, float speed)
	    {
	
	        Quaternion newRot = Quaternion.identity;
	
	        float deltaTime;
	#if UNITY_EDITOR
	        if (Application.isPlaying) // deltaTime doesn't work in the editor
	        {
	#endif
	            deltaTime = Time.deltaTime;
	#if UNITY_EDITOR
	        }
	        else
	        {
	            float lastTime = UnityConstraints.lastRealtimeSinceStartup;
	            deltaTime = Time.realtimeSinceStartup - lastTime;
	            UnityConstraints.lastRealtimeSinceStartup = Time.realtimeSinceStartup;
	        }
	#endif
	
	
	        switch (interpolation)
	        {
	            case INTERP_OPTIONS.Linear:
	                newRot = Quaternion.Lerp(currentRot, targetRot, deltaTime * speed);
	                break;
	
	            case INTERP_OPTIONS.Spherical:
	                newRot = Quaternion.Slerp(currentRot, targetRot, deltaTime * speed);
	                break;
	
	            case INTERP_OPTIONS.SphericalLimited:
	                newRot = Quaternion.RotateTowards(currentRot, targetRot, speed * Time.timeScale);
	                break;
	
	        }
			
			return newRot;
			
		}
		
		
	    /// <summary>
	    /// Rotates a Transform toward a target rotation based on the given interpolation 
	    /// method in world space.
	    /// </summary>
	    /// </summary>
	    /// <param name='xform'>
	    /// Transform to rotate.
	    /// </param>
	    /// <param name='targetRot'>
	    /// Target rot to rotate toward.
	    /// </param>
	    /// <param name='interpolation'>
	    /// Interpolation mode determined by the INTERP_OPTIONS enum.
	    /// </param>
	    /// <param name='speed'>
	    /// Speed to rotate. This may be used differently by each interpolation mode.
	    /// </param>
	    public static void InterpolateRotationTo(Transform xform, Quaternion targetRot,
	                                             INTERP_OPTIONS interpolation, float speed)
		{
	        xform.rotation = GetInterpolateRotationTo
			(
				xform.rotation, 
				targetRot, 
				interpolation, 
				speed
			);
	    }
		
		
	    /// <summary>
	    /// Rotates a Transform toward a target rotation based on the given interpolation 
	    /// method in world space.
	    /// </summary>
	    /// <param name='xform'>
	    /// Transform to rotate.
	    /// </param>
	    /// <param name='targetRot'>
	    /// Target rot to rotate toward.
	    /// </param>
	    /// <param name='interpolation'>
	    /// Interpolation mode determined by the INTERP_OPTIONS enum.
	    /// </param>
	    /// <param name='speed'>
	    /// Speed to rotate. This may be used differently by each interpolation mode.
	    /// </param>
	    public static void InterpolateLocalRotationTo(Transform xform, Quaternion targetRot,
	                                             	  INTERP_OPTIONS interpolation, float speed)
	    {
	        xform.localRotation = GetInterpolateRotationTo
			(
				xform.localRotation, 
				targetRot, 
				interpolation, 
				speed
			);
	    }
	
	
	    /// <summary>
	    /// Applies the rotation options to a passed transform. The rotation should
	    /// already be set. This just returns axis to 0 depending on the constraint
	    /// OUTPUT_ROT_OPTIONS
	    /// </summary>
	    /// <param name="xform">The transform to process for</param>
	    /// <param name="option">The UnityConstraints.OUTPUT_ROT_OPTIONS to use</param>
	    public static void MaskOutputRotations(Transform xform, OUTPUT_ROT_OPTIONS option)
	    {
	        Vector3 angles;
	        switch (option)
	        {
	            case UnityConstraints.OUTPUT_ROT_OPTIONS.WorldAll:
	                // Already done
	                break;
	
	            case UnityConstraints.OUTPUT_ROT_OPTIONS.WorldX:
	                angles = xform.eulerAngles;
	                angles.y = 0;
	                angles.z = 0;
	                xform.eulerAngles = angles;
	                break;
	
	            case UnityConstraints.OUTPUT_ROT_OPTIONS.WorldY:
	                angles = xform.eulerAngles;
	                angles.x = 0;
	                angles.z = 0;
	                xform.eulerAngles = angles;
	                break;
	
	            case UnityConstraints.OUTPUT_ROT_OPTIONS.WorldZ:
	                angles = xform.eulerAngles;
	                angles.x = 0;
	                angles.y = 0;
	                xform.eulerAngles = angles;
	                break;
	
	            case UnityConstraints.OUTPUT_ROT_OPTIONS.LocalX:
	                angles = xform.localEulerAngles;
	                angles.y = 0;
	                angles.z = 0;
	                xform.localEulerAngles = angles;
	                break;
	
	            case UnityConstraints.OUTPUT_ROT_OPTIONS.LocalY:
	                angles = xform.localEulerAngles;
	                angles.x = 0;
	                angles.z = 0;
	                xform.localEulerAngles = angles;
	                break;
	
	            case UnityConstraints.OUTPUT_ROT_OPTIONS.LocalZ:
	                angles = xform.localEulerAngles;
	                angles.x = 0;
	                angles.y = 0;
	                xform.localEulerAngles = angles;
	                break;
	        }
	    }
	
	}
	
}
