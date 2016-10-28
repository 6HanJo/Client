/// <Licensing>
/// ©2011-2014 (Copyright) Path-o-logical Games, LLC
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
    [AddComponentMenu("Path-o-logical/TriggerEventPRO/TriggerEventPRO Modifier - Fire Distance")]
    [RequireComponent(typeof(EventFireController))]
    public class FireDistanceModifier : TriggerEventProModifier
    {
		public enum ORIGIN_MODE 
		{ 
			TargetTracker,
			TargetTrackerArea,
			FireController,
			FireControllerEmitter 
		}
		
		/// <summary>
		/// The origin is the point from which the distance is measuered.
		/// </summary>
		public ORIGIN_MODE originMode = ORIGIN_MODE.FireController;
		
		public float minDistance = 0;
		public float maxDistance = 1;
        public DEBUG_LEVELS debugLevel = DEBUG_LEVELS.Off;

#if UNITY_EDITOR
		// Inspector-Only
        public bool drawGizmo = true;
        public Color gizmoColor = new Color(0, 0.7f, 1, 1); // Cyan-blue mix
        public Color defaultGizmoColor { get { return new Color(0, 0.7f, 1, 1); } }
        public bool overrideGizmoVisibility = false;
#endif
		
        // Public for reference and Inspector logic
        public EventFireController fireCtrl;
		public Transform origin
		{
			get 
			{
				Transform xform = this.transform;
				switch (this.originMode)
				{
					case ORIGIN_MODE.TargetTracker:
						xform = this.fireCtrl.targetTracker.transform;
						break;
						
					case ORIGIN_MODE.TargetTrackerArea:
						// If there is no Area, use the TargetTracker itself
						Area area = this.fireCtrl.targetTracker.area;
						if (area == null)
							throw new MissingReferenceException(string.Format
							(
								"FireController {0}'s TargetTracker doesn't have an Area" + 
								"If by design, such as a CollisionTargetTracker, use the " + 
								"'TargetTracker' or other Origin Mode option.",
								this.fireCtrl.name
							));
						
						xform = area.transform;
						break;
						
					case ORIGIN_MODE.FireController:
			            xform = this.origin = this.transform;
						break;
						
					case ORIGIN_MODE.FireControllerEmitter:
						// If there is no emitter set, use this (same as FireController).
			            if (this.fireCtrl.spawnEventTriggerAtTransform == null)
							throw new MissingReferenceException(string.Format
							(
								"FireController {0} doesn't have an emitter set. " + 
								"Add one or use the 'Fire Controller' Origin Mode option.",
								this.fireCtrl.name
							));

			            xform = this.fireCtrl.spawnEventTriggerAtTransform;
			            break;
				}
				
				return xform;
			}
			set {}
		}
		
		protected Target currentTarget;  // Loop cache
		protected List<Target> iterTargets = new List<Target>();  // Loop cache
		
        protected void Awake()
        {
            this.fireCtrl = this.GetComponent<EventFireController>();
        }
		
		// OnEnable and OnDisable add the check box in the inspector too.
		protected void OnEnable()
		{
			this.fireCtrl.AddOnPreFireDelegate(this.FilterFireTargetList);
		}
		
		protected void OnDisable()
		{
			if (this.fireCtrl != null)  // For when levels or games are dumped
					this.fireCtrl.RemoveOnPreFireDelegate(this.FilterFireTargetList);
		}
		
        protected void FilterFireTargetList(TargetList targets)
        {			
			// Get the position expected to be used to fire from.
            Vector3 fromPos = this.origin.position;
			
#if UNITY_EDITOR
            var debugRemoveNames = new List<string>();
#endif
			this.iterTargets.Clear();
            this.iterTargets.AddRange(targets);
			float dist;
            for (int i = 0; i < iterTargets.Count; i++)
            {
				this.currentTarget = iterTargets[i];
				dist = this.currentTarget.targetable.GetDistToPos(fromPos);

                // Skip if the target is in the distance range
                if (dist > this.minDistance && dist < this.maxDistance)
				{
#if UNITY_EDITOR
	                if (this.debugLevel > DEBUG_LEVELS.Off)
	                    Debug.DrawLine
						(
							fromPos, 
							this.currentTarget.targetable.transform.position, 
							Color.green, 
							0.01f
						);
#endif
                    continue;
				}
				                     
#if UNITY_EDITOR
                if (this.debugLevel > DEBUG_LEVELS.Off)
                    Debug.DrawLine
					(
						fromPos, 
						this.currentTarget.targetable.transform.position, 
						Color.red, 
						0.01f
					);

#endif
				targets.Remove(this.currentTarget);

#if UNITY_EDITOR
                debugRemoveNames.Add(this.currentTarget.targetable.name);
#endif
                
            }

#if UNITY_EDITOR
            if (this.debugLevel == DEBUG_LEVELS.High && debugRemoveNames.Count > 0)
			{
                string msg = string.Format
				(
					"Holding fire due to distance: {0}",
					string.Join(", ", debugRemoveNames.ToArray())
				);
				
				Debug.Log(string.Format("{0}: {1}", this, msg));
			}
#endif

        }

    }
}