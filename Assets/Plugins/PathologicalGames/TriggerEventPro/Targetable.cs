/// <Licensing>
/// ©2011-2014 (Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEngine;
using System.Collections.Generic;


namespace PathologicalGames
{
    /// <summary>
    ///	Adding this component to a gameObject will make it detectable to TargetTrackers, recieve 
	/// EventInfo and expose event delegates to run attached custom compponent methods.
    /// </summary>
    [AddComponentMenu("Path-o-logical/TriggerEventPRO/TriggerEventPRO Targetable")]
    public class Targetable : MonoBehaviour
    {
        #region Public Parameters
		/// <summary>
		/// Indicates whether this <see cref="PathologicalGames.Targetable"/> is targetable. 
		/// If the Targetable is being tracked when this is set to false, it will be removed 
		/// from all Areas. When set to true, it will be added to any Perimieters it is 
		/// inside of, if applicable.
		/// </summary>
		public bool isTargetable
		{
		    get 
			{ 
				// Don't allow targeting if disabled in any way.
				//	If both are false and then the gameobject becomes active, Perimieters will  
				//	detect this rigidbody enterig. Returning false will prevent this odd behavior.
				if (!this.go.activeInHierarchy || !this.enabled)
					return false;
				
				return this._isTargetable; 
			}

		    set
		    {
				// Singleton. Only do something if value changed
				if (this._isTargetable == value)
					return;
						
		        this._isTargetable = value;
				
				// Don't execute logic if this is disabled in any way.
				if (!this.go.activeInHierarchy || !this.enabled)
					return;
				
		        if (!this._isTargetable)
		            this.CleanUp();
				else
					this.BecomeTargetable();
		    }
		}

		public bool _isTargetable = true;  // Public for inspector use

        public DEBUG_LEVELS debugLevel = DEBUG_LEVELS.Off;

        internal List<TargetTracker> trackers = new List<TargetTracker>();

        // Delegate type declarations
		//! [snip_OnDetectedDelegate]
        public delegate void OnDetectedDelegate(TargetTracker source);
		//! [snip_OnDetectedDelegate]
		//! [snip_OnNotDetectedDelegate]
        public delegate void OnNotDetectedDelegate(TargetTracker source);
		//! [snip_OnNotDetectedDelegate]
		//! [snip_OnHit]
        public delegate void OnHitDelegate(EventInfoList eventInfoList, Target target);
		//! [snip_OnHit]
        #endregion Public Parameters


        #region protected Parameters
        public GameObject go;
		public Collider coll;
		public Collider2D coll2D;

		// Internal lists for each delegate type
        protected OnDetectedDelegate onDetectedDelegates;
        protected OnNotDetectedDelegate onNotDetectedDelegates;
        protected OnHitDelegate onHitDelegates;	
        #endregion protected Parameters


        #region Events
        protected void Awake()
        {
            // Cache
            this.go = this.gameObject;				
			this.coll = this.GetComponent<Collider>();
			this.coll2D = this.GetComponent<Collider2D>();
		}

        protected void OnDisable() { this.CleanUp(); }
		
        protected void OnDestroy() { this.CleanUp(); }
        
		protected void CleanUp()
        {
            if (!Application.isPlaying) return; // Game was stopped.
			
			var copy = new List<TargetTracker>(this.trackers);
            foreach (TargetTracker tt in copy)
            {
                // Protect against async destruction
                if (tt == null || tt.area == null || tt.area.Count == 0)
                    continue;

                tt.area.Remove(this);
#if UNITY_EDITOR
                if (this.debugLevel > DEBUG_LEVELS.Off)
                    Debug.Log(string.Format
                    (
                        "Targetable ({0}): On Disabled, Destroyed or !isTargetable- " +
                        	"Removed from {1}.",
                        this.name,
                        tt.name
                    ));
#endif
            }
			
			this.trackers.Clear();

        }
				
