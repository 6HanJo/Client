/// <Licensing>
/// � 2011 (Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PathologicalGames
{
    /// <summary>
    ///	Adds the ability to only fire on Targets within a distance range of a TargetTracker.
	/// This is like having a second spherical Area. Useful when you want objects to be 
	/// "detected" but not actually able to be fired on until they get closer.
    /// </summary>
    [RequireComponent(typeof(EventFireController))]
    public class WaitForAlignmentModifier : TriggerEventProModifier
    {
		/// <summary>
		/// The transform used to determine alignment. If not used, this will be the 
		/// transform of the GameObject this component is attached to.
		/// </summary>
        public Transform origin
		{
			get 
			{
				if (this._origin == null)
					return this.transform;
				else
					return this._origin;
			}
			
			set
			{
				this._origin = value;
			}
		}
		[SerializeField]
		protected Transform _origin = null;  // Explicit for clarity

		/// <summary>
        /// The angle, in degrees, to use for checks.
        /// </summary>
        public float angleTolerance = 5;

		/// <summary>
        /// If false the actual angles will be compared for alignment (More precise. origin  
        /// must point at target). If true, only the direction matters. (Good when turning 
        /// in a direction but perfect alignment isn't needed, e.g. lobing bombs as opposed 
		/// to shooting AT something)
        /// </summary>
        public bool flatAngleCompare = false;
		
		public enum COMPARE_AXIS {X, Y, Z}
		
		/// <summary>
		/// Determines the axis to use when doing angle comparisons.
		/// </summary>
		public COMPARE_AXIS flatCompareAxis = COMPARE_AXIS.Z;
		
		public enum FILTER_TYPE {IgnoreTargetTracking, WaitToFireEvent}
		
		/// <summary>
		/// Determines if filtering will be performed on a TargetTracker's targets or an 
		/// EventTrigger's targets. See the component description for more detail. This is 
		/// ignored unless a its GameObject has both. Otherwise the mode will be auto-detected.
		/// </summary>
		public FILTER_TYPE filterType = FILTER_TYPE.WaitToFireEvent;
		
		public DEBUG_LEVELS debugLevel = DEBUG_LEVELS.Off;		
		
		protected EventFireController fireCtrl;
		protected TargetTracker tracker;
		protected Target currentTarget;  // Loop cache
		protected List<Target> iterTargets = new List<Target>();  // Loop cache
		

		#region CHANGED AWAKE TO MIMIC OLD VERSION OF THIS COMPONENT
        protected void Awake()
        {
			Debug.LogWarning(string.Format(
				"WaitForAlignementModifier on GameObject {0} has been deprecated. Replace the " +
				"component with AngleLimitModifier (with the filterType set to 'Wait to Fire " +
				"Event'). You can do this without losing your other settings by switching the " +
				"Inspector tab to 'Debug' and changing the script field.",
				this.name
			));
			
            this.fireCtrl = this.GetComponent<EventFireController>();

			// Force because this old component could only do this
			this.filterType = FILTER_TYPE.WaitToFireEvent;
			
        }
		#endregion CHANGED AWAKE TO MIMIC OLD VERSION OF THIS COMPONENT
		
		
		
		
		// OnEnable and OnDisable add the check box in the inspector too.
		protected void OnEnable()
		{
            if (this.tracker != null)
				this.tracker.AddOnPostSortDelegate(this.FilterTrackerTargetList);

			if (this.fireCtrl != null)
                this.fireCtrl.AddOnPreFireDelegate(this.FilterFireCtrlTargetList);			
		}
		
		protected void OnDisable()
		{
            if (this.tracker != null)
				this.tracker.RemoveOnPostSortDelegate(this.FilterTrackerTargetList);

			if (this.fireCtrl != null)
                this.fireCtrl.RemoveOnPreFireDelegate(this.FilterFireCtrlTargetList);

		}
		
        protected void FilterFireCtrlTargetList(TargetList targets)
        {	
			if (this.filterType == FILTER_TYPE.WaitToFireEvent)
				this.FilterTargetList(targets);
			
			// Else do nothing.
		}
		
        protected void FilterTrackerTargetList(TargetTracker source, TargetList targets)
        {	
			if (this.filterType == FILTER_TYPE.IgnoreTargetTracking)
				this.FilterTargetList(targets);
			
			// Else do nothing.
		}
		
        protected void FilterTargetList(TargetList targets)
        {			
#if UNITY_EDITOR
            var debugRemoveNames = new List<string>();
#endif
			this.iterTargets.Clear();
            this.iterTargets.AddRange(targets);
            for (int i = 0; i < iterTargets.Count; i++)
            {
				this.currentTarget = iterTargets[i];

                if (this.IsInAngle(this.currentTarget))
                    continue;  // This target is good. Don't ignore it. Continue.
				                     
#if UNITY_EDITOR
				debugRemoveNames.Add(this.currentTarget.targetable.name);
#endif
				targets.Remove(this.currentTarget);                
            }

#if UNITY_EDITOR
            if (this.debugLevel == DEBUG_LEVELS.High && debugRemoveNames.Count > 0)
			{
                string msg = string.Format
				(
					"Holding fire due to misalignment: {0}",
					string.Join(", ", debugRemoveNames.ToArray())
				);
				
				Debug.Log(string.Format("{0}: {1}", this, msg));
			}
#endif
        }
	
		
		protected bool IsInAngle(Target target)
        {
            Vector3 targetDir = target.transform.position - this.origin.position;
            Vector3 forward = this.origin.forward;
            if (this.flatAngleCompare) 
			{
				switch (this.flatCompareAxis)
				{
					case COMPARE_AXIS.X:
						targetDir.x = forward.x = 0; // Flatten Vectors
						break;
					
					case COMPARE_AXIS.Y:
						targetDir.y = forward.y = 0; // Flatten Vectors
						break;
					
					case COMPARE_AXIS.Z:
						targetDir.z = forward.z = 0; // Flatten Vectors
						break;
				}
				
			}

			bool isInAngle = Vector3.Angle(targetDir, forward) < this.angleTolerance;

#if UNITY_EDITOR
			if (this.debugLevel > DEBUG_LEVELS.Off)
			{
				// Multiply the direction Vector by distance to make the line longer
				forward = this.origin.forward * targetDir.magnitude;
				if (isInAngle)
				{
	                Debug.DrawRay(this.origin.position, forward, Color.green, 0.01f);
				}
				else
				{
	                Debug.DrawRay(this.origin.position, forward, Color.red, 0.01f);
	                Debug.DrawRay(this.origin.position, targetDir, Color.yellow, 0.01f);
				}				
			}
#endif
			return isInAngle;
        }

    }
}