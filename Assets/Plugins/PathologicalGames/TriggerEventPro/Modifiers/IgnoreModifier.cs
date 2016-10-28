/// <Licensing>
/// � 2011(Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License(the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace PathologicalGames
{
    /// <summary>
    ///	Explcictly ignore Targets. Useful when a TargetTracker prefab is also a Target on the 
	/// same layer as other Targets.
    /// </summary>
    [AddComponentMenu("Path-o-logical/TriggerEventPRO/TriggerEventPRO Modifier - Ignore Targetables")]
	[RequireComponent(typeof(TargetTracker))]
    public class IgnoreModifier : MonoBehaviour
    {
		/// <summary>
		/// DO NOT USE DIRECTLY. This is only public for inspector use. 
		/// Use Add, Remove or Clear methods instead.
		/// </summary>
		public List<Targetable> _ignoreList = new List<Targetable>();
		
		public DEBUG_LEVELS debugLevel = DEBUG_LEVELS.Off;
		
		// Cache and re-use
		protected TargetTracker tracker;
		protected Targetable currentTargetable;
		
        protected void Awake()
        {
            this.tracker = this.GetComponent<TargetTracker>();
        }
		
		protected void OnEnable()
		{
			this.tracker.AddOnNewDetectedDelegate(this.OnNewDetected);
						
			if (this._ignoreList.Count == 0)
				return;
			
			// Unless this is a AreaTargetTracker. Stop here.
			if (this.tracker.area == null)
				return;
						
			// Sync the ignore list with the TargetTracker by removing any ignore targetables.				
			var areaListCopy = new TargetList(this.tracker.area);
			for (int i = 0; i < areaListCopy.Count; i++)
			{
				this.currentTargetable = areaListCopy[i].targetable;
				if (this.currentTargetable == null)
					continue;
				
				if (this._ignoreList.Contains(this.currentTargetable))
				{
					this.tracker.area.Remove(this.currentTargetable);
				}
			}
		}
		
		protected void OnDisable()
		{
			if (this.tracker != null)  // For when levels or games are dumped
				this.tracker.RemoveOnNewDetectedDelegate(this.OnNewDetected);
			
			// If this isn't an AreaTargetTracker stop here.
			if (this.tracker.area == null)
				return;

			foreach (Targetable targetable in this._ignoreList)
			{
				if (targetable != null &&
				    this.tracker.area != null && 
				    this.tracker.area.IsInRange(targetable))
				{
					this.tracker.area.Add(targetable);
				}
			}
		}
		
		protected bool OnNewDetected(TargetTracker targetTracker, Target target)
		{
#if UNITY_EDITOR
			// Print the target.gameObject's name name and list of ignore targetables
			if (this.debugLevel > DEBUG_LEVELS.Normal)
			{
				string[] names = new string[this._ignoreList.Count];
				for (int i = 0; i < this._ignoreList.Count; i++)
					names[i] = this._ignoreList[i].name;
						
				string msg = string.Format
				(
					"Testing target '{0}' against ignore list: '{1}'", 
					target.gameObject.name, 
					string.Join("', '", names)
				);
				Debug.Log(string.Format("IgnoreModifier ({0}): {1}", this.name, msg));
			}
#endif

			if (this._ignoreList.Contains(target.targetable))
			{
#if UNITY_EDITOR					
				if (this.debugLevel > DEBUG_LEVELS.Off)
				{
					string msg = string.Format("Ignoring target '{0}'", target.gameObject.name);
					Debug.Log(string.Format("IgnoreModifier ({0}): {1}", this.name, msg));
				}
#endif
				return false;  // Stop looking and ignore
			}
			
			return true;
		}
	
		/// <summary>
		/// Use like List<Targetable>.Add() to add the passed targetable to the ignore 
		///  list and remove it from the TargetTracker
		/// </summary>
		/// <param name='targetable'>
		/// Targetable.
		/// </param>
		public void Add(Targetable targetable) 
		{
			if (targetable == null)
				return;
			
			// Only add this once.
			if (!this._ignoreList.Contains(targetable))
				this._ignoreList.Add(targetable);

			// Don't affect the TargetTracker if this is disabled. It will sync OnEnable
			if (this.enabled && 
			    this.tracker != null && 
			    this.tracker.area != null && 
			    targetable.trackers.Contains(this.tracker))
			{
				// Removing multiple times doesn't hurt and lets the inspector use this.
				this.tracker.area.Remove(targetable);
			}
		}
		
		/// <summary>
		/// Use like List<Targetable>.Remove to remove the passed targetable from the ignore 
		/// list and add it to the TargetTracker
		/// </summary>
		/// <param name='targetable'>
		/// Targetable.
		/// </param>
		public void Remove(Targetable targetable) 
		{
			if (targetable == null)
				return;

			// Does nothing if there is nothing to remove.
			this._ignoreList.Remove(targetable);			
			
			// Don't affect the TargetTracker if this is disabled. If disabled, all 
			//	 are added back to the Area anyway and OnEnable only Remove is done.
			if (this.enabled && this.tracker.area != null)
				// The TargetTracker will handle preventing multiples from being added
				this.tracker.area.Add(targetable);
		}
		
		/// <summary>
		/// Clear the ignore list. Remove all targetables from the TargetTracker
		/// </summary>
		public void Clear()
		{
			foreach (Targetable targetable in this._ignoreList)
				this.Remove(targetable);
		}
		
    }
}