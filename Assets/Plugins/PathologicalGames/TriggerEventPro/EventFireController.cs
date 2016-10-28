/// <Licensing>
/// ©2011 (Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;

namespace PathologicalGames
{
    /// <summary>
    ///	Handles target notification directly, or by spawning an EventTrigger instance, when the 
	/// given parameters are met.
    /// </summary>
    [AddComponentMenu("Path-o-logical/TriggerEventPRO/TriggerEventPRO EventFireController")]
    public class EventFireController : MonoBehaviour
    {
        #region Public Parameters
        /// <summary>
        /// The interval in seconds between firing.
        /// </summary>
        public float interval;

        /// <summary>
        /// When true this controller will fire immediately when it first finds a target, then
        /// continue to count the interval normally.
        /// </summary>
        public bool initIntervalCountdownAtZero = true;

        /// <summary>
        /// Sets the target notification behavior. Telling targets they are hit is optional 
        /// for situations where a delayed response is required, such as launching a projectile, 
        /// or for custom handling.
        /// 
        /// MODES:
        /// \par Off
        ///         Do not notify anything. delegates can still be used for custom handling
		/// \par Direct
        ///         OnFire targets will be notified of this controllers eventInfo
		/// \par PassInfoToEventTrigger
		///         OnFire, For every Target hit, a new EventTrigger will be spawned and passed  
		///         this EventTrigger's EventInfo.
		/// \par UseEventTriggerInfo
		///         Same as PassInfoToEventTrigger but the new EventTrigger will use its own 
		///         EventInfo (this EventFireController's EventInfo will be ignored). 
        /// </summary>
        public NOTIFY_TARGET_OPTIONS notifyTargets = NOTIFY_TARGET_OPTIONS.Direct;
        public enum NOTIFY_TARGET_OPTIONS 
		{ 
			Off, 
			Direct, 
			PassInfoToEventTrigger, 
			UseEventTriggerInfo
		}

        /// <summary>
        /// An optional EventTrigger to spawn OnFire depending on notifyTarget's 
		/// NOTIFY_TARGET_OPTIONS.
        /// </summary>
        public EventTrigger eventTriggerPrefab;
		
		/// <summary>
		/// If false, do not add the new instance to a pool. Use Unity's Instantiate/Destroy
		/// </summary>
		public bool usePooling = true;

		/// <summary>
		/// The name of a pool to be used with PoolManager or other pooling solution. 
		/// If not using pooling, this will do nothing and be hidden in the Inspector.
		/// WARNING: If poolname is set to "", Pooling will be disabled and Unity's 
		/// Instantiate will be used.
		/// </summary>
		public string eventTriggerPoolName = "EventTriggers";
		
		/// <summary>
		/// If an eventTriggerPrefab is spawned, setting this to true will override the  
		/// EventTrigger's poolName and use this instead. The instance will also be passed 
		/// this FireController's eventTriggerPoolName to be used when the EventTrigger is 
		/// desapwned.
		/// </summary>
		public bool overridePoolName = false;

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
                var returnInfoList = new EventInfoList();
				foreach (EventInfoListGUIBacker infoBacker in this._eventInfoList)
                {
                    // Create and add a struct-form of the backing-field instance
                    returnInfoList.Add
                    (
                        new EventInfo
                        {
                            name = infoBacker.name,
                            value = infoBacker.value,
                            duration = infoBacker.duration,
                        }
                    );
                }

                return returnInfoList;
            }

