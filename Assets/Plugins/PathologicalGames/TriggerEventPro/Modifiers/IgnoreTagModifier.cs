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


namespace PathologicalGames
{
    /// <summary>
    ///	Explcictly ignore Targets. Useful when a TargetTracker prefab is also a Target on the 
	/// same layer as other Targets.
    /// </summary>
    [AddComponentMenu("Path-o-logical/TriggerEventPRO/TriggerEventPRO Modifier - Ignore Tags")]
	[RequireComponent(typeof(TargetTracker))]
    public class IgnoreTagModifier : TriggerEventProModifier
    {
		public List<string> ignoreList = new List<string>();
		
		public DEBUG_LEVELS debugLevel = DEBUG_LEVELS.Off;
		
		protected TargetTracker tracker;         // Cache
		protected string currentTag;         // Loop Cache

        protected void Awake()
        {
            this.tracker = this.GetComponent<TargetTracker>();
        }
		
		protected void OnEnable()
		{
			this.tracker.AddOnNewDetectedDelegate(this.OnNewDetected);
		}
		
		protected void OnDisable()
		{
			if (this.tracker != null)  // For when levels or games are dumped
				this.tracker.RemoveOnNewDetectedDelegate(this.OnNewDetected);
		}
		
		protected bool OnNewDetected(TargetTracker targetTracker, Target target)
		{
#if UNITY_EDITOR
			if (this.debugLevel > DEBUG_LEVELS.Normal)
			{
				string msg = string.Format
				(
					"Testing target '{0}' with tag '{1}' against ignore tags: '{2}'", 
					target.gameObject.name, 
					target.gameObject.tag, 
					string.Join("', '", this.ignoreList.ToArray())
				);
				Debug.Log(string.Format("IgnoreTagModifier ({0}): {1}", this.name, msg));
			}
#endif
			for (int i = 0; i < this.ignoreList.Count; i++)
			{
				this.currentTag = this.ignoreList[i];
				if (target.gameObject.tag == this.currentTag)
				{
#if UNITY_EDITOR					
					if (this.debugLevel > DEBUG_LEVELS.Off)
					{
						string msg = string.Format(
							"Ignoring target '{0}' due to tag: '{1}'", 
							target.gameObject.name, 
							this.currentTag
						);
						Debug.Log(string.Format("IgnoreTagModifier ({0}): {1}", this.name, msg));
					}
#endif
					return false;  // Stop looking and ignore
				}
			}

			return true;
		}
	
    }
}