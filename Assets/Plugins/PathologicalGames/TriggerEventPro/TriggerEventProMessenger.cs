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
    ///	Provides messaging for event delegates primarily for use with UnityScript since only
    ///	C# has delegates. 
	/// 
	///	  To use:
	///		1. Add this component to the same GameObject as the component you want to send event 
	///        messages for.
	///     2. Set the "For Component" to match the component on the GameObject to display the 
	///        events available.
	///     3. Choose the events you want to use.
	///     4. In a script, simply create a function of the same name as the message and it will 
	///        be triggered. If the script is on another GameObject, drag and drop it on to Other 
	///        Message Target.
	/// 
    /// </summary>
	[AddComponentMenu("Path-o-logical/TriggerEventPRO/TriggerEventPRO Messenger")]
    public class TriggerEventProMessenger : MonoBehaviour
    {
        [System.Flags]
		public enum COMPONENTS
        {
            FireController  = 1,
            EventTrigger    = 2,  // Breaking alpha-order for backwards compatability
            Targetable      = 4,
            TargetTracker   = 8
        }
        public COMPONENTS forComponent = 0;


        public enum MESSAGE_MODE
        {
            Send,
            Broadcast
        }
        public MESSAGE_MODE messageMode;

        /// <summary>
        /// An optional GameObject to message instead of this component's GameObject
        /// </summary>
        public GameObject otherTarget = null;

        public DEBUG_LEVELS debugLevel = DEBUG_LEVELS.Off;

        // TargetTracker Events
        public bool targetTracker_OnPostSort = false;
        public bool targetTracker_OnNewDetected = false;
        public bool targetTracker_OnTargetsChanged = false;

        // FireController Events
        public bool fireController_OnStart = false;
        public bool fireController_OnUpdate = false;
        public bool fireController_OnTargetUpdate = false;
        public bool fireController_OnIdleUpdate = false;
		public bool fireController_OnPreFire = false;
        public bool fireController_OnFire = false;
        public bool fireController_OnStop = false;

        // EventTrigger Events
        public bool eventTrigger_OnListenStart = false;
		public bool eventTrigger_OnListenUpdate = false;
        public bool eventTrigger_OnFire = false;
        public bool eventTrigger_OnFireUpdate = false;
        public bool eventTrigger_OnHitTarget = false;


        // Targetable Events
        public bool targetable_OnHit = false;
        public bool targetable_OnHitCollider = false;
        public bool targetable_OnDetected = false;
        public bool targetable_OnNotDetected = false;

        protected void Awake()
        {
            // Register all to allow run-time changes to individual events
            //  The delegates don't send messages unless the bool is true anyway.
            this.RegisterTargetTracker();
            this.RegisterFireController();
            this.RegisterEventTrigger();
            this.RegisterTargetable();
        }


        /// <summary>
        /// Generic message handler to process options.
        /// </summary>
        /// <param name="msg"></param>
        protected void handleMsg(string msg)
        {
            GameObject GO;
            if (this.otherTarget == null)
                GO = this.gameObject;
            else
                GO = this.otherTarget;

            if (this.debugLevel > DEBUG_LEVELS.Off)
                Debug.Log(string.Format("Sending message '{0}' to '{1}'", msg, GO));

            if (this.messageMode == MESSAGE_MODE.Send)
                GO.SendMessage(msg, SendMessageOptions.DontRequireReceiver);
            else
                GO.BroadcastMessage(msg, SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// Generic message handler to process options.
        /// </summary>
        /// <param name="msg"></param>
        protected void handleMsg<T>(string msg, T arg)
        {
            GameObject GO;
            if (this.otherTarget == null)
                GO = this.gameObject;
            else
                GO = this.otherTarget;

            if (this.debugLevel > DEBUG_LEVELS.Off)
                Debug.Log
                (
                    string.Format("Sending message '{0}' to '{1}' with argument {2}",
                    msg,
                    GO,
                    arg)
                );

            if (this.messageMode == MESSAGE_MODE.Send)
                GO.SendMessage(msg, arg, SendMessageOptions.DontRequireReceiver);
            else
                GO.BroadcastMessage(msg, arg, SendMessageOptions.DontRequireReceiver);
        }


        # region TargetTracker
        protected void RegisterTargetTracker()
        {
            // There is no need to initialize delegates if there is no component
            var component = this.GetComponent<TargetTracker>();
            if (component == null) return;

            component.AddOnPostSortDelegate(this.OnPostSortDelegate);
            component.AddOnNewDetectedDelegate(this.OnNewDetectedDelegate);
            component.AddOnTargetsChangedDelegate(this.OnTargetsChangedDelegate);
        }

        protected void OnPostSortDelegate(TargetTracker source, TargetList targets)
        {
            if (this.targetTracker_OnPostSort == false) return;

            // Pack the data in to a struct since we can only pass one argument
            var data = new MessageData_TargetTrackerEvent(source, targets);

            this.handleMsg<MessageData_TargetTrackerEvent>("TargetTracker_OnPostSort", data);
        }

        protected bool OnNewDetectedDelegate(TargetTracker source, Target target)
        {
            if (this.targetTracker_OnNewDetected == false) return true;

            // Pack the data in to a struct since we can only pass one argument
            var data = new MessageData_TargetTrackerEvent(source, target);

            this.handleMsg<MessageData_TargetTrackerEvent>("TargetTracker_OnNewDetected", data);
			
			return true;
        }

        protected void OnTargetsChangedDelegate(TargetTracker source)
        {
            if (this.targetTracker_OnTargetsChanged == false) return;

            this.handleMsg<TargetTracker>("TargetTracker_OnTargetsChanged", source);
        }
        # endregion TargetTracker        


		# region FireController
        protected void RegisterFireController()
        {
            // There is no need to initialize delegates if there is no component
            var component = this.GetComponent<EventFireController>();
            if (component == null) return;

            component.AddOnStartDelegate(this.OnStartDelegate);
            component.AddOnUpdateDelegate(this.OnUpdateDelegate);
            component.AddOnTargetUpdateDelegate(this.OnTargetUpdateDelegate);
            component.AddOnIdleUpdateDelegate(this.OnIdleUpdateDelegate);
            component.AddOnPreFireDelegate(this.OnPreFireDelegate);
            component.AddOnFireDelegate(this.OnFireDelegate);
            component.AddOnStopDelegate(this.OnStopDelegate);
        }

        protected void OnStartDelegate()
        {
            if (this.fireController_OnStart == false) return;
            this.handleMsg("FireController_OnStart");
        }

        protected void OnUpdateDelegate()
        {
            if (this.fireController_OnUpdate == false) return;
            this.handleMsg("FireController_OnUpdate");
        }

        protected void OnTargetUpdateDelegate(TargetList targets)
        {
            if (this.fireController_OnTargetUpdate == false) return;
            this.handleMsg<TargetList>("FireController_OnTargetUpdate", targets);
        }

        protected void OnIdleUpdateDelegate()
        {
            if (this.fireController_OnIdleUpdate == false) return;
            this.handleMsg("FireController_OnIdleUpdate");
        }

        protected void OnPreFireDelegate(TargetList targets)
        {
            if (this.fireController_OnPreFire == false) return;
            this.handleMsg<TargetList>("FireController_OnPreFire", targets);
        }

        protected void OnFireDelegate(TargetList targets)
        {
            if (this.fireController_OnFire == false) return;
            this.handleMsg<TargetList>("FireController_OnFire", targets);
        }

        protected void OnStopDelegate()
        {
            if (this.fireController_OnStop == false) return;
			this.handleMsg("FireController_OnStop");
		}
		# endregion FireController


        # region EventTrigger
        protected void RegisterEventTrigger()
        {
            // There is no need to initialize delegates if there is no component
            var component = this.GetComponent<EventTrigger>();
            if (component == null) return;

            component.AddOnListenStartDelegate(this.OnListenStartDelegate);
            component.AddOnListenUpdateDelegate(this.OnListenUpdateDelegate);

			component.AddOnFireDelegate(this.EventTrigger_OnFireDelegate);
			component.AddOnFireUpdateDelegate(this.OnFireUpdateDelegate);
			component.AddOnHitTargetDelegate(this.OnHitTargetDelegate);
        }

        protected void OnListenStartDelegate()
        {
            if (this.eventTrigger_OnListenStart == false) return;
			this.handleMsg("EventTrigger_OnListenStart");
        }

		protected void OnListenUpdateDelegate()
		{
			if (this.eventTrigger_OnListenUpdate == false) return;
			this.handleMsg("EventTrigger_OnListenUpdate");
		}
		
		protected void EventTrigger_OnFireDelegate(TargetList targets)
        {
			if (this.eventTrigger_OnFire == false) return;
			this.handleMsg<TargetList>("EventTrigger_OnFire", targets);
        }

		protected void OnFireUpdateDelegate(float progress)
        {
			if (this.eventTrigger_OnFireUpdate == false) return;
			this.handleMsg<float>("EventTrigger_OnFireUpdate", progress);
        }

		protected void OnHitTargetDelegate(Target target)
		{
			if (this.eventTrigger_OnHitTarget == false) return;
			this.handleMsg<Target>("EventTrigger_OnHitTarget", target);
		}
		# endregion EventTrigger


        # region Targetable
        protected void RegisterTargetable()
        {
            // There is no need to initialize delegates if there is no component
            var component = this.GetComponent<Targetable>();
            if (component == null) return;

            component.AddOnHitDelegate(this.OnHitDelegate);
            component.AddOnDetectedDelegate(this.OnDetectedDelegate);
            component.AddOnNotDetectedDelegate(this.OnNotDetectedDelegate);
        }

        protected void OnHitDelegate(EventInfoList eventInfoList, Target target)
        {
            if (this.targetable_OnHit == false) return;

            // Pack the data in to a struct since we can only pass one argument
            var data = new MessageData_TargetableOnHit(eventInfoList, target);

            this.handleMsg<MessageData_TargetableOnHit>("Targetable_OnHit", data);
        }

		protected void OnHitColliderDelegate(EventInfoList eventInfoList, Target target, Collider other)
        {
            if (this.targetable_OnHit == false) return;

            // Pack the data in to a struct since we can only pass one argument
            var data = new MessageData_TargetableOnHit(eventInfoList, target, other);

            this.handleMsg<MessageData_TargetableOnHit>("Targetable_OnHitCollider", data);
        }

        protected void OnDetectedDelegate(TargetTracker source)
        {
            if (targetable_OnDetected == false) return;

            this.handleMsg<TargetTracker>("Targetable_OnDetected", source);
        }

        protected void OnNotDetectedDelegate(TargetTracker source)
        {
            if (this.targetable_OnNotDetected == false) return;

            this.handleMsg<TargetTracker>("Targetable_OnNotDetected", source);
        }
        # endregion Targetable

    }

    /// <summary>
    /// This is used to pass arguments for the Targetable OnHit events because Unity's
    /// SendMessage only takes a single argument
    /// </summary>
    public struct MessageData_TargetableOnHit
    {
        public EventInfoList eventInfoList;
        public Target target;
        public Collider collider;

		public MessageData_TargetableOnHit(EventInfoList eventInfoList, Target target)
        {
            this.eventInfoList = eventInfoList;
            this.target = target;
            this.collider = null;
        }
		
		public MessageData_TargetableOnHit(EventInfoList eventInfoList, Target target, Collider other)
        {
            this.eventInfoList = eventInfoList;
            this.target = target;
			this.collider = other;
        }
    }

    /// <summary>
    /// This is used to pass arguments for the TargetTracker events because Unity's
    /// SendMessage only takes a single argument
    /// </summary>
    public struct MessageData_TargetTrackerEvent
    {
        public TargetTracker targetTracker;
        public Target target;
        public TargetList targets;

        public MessageData_TargetTrackerEvent(TargetTracker targetTracker, Target target)
        {
            this.targetTracker = targetTracker;
            this.target = target;
            this.targets = new TargetList();
        }
		
        public MessageData_TargetTrackerEvent(TargetTracker targetTracker, TargetList targets)
        {
            this.targetTracker = targetTracker;
            this.target = Target.Null;
            this.targets = targets;
        }
    }

}