            set
            {
                // Clear and set the backing-field list also used by the GUI
                this._eventInfoList.Clear();

				EventInfoListGUIBacker eventInfoBacker;
                foreach (var info in value)
                {
                    eventInfoBacker = new EventInfoListGUIBacker(info);
                    this._eventInfoList.Add(eventInfoBacker);
                }
            }
        }
		/// <summary>
		/// Public for Inspector use only.
		/// </summary>
		public List<EventInfoListGUIBacker> _eventInfoList = new List<EventInfoListGUIBacker>();
		
		/// <summary>
		/// This transform is optionally used as the position at which an EventTrigger prefab is 
		/// spawned from. Some Utility components may also use this as a position reference if 
		/// chosen.
		/// </summary>
		public Transform spawnEventTriggerAtTransform
		{
			get 
			{
				if (this._spawnEventTriggerAtTransform == null)
					return this.transform;
				else
					return this._spawnEventTriggerAtTransform;
			}
			
			set
			{
				this._spawnEventTriggerAtTransform = value;
			}
		}
        [SerializeField]  // protected backing fields must be serializeable.
		protected Transform _spawnEventTriggerAtTransform = null;  // Explicit for clarity

        /// <summary>
        /// Turn this on to print a stream of messages to help you see what this
        /// FireController is doing
        /// </summary>
        public DEBUG_LEVELS debugLevel = DEBUG_LEVELS.Off;

        /// <summary>
        /// The current counter used for firing. Gets reset at the interval when 
        /// successfully fired, otherwise will continue counting down in to negative
        /// numbers
        /// </summary>
        public float fireIntervalCounter = 99999;
		
		/// <summary>
		/// This FireController's TargetTracker. Defaults to one on the same GameObject.
		/// </summary>
		public TargetTracker targetTracker;
        
        // Delegate type declarations
        public delegate void OnStartDelegate();
        public delegate void OnUpdateDelegate();
        public delegate void OnTargetUpdateDelegate(TargetList targets);
        public delegate void OnIdleUpdateDelegate();
        public delegate void OnStopDelegate();
        public delegate void OnPreFireDelegate(TargetList targets);
        public delegate void OnFireDelegate(TargetList targets);
		public delegate void OnEventTriggerSpawnedDelegate(EventTrigger eventTrigger);

        // Keeps the state of each individual foldout item during the editor session
        public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();
        #endregion Public Parameters


        #region cache
        protected TargetList targets = new TargetList();
		protected TargetList targetsCopy = new TargetList();

		
        // Emtpy delegate used for collection of user added/removed delegates
        protected OnStartDelegate onStartDelegates;
        protected OnUpdateDelegate onUpdateDelegates;
        protected OnTargetUpdateDelegate onTargetUpdateDelegates;
        protected OnIdleUpdateDelegate onIdleUpdateDelegates;
        protected OnStopDelegate onStopDelegates;
        protected OnPreFireDelegate onPreFireDelegates;
        protected OnFireDelegate onFireDelegates;
		protected OnEventTriggerSpawnedDelegate onEventTriggerSpawnedDelegates;
        #endregion Pcache


        #region Events
		protected bool keepFiring = false;

        /// <summary>
        /// Turn on the firing system when this component is enabled, which includes 
        /// creation
        /// </summary>
        protected void OnEnable()
        {
            this.StartCoroutine(this.FiringSystem());   // Start event is inside this
        }

        /// <summary>
        /// Turn off the firing system if this component is disabled or destroyed
        /// </summary>
        protected void OnDisable()
        {
            // This has to be here because if it is in the TargetingSystem coroutine
            //   when the coroutine is stopped, it will get skipped, not ran last.
            this.OnStop();    // EVENT TRIGGER
        }

        /// <summary>
        /// Runs once when when the targeting system starts up. This happens OnEnable(),
        /// which includes destroying this component
        /// </summary>
        protected void OnStart()
        {
#if UNITY_EDITOR
            // Higest level debug
            if (this.debugLevel > DEBUG_LEVELS.Off)
            {
                string msg = "Starting Firing System...";
                Debug.Log(string.Format("{0}: {1}", this, msg));
            }
#endif
            if (this.onStartDelegates != null) this.onStartDelegates();
        }

        /// <summary>
        /// Runs each frame while the targeting system is active, no matter what.
        /// </summary>
        protected void OnUpdate()
        {
            if (this.onUpdateDelegates != null) this.onUpdateDelegates();
        }

        /// <summary>
        /// Runs each frame while tracking a target. This.targets is not empty!
        /// </summary>
        protected void OnTargetUpdate(TargetList targets)
        {
            if (this.onTargetUpdateDelegates != null) this.onTargetUpdateDelegates(targets);
        }

        /// <summary>
        /// Runs each frame while tower is idle (no targets)
        /// </summary>
        protected void OnIdleUpdate()
        {
            if (this.onIdleUpdateDelegates != null) this.onIdleUpdateDelegates();
        }

        /// <summary>
        /// Runs once when when the targeting system is stopped. This happens OnDisable(),
        /// which includes destroying this component
        /// </summary>
        protected void OnStop()
        {
#if UNITY_EDITOR
            // Higest level debug
            if (this.debugLevel > DEBUG_LEVELS.Off)
            {
                string msg = "stopping Firing System...";
                Debug.Log(string.Format("{0}: {1}", this, msg));
            }
#endif
            if (this.onStopDelegates != null) this.onStopDelegates();
			
			this.keepFiring = false;  // Actually stops the firing system			
        }

        /// <summary>
        /// Fire on the targets
        /// </summary>
        protected void Fire()
        {
#if UNITY_EDITOR
            // Log a message to show what is being fired on
            if (this.debugLevel > DEBUG_LEVELS.Off)
            {
                string[] names = new string[this.targets.Count];
                for (int i = 0; i < this.targets.Count; i++)
                    names[i] = this.targets[i].transform.name;

                string msg = string.Format
				(
					"Firing on: {0}\nEventInfo: {1}",
					string.Join(", ", names), 
					this.eventInfoList.ToString()
				);
				
                Debug.Log(string.Format("{0}: {1}", this, msg));
            }
#endif 
			
            // 
			// Create a new list of targets which have this fire controller reference.
			//
            var targetCopies = new TargetList();
            Target newTarget;
            foreach (Target target in this.targets)
            {
                // Can't edit a struct in a foreach loop, so need to copy and store
                newTarget = new Target(target);
                newTarget.fireController = this;  // Add reference. null before this
                targetCopies.Add(newTarget);
            }

            // Write the result over the old target list. This is for output so targets
            //   which are handled at all by this target tracker are stamped with a 
            //   reference.
            this.targets = targetCopies;
			
			//
			// Hnadle delivery
			//
            foreach (Target target in this.targets)
            {
                switch (this.notifyTargets)
                {
                    case NOTIFY_TARGET_OPTIONS.Direct:
                        target.targetable.OnHit(this.eventInfoList, target);
                        break;

                    case NOTIFY_TARGET_OPTIONS.PassInfoToEventTrigger:
                        this.SpawnEventTrigger(target, true);
                        break;

                    case NOTIFY_TARGET_OPTIONS.UseEventTriggerInfo:
                        this.SpawnEventTrigger(target, false);
                        break;
                }

			}

#if UNITY_EDITOR
			// When in the editor, if debugging, draw a line to each hit target.
			if (this.debugLevel > DEBUG_LEVELS.Off && this.notifyTargets > NOTIFY_TARGET_OPTIONS.Off)
			{
				foreach (Target target in this.targets)	
                    Debug.DrawLine(this.spawnEventTriggerAtTransform.position, target.transform.position, Color.red);
			}
#endif	
            // Trigger the delegates
            if (this.onFireDelegates != null) this.onFireDelegates(this.targets);
        }

        protected void SpawnEventTrigger(Target target, bool passInfo)
        {
			// This is optional. If no eventTriggerPrefab is set, quit quietly
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
                this.spawnEventTriggerAtTransform.position,
                this.spawnEventTriggerAtTransform.rotation
            );
			
            var eventTrigger = inst.GetComponent<EventTrigger>();

            // Pass informaiton
            eventTrigger.fireController = this;
            eventTrigger.target = target;
			eventTrigger.poolName = poolName;  // Will be the correct pool name due to test above.

			if (passInfo) 
				eventTrigger.eventInfoList = this.eventInfoList;
			
			if (this.onEventTriggerSpawnedDelegates != null)
				this.onEventTriggerSpawnedDelegates(eventTrigger);
        }
		
		
        #region Delegate Add/Set/Remove methods

        #region OnStartDelegates Add/Set/Remove
        /// <summary>
        /// Add a new delegate to be triggered when the firing system first starts up. 
        /// This happens on OnEnable (which is also run after Awake when first instanced)
        /// The delegate signature is:  delegate()
        /// See TargetTracker documentation for usage of the provided '...'
        /// **This will only allow a delegate to be added once.**
        /// </summary>
        /// <param name="del">An OnStartDelegate</param>
        public void AddOnStartDelegate(OnStartDelegate del)
        {
            this.onStartDelegates -= del;  // Cheap way to ensure unique (add only once)
            this.onStartDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for AddOnStartDelegate()
        /// </summary>
        /// <param name="del">An OnStartDelegate</param>
        public void SetOnStartDelegate(OnStartDelegate del)
        {
            this.onStartDelegates = del;
        }

        /// <summary>
        /// Removes a OnDetectedDelegate
        /// See docs for AddOnStartDelegate()
        /// </summary>
        /// <param name="del">An OnStartDelegate</param>
        public void RemoveOnStartDelegate(OnStartDelegate del)
        {
            this.onStartDelegates -= del;
        }
        #endregion OnDetectedDelegates Add/Set/Remove



        #region OnUpdateDelegates Add/Set/Remove
        /// <summary>
        /// Add a new delegate to be triggered everyframe while active, no matter what.
        /// There are two events which are more specific to the two states of the system:
        ///   1. When Idle (No Target)  - See the docs for OnIdleUpdateDelegate()
        ///   2. When There IS a target - See the docs for OnTargetUpdateDelegate()
        /// The delegate signature is:  delegate()
        /// **This will only allow a delegate to be added once.**
        /// </summary>
        /// <param name="del">An OnUpdateDelegate</param>
        public void AddOnUpdateDelegate(OnUpdateDelegate del)
        {
            this.onUpdateDelegates -= del;  // Cheap way to ensure unique (add only once)
            this.onUpdateDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnUpdateDelegate</param>
        public void SetOnUpdateDelegate(OnUpdateDelegate del)
        {
            this.onUpdateDelegates = del;
        }

        /// <summary>
        /// Removes a OnDetectedDelegate
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnUpdateDelegate</param>
        public void RemoveOnUpdateDelegate(OnUpdateDelegate del)
        {
            this.onUpdateDelegates -= del;
        }
        #endregion OnUpdateDelegates Add/Set/Remove



        #region OnTargetUpdateDelegates Add/Set/Remove
        /// <summary>
        /// Add a new delegate to be triggered each frame when a target is being tracked.
        /// For other 'Update' events, see the docs for OnUpdateDelegates()
        /// The delegate signature is:  delegate(TargetList targets)
        /// See TargetTracker documentation for usage of the provided 'Target' in this list.
		/// **This will only allow a delegate to be added once.**
        /// </summary>
        /// <param name="del">An OnTargetUpdateDelegate</param>
        public void AddOnTargetUpdateDelegate(OnTargetUpdateDelegate del)
        {
            this.onTargetUpdateDelegates -= del;  // Cheap way to ensure unique (add only once)
            this.onTargetUpdateDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnTargetUpdateDelegate</param>
        public void SetOnTargetUpdateDelegate(OnTargetUpdateDelegate del)
        {
            this.onTargetUpdateDelegates = del;
        }

        /// <summary>
        /// Removes a OnDetectedDelegate
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnTargetUpdateDelegate</param>
        public void RemoveOnTargetUpdateDelegate(OnTargetUpdateDelegate del)
        {
            this.onTargetUpdateDelegates -= del;
        }
        #endregion OnTargetUpdateDelegates Add/Set/Remove



        #region OnIdleUpdateDelegates Add/Set/Remove
        /// <summary>
        /// Add a new delegate to be triggered every frame when there is no target to track.
        /// The delegate signature is:  delegate()
        /// **This will only allow a delegate to be added once.**
        /// </summary>
        /// <param name="del">An OnIdleUpdateDelegate</param>
        public void AddOnIdleUpdateDelegate(OnIdleUpdateDelegate del)
        {
            this.onIdleUpdateDelegates -= del;  // Cheap way to ensure unique (add only once)
            this.onIdleUpdateDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnIdleUpdateDelegate</param>
        public void SetOnIdleUpdateDelegate(OnIdleUpdateDelegate del)
        {
            onIdleUpdateDelegates = del;
        }

        /// <summary>
        /// Removes a OnDetectedDelegate
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnIdleUpdateDelegate</param>
        public void RemoveOnIdleUpdateDelegate(OnIdleUpdateDelegate del)
        {
            onIdleUpdateDelegates -= del;
        }
        #endregion OnIdleUpdateDelegates Add/Set/Remove



        #region OnStopDelegates Add/Set/Remove
        /// <summary>
        /// Add a new delegate to be triggered when the firing system is stopped. 
        /// This is caused by destroying the object which has this component or if
        /// the object or component are disabled (The system will restart when  
        /// enabled again)
        /// The delegate signature is:  delegate()
        /// **This will only allow a delegate to be added once.**
        /// </summary>
        /// <param name="del">An OnStopDelegate</param>
        public void AddOnStopDelegate(OnStopDelegate del)
        {
            this.onStopDelegates -= del;  // Cheap way to ensure unique (add only once)
            this.onStopDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnStopDelegate</param>
        public void SetOnStopDelegate(OnStopDelegate del)
        {
            this.onStopDelegates = del;
        }

        /// <summary>
        /// Removes a OnDetectedDelegate
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnStopDelegate</param>
        public void RemoveOnStopDelegate(OnStopDelegate del)
        {
            this.onStopDelegates -= del;
        }
        #endregion OnStopDelegates Add/Set/Remove



        #region OnPreFireDelegates Add/Set/Remove
        /// <summary>
        /// Runs just before any OnFire target checks to allow custom target list 
        /// manipulation and other pre-fire logic.
        /// The delegate signature is:  delegate(TargetList targets)
        /// See TargetTracker documentation for usage of the provided '...'
        /// **This will only allow a delegate to be added once.**
        /// </summary>
        /// <param name="del">An OnPreFireDelegate</param>
        public void AddOnPreFireDelegate(OnPreFireDelegate del)
        {
            this.onPreFireDelegates -= del;  // Cheap way to ensure unique (add only once)
            this.onPreFireDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for AddOnPreFireDelegate()
        /// </summary>
        /// <param name="del">An OnPreFireDelegate</param>
        public void SetOnPreFireDelegate(OnPreFireDelegate del)
        {
            this.onPreFireDelegates = del;
        }

        /// <summary>
        /// Removes a OnPostSortDelegate
        /// See docs for AddOnPreFireDelegate()
        /// </summary>
        /// <param name="del">An OnPreFireDelegate</param>
        public void RemoveOnPreFireDelegate(OnPreFireDelegate del)
        {
            this.onPreFireDelegates -= del;
        }
        #endregion OnPreFireDelegates Add/Set/Remove



        #region OnFireDelegates Add/Set/Remove
        /// <summary>
        /// Add a new delegate to be triggered when it is time to fire/notify a target(s).
        /// The delegate signature is:  delegate(TargetList targets)
        /// See TargetTracker documentation for usage of the provided 'Target' in this list.
        /// **This will only allow a delegate to be added once.**
        /// </summary>
        /// <param name="del">An OnFireDelegate</param>
        public void AddOnFireDelegate(OnFireDelegate del)
        {
            this.onFireDelegates -= del;  // Cheap way to ensure unique (add only once)
            this.onFireDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnFireDelegate</param>
        public void SetOnFireDelegate(OnFireDelegate del)
        {
            this.onFireDelegates = del;
        }

        /// <summary>
        /// Removes a OnDetectedDelegate
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnFireDelegate</param>
        public void RemoveOnFireDelegate(OnFireDelegate del)
        {
            this.onFireDelegates -= del;
        }
        #endregion OnFireDelegates Add/Set/Remove
		

        #region OnEventTriggerSpawnedDelegates Add/Set/Remove
        /// <summary>
        /// Add a new delegate to be triggered when an EventTrigger is spawned. This only happens 
        /// if the notification option is not off or 'Direct'.
        /// **This will only allow a delegate to be added once.**
        /// </summary>
        /// <param name="del">An OnEventTriggerSpawnedDelegate</param>
        public void AddOnEventTriggerSpawnedDelegate(OnEventTriggerSpawnedDelegate del)
        {
            this.onEventTriggerSpawnedDelegates -= del;  // Cheap way to ensure unique (add only once)
            this.onEventTriggerSpawnedDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnEventTriggerSpawnedDelegate</param>
        public void SetOnEventTriggerSpawnedDelegate(OnEventTriggerSpawnedDelegate del)
        {
            this.onEventTriggerSpawnedDelegates = del;
        }

        /// <summary>
        /// Removes a OnDetectedDelegate
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnEventTriggerSpawnedDelegate</param>
        public void RemoveOnEventTriggerSpawnedDelegate(OnEventTriggerSpawnedDelegate del)
        {
            this.onEventTriggerSpawnedDelegates -= del;
        }
        #endregion OnFireDelegates Add/Set/Remove


        #endregion Delegate Add/Set/Remove methods

        #endregion Events



        #region Public Methods
        /// <summary>
        /// Can be run to trigger this FireController to fire immediately regardless of
        /// counter or other settings.
        /// 
        /// This still executes any PreFireDelegates
        /// </summary>
        /// <param name="resetIntervalCounter">Should the count be reset or continue?</param>
        public void FireImmediately(bool resetIntervalCounter)
        {
            if (resetIntervalCounter)
                this.fireIntervalCounter = this.interval;

            // Can alter this.targets
            if (this.onPreFireDelegates != null) this.onPreFireDelegates(this.targets);

            this.Fire();
        }

        #endregion Public Methods


        #region protected Methods
        /// <summary>
        /// Handles all firing events including target aquisition and firing. 
        /// Events are:
        ///     OnStart() :
        ///         Runs once when the firing system first becomes active
        ///     OnUpdate() :
        ///         Runs each frame while the firing system is active
        ///     OnTargetUpdate() : 
        ///         Runs each frame while tracking a target (there is at least one target.)
        ///     OnIdleUpdate() :
        ///         Runs each frame while the firing system is idle (no targets)
        ///     OnFire() :
        ///         Runs when it is time to fire.
        ///     
        /// Counter Behavior Notes:
        ///   * If there are no targets. the counter will keep running up. 
        ///     This means the next target to enter will be fired upon 
        ///     immediatly.
        ///     
        ///   * The counter is always active so if the last target exits, then a 
        ///     new target enters right after that, there may still be a wait.	
        /// </summary>
        protected IEnumerator FiringSystem()
        {
			
			if (this.targetTracker == null)
			{
            	this.targetTracker = this.GetComponent<TargetTracker>();
				if (this.targetTracker == null)
				{
					// Give it a frame to see if this.targetTracker is being set by code.
					yield return null;
				
					if (this.targetTracker == null)
						throw new MissingComponentException
						(
							"FireControllers must be on the same GameObject as a TargetTracker " +
							"or have it's targetTracker property set by code or drag-and-drop " + 
							"in the inspector."
						);
				}
			}

            // While (true) because of the timer, we want this to run all the time, not 
            //   start and stop based on targets in range
            if (this.initIntervalCountdownAtZero)
                this.fireIntervalCounter = 0;
            else
                this.fireIntervalCounter = this.interval;

            this.targets.Clear();
            this.OnStart();   // EVENT TRIGGER
			
			this.keepFiring = true; // Can be turned off elsewhere to kill the firing system
			
            while (this.keepFiring)
            {
                // if there is no target, counter++, handle idle behavior, and 
                //   try next frame.
                // Will init this.targets for child classes as well.
                this.targets = new TargetList(this.targetTracker.targets);

                if (this.targets.Count != 0)
                {
					if (this.fireIntervalCounter <= 0)
					{
						// Let the delegate filter a copy of the list just for the OnFire 
	                    //   Test. We still want this.targets to remain as is.
	                    //   Do this in here to still trigger OnTargetUpdate
	                    this.targetsCopy.Clear();
	                    this.targetsCopy.AddRange(this.targets);

						if (targetsCopy.Count != 0 && this.onPreFireDelegates != null)
							this.onPreFireDelegates(this.targetsCopy);
						
	                    // If all is right, fire 
						// Check targetsCopy incase of pre-fire delegate changes
	                    if (targetsCopy.Count != 0)
	                    {
	                        this.Fire();
	                        this.fireIntervalCounter = this.interval;  // Reset
	                    }
					}
					
                    // Update event while tracking a target
                    this.OnTargetUpdate(this.targets);   // EVENT TRIGGER
                }
                else
                {
                    // Update event while NOT tracking a target
                    this.OnIdleUpdate();   // EVENT TRIGGER
                }

                this.fireIntervalCounter -= Time.deltaTime;

                // Update event no matter what
                this.OnUpdate();   // EVENT TRIGGER

                // Stager calls to get Target (the whole system actually)
                yield return null;
            }
			
			// Wipe out the target list when stopped
            this.targets.Clear();
        }

		#endregion protected Methods

    }
}