		protected void BecomeTargetable()
		{
			// Toggle the collider, only if it was enabled to begin with, to let the physics 
			//   systems refresh.
			if (this.coll != null && this.coll.enabled)
			{
				this.coll.enabled = false;
				this.coll.enabled = true;
			}
#if (!UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2)
			else if (this.coll2D != null && this.coll2D.enabled)
			{
				this.coll2D.enabled = false;
				this.coll2D.enabled = true;
			}
#endif
		}
		


        /// <summary>
        /// Triggered when a target is hit
        /// </summary>
		/// <param name="source">The EventInfoList to send</param>
        /// <param name="source">
        /// The target struct used to cache this target when sent
        /// </param>
        public void OnHit(EventInfoList eventInfoList, Target target)
        {
#if UNITY_EDITOR
            // Normal level debug and above
            if (this.debugLevel > DEBUG_LEVELS.Off)
            {
                Debug.Log
                (
                    string.Format
                    (
						"Targetable ({0}): EventInfoList[{1}]",
                        this.name,
                        eventInfoList.ToString()
                    )
                );
            }
#endif

            // Set the hitTime for all eventInfos in the list.
            eventInfoList = eventInfoList.CopyWithHitTime();

            if (this.onHitDelegates != null)
                this.onHitDelegates(eventInfoList, target);
        }

        /// <summary>
        /// Triggered when a target is first found by an Area
        /// </summary>
        /// <param name="source">The TargetTracker which triggered this event</param>
        internal void OnDetected(TargetTracker source)
        {
#if UNITY_EDITOR
            // Higest level debug
            if (this.debugLevel > DEBUG_LEVELS.Normal)
            {
                string msg = "Detected by " + source.name;
                Debug.Log(string.Format("Targetable ({0}): {1}", this.name, msg));
            }
#endif
			this.trackers.Add(source);

            if (this.onDetectedDelegates != null) this.onDetectedDelegates(source);
        }

        /// <summary>
		/// Triggered when a target is first found by an Area
        /// </summary>
        /// <param name="source">The TargetTracker which triggered this event</param>
        internal void OnNotDetected(TargetTracker source)
        {
#if UNITY_EDITOR
            // Higest level debug
            if (this.debugLevel > DEBUG_LEVELS.Normal)
            {
                string msg = "No longer detected by " + source.name;
                Debug.Log(string.Format("Targetable ({0}): {1}", this.name, msg));
            }
#endif
			this.trackers.Remove(source);

			if (this.onNotDetectedDelegates != null) this.onNotDetectedDelegates(source);
        }
        #endregion Events



        #region Target Tracker Members
        public float strength { get; set; }

        /// <summary>
        /// Waypoints is just a list of positions used to determine the distance to
        /// the final destination. See distToDest.
        /// </summary>
        [HideInInspector]
        public List<Vector3> waypoints = new List<Vector3>();

        /// <summary>
        /// Get the distance from this GameObject to the nearest waypoint and then
        /// through all remaining waypoints.
        /// Set wayPoints (List of Vector3) to use this feature.
        /// The distance is kept as a sqrMagnitude for faster performance and
        /// comparison.
        /// </summary>
        /// <returns>The distance as sqrMagnitude</returns>
        public float distToDest
        {
            get
            {
                if (this.waypoints.Count == 0) return 0;  // if no points, return

                // First get the distance to the first point from the current position
                float dist = this.GetSqrDistToPos(waypoints[0]);

                // Add the distance to each point from the one before.
                for (int i = 0; i < this.waypoints.Count - 2; i++)  // -2 keeps this in bounds
                    dist += (waypoints[i] - waypoints[i + 1]).sqrMagnitude;

                return dist;
            }
        }


        /// <summary>
        /// Get the distance from this Transform to another position in space.
        /// The distance is returned as a sqrMagnitude for faster performance and
        /// comparison
        /// </summary>
        /// <param name="other">The position to find the distance to</param>
        /// <returns>The distance as sqrMagnitude</returns>
        public float GetSqrDistToPos(Vector3 other)
        {
            return (this.transform.position - other).sqrMagnitude;
        }

        /// <summary>
        /// Get the distance from this Transform to another position in space.
        /// The distance is returned as a float for simple min/max testing, etc. 
        /// For distance comparisons, use GetSqrDistToPos(...)
        /// </summary>
        /// <param name="other">The position to find the distance to</param>
        /// <returns>The distance as a simple float</returns>
        public float GetDistToPos(Vector3 other)
        {
            return (this.transform.position - other).magnitude;
        }


        #region Delegate Add/Set/Remove Functions
        #region OnDetectedDelegates
        /// <summary>
		/// Add a new delegate to be triggered when a target is first found by an Area.
        /// The delegate signature is:  delegate(TargetTracker source)
        /// See TargetTracker documentation for usage of the provided 'source'
        /// **This will only allow a delegate to be added once.**
        /// </summary>
        /// <param name="del"></param>
        public void AddOnDetectedDelegate(OnDetectedDelegate del)
        {
            this.onDetectedDelegates -= del;  // Cheap way to ensure unique (add only once)
            this.onDetectedDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for AddOnDetectedDelegate()
        /// </summary>
        /// <param name="del"></param>
        public void SetOnDetectedDelegate(OnDetectedDelegate del)
        {
            this.onDetectedDelegates = del;
        }

        /// <summary>
        /// Removes a OnDetectedDelegate 
        /// See docs for AddOnDetectedDelegate()
        /// </summary>
        /// <param name="del"></param>
        public void RemoveOnDetectedDelegate(OnDetectedDelegate del)
        {
            this.onDetectedDelegates -= del;
        }
        #endregion OnDetectedDelegates


        #region OnNotDetectedDelegate
        /// <summary>
        /// Add a new delegate to be triggered when a target is dropped by a perimieter for
        /// any reason; leaves or is removed.
        /// The delegate signature is:  delegate(TargetTracker source)
        /// See TargetTracker documentation for usage of the provided 'source'
        /// **This will only allow a delegate to be added once.**
        /// </summary>
        /// <param name="del"></param>
        public void AddOnNotDetectedDelegate(OnNotDetectedDelegate del)
        {
            this.onNotDetectedDelegates -= del;  // Cheap way to ensure unique (add only once)
            this.onNotDetectedDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for AddOnNotDetectedDelegate()
        /// </summary>
        /// <param name="del"></param>
        public void SetOnNotDetectedDelegate(OnNotDetectedDelegate del)
        {
            this.onNotDetectedDelegates = del;
        }

        /// <summary>
        /// Removes a OnNotDetectedDelegate 
        /// See docs for AddOnNotDetectedDelegate()
        /// </summary>
        /// <param name="del"></param>
        public void RemoveOnNotDetectedDelegate(OnNotDetectedDelegate del)
        {
            this.onNotDetectedDelegates -= del;
        }
        #endregion OnNotDetectedDelegate


        #region OnHitDelegate
        /// <summary>
        /// Add a new delegate to be triggered when the target is hit.
		/// The delegate signature is:  delegate(EventInfoList eventInfoList, Target target)
		/// See EventInfoList documentation for usage.
        /// **This will only allow a delegate to be added once.**
        /// </summary>
        /// <param name="del"></param>
        public void AddOnHitDelegate(OnHitDelegate del)
        {
            this.onHitDelegates -= del;  // Cheap way to ensure unique (add only once)
            this.onHitDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for AddOnHitDelegate()
        /// </summary>
        /// <param name="del"></param>
        public void SetOnHitDelegate(OnHitDelegate del)
        {
            this.onHitDelegates = del;
        }

        /// <summary>
        /// Removes a OnHitDelegate 
        /// See docs for AddOnHitDelegate()
        /// </summary>
        /// <param name="del"></param>
        public void RemoveOnHitDelegate(OnHitDelegate del)
        {
            this.onHitDelegates -= del;
        }
        #endregion OnHitDelegate

        #endregion Delegate Add/Set/Remove Functions

        #endregion Target Tracker Members

    }
}