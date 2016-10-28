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
using System;


namespace PathologicalGames
{
    /// <summary>
    ///	This class is a custom implimentation of the IList interface so it is a custom
    ///	list that can take advantage of Unity's collider OnTrigger events to add and remove
    ///	Targets (and much more). This is tightly coupled with the TargetTracker instance
    ///	which creates it and cannot be used standalone.
    /// </summary>
	[AddComponentMenu("")]  // I hope this is an OK way to hide from Inspector Add menues

    public class Area : MonoBehaviour, IList<Target>
    {
        /// <summary>
        /// The targetTracker which created this Area
        /// </summary>
        public AreaTargetTracker targetTracker;

        /// <summary>
		/// The primary data List for the Area.
        /// </summary>
        internal TargetList targets = new TargetList();

        // Cache
		public GameObject go;
		public Collider coll;
		public Rigidbody rbd;
		public Collider2D coll2D;
		public Rigidbody2D rbd2D;

        #region Init, Sort, Utilities
        /// <summary>
        /// Cache
        /// </summary>
        protected void Awake()
        {
			this.go = this.gameObject;
						
			// Protection. Should be added by AreaTargetTracker before adding this script.
			if (this.GetComponent<Rigidbody>() == null && this.GetComponent<Rigidbody2D>() == null)
			{
				string msg = "Areas must have a Rigidbody or Rigidbody2D.";
				throw new MissingComponentException(msg);
			}

			// Note: Coliders are set after Awake during creation by AreaTargetTrackers
			this.rbd2D = this.GetComponent<Rigidbody2D>();
			this.coll2D = this.GetComponent<Collider2D>();
			this.rbd = this.GetComponent<Rigidbody>();
			this.coll = this.GetComponent<Collider>();
        }
		
		/// <summary>
		/// Uses a physics test based on the largest bounds dimension to see if the targetable's 
		/// collider is in range of this Area.
		/// This has to use the largest dimension for non-uniform sized colliders. It avoids 
		/// situations where the object is inside but not added. the check is still a radius 
		/// though, so a long object that is oriented in a way that doesn't cross in to the 
		/// Area will still return true.
		/// </summary>
		/// <returns>
		/// <param name='targetable'>
		/// Targetable.
		/// </param>
		public bool IsInRange(Targetable targetable)
		{
			if (this.coll != null)
			{
				Vector3 size = this.coll.bounds.size;
				float testSize = Mathf.Max(Mathf.Max(size.x, size.y), size.z);
				var colls = new List<Collider>(Physics.OverlapSphere(this.transform.position, testSize)); 
				return colls.Contains(this.coll);
			}
			else if (this.coll2D != null)
			{
				var coll2Ds = new List<Collider2D>();
				var box2d = this.coll2D as BoxCollider2D;
				if (box2d != null)
				{
					var pos2D = new Vector2(this.transform.position.x, this.transform.position.y);
					Vector2 worldPos2D = box2d.offset + pos2D;
					Vector2 extents = box2d.size * 0.5f;

					var pntA = worldPos2D + extents;
					var pntB = worldPos2D - extents;

					coll2Ds.AddRange(Physics2D.OverlapAreaAll(pntA, pntB));
				}
				else
				{
					var circ2D = this.coll2D as CircleCollider2D;
					if (circ2D != null)
					{
						coll2Ds.AddRange
						(
							Physics2D.OverlapCircleAll(this.transform.position, circ2D.radius * 2)
						);
					}
				}

				return coll2Ds.Contains(this.coll2D);
			}
			Debug.Log("IsInRange returning false due to no collider set. This may be fine.");
			return false;
		}

        #endregion Init, Sort, Utilities


        #region Events
        /// <summary>
        /// 3D Collider Event
        /// </summary>
        protected void OnTriggerEnter(Collider other)
        {
 			this.OnTriggerEntered(other);
        }

        /// <summary>
        /// 3D Collider Event
        /// </summary>
        protected void OnTriggerExit(Collider other)
        {
            // Will only work if there is something to remove. Debug logging inside too.
            this.OnTriggerExited(other);
        }

		/// <summary>
        /// 2D Collider Event
        /// </summary>
        protected void OnTriggerEnter2D(Collider2D other)
        {
			this.OnTriggerEntered(other);
        }

        /// <summary>
        /// 2D Collider Event
        /// </summary>
        protected void OnTriggerExit2D(Collider2D other)
        {
            // Will only work if there is something to remove. Debug logging inside too.
            this.OnTriggerExited(other);
        }

