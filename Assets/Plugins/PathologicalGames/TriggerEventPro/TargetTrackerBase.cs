/// <Licensing>
/// © 2011 (Copyright) Path-o-logical Games, LLC
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
	/// TargetTracker event that allows filtering the target list before it is passed on.
	/// </summary>
	public delegate void OnPostSortDelegate(TargetTracker source, TargetList targets);
	
	/// <summary>
	/// TargetTrackers with Areas should provide this event for Areas to trigger. 
	/// Return false to block a target from being detected.
	/// </summary>
	public delegate bool OnNewDetectedDelegate(TargetTracker source, Target target);
	
	/// <summary>
	/// TargetTrackers with Areas should provide this event for Areas to trigger. 
	/// Triggered everytime a target leaves an area. Targets already recieve a callback 
	/// targetable.OnNotDetected()
	/// </summary>
	public delegate void OnNotDetectedDelegate(TargetTracker source, Target target);

	/// <summary>
	/// TargetTracker event that Runs when there is a change to the TargetTracker's target list 
	/// or when it is set to dirty for any reason, commonly to trigger a re-sort. This will run 
	/// after the dirty state or update is handled, and after the post sort delgates are done.
	/// </summary>
	public delegate void OnTargetsChangedDelegate(TargetTracker source);

	/// <summary>
	/// The base class for all TargetTrackers. This class is abstract so it can only be 
	/// inherited and implimented, not used directly.
	/// </summary>
	/// <exception cref='System.NotImplementedException'>
	/// Is thrown when a requested operation is not implemented for a given type.
	/// </exception>
	public abstract class TargetTracker : MonoBehaviour, ITargetTracker
	{
		/// <summary>
        /// The number of targets to return. Set to -1 to return all targets
        /// </summary>
        public int numberOfTargets = 1;

        /// <summary>
		/// The layers in which the Area is allowed to find targets.
        /// </summary>
        public LayerMask targetLayers;
		
		/// <summary>
        /// The style of sorting to use
        /// </summary>
        public SORTING_STYLES sortingStyle
        {
            get { return this._sortingStyle; }
            set
            {
				// Only trigger logic if this changed
				if (this._sortingStyle == value)
					return;
				
                this._sortingStyle = value;

                // Trigger sorting due to this change
                this.dirty = true;
            }
        }
        [SerializeField]  // protected backing fields must be SerializeField. For instances.
        protected SORTING_STYLES _sortingStyle = SORTING_STYLES.Nearest;

        /// <summary>
        /// How often the target list will be sorted. If set to 0, sorting will only be 
        /// triggered when Targets enter or exit range.
        /// </summary>
        public float updateInterval = 0.1f;

		/// <summary>
		/// Set this to true to force the Area to sort and other refresh actions, if 
		/// applicable.
		/// Setting this to false does nothing.
		/// </summary>
		/// <value>
		/// Always false because setting to true is handled immediatly.
		/// </value>
		public virtual bool dirty
		{
			get { return false; }  // Always instantly processed and back to false
			set 
			{ 
#if UNITY_EDITOR
				// Stop inspector logic from running this
				if (!Application.isPlaying)
					return;
#endif			
				if (!value)
					return;
					
				// Trigger re-sort, events, etc.
				// WARNING!!! Copy this or "value" in the targets setter will be 
				//	 this list (not a copy) and it will clear before trying to 
				//	 AddRange to itself! (The list will be cleared forever)
				this.targets = new TargetList(this._unfilteredTargets); 
			}  
		}
				
		/// <summary>
		/// Setting this to true is like setting dirty to true except it can be set 
		/// many times and will only set the true dirty state once at the end of the 
		/// frame.
		/// This is used internally to handle situations when multiple trackers 
		/// overlap and trigger dirty multiple times for the same event.
		/// </summary>
		public bool batchDirty 
		{
			get { return this.batchDirtyRunning; }
			set 
			{ 
				// Singleton. Quit one is already running
				if (this.batchDirtyRunning)
					return;
				
				this.StartCoroutine(this.RunBatchDirty());
			}
		}
		protected bool batchDirtyRunning = false;
		protected IEnumerator RunBatchDirty()
		{
			this.batchDirtyRunning = true;
				
			yield return new WaitForEndOfFrame();
			
			this.batchDirtyRunning = false;
			this.dirty = true;
		}
		
        /// <summary>
        /// A list of sorted targets. The contents depend on numberOfTargets requested
		/// (-1 for all targets in the Area), any modifications done by any 
        /// onPostSortDelegates, and the sorting style used.
		/// Getting targets every frame has virtually no overhead because all sorting is 
		///	done when targets are set (TargetTracker is "dirty"). This can be triggered 
		///	by a user, Area or internal co-routine. If at least 2 
		///	targets are available a co-routine is started to update the sort based on 
		///	the sort interval. The co-routine will stop if the number of targets falls 
		/// back under 2
		/// 
		/// IMPORTANT: Setting this equal to another list will replace targets in this 
		/// targets list, like a copy operation but using AddRange internally. It will NOT 
		/// make this targets list equal to the other list. This is important protection.
        /// </summary>
        public virtual TargetList targets
        {
            get { return _targets; }
			set
			{
				// Start with an empty list each pass.
				this._unfilteredTargets.Clear();
				this._targets.Clear();
				
	            // If none are wanted or no targets available, quit
	            if (this.numberOfTargets == 0 || value.Count == 0)
				{
					if (this.onTargetsChangedDelegates != null)
						this.onTargetsChangedDelegates(this);
					
	                return;
				}
				
				// Start with the un-filtered list for internal logic use.
				//   This enables targets to be "detected" even if they are modified below 
				//   to be ignored further and so the sort update co-routine can keep running.
				//	 It is also used by this.dirty to re-run all this.
				this._unfilteredTargets.AddRange(value);
				
				this._targets.AddRange(this._unfilteredTargets);
				
				if (this.sortingStyle != SORTING_STYLES.None)
				{
					if (this._unfilteredTargets.Count > 1)  // Don't bother with the overhead untill 2+)
					{
						var comparer = new TargetTracker.TargetComparer
		                (
		                    this.sortingStyle,
		                    this.transform
		                );
						
						this._targets.Sort(comparer);
						
#if UNITY_EDITOR
	                    if (this.debugLevel > DEBUG_LEVELS.Normal)
		                    Debug.Log(string.Format
							(
								"{0} : SORTED: {1}", this, this._targets.ToString()
							));
#endif
					}

					// Start coroutine to trigger periodic sorting?
					// This has to happen before delegates and numberOfTargets to be 
					// able to respond to changes.
		            // Only If not already running.
					//	 Will do nothing if there aren't at least 2 targets.
					// 	 Will wait at least 1 frame, so sorting won't happen twice 
		            if (!this.isUpdateTargetsUpdateRunning &&
		                this.sortingStyle != AreaTargetTracker.SORTING_STYLES.None)
					{
						this.StartCoroutine(this.UpdateTargets());
					}
				}
				
				if (this.onPostSortDelegates != null) 
					this.onPostSortDelegates(this, this._targets);
				
	            // If not -1 (all), get the first X item(s). Since everything is sorted based on
	            //   the sortingStyle, the first item(s) will always work
	            if (this.numberOfTargets > -1)
	            {
	                // Grab the first item(s)
					int count = this._targets.Count;
	                int num = Mathf.Clamp(this.numberOfTargets, 0, count);
					this._targets.RemoveRange(num, count - num);
	            }
#if UNITY_EDITOR
	            if (this.debugLevel > DEBUG_LEVELS.Normal)  // All higher than normal
	            {
	                string msg = string.Format("Updated targets to: {0}", this._targets.ToString());
	                Debug.Log(string.Format("{0}: {1}", this, msg));
	            }
#endif		
				if (this.onTargetsChangedDelegates != null)
					this.onTargetsChangedDelegates(this);
			}
        }
        protected TargetList _targets = new TargetList();
		
		/// <summary>
		/// Makes the update co-routine a singleton.
		/// </summary>
		protected bool isUpdateTargetsUpdateRunning = false;
		
		/// <summary>
		///  Start with the un-filtered list for internal logic use.
		///  This enables targets to be "detected" even if they are modified below 
		///  to be ignored further and so the sort update co-routine can keep running.
		///	 It is also used by this.dirty to re-run all this.
		/// </summary>
        protected TargetList _unfilteredTargets = new TargetList();
		
        /// <summary>
        /// Runs a sort when the list is dirty (has been changed) or when the timer expires.
        /// This co-routine will stop if there are no targets. and must be restarted when a
        /// target enters.
        /// </summary>
        /// <returns></returns>
        protected IEnumerator UpdateTargets()
        {
			this.isUpdateTargetsUpdateRunning = true;
			
#if UNITY_EDITOR
            if (this.debugLevel > DEBUG_LEVELS.Off) // If on at all
                Debug.Log(string.Format("{0} : Starting update timer co-routine.", this));
#endif
            while (this._unfilteredTargets.Count > 0)   // Quit if there aren't any targets
            {
				// Have to wait at least 1 frame to avoid an infinte loop. So if 
				//	 the update interval is 0, wait the minimum...1 frame.
				if (this.updateInterval == 0)
					yield return null;
				else
					yield return new WaitForSeconds(this.updateInterval);

                this.dirty = true;
            }
#if UNITY_EDITOR
            if (this.debugLevel > DEBUG_LEVELS.Off)  // If on at all
                Debug.Log(string.Format("{0} : Ended update timer co-routine. Target count < 2", this));
#endif
			
			this.isUpdateTargetsUpdateRunning = false;
        }
		
		/// <summary>
		/// Not all TargetTrackers have Area. This is implemented to make it easy and 
		/// cheap test for. Otherwise casting tests would be needed.
		/// </summary>
        public virtual Area area { get { return null; } protected set {} }
		
        /// <summary>
        /// Set to get visual and log feedback of what is happening in this TargetTracker
		/// and Area
        /// </summary>
        public DEBUG_LEVELS debugLevel = DEBUG_LEVELS.Off;
		
#if UNITY_EDITOR
        /// <summary>
        /// Used by the Inspector to displays gizmos if applicable.
        /// </summary>
        public bool drawGizmo = true;

        /// <summary>
        /// The color of the gizmo when displayed
        /// </summary>
        public Color gizmoColor = new Color(0, 0.7f, 1, 0.5f); // Cyan-blue mix
        public Color defaultGizmoColor { get { return new Color(0, 0.7f, 1, 0.5f); } }

        // Used by inspector scripts as an override to hide the gizmo
        public bool overrideGizmoVisibility = false;
#endif
		
        protected virtual void Awake()
        {
			// Pre-Unity5 this stored a cache to this.transform, which is no longer necessary
			//	This Awake was kept to avoid changing sub-class accessors. No harm keeping this 
			//	in case it is needed in the future.
		}
		
		/// <summary>
		/// Set to dirty and manage events if applicable. Enable Area or if 
		/// inheriting and not using a Area, override to leave it out.
		/// </summary>
		protected virtual void OnEnable()
		{
			this.dirty = true;
		}

		
		#region Utilities
		/// <summary>
		/// onNewDetectedDelegates can determine if a target is ignored entirely by returning 
		/// false. Otherwise, the target is tracked. This will return true if the given target 
		/// should be ignored (which means at least 1 of the delegates returned false).
		/// </summary>
		/// <returns>
		/// Returns true (ignore the target) if *any* onNewDetectedDelegates return false. 
		/// If there are no delegates this function returns false (nothing to ignore). 
		/// </returns>
		internal bool IsIgnoreTargetOnNewDetectedDelegatesExecute(Target target)
		{
			if (this.onNewDetectedDelegates == null)
				return false;
						
			// This executes onNewDetectedDelegates by iterating over them. If the delegates were 
			// 	 not executed by iterating, only the return value of the last one would count and 
			// 	 order is not guarnateed either.
			Delegate[] dels = this.onNewDetectedDelegates.GetInvocationList();
			foreach (OnNewDetectedDelegate del in dels)
				if (!del(this, target))
					return true;
			
			return false;
		}
		#endregion
	

		#region Delegates

        #region OnPostSortDelegates Add/Set/Remove
        internal protected OnPostSortDelegate onPostSortDelegates;

		/// <summary>
        /// Runs just before returning the targets list to allow for custom sorting.
        /// The delegate signature is:  delegate(TargetList targets)
        /// See TargetTracker documentation for usage of the provided '...'
        /// This will only allow a delegate to be added once.
        /// **This will only allow a delegate to be added once.**
        /// </summary>
        /// <param name="del">An OnPostSortDelegate</param>
        public void AddOnPostSortDelegate(OnPostSortDelegate del)
        {
            this.onPostSortDelegates -= del;  // Cheap way to ensure unique (add only once)
            this.onPostSortDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for AddOnPostSortDelegate()
        /// </summary>
        /// <param name="del">An OnPostSortDelegate</param>
        public void SetOnPostSortDelegate(OnPostSortDelegate del)
        {
            this.onPostSortDelegates = del;
        }

        /// <summary>
        /// Removes a OnPostSortDelegate
        /// See docs for AddOnPostSortDelegate()
        /// </summary>
        /// <param name="del">An OnPostSortDelegate</param>
        public void RemoveOnPostSortDelegate(OnPostSortDelegate del)
        {
            this.onPostSortDelegates -= del;
        }
        #endregion OnPostSortDelegates Add/Set/Remove
		
        #region OnTargetsChangedDelegates Add/Set/Remove
        internal protected OnTargetsChangedDelegate onTargetsChangedDelegates;

		/// <summary>
        /// Runs when there is a change to the TargetTracker's target list or when it is set 
        /// to dirty for any reason, commonly to trigger a re-sort. This will run after the 
        /// dirty state or update is handled.
        /// **This will only allow a delegate to be added once.**
        /// </summary>
        /// <param name="del">An OnTargetsChangedDelegate</param>
        public void AddOnTargetsChangedDelegate(OnTargetsChangedDelegate del)
        {
            this.onTargetsChangedDelegates -= del;  // Cheap way to ensure unique (add only once)
            this.onTargetsChangedDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for AddOnTargetsChangedDelegate()
        /// </summary>
        /// <param name="del">An OnTargetsChangedDelegate</param>
        public void SetOnTargetsChangedDelegate(OnTargetsChangedDelegate del)
        {
            this.onTargetsChangedDelegates = del;
        }

        /// <summary>
        /// Removes a OnTargetsChangedDelegate
        /// See docs for AddOnTargetsChangedDelegate()
        /// </summary>
        /// <param name="del">An OnTargetsChangedDelegate</param>
        public void RemoveOnTargetsChangedDelegate(OnTargetsChangedDelegate del)
        {
            this.onTargetsChangedDelegates -= del;
        }
        #endregion OnTargetsChangedDelegates Add/Set/Remove
		
        #region OnDetectedDelegates
		internal protected OnNewDetectedDelegate onNewDetectedDelegates;  // Executed by Area

        /// <summary>
		/// Add a new delegate to be triggered when a target is first found by an Area.
        /// The delegate signature is:  bool delegate(TargetTracker source, Target target)
        /// Return true to have no effect on the TargetTracker functioning
        /// Return false to cause the TargetTracker to ignore the new target entirely.
        /// **This will only allow a delegate to be added once.**
        /// </summary>
        /// <param name="del"></param>
        public void AddOnNewDetectedDelegate(OnNewDetectedDelegate del)
        {
            this.onNewDetectedDelegates -= del;  // Cheap way to ensure unique (add only once)
            this.onNewDetectedDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for AddOnNewDetectedDelegate()
        /// </summary>
        /// <param name="del"></param>
        public void SetOnNewDetectedDelegate(OnNewDetectedDelegate del)
        {
            this.onNewDetectedDelegates = del;
        }

        /// <summary>
        /// Removes a OnDetectedDelegate 
        /// See docs for AddOnDetectedDelegate()
        /// </summary>
        /// <param name="del"></param>
        public void RemoveOnNewDetectedDelegate(OnNewDetectedDelegate del)
        {
            this.onNewDetectedDelegates -= del;
        }
        #endregion OnDetectedDelegates

		#region OnNotDetectedDelegates
		internal protected OnNotDetectedDelegate onNotDetectedDelegates;  // Executed by Area

		/// <summary>
		/// Add a new delegate to be triggered when a target is first found by an Area.
		/// The delegate signature is:  bool delegate(TargetTracker source, Target target)
		/// Return true to have no effect on the TargetTracker functioning
		/// Return false to cause the TargetTracker to ignore the new target entirely.
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
		#endregion OnNotDetectedDelegates

		#endregion Delegates


        #region Sorting Options and Functionality
        /// <summary>
        /// Target Style Options used to keep targets sorted
        /// </summary>
        public enum SORTING_STYLES
        {
            None = 0,           // Also good for area-of-effects where no sort is needed
            Nearest = 1,        // Closest to the perimter's center (localPostion)
            Farthest = 2,       // Farthest from the perimter's center (localPostion)
            NearestToDestination = 3,    // Nearest to a destination along waypoints
            FarthestFromDestination = 4, // Farthest from a destination along waypoints
            MostPowerful = 5,  // Most powerful based on a iTargetable parameter
            LeastPowerful = 6, // Least powerful based on a iTargetable parameter
        }
		
		#endregion Sorting Options and Functionality
		

        #region Comparers for Perimter.Sort()
        /// <summary>
        /// The interface for all target comparers.
        /// </summary>
        public interface iTargetComparer : IComparer<Target>
        {
            new int Compare(Target targetA, Target targetB);
        }


        /// <summary>
        /// Returns a comparer based on a targeting style.
        /// </summary>
        public class TargetComparer : iTargetComparer
        {
            protected Transform xform;
            protected TargetTracker.SORTING_STYLES sortStyle;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="sortStyle">The target style used to return a comparer</param>
            /// <param name="xform">Position for distance-based sorting</param>
            public TargetComparer(TargetTracker.SORTING_STYLES sortStyle, Transform xform)
            {
                this.xform = xform;
                this.sortStyle = sortStyle;
            }


            /// <summary>
            /// Used by List.Sort() to custom sort the targets list.
            /// </summary>
            /// <param name="targetA">The first object for comparison</param>
            /// <param name="targetB">The second object for comparison</param>
            /// <returns></returns>
            public int Compare(Target targetA, Target targetB)
            {
                switch (this.sortStyle)
                {
                    case SORTING_STYLES.Farthest:
                        float na = targetA.targetable.GetSqrDistToPos(this.xform.position);
                        float nb = targetB.targetable.GetSqrDistToPos(this.xform.position);
                        return nb.CompareTo(na);


                    case SORTING_STYLES.Nearest:
                        float fa = targetA.targetable.GetSqrDistToPos(this.xform.position);
                        float fb = targetB.targetable.GetSqrDistToPos(this.xform.position);
                        return fa.CompareTo(fb);


                    case SORTING_STYLES.FarthestFromDestination:
                        return targetB.targetable.distToDest.CompareTo(
                                                            targetA.targetable.distToDest);


                    case SORTING_STYLES.NearestToDestination:
                        return targetA.targetable.distToDest.CompareTo(
                                                            targetB.targetable.distToDest);


                    case SORTING_STYLES.LeastPowerful:
                        return targetA.targetable.strength.CompareTo(
                                                            targetB.targetable.strength);

                    case SORTING_STYLES.MostPowerful:
                        return targetB.targetable.strength.CompareTo(
                                                            targetA.targetable.strength);


                    case AreaTargetTracker.SORTING_STYLES.None:  // Only in error
                        throw new System.NotImplementedException("Unexpected option. " +
                                "SORT_OPTIONS.NONE should bypass sorting altogether.");
                }

                // Anything unexpected
                throw new System.NotImplementedException(
                               string.Format("Unexpected option '{0}'.", this.sortStyle));
            }
        }
        #endregion Target Comparers for Perimter.Sort()
	}
	
	/// <summary>
	/// The minimum interface for TargetTrackers
	/// </summary>
    interface ITargetTracker
    {

        /// <summary>
        /// A list of sorted targets. The contents depend on numberOfTargets requested
		/// (-1 for all targets in the Area), and the sorting style userd.
        /// </summary>
        TargetList targets { get; set; }

		/// <summary>
		/// Set this to true to force the Area to sort and other refresh actions, if 
		/// applicable.
		/// Setting this to false does nothing.
		/// </summary>
		/// <value>
		/// Always false because setting to true is handled immediatly.
		/// </value>
		bool dirty { get; set; }

		/// <summary>
		/// Not all TargetTrackers have Areas. This is implemented to make it easy and 
		/// cheap test for. Otherwise casting tests would be needed.
		/// </summary>
        Area area { get; }
		
		/// <summary>
        /// Runs just before returning the targets list to allow for custom sorting.
        /// The delegate signature is:  delegate(TargetList targets)
        /// See TargetTracker documentation for usage of the provided '...'
        /// </summary>
        /// <param name="del">An OnPostSortDelegate</param>
        void AddOnPostSortDelegate(OnPostSortDelegate del);

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for AddOnPostSortDelegate()
        /// </summary>
        /// <param name="del">An OnPostSortDelegate</param>
        void SetOnPostSortDelegate(OnPostSortDelegate del);

        /// <summary>
        /// Removes a OnPostSortDelegate
        /// See docs for AddOnPostSortDelegate()
        /// </summary>
        /// <param name="del">An OnPostSortDelegate</param>
        void RemoveOnPostSortDelegate(OnPostSortDelegate del);
		
        /// <summary>
		/// Add a new delegate to be triggered when a target is first found by a Area.
        /// Return true to have no effect on the TargetTracker functioning
        /// Return false to cause the TargetTracker to ignore the new target entirely.
        /// </summary>
        /// <param name="del"></param>
        void AddOnNewDetectedDelegate(OnNewDetectedDelegate del);

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for AddOnNewDetectedDelegate()
        /// </summary>
        /// <param name="del"></param>
        void SetOnNewDetectedDelegate(OnNewDetectedDelegate del);

        /// <summary>
        /// Removes a OnDetectedDelegate 
        /// See docs for AddOnDetectedDelegate()
        /// </summary>
        /// <param name="del"></param>
        void RemoveOnNewDetectedDelegate(OnNewDetectedDelegate del);

    }
}