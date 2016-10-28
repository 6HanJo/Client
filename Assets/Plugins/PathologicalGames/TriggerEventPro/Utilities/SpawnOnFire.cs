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
    ///	Spawn any instance when a FireController fires. This includes options to decide where 
	/// the new instance should be placed. Also offers pooling options if enabled.
    /// </summary>
    [AddComponentMenu("Path-o-logical/TriggerEventPRO/TriggerEventPRO Utility - Spawn On Fire (FireController)")]
    public class SpawnOnFire : MonoBehaviour
    {
		/// <summary>
		/// The prefab to spawn Instances from.
		/// </summary>
		public Transform prefab;
		
		/// <summary>
		/// This Transform's position and rotation will be used to spawn the instance if the 
		/// origin mode is set to 'OtherTransform'.
		/// </summary>
		public Transform spawnAtTransform;
		
		/// <summary>
		/// The name of a pool to be used with PoolManager or other pooling solution. 
		/// If not using pooling, this will do nothing and be hidden in the Inspector.
		/// </summary>
		public string poolName = "";

		/// <summary>
		/// If false, do not add the new instance to a pool. Use Unity's Instantiate/Destroy
		/// </summary>
		public bool usePooling = true;
		
		/// <summary>
		/// Use this when the FireController or EventTrigger with OnFire event is on another GameObject
		/// </summary>
		public GameObject altSource;
		
		public enum SPAWN_AT_MODE 
		{ 
			FireControllerSpawnAt,
			FireControllerTargetTracker,
			Area,
			ThisTransform,
			OtherTransform
		}
		
		/// <summary>
		/// The origin is the point from which the distance is measuered.
		/// </summary>
		public SPAWN_AT_MODE spawnAtMode = SPAWN_AT_MODE.ThisTransform;
		
		/// <summary>
		/// The Transform used for spawn position and rotation.
		/// </summary>
		public Transform origin
		{
			get 
			{
				Transform xform = this.transform;
				switch (this.spawnAtMode)
				{
					case SPAWN_AT_MODE.FireControllerTargetTracker:
						if (this.fireCtrl == null)
							throw new MissingComponentException
							(
								"Must have a FireController to use the 'FireControllerTargetTracker' option"
							);
					
						xform = this.fireCtrl.targetTracker.transform;
						break;
						
					case SPAWN_AT_MODE.Area:
						Area area;
						
						if (this.fireCtrl != null)
						{
							area = this.fireCtrl.targetTracker.area;
							if (area == null)
								throw new MissingReferenceException(string.Format
								(
									"FireController {0}'s TargetTracker doesn't have a Area. " + 
									"If by design, such as a CollisionTargetTracker, use the " + 
									"'TargetTracker' or other Origin Mode option.",
									this.fireCtrl.name
								));
						}
						else
						{	
							if (!this.eventTrigger.areaHit)
								throw new MissingReferenceException(string.Format
								(
									"EventTrigger {0} areaHit is false. Turn this on before using the 'Area' option",
									this.eventTrigger.name
								));						

							area = this.eventTrigger.area;
						}
					
						xform = area.transform;
						break;
						
					case SPAWN_AT_MODE.FireControllerSpawnAt:
						if (this.fireCtrl == null)
							throw new MissingComponentException
							(
								"Must have a FireController to use the FireControllerTargetTracker option"
							);
					
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

					case SPAWN_AT_MODE.ThisTransform:
			            xform = this.transform;
						break;
						
					case SPAWN_AT_MODE.OtherTransform:
			            xform = this.spawnAtTransform;
						break;
						
				}
				
				return xform;
			}
			set {}
		}
		
		protected EventFireController fireCtrl = null;		
		protected EventTrigger eventTrigger = null;		

	    void Awake()
	    {
			GameObject source;
			if (this.altSource)
				source = this.altSource;
			else
				source = this.gameObject;
			
			var ctrl = source.GetComponent<EventFireController>();
			if (ctrl != null)
			{
				this.fireCtrl = ctrl;
				this.fireCtrl.AddOnFireDelegate(this.OnFire);
			}
			else
			{
				var eventTrigger = source.GetComponent<EventTrigger>();
				if (eventTrigger != null)
				{
					this.eventTrigger = eventTrigger;
					this.eventTrigger.AddOnFireDelegate(this.OnFire);
				}
			}
			
			if (this.fireCtrl == null && this.eventTrigger == null)
				throw new MissingComponentException("Must have either an EventFireController or EventTrigger.");

		}
	
	    protected void OnFire(List<Target> targets)
	    {
	        if (this.prefab == null)
				return;
			
			InstanceManager.Spawn
			(
				this.poolName, 
				this.prefab, 
				this.origin.position, 
				this.origin.rotation
			);
	    }
        
	}
}