		/// <summary>
		/// Uses the Collider and Collider2D base class to handle any collider event.
        /// </summary>
		protected void OnTriggerEntered(Component other)
		{

            // Do Add() only if this is ITargetable
			var targetable = other.GetComponent<Targetable>();
            if (targetable == null) 
				return;
			
			// Add for targetables will perform checks and then process the addition
            this.Add(targetable);

		}
		
		/// <summary>
		/// Uses the Collider and Collider2D base class to handle any collider event.
        /// </summary>
        protected void OnTriggerExited(Component other)
		{
            // Will only work if there is something to remove. Debug logging inside too.
            this.Remove(other.transform);
			
		}
        #endregion Events


		
        #region List Interface
        public void Add(Target target)
        {
			// Cheapest quickest check to exit first.
			if (!target.targetable.isTargetable ||
				!this.go.activeInHierarchy ||
				this.targets.Contains(target))
				return;
			
			
			// Is it a layer this targetable detects?
            if (!this.IsInLayerMask(target.targetable.go)) 
				return;
					
			// Give delegates the chance to ignore this Target.
			if (this.targetTracker.IsIgnoreTargetOnNewDetectedDelegatesExecute(target))
				return;

			this.targets.Add(target);  // ADD

#if UNITY_EDITOR
            if (this.targetTracker.debugLevel > DEBUG_LEVELS.Off) // If on at all
                Debug.Log(string.Format
				(
					"{0}  : Target ADDED - {1}",
                    this.targetTracker,
                    target.targetable
				));
#endif
            // Trigger the delegate execution for this event
            target.targetable.OnDetected(this.targetTracker);
						
            // Trigger target update: Sorting, filtering, events.
			//	 This uses AddRange internally to copy the targets so '=' is safe
            this.targetTracker.targets = this.targets;
        }
		
		
		/// <summary>
		/// Add the specified targetable. 
		/// Performs some checks to make sure the targetable is valid for the Area.
		/// </summary>
		/// <param name='targetable'>
		/// If set to <c>true</c> targetable.
		/// </param>
		public void Add(Targetable targetable)
        {
            // Get a target struct which will also cache information, such as the Transfrom,
            //   GameObject and ITargetable component
			// Note this uses the targetable overload which has no component lookups so this 
			//	is light.
            var target = new Target(targetable, this.targetTracker);
			
			// Run the main Add overload.
            this.Add(target);
		}
		
        /// <summary>
        /// Remove an object from the list explicitly. 
        /// This works even if the object is still in range, effectivley hiding it from the 
        /// perimiter.
        /// </summary>
        /// <param name="xform">The transform component of the target to remove</param>
        /// <returns>True if somethign was removed</returns>
        public bool Remove(Transform xform)
        {
            return this.Remove(new Target(xform, this.targetTracker));
        }

        /// <summary>
        /// Remove a Targetable
        /// </summary>
        /// <param name="xform">The transform component of the target to remove</param>
        /// <returns>True if somethign was removed</returns>
        public bool Remove(Targetable targetable)
        {
            return this.Remove(new Target(targetable, this.targetTracker));
        }

        /// <summary>
        /// Remove an object from the list explicitly. 
        /// This works even if the object is still in range, effectivley hiding it from the 
        /// perimiter.
        /// </summary>
        /// <param name="target">The Target to remove</param>
        /// <returns>True if somethign was removed</returns>
        public bool Remove(Target target)
        {
            // Quit if nothing was removed
            if (!this.targets.Remove(target)) return false;

            // Silence errors on game exit / unload clean-up
            if (target.transform == null || this.transform == null || this.transform.parent == null)
                return false;

#if UNITY_EDITOR
            if (this.targetTracker.debugLevel > DEBUG_LEVELS.Off) // If on at all
                Debug.Log(string.Format
				(
					"{0}  : Target Removed - {1}",
                    this.targetTracker,
                    target.targetable
				));
#endif

            // Trigger the delegate execution for this event
            if (this.targetTracker.onNotDetectedDelegates != null)
				this.targetTracker.onNotDetectedDelegates(this.targetTracker, target);

			target.targetable.OnNotDetected(this.targetTracker);

            // Trigger target update: Sorting, filtering, events.
			//	 This uses AddRange internally to copy the targets so '=' is safe			
            this.targetTracker.targets = this.targets;

            return true;
        }


        /// <summary>
        /// Read-only index access
        /// </summary>
        /// <param name="index">int address of the item to get</param>
        /// <returns></returns>
        public Target this[int index]
        {
            get { return this.targets[index]; }
            set { throw new System.NotImplementedException("Read-only."); }
        }


