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
    ///	Put this on the same GameObject as any UnityConstraint to have a FireController set the 
	/// constraint's target when the FireController target changes. This is great for using 
	/// constraints to track or look-at a target. For example, a "locked on target" sprite could 
	/// track the current target of a missile, or a turret could look at its current target.    
    /// </summary>
    [AddComponentMenu("Path-o-logical/TriggerEventPRO/TriggerEventPRO Utility - Set Unity Constraint Target")]
    public class SetConstraintTarget : MonoBehaviour
    {
		public ConstraintBaseClass unityConstraint;

		/// <summary>
		/// A FireController Or EventTrigger whose events will set the target of the attached UnityConstraint
		/// </summary>
		public Component targetSource
		{
			get 
			{
				return this._targetSource;
			}
			
			set
			{
				//
				// Clear all states before processing new value...
				//
				// If there was a FireController before, unregister the delegates
				if (this.fireCtrl != null)
				{
					this.fireCtrl.RemoveOnTargetUpdateDelegate(this.OnFireCtrlTargetUpdate);
					this.fireCtrl.RemoveOnIdleUpdateDelegate(this.OnOnFireCtrlIdleUpdate);									
				}
				
				// If there was an EventTrigger before, unregister the delegates
				if (this.eventTrigger != null)
				{
					this.eventTrigger.RemoveOnListenUpdateDelegate(this.OnEventTriggerListenStart);					
				}
				
				// Reset states to ensure sync
				this._targetSource = null;
				this.eventTrigger = null;
				this.fireCtrl = null;

				//
				// Process value...
				//
				if (value == null)
					return;

				// Accept the first matching component...
				this.fireCtrl = value as EventFireController;
				if (this.fireCtrl != null)
				{
					this._targetSource = this.fireCtrl;

					this.fireCtrl.AddOnTargetUpdateDelegate(this.OnFireCtrlTargetUpdate);
					this.fireCtrl.AddOnIdleUpdateDelegate(this.OnOnFireCtrlIdleUpdate);

					return;
				}

				this.eventTrigger = value as EventTrigger;
				if (this.eventTrigger != null)
				{
					this._targetSource = this.eventTrigger;

					this.eventTrigger.AddOnListenUpdateDelegate(this.OnEventTriggerListenStart);					

					return;
				}
				
				throw new System.InvalidCastException
				(
					"Component not a supported type. Use a FireController or EventTrigger."
				);
			}
			
		}
		[SerializeField] protected Component _targetSource;

		protected EventFireController fireCtrl;
		protected EventTrigger eventTrigger;

		protected void Awake()
	    {
			// Constraint componant not required by this class because of the use of the 
			//   base class. So, instead, test for null and notify the user if not found.
			if (this.unityConstraint == null)
			{
				this.unityConstraint = this.GetComponent<ConstraintBaseClass>();
				if (this.unityConstraint == null)
					throw new MissingComponentException
					(
						string.Format("No UnityConstraint was found on GameObject '{0}'", this.name)
					);
			}

			// Trigger property logic if a FireController is found or on this GameObject
			if (this._targetSource == null)
			{
				EventFireController ctrl = this.GetComponent<EventFireController>();
				if (ctrl != null)
					this.targetSource = ctrl;			
			}
			else
			{
				this.targetSource = this._targetSource;
			}
	    }
	
		protected void OnEventTriggerListenStart()
		{
			if (this.eventTrigger.target.isSpawned)
			{
				this.unityConstraint.target = this.eventTrigger.target.transform;
			}
			else
			{
				this.unityConstraint.target = null;
			}

		}

		protected void OnFireCtrlTargetUpdate(List<Target> targets)
	    {
	        this.unityConstraint.target = targets[0].transform;
	    }
	
		protected void OnOnFireCtrlIdleUpdate()
	    {
	        this.unityConstraint.target = null;
	    }
        
        
	}
}