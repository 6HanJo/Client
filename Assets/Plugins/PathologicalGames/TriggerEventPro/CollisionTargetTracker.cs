/// <Licensing>
/// ©2011-2014 (Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace PathologicalGames
{
    /// <summary>
    ///	A TargetTracker that detects Targetables that are physically in contact with the 
	/// Tracker GameObject's collider (Rigidbody.isTrigger = False);. To detect Targetables 
	/// in range, use an AreaTargetTracker. This also works with Compound Colliders which Unity 
	/// doesn't support for triggers.
    /// </summary>
    [AddComponentMenu("Path-o-logical/TriggerEventPRO/TriggerEventPRO Collision TargetTracker")]
    public class CollisionTargetTracker : TargetTracker
    {
        /// <summary>
        /// A list of sorted targets. The contents depend on numberOfTargets requested
		/// (-1 for all targets in the Area), and the sorting style used.
        /// </summary>
        public override TargetList targets
        {
            get
            {
                // Start with an empty list each pass.
                this._targets.Clear();

                // If none are wanted, quit
                if (this.numberOfTargets == 0)
                    return this._targets;

                // Filter for any targets that are no longer active before returning them
                var validTargets = new List<Target>(this.allTargets);
                foreach (Target target in this.allTargets)
                    if (target.gameObject == null || !target.gameObject.activeInHierarchy)
                        validTargets.Remove(target);

                // If no targets available, quit
                if (validTargets.Count == 0)
                    return this._targets;

                // None == Area-of-effect, so get everything in range, otherwise, 
                //   Get the first item(s). Since everything is sorted based on the
                //   sortingStyle, the first item(s) will always work
                if (this.numberOfTargets == -1)
                {
                    this._targets.AddRange(validTargets);
                }
                else
                {
                    // Grab the first item(s)
                    int num = Mathf.Clamp(this.numberOfTargets, 0, validTargets.Count);
                    for (int i = 0; i < num; i++)
                        this._targets.Add(validTargets[i]);
                }

                if (this.onPostSortDelegates != null) 
					this.onPostSortDelegates(this, this._targets);

#if UNITY_EDITOR
                if (this.debugLevel > DEBUG_LEVELS.Normal)  // All higher than normal
                {
                    string msg = string.Format("returning targets: {0}",
                                               this._targets.ToString());
                    Debug.Log(string.Format("{0}: {1}", this, msg));
                }
#endif
                return _targets;
            }
        }
        protected TargetList allTargets = new TargetList();

        public Collider coll;
		public Collider2D coll2D;

        protected override void Awake()
        {
            base.Awake();

            this.coll = this.GetComponent<Collider>();
            this.coll2D = this.GetComponent<Collider2D>();
			if (this.coll == null && this.coll2D == null)
			{
				string msg = "No 2D or 3D collider or compound (child) collider found.";
                throw new MissingComponentException(msg);
			}
			
			// Make sure the collider used is not set to be a trigger.
            if ((this.coll != null && this.coll.isTrigger) ||
				(this.coll2D != null && this.coll2D.isTrigger))
				
			{
                throw new Exception
                (
                    "CollisionTargetTrackers do not work with trigger colliders." +
                    "It is designed to work with Physics OnCollider events only."
                );
			}
        }

		
		
        #region Events
        /// <summary>
        /// 3D Collider Event
        /// </summary>
        protected void OnCollisionEnter(Collision collisionInfo)
        {
 			this.OnCollisionEnterAny(collisionInfo.gameObject);
        }

        /// <summary>
        /// 3D Collider Event
        /// </summary>
        protected void OnCollisionExit(Collision collisionInfo)
        {
            // Will only work if there is something to remove. Debug logging inside too.
            this.OnCollisionExitAny(collisionInfo.gameObject);
        }
		
		/// <summary>
        /// 2D Collider Event
        /// </summary>
        protected void OnCollisionEnter2D(Collision2D collisionInfo)
        {
			this.OnCollisionEnterAny(collisionInfo.gameObject);
        }

        /// <summary>
        /// 2D Collider Event
        /// </summary>
        protected void OnCollisionExit2D(Collision2D collisionInfo)
        {
            // Will only work if there is something to remove. Debug logging inside too.
            this.OnCollisionExitAny(collisionInfo.gameObject);
        }

		/// <summary>
		/// Handles 2D or 3D event the same way
        /// </summary>
		protected void OnCollisionEnterAny(GameObject otherGo)
		{
            if (!this.IsInLayerMask(otherGo)) 
				return;

            // Get a target struct which will also cache information, such as the Transfrom,
            //   GameObject and ITargetable component
            var target = new Target(otherGo.transform, this);
            if (target == Target.Null)
				return;

            // Ignore if isTargetable is false
            if (!target.targetable.isTargetable)
				return;
			
			// Give delegates the chance to ignore this Target.
			if (this.IsIgnoreTargetOnNewDetectedDelegatesExecute(target))
				return;
			
            if (!this.allTargets.Contains(target))
                this.allTargets.Add(target);
			
			// TODO: TRIGGER ON DETECTED
#if UNITY_EDITOR
            if (this.debugLevel > DEBUG_LEVELS.Off)
            {
                string msg = string.Format
                (
                    "OnCollisionEnter detected target: {0} | All Targets = [{1}]",
                    target.targetable.name,
                    this.allTargets.ToString()
                );
                Debug.Log(string.Format("{0}: {1}", this, msg));
            }
#endif

		}
		
		/// <summary>
		/// Handles 2D or 3D event the same way
        /// </summary>
        protected void OnCollisionExitAny(GameObject otherGO)
		{
            // Note: Iterating and comparing cached data should be the fastest way...
            var target = new Target();
            foreach (Target currentTarget in this.allTargets)
                if (currentTarget.gameObject == otherGO)
                    target = currentTarget;

            if (target == Target.Null)
                return;

            this.StartCoroutine(this.DelayRemove(target));

#if UNITY_EDITOR
            if (this.debugLevel > DEBUG_LEVELS.Off)  // All higher than normal
            {
                string msg = string.Format
                (
                    "OnCollisionExit no longer tracking target: {0} | All Targets = [{1}]",
                    target.targetable.name,
                    this.allTargets.ToString()
                );
                Debug.Log(string.Format("{0}: {1}", this, msg));
            }
#endif		
		}
        #endregion Events		
		

        protected IEnumerator DelayRemove(Target target)
        {
            yield return null;

            if (this.allTargets.Contains(target))
                this.allTargets.Remove(target);
        }

        #region protected Utilities
        /// <summary>
        /// Checks if a GameObject is in a LayerMask
        /// </summary>
        /// <param name="obj">GameObject to test</param>
        /// <param name="layerMask">LayerMask with all the layers to test against</param>
        /// <returns>True if in any of the layers in the LayerMask</returns>
        protected bool IsInLayerMask(GameObject obj)
        {
            // Convert the object's layer to a bit mask for comparison
            LayerMask objMask = 1 << obj.layer;
            LayerMask targetMask = this.targetLayers;
            if ((targetMask.value & objMask.value) == 0) // Extra brackets required!
                return false;
            else
                return true;
        }

        #endregion protected Utilities

    }
}