        /// <summary>
        /// Clears the entire list explicitly
        /// This works even if the object is still in range, effectivley hiding it from the 
        /// perimiter.
        /// </summary>
        public void Clear()
        {
            // Trigger the delegate execution for this event
            foreach (Target target in this.targets)
			{
				if (this.targetTracker.onNotDetectedDelegates != null)
					this.targetTracker.onNotDetectedDelegates(this.targetTracker, target);

                target.targetable.OnNotDetected(this.targetTracker);
			}

            this.targets.Clear();

#if UNITY_EDITOR
			if (this.targetTracker.debugLevel > DEBUG_LEVELS.Normal) // If on at all
                Debug.Log(string.Format
				(
					"{0}  : All Targets Cleared!",
                    this.targetTracker
				));
#endif	
            // Trigger target update: Sorting, filtering, events.
			//	 This uses AddRange internally to copy the targets so '=' is safe
			this.targetTracker.targets = this.targets;
        }


        /// <summary>
        /// Tests to see if an item is in the list
        /// </summary>
        /// <param name="item">The transform component of the target to test</param>
        /// <returns>True of the item is in the list, otherwise false</returns>
        public bool Contains(Transform transform)
        {
            return this.targets.Contains(new Target(transform, this.targetTracker));
        }


        /// <summary>
        /// Tests to see if an item is in the list
        /// </summary>
        /// <param name="target">The target object to test</param>
        /// <returns>True of the item is in the list, otherwise false</returns>
        public bool Contains(Target target)
        {
            return this.targets.Contains(target);
        }


        /// <summary>
        /// Impliments the ability to use this list in a foreach loop
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Target> GetEnumerator()
        {
            for (int i = 0; i < this.targets.Count; i++)
                yield return this.targets[i];
        }

        // Non-generic version? Not sure why this is used by the interface
        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < this.targets.Count; i++)
                yield return this.targets[i];
        }


        /// <summary>
        /// Used by OTHERList.AddRange()
        /// This adds this list to the passed list
        /// </summary>
        /// <param name="array">The list AddRange is being called on</param>
        /// <param name="arrayIndex">
        /// The starting index for the copy operation. AddRange seems to pass the last index.
        /// </param>
        public void CopyTo(Target[] array, int arrayIndex)
        {
            this.targets.CopyTo(array, arrayIndex);
        }


        // Not implimented from iList
        public int IndexOf(Target item) { throw new System.NotImplementedException(); }
        public void Insert(int index, Target item) { throw new System.NotImplementedException(); }
        public void RemoveAt(int index) { throw new System.NotImplementedException(); }
        public bool IsReadOnly { get { throw new System.NotImplementedException(); } }
        #endregion List Interface



        #region List Utilities
        /// <summary>
        /// Returns the number of items in this (the collection). Readonly.
        /// </summary>
        public int Count { get { return this.targets.Count; } }


        /// <summary>
        ///	Returns a string representation of this (the collection)
        /// </summary>
        public override string ToString()
        {
            // Build an array of formatted strings then join when done
            string[] stringItems = new string[this.targets.Count];
            string stringItem;
            int i = 0;   // Index counter
            foreach (Target target in this.targets)
            {
                // Protection against async destruction.
                if (target.transform == null)
                {
                    stringItems[i] = "null";
                    i++;
                    continue;
                }

                stringItem = string.Format("{0}:Layer={1}",
                                            target.transform.name,
                                            LayerMask.LayerToName(target.gameObject.layer));

                // Finish the string for this target based on the target style
                switch (this.targetTracker.sortingStyle)
                {
                    case AreaTargetTracker.SORTING_STYLES.None:
                        // Do nothing
                        break;

                    case AreaTargetTracker.SORTING_STYLES.Nearest:
                    case AreaTargetTracker.SORTING_STYLES.Farthest:
                        float d;
                        d = target.targetable.GetSqrDistToPos(this.transform.position);
                        stringItem += string.Format(",Dist={0}", d);
                        break;

                    case AreaTargetTracker.SORTING_STYLES.NearestToDestination:
                    case AreaTargetTracker.SORTING_STYLES.FarthestFromDestination:
                        stringItem += string.Format(",DistToDest={0}",
                                                    target.targetable.distToDest);
                        break;
                }

                stringItems[i] = stringItem;
                i++;
            }

            // Return a comma-sperated list inside square brackets (Pythonesque)
            return string.Format("[{0}]", System.String.Join(", ", stringItems));
        }
        #endregion List Utilities



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
            LayerMask targetMask = this.targetTracker.targetLayers;
            if ((targetMask.value & objMask.value) == 0) // Extra brackets required!
                return false;
            else
                return true;
        }

        #endregion protected Utilities
    }

}