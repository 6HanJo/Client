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
	///	Adds Angle filtering to TargetPRO components. Angle filtering is defined as a target
	/// being in a specific firing arc or it is ignored.
	///	
	/// If added to the same GameObject as a TargetTracker it can filter out any targets 
	/// which are not currently in the arc.
	/// 
	/// If added to the same GameObject as a FireController it can prevent firing on any 
	/// targets which are not currently in the arc. When the angle is set low, 5 degrees, 
	/// 5 degrees for example, it can be used to only fire when something is in front of 
	/// the origin. This can be handy for creating things which need to turn before firing, 
	/// such as turrets and enimies. Or it can be used for triggering things like booby traps 
	/// and detecting when something enters an area or room.
    /// </summary>
    [AddComponentMenu("Path-o-logical/TriggerEventPRO/TriggerEventPRO Modifier - Angle Limit Modifier")]
    public class AngleLimitModifier : TriggerEventProModifier
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
        public float angle = 5;

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
		
        protected void Awake()
        {
            this.fireCtrl = this.GetComponent<EventFireController>();

			// If a fireController was found it is cheaper to use its cached reference to get the 
			//	 TargetTracker
			if (this.fireCtrl != null)
				this.tracker = this.fireCtrl.targetTracker;
			else
				this.tracker = this.GetComponent<TargetTracker>();
			
			// Just in case
			if (this.fireCtrl == null && this.tracker == null)
			{
				throw new MissingComponentException
				(
					"Must have at least a TargetTracker or EventFireController"
				);			
			}
			
			// If only 1 is set, force the filterType
			if (this.fireCtrl == null || this.tracker == null)
			{
				if (this.fireCtrl != null)
					this.filterType = FILTER_TYPE.WaitToFireEvent;
				else
					this.filterType = FILTER_TYPE.IgnoreTargetTracking;
			}
			
        }
		
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

			bool isInAngle = Vector3.Angle(targetDir, forward) < this.angle;

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