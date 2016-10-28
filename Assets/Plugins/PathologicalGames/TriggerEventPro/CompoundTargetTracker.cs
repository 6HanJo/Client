using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PathologicalGames
{
    /// <summary>
	///	Combines multiple TargetTrackers in to one. This is great for creating more complex shapes 
	/// such as cross-streets or irregular structures. 
	/// 
	/// Simply create AreaTargetTrackers and add them to the targetTrackers list to combine them so   
	/// they behave as a single Area.
    /// </summary>
    [AddComponentMenu("Path-o-logical/TriggerEventPRO/TriggerEventPRO Compound TargetTracker")]
	public class CompoundTargetTracker : TargetTracker
	{
		
		/// <summary>
		/// The TargetTrackers whos targets will be combined by this CompoundTargetTracker 
		/// </summary>
		public List<TargetTracker> targetTrackers = new List<TargetTracker>();
		
        /// <summary>
        /// A combined list of sorted targets from all of this CompoundTargetTracker's 
        /// TargetTracker-members. The contents depend on numberOfTargets requested
		/// (-1 for all targets in the Area), any modifications done by any 
        /// onPostSortDelegates, and the sorting style used.
		/// Getting targets every frame has virtually no overhead because all sorting is 
		///	done when targets are set (TargetTracker is "dirty"). This can be triggered 
		///	by a user, Area or internal co-routine. If at least 2 
		///	targets are available a co-routine is started to update the sort based on 
		///	the sort interval. The co-routine will stop if the number of targets falls 
		/// back under 2
        /// </summary>
		public override TargetList targets {
			get 
			{
				return base.targets;
			}
			set 
			{
				this.combinedTargets.Clear();
				for (int i = 0; i < this.targetTrackers.Count; i++)
					this.combinedTargets.AddRange(this.targetTrackers[i].targets);
				
				base.targets = this.combinedTargets;
			}
		}
		protected TargetList combinedTargets = new TargetList();
				
		/// <summary>
		/// Refresh targets, sorting, event memberships, etc.
		/// Setting this to false does nothing.
		/// </summary>
		/// <value>
		/// Always false because setting to true is handled immediatly.
		/// </value>
		public override bool dirty
		{
			get
			{
				return false;
			}
			
			set
			{		
#if UNITY_EDITOR
				// Stop inspector logic from running this
				if (!Application.isPlaying)
					return;
#endif	
				// Make sure all trackers have events registered. Events are only added once.
				for (int i = 0; i < this.targetTrackers.Count; i++)
				{
					this.targetTrackers[i].AddOnTargetsChangedDelegate(this.OnTargetsChanged);
					this.targetTrackers[i].AddOnNewDetectedDelegate(this.OnTrackersNew);
				}

				// Trigger re-sort, etc (runs this.targets setter)
				base.dirty = value;
			}
		}
				
		/// <summary>
		/// CompoundTargetTrackers do not support targetLayers because they delegate detection 
		/// to their list of TargetTrackers
		/// </summary>
		/// <exception cref='System.NotImplementedException'>
		/// Is thrown when get or set.
		/// </exception>
		public new LayerMask targetLayers 
		{
			get 
			{
				string msg = "CompoundTargetTrackers do not support targetLayers because they " +
							 "delegate detection to their list of TargetTrackers";
				throw new System.NotImplementedException(msg);
			}
			
			set
			{
				string msg = "CompoundTargetTrackers do not support targetLayers because they " +
							 "delegate detection to their list of TargetTrackers";
				throw new System.NotImplementedException(msg);
			}
		}

		/// <summary>
		/// When this CompoundTargetTracker is enabled, which also runs right after Awake(), 
		/// cause it to refresh its targets, sorting, event memberships, etc
		/// </summary>		
		protected override void OnEnable()
		{
			this.dirty = true;
		}
		
		/// <summary>
		/// If this CompoundTargetTracker is disabled, un-register its delegates from all 
		/// of its targetTrackers;
		/// </summary>
		protected void OnDisable()
		{
			for (int i = 0; i < this.targetTrackers.Count; i++)
			{
				this.targetTrackers[i].RemoveOnTargetsChangedDelegate(this.OnTargetsChanged);
				this.targetTrackers[i].RemoveOnNewDetectedDelegate(this.OnTrackersNew);
			}
		}
		
		/// <summary>
		/// Causes this CompoundTargetTracker to refresh its targets, sorting, event 
		/// memberships, etc. when any of its trackers trigger this delegate
		/// </summary>
		protected void OnTargetsChanged(TargetTracker source)
		{
			this.batchDirty = true;
		}
		
		/// <summary>
		/// Provide the same event interface as other TargetTrackers. This one is triggerd 
		/// when any of its trackers are triggered.
		/// </summary>
		/// <param name='source'>
		/// The TargetTracker that triggered the current call.
		/// </param>
		/// <param name='target'>
		/// The Target that was detected and triggered the current call.
		/// </param>
		protected bool OnTrackersNew(TargetTracker source, Target target)
		{
			if (this.onNewDetectedDelegates == null)
				return true;
			
			// If keeping the target, the post sort delegate will trigger this.dirty = true;
			return this.onNewDetectedDelegates(this, target);
		}
		
	}
}