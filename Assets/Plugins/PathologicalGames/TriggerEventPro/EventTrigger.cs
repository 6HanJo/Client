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
    ///	Handles information delivery with a lot of flexibility to create various behaviors. For 
	/// example, you could attach this to a projectile and use it to trigger detonation when it 
	/// hits a target, target layer or after a time limit, depending on the settings you choose. 
	/// You could use the duration and startRange settings to create a shockwave that expands 
	/// over time to notify targets it hits. You could also set the duration to -1 to create a 
	/// persistant area trigger where you can subscribe to the AddOnHitTargetDelegate event 
	/// delegate to respond to each new target that enters range.
	/// 
	/// This will only hit each target in range once, unless the target leaves and comes back in 
	/// to range. Use an EventFireController to notify targets in range multiple times.
    /// </summary>
    [AddComponentMenu("Path-o-logical/TriggerEventPRO/TriggerEventPRO EventTrigger")]
    public class EventTrigger : AreaTargetTracker
    {
        #region Parameters
        /// <summary>
        /// Holds the target passed by the EventFireController on launch or set directly by script.
        /// </summary>
		public Target target;

        /// <summary>
        /// This is used internally to provide an interface in the inspector and to store
        /// structs as serialized objects.
        /// </summary>
        public List<EventInfoListGUIBacker> _eventInfoList = new List<EventInfoListGUIBacker>();

		/// <summary>
		/// A list of EventInfo structs which hold one or more descriptions
		/// of how a Target can be affected. To alter this from code. Get the list, edit it, then 
		/// set the whole list back. (This cannot be edited "in place").
		/// </summary>
		// Encodes / Decodes EventInfos to and from EventInfoGUIBackers
		public EventInfoList eventInfoList
		{
            get
            {
                var returnEventInfos = new EventInfoList();
                foreach (var infoBacker in this._eventInfoList)
                {
                    // Create and add a struct-form of the backing-field instance
                    returnEventInfos.Add
                    (
                        new EventInfo
                        {
                            name = infoBacker.name,
                            value = infoBacker.value,
                            duration = infoBacker.duration,
                        }
                    );
                }

                return returnEventInfos;
            }

            set
            {
                // Clear and set the backing-field list also used by the GUI
                this._eventInfoList.Clear();

                EventInfoListGUIBacker infoBacker;
                foreach (var info in value)
                {
                    infoBacker = new EventInfoListGUIBacker(info);
                    this._eventInfoList.Add(infoBacker);
                }
            }
        }

        /// <summary>
        /// If true, more than just the primary target will be affected when this EventTrigger
        /// fires. Use the range options to determine the behavior.
        /// </summary>
        public bool areaHit = true;

        /// <summary>
        /// If this EventTrigger has a rigidbody, setting this to true will cause it to 
	    /// fire if it falls asleep.
        /// See Unity's docs for more information on how this happens.
        /// </summary>
        public bool fireOnRigidBodySleep = true;

        /// <summary>
        /// Determines what should cause this EventTrigger to fire.
        ///     TargetOnly
        ///         Only a direct hit will trigger the OnFire event.
        ///     HitLayers
        ///         Contact with any colliders in any of the layers in the HitLayers mask 
		///         will trigger the OnFire event.
        /// </summary>
        public HIT_MODES hitMode = HIT_MODES.HitLayers;
        public enum HIT_MODES { TargetOnly, HitLayers }

        /// <summary>
        /// An optional timer to allow this EventTrigger to timeout and self-destruct. When set 
		/// to a value above zero, when this reaches 0, the Fire coroutine will be started and 
		/// anything in range may be hit (depending on settings). This can be used to give  
		/// a projectile a max amount of time it can fly around before it dies, or a time-based 
		/// land mine or pick-up.
        /// </summary>
        public float listenTimeout = 0;

		/// <summary>
		/// An optional duration to control how long this EventTrigger stays active. Each target 
		/// will only be hit once with the event notification unless the Target leaves and then 
		/// re-enters range. Set this to -1 to keep it alive forever.
		/// </summary>
		public float duration = 0;

		/// <summary>
		/// When duration is greater than 0 this can be used have the range change over the course 
		/// of the duration. This is used for things like a chockwave from a large explosion, which 
		/// grows over time. 
		/// </summary>
		public Vector3 startRange = Vector3.zero;
		
		/// <summary>
		/// TThe same as range, but when duration is used, range may change over time while this 
		/// will remain static.
		/// </summary>
		public Vector3 endRange {
			get { return this._endRange; }
			//set;
		}
		internal Vector3 _endRange = Vector3.zero;
		
		/// <summary>
        /// Sets the target notification behavior. Telling targets they are hit is optional 
        /// for situations where a delayed response is required, such as launching a projectile, 
        /// or for custom handling
		/// 
		/// MODES:
		/// \par Off
		///         Do not notify anything. delegates can still be used for custom handling
        /// \par Direct
        ///         OnFire targets will be notified immediately
		/// \par PassInfoToEventTrigger
		///         For every Target hit, a new EventTrigger will be spawned and passed this 
		///         EventTrigger's EventInfo. PassToEventTriggerOnce is more commonly used when 
		///         a secondary EventTrigger is needed, but this can be used for some creative 
		///         or edge use-cases.
		/// \par PassInfoToEventTriggerOnce
		///         OnFire a new EventTrigger will be spawned and passed this EventTrigger's 
		///         EventInfo. Only 1 will be spawned. This is handy for making bombs where only 
		///         the first Target would trigger the event and only 1 EventTrigger would be 
		///         spawned to expand over time (using duration and start range attributes).
		/// \par UseEventTriggerInfo
		///         Same as PassInfoToEventTrigger but the new EventTrigger will use its own 
		///         EventInfo (this EventTrigger's EventInfo will be ignored).
		/// \par UseEventTriggerInfoOnce
		///         Same as PassInfoToEventTriggerOnce but the new EventTrigger will use its own 
		///         EventInfo (this EventTrigger's EventInfo will be ignored).
		/// </summary>
        public NOTIFY_TARGET_OPTIONS notifyTargets = NOTIFY_TARGET_OPTIONS.Direct;
		public enum NOTIFY_TARGET_OPTIONS 
		{ 
			Off, 
			Direct, 
			PassInfoToEventTrigger,
			PassInfoToEventTriggerOnce,
			UseEventTriggerInfo,
			UseEventTriggerInfoOnce
		}

        /// <summary>
        /// An optional prefab to instance another EventTrigger. This can be handy if you want 
		/// to use a 'one-shot' event trigger to then spawn one that expands over time using the 
		/// duration and startRange to simulate a huge explosion.
        /// </summary>
        public EventTrigger eventTriggerPrefab;
		
        /// <summary>
        /// The FireController which spawned this EventTrigger
        /// </summary>
        public EventFireController fireController;

		/// <summary>
		/// If true, the event will be fired as soon as this EventTrigger is spawned by 
		/// instantiation or pooling.
		/// </summary>
		public bool fireOnSpawn = false;

		/// <summary>
		/// If false, do not add the new instance to a pool. Use Unity's Instantiate/Destroy
		/// </summary>
		public bool usePooling = true;
		
		/// <summary>
		/// If an eventTriggerPrefab is spawned, setting this to true will override the  
		/// EventTrigger's poolName and use this instead. The instance will also be passed 
		/// this EventTrigger's eventTriggerPoolName to be used when the EventTrigger is 
		/// desapwned.
		/// </summary>
		public bool overridePoolName = false;
		
		/// <summary>
		/// The name of a pool to be used with PoolManager or other pooling solution. 
		/// If not using pooling, this will do nothing and be hidden in the Inspector.
		/// WARNING: If poolname is set to "", Pooling will be disabled and Unity's 
		/// Instantiate will be used.
		/// </summary>
		public string eventTriggerPoolName = "EventTriggers";
		
		/// <summary>
		/// The name of a pool to be used with PoolManager or other pooling solution. 
		/// If not using pooling, this will do nothing and be hidden in the Inspector.
		/// </summary>
		public string poolName = "EventTriggers";
        #endregion Parameters


        #region Cache
        // Keeps the state of each individual foldout item during the editor session - tiny data
        public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();

        protected float curTimer;

		// Target notification handling...
		protected bool blockNotifications = false;
		protected TargetList detectedTargetsCache = new TargetList();

		// Physics...
        public Rigidbody rbd;
		public Collider coll;
        public Rigidbody2D rbd2D;
		public Collider2D coll2D;
        #endregion Cache

        protected override void Awake()
        {
			// Override starting state
			this.areaColliderEnabledAtStart = false;

            base.Awake();	

//			if (!this.fireOnSpawn && this.rigidbody == null && this.rigidbody2D == null)
//			if (!this.fireOnSpawn && this.rigidbody2D == null)
//			{
//				string msg = "EventTriggers must have a Rigidbody or Rigidbody2D.";
//				throw new MissingComponentException(msg);
//			}
			
			this.rbd2D = this.GetComponent<Rigidbody2D>();
			this.coll2D = this.GetComponent<Collider2D>();  // Cache used by other components and Target
			this.rbd = this.GetComponent<Rigidbody>();
			this.coll = this.GetComponent<Collider>();		// Cache used by other components and Target
        }


        protected override void OnEnable()
        {
            base.OnEnable();

			this._endRange = this.range;  // This is also used in OnDisable to ensure a state reset for pooling

			if (this.fireOnSpawn)
			{
				this.StartCoroutine(this.Fire());
			}
			else
			{
                this.StartCoroutine(this.Listen());
			}

			this.AddOnNewDetectedDelegate(this.OnNewTargetHandler);
		}

		/// <summary>
		/// This is important for pooling as it leaves some states clean for reuse.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();

			//
			// Clean-up...
			//
			//   Mostly in case pooling is used
			//
			this.target = Target.Null;

			// Originally set OnFire. Set it back to be sure of a perfect state if canceled by deactivating
			this.range = this._endRange;
			
			this.RemoveOnNewDetectedDelegate(this.OnNewTargetHandler);
		}

		protected IEnumerator Listen()
        {
#if UNITY_EDITOR
            if (this.debugLevel > DEBUG_LEVELS.Normal)
            {
                string msg = "Listening for Targets...";
                Debug.Log(string.Format("{0}: {1}", this, msg));
            }
#endif
            // Reset
            this.curTimer = this.listenTimeout;

            // Wait for next frame to begin to be sure targets have been propegated
            // This also makes this loop easier to manage.
			//yield return new WaitForFixedUpdate();  // Seems logical but doesn't work. 
			yield return null;

            // START EVENT
            if (this.OnListenStartDelegates != null) this.OnListenStartDelegates();

            // The timer can exit the loop if used 
            while (true)
            {
				// If depsawned by deactivation (pooling for example), drop the Target 
				//   just like if it was destroyed.
				if (!this.target.isSpawned)
				{
					this.target = Target.Null;
				}

                // UPDATE EVENT
                if (this.OnListenUpdateDelegates != null) 
					this.OnListenUpdateDelegates();

                // Fire if rigidbody falls asleep - optional.
                if (this.fireOnRigidBodySleep && 
					(this.rbd != null && this.rbd.IsSleeping()) ||
					(this.rbd2D != null && this.rbd2D.IsSleeping()))
				{
                    break;
				}
                // Only work with the timer if the user entered a value over 0
                if (this.listenTimeout > 0)
                {
                    if (this.curTimer <= 0) 
						break;

                    this.curTimer -= Time.deltaTime;
                }


				//yield return new WaitForFixedUpdate();  // Seems logical but doesn't work. 
				yield return null;
            }

			// Any break will start Fire. Use 'yield return break' to skip.
			this.StartCoroutine(this.Fire());

		}
		
		/// <summary>
        /// Destroys the EventTrigger on impact with target or if no target, a collider 
		/// in the layermask, or anything: depends on the chosen HIT_MODES
        /// </summary>
        protected void OnTriggered(GameObject other)
        {
            switch (this.hitMode)
            {
                case HIT_MODES.HitLayers:
                    // LayerMask compare
                    if (((1 << other.layer) & this.targetLayers) != 0)
                    {
                        // If this was set to hit layers but still not an area hit, try to hit the target.
                        if (!this.areaHit)
                        {
                            var targetable = other.GetComponent<Targetable>();

                            // If the collider's gameObject doesn't have a targetable
                            //   component, it can't be a valid target, so ignore it
                            if (targetable != null)
                                this.target = new Target(targetable, this);
                        }

						this.StartCoroutine(this.Fire());
					}
					
					return;

                case HIT_MODES.TargetOnly:
                    if (this.target.isSpawned && this.target.gameObject == other) // Target was hit
						this.StartCoroutine(this.Fire());

					return;
			}

            // else keep flying...
        }
		
		// 3D
        protected void OnTriggerEnter(Collider other)
        {
			this.OnTriggered(other.gameObject);
		}
		
		// 2D
		protected void OnTriggerEnter2D(Collider2D other)
		{
			this.OnTriggered(other.gameObject);
		}

		/// <summary>
		/// Triggered when the Area detects a target. If false is returned, the 
		/// target will be ignored unless detected again. Since an EventTrigger is like 
		/// an expanding shockwave, if a target is fast enough to get hit, leave and then 
		/// re-enter it makes sense to process it again.
		/// </summary>
		/// <param name='targetTracker'>
		/// The TargetTracker whose Area triggered the event. Since this EventTrigger  
		/// IS the target tracker. This is ignored. It is for event recievers that use 
		/// more than one TargetTracker.
		/// </param>
		/// <param name='target'>
		/// The target Detected
		/// </param>
		protected bool OnNewTargetHandler(TargetTracker targetTracker, Target target)
		{
			// Null detection is for direct calls rather than called-as-delegate
			if (target == Target.Null)
				return false;

#if UNITY_EDITOR
			if (this.debugLevel > DEBUG_LEVELS.Normal)
			{
				string msg = string.Format("Processing detected Target: {0}", target.transform.name);
				Debug.Log(string.Format("{0}: {1}", this, msg));
			}
#endif
			var updatedTarget = new Target(target.targetable, this);

			this.detectedTargetsCache.Add(updatedTarget);

			if (!this.blockNotifications)
				this.HandleNotifyTarget(target);

			return false;  // Force ignore
		}

		protected void HandleNotifyTarget(Target target)
		{		
#if UNITY_EDITOR
			if (this.debugLevel > DEBUG_LEVELS.Off)
			{
				string msg = string.Format
				(
					"Handling notification of '{0}' with notifyTargets option '{1}'", 
					target.transform.name,
					this.notifyTargets.ToString()
				);
				Debug.Log(string.Format("{0}: {1}", this, msg));
			}
#endif
			switch (this.notifyTargets)
			{
				case NOTIFY_TARGET_OPTIONS.Direct:
					target.targetable.OnHit(this.eventInfoList, target);
					
					if (this.OnHitTargetDelegates != null) 
						this.OnHitTargetDelegates(target);
					
					break;
					
				case NOTIFY_TARGET_OPTIONS.UseEventTriggerInfoOnce:
					this.SpawnOnFirePrefab(false);
					break;
					
				case NOTIFY_TARGET_OPTIONS.PassInfoToEventTriggerOnce:
					this.SpawnOnFirePrefab(true);
					break;
			}
		}

		/// <summary>
		/// Despawns this EventTrigger on impact, timeout or sleep (depending on options) 
		/// and notifies objects in range with EventInfo.
        /// </summary>
        public virtual IEnumerator Fire()
        {
			// Prevent being run more than once in a frame where it has already 
            //   been despawned.
            if (!this.gameObject.activeInHierarchy) 
				yield break;

			if (this.duration > 0)
				this.range = this.startRange;

			// This TargetList collects targets that are notified below the 
			this.detectedTargetsCache.Clear();

			// Prevent the Area event from notifying targets in this.OnTargetDetectedNotify()
			this.blockNotifications = true;
			
			if (this.areaHit)
            {				
                // This is turned back off OnDisable() (base class)
				// Also triggers OnDetected via an event to handle all targets found when enabled!
				this.setAreaColliderEnabled(true);

				// Need to wait a frame to let Unity send the physics Enter events to the Area.
				// Wait for next frame to begin to be sure targets have been propegated
				// This also makes this loop easier to manage.
				//yield return new WaitForFixedUpdate();  // This seems logical but doesn't work
				yield return null;
            }
            else
            {
                this.OnNewTargetHandler(this, this.target);
            }

			// Let future detections notifiy targets (in case duration is != 0)
			// This HAS to be bellow the yield or it will only work sometimes.
			this.blockNotifications = false;  

			// Trigger delegates BEFORE processing notifications in case a delegate modifies the 
			//   list of targets
			if (this.OnFireDelegates != null) 
				this.OnFireDelegates(this.detectedTargetsCache);

			foreach (Target processedTarget in this.detectedTargetsCache)
				this.HandleNotifyTarget(processedTarget);

			this.detectedTargetsCache.Clear();  // Just to be clean.

			if (this.notifyTargets == NOTIFY_TARGET_OPTIONS.PassInfoToEventTrigger)
				this.SpawnOnFirePrefab(true);

			// Hnadle keeping this EventTrigger alive and the range change over time.
			//   Any new targets from this point on will be handled by OnNewDetected, which 
			//   is triggered by AreaTargetTracker logic.
			if (this.duration != 0)
			{
				// Determine which while() statement is needed. One that runs out after a 
				//  certain duration or one that runs forever while this component is active.
				if (this.duration < 0)
				{
					while (true)
					{
						// UPDATE EVENT
						if (this.OnFireUpdateDelegates != null)
							this.OnFireUpdateDelegates(-1);

						//yield return new WaitForFixedUpdate();  // Seems logical but doesn't work. 
						yield return null;
					}
				}
				else
				{
					float timer = 0;
					float progress = 0;  // Normalized value that will be 0-1
					while (progress < 1)
					{
						// UPDATE EVENT
						if (this.OnFireUpdateDelegates != null)
							this.OnFireUpdateDelegates(progress);

						timer += Time.deltaTime;
						progress = timer / this.duration;
						this.range = this._endRange * progress;

						//yield return new WaitForFixedUpdate();  // Seems logical but doesn't work. 
						yield return null;

					}

					this.range = this._endRange;  // for percision
				}
			}

			//
			// Handle despoawn...
			//
			// Do this check again due to duration.
			// Prevent being run more than once in a frame where it has already 
			//   been despawned. 
			if (!this.gameObject.activeInHierarchy) 
				yield break;

            InstanceManager.Despawn(this.poolName, this.transform);

			yield return null;
        }

        /// <summary>
		/// Spawns a prefab if this.eventTriggerPrefab is not null.
		/// It is optionally passed this EventTriggers eventInfo and range.
        /// </summary>
        /// <param name="passInfo">
		/// True to pass eventInfo and range to the spawned instance EventTriger.
        /// </param>
        protected void SpawnOnFirePrefab(bool passInfo)
        {
            // This is optional. If no ammo prefab is set, quit quietly
            if (this.eventTriggerPrefab == null) return;

			string poolName;
			if (!this.usePooling)
				poolName = "";  // Overloaded string "" to = pooling off
			else if (!this.overridePoolName)
				poolName = this.eventTriggerPrefab.poolName;
			else
				poolName = this.eventTriggerPoolName;

            Transform inst = PathologicalGames.InstanceManager.Spawn
            (
				poolName,
                this.eventTriggerPrefab.transform,
                this.transform.position,
                this.transform.rotation
            );

			// Pass info...
			// If this is null a parent may have been spawned. Try children.
			var otherEventTrigger = inst.GetComponent<EventTrigger>();
			otherEventTrigger.poolName = poolName;  // Will be correct pool name due to test above
			otherEventTrigger.fireController = this.fireController;
            
			if (passInfo) 
			{
				otherEventTrigger.areaShape = this.areaShape;
				otherEventTrigger.range = this.range;
				otherEventTrigger.eventInfoList = this.eventInfoList;
			}
			
        }


		#region OnListenStart Delegates
        /// <summary>
        /// Runs when this EventTrigger is spawned and starts to listen for Targets
        /// </summary>
        public delegate void OnListenStart();
		protected OnListenStart OnListenStartDelegates;

        public void AddOnListenStartDelegate(OnListenStart del)
        {
            this.OnListenStartDelegates += del;
        }

        public void SetOnListenStartDelegate(OnListenStart del)
        {
            this.OnListenStartDelegates = del;
        }

        public void RemoveOnListenStartDelegate(OnListenStart del)
        {
            this.OnListenStartDelegates -= del;
        }
		#endregion OnListenStart Delegates


		#region OnListenUpdate Delegates
        /// <summary>
        /// Runs every frame
        /// </summary>
        public delegate void OnListenUpdate();
		protected OnListenUpdate OnListenUpdateDelegates;

        public void AddOnListenUpdateDelegate(OnListenUpdate del)
        {
            this.OnListenUpdateDelegates += del;
        }

        public void SetOnListenUpdateDelegate(OnListenUpdate del)
        {
            this.OnListenUpdateDelegates = del;
        }

        public void RemoveOnListenUpdateDelegate(OnListenUpdate del)
        {
            this.OnListenUpdateDelegates -= del;
        }
		#endregion OnListenUpdate Delegates


        #region OnFire Delegates
        /// <summary>
        /// Runs when this EventTrigger starts the Fire coroutine.
        /// </summary>
        /// <param name="targets"></param>
        public delegate void OnFire(TargetList targets);
        protected OnFire OnFireDelegates;

        public void AddOnFireDelegate(OnFire del)
        {
            this.OnFireDelegates += del;
        }

        public void SetOnFireDelegate(OnFire del)
        {
            this.OnFireDelegates = del;
        }

        public void RemoveOnFireDelegate(OnFire del)
        {
            this.OnFireDelegates -= del;
        }
        #endregion OnFire Delegates

		
		#region OnFireUpdate Delegates
		/// <summary>
		/// Runs every frame while this EventTrigger is actively firing due to the use of 
		/// the duration attribute.
		/// </summary>
		public delegate void OnFireUpdate(float progress);
		protected OnFireUpdate OnFireUpdateDelegates;
		
		public void AddOnFireUpdateDelegate(OnFireUpdate del)
		{
			this.OnFireUpdateDelegates += del;
		}
		
		public void SetOnFireUpdateDelegate(OnFireUpdate del)
		{
			this.OnFireUpdateDelegates = del;
		}
		
		public void RemoveOnFireUpdateDelegate(OnFireUpdate del)
		{
			this.OnFireUpdateDelegates -= del;
		}
		#endregion OnFire Delegates
		
		
		#region OnHitTarget Delegates
		/// <summary>
		/// Runs every frame while this EventTrigger is actively firing due to the use of 
		/// the duration attribute.
		/// </summary>
		public delegate void OnHitTarget(Target target);
		protected OnHitTarget OnHitTargetDelegates;
		
		public void AddOnHitTargetDelegate(OnHitTarget del)
		{
			this.OnHitTargetDelegates += del;
		}
		
		public void SetOnHitTargetDelegate(OnHitTarget del)
		{
			this.OnHitTargetDelegates = del;
		}
		
		public void RemoveOnHitTargetDelegate(OnHitTarget del)
		{
			this.OnHitTargetDelegates -= del;
		}
		#endregion OnHitTarget Delegates
		
	}
}