/// <Licensing>
/// ©2011-2014 (Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>

// for information about using the POOLMANAGER_INSTALLED or USE_OTHER_POOLING global Custom 
//	 Defines, See the section "Platform Custom Defines" here: 
//	 https://docs.unity3d.com/Documentation/Manual/PlatformDependentCompilation.html
// 	 Or you may wish to use *.rsp files.

#if POOLMANAGER_INSTALLED && USE_OTHER_POOLING
#error Do not use both Custome Defines POOLMANAGER_INSTALLED *and* USE_OTHER_POOLING. Pick one.
#endif


using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PathologicalGames
{
    /// <summary>
    /// Options used to print a stream of messages for the implimenting conponent
    /// </summary>
    public enum DEBUG_LEVELS { Off, Normal, High }

    /// <summary>
    /// Methods which interface with PoolManager if installed and the preprocessor
    /// directive at the top of this file is uncommented. Otherwise will run 
    /// Unity's Instantiate() and Destroy() based functionality.
    /// </summary>
    public static class InstanceManager  
    {
		/// <summary>
		/// Can be used to test if a pooling preprocessor directive is enabled.
		/// </summary>
#if USE_OTHER_POOLING || POOLMANAGER_INSTALLED 
		public static bool POOLING_ENABLED = true;
#else
		public static bool POOLING_ENABLED = false;
#endif


#if USE_OTHER_POOLING || POOLMANAGER_INSTALLED
		public delegate Transform OnSpawn
		(
			string poolName, 
			Transform prefab, 
			Vector3 pos, 
			Quaternion rot
		);
		
		public delegate Transform OnDespawn(string poolName, Transform instance);
		
		public static OnSpawn OnSpawnDelegates;
		public static OnDespawn OnDepawnDelegates;
#endif

		#region Static Methods
        /// <summary>
        /// Creates a new instance. 
        /// 
        /// If PoolManager is installed and the pre-proccessor directive is 
        /// uncommented at the top of TargetTracker.cs, this will use PoolManager to 
        /// pool ammo instances.
        /// 
        /// If the pool doesn't exist before this is used, it will be created.
        /// 
        /// Otherwise, Unity's Object.Instantiate() is used.
        /// </summary>
        /// <param name="prefab">The prefab to spawn an instance from</param>
        /// <param name="pos">The position to spawn the instance</param>
        /// <param name="rot">The rotation of the new instance</param>
        /// <returns></returns>
        public static Transform Spawn(string poolName, Transform prefab, Vector3 pos, Quaternion rot)
        {
#if POOLMANAGER_INSTALLED || USE_OTHER_POOLING
			if (poolName == "")  // Overload use of poolName to toggle pooling
				return (Transform)Object.Instantiate(prefab, pos, rot);
#endif
			
#if POOLMANAGER_INSTALLED			
            // If the pool doesn't exist, it will be created before use
            if (!PoolManager.Pools.ContainsKey(poolName))
                (new GameObject(poolName)).AddComponent<SpawnPool>();
			
            return PoolManager.Pools[poolName].Spawn(prefab, pos, rot);
#elif USE_OTHER_POOLING
			InstanceManager.OnSpawnDelegates(poolName, prefab, pos, rot);
#else
            return (Transform)Object.Instantiate(prefab, pos, rot);
#endif
        }


        /// <summary>
        /// Despawnsan instance. 
        /// 
        /// If PoolManager is installed and the pre-proccessor directive is 
        /// uncommented at the top of TargetTracker.cs, this will use PoolManager
        /// to despawn pooled ammo instances.
        /// 
        /// Otherwise, Unity's Object.Destroy() is used.
        /// </summary>
        public static void Despawn(string poolName, Transform instance)
        {
#if POOLMANAGER_INSTALLED || USE_OTHER_POOLING
			if (poolName == "")  // Overload use of poolName to toggle pooling
			{
				Object.Destroy(instance.gameObject);
				return;
			}	
#endif

#if POOLMANAGER_INSTALLED
            PoolManager.Pools[poolName].Despawn(instance);
#elif USE_OTHER_POOLING
			InstanceManager.OnDespawnDelegates(poolName, instance);
#else
            Object.Destroy(instance.gameObject);
#endif
        }
    }
    #endregion Static Methods


    #region Classes, Structs, Interfaces
    /// <summary>
    /// Carries information about a target including a reference to its Targetable
    /// component (targetable), Transform (transform) and GameObject (gameObject) as 
    /// well as the source TargetTracker (targetTracker) and 
    /// FireController (fireController) components. FireController is null if not used.
    /// </summary>
    public struct Target : System.IComparable<Target>
    {
        // Target cache
        public GameObject gameObject;
        public Transform transform;
        public Targetable targetable;

        // Source cache for easy access/reference passing/etc
        public TargetTracker targetTracker;
        public EventFireController fireController;
        public EventTrigger eventTrigger;

        public Collider collider;
		public Collider2D collider2D;

        /// <summary>
        /// A cached "empty" Target struct which can be used for null-like 'if' checks
        /// </summary>
        public static Target Null { get { return _Null; } }
        private static Target _Null = new Target();
		
		/// <summary>
		/// Initializes a new instance of the <see cref="PathologicalGames.Target"/> struct.
		/// </summary>
		/// <param name='transform'>
		/// Transform that has a Targetable component
		/// </param>
		/// <param name='targetTracker'>
		/// Target tracker that detected this Target
		/// </param>
        public Target(Transform transform, TargetTracker targetTracker)
		{
			// Subtle but important difference with this constructure overload is 
			//	it allows targetable to be 'null' which is used to avoid missing 
			//	component exceptions in Area.
			this.gameObject = transform.gameObject;
            this.transform = transform;
			
            this.targetable = transform.GetComponent<Targetable>();

            this.targetTracker = targetTracker;

            // The targetTracker arg could also be a derived type. If it is. populate more.
			// Also handle colliders to make the struct easier to use when trying to figure 
			//	 out what collider triggered the OnHit event.
            this.eventTrigger = null;
			this.collider = null;
			this.collider2D = null;			
			this.eventTrigger = targetTracker as EventTrigger;
			if (this.eventTrigger != null)
			{
				this.collider = this.eventTrigger.coll;
				this.collider2D = this.eventTrigger.coll2D;
			}

            this.fireController = null;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="PathologicalGames.Target"/> struct.
		/// This is the most efficient constructor because it just stores references to 
		/// caches that the Targetable already holds.
		/// </summary>
		/// <param name='targetable'>
		/// Targetable.
		/// </param>
		/// <param name='targetTracker'>
		/// Target tracker that detected the targetable.
		/// </param>
        public Target(Targetable targetable, TargetTracker targetTracker)
        {
            this.gameObject = targetable.go;
            this.transform = targetable.transform;
			
            this.targetable = targetable;

            this.targetTracker = targetTracker;

            // The targetTracker arg could also be serived type. If it is. populate more.
			// Also handle colliders to make the struct easier to use when trying to figure 
			//	 out what collider triggered the OnHit event.
            this.eventTrigger = null;
			this.collider = null;
			this.collider2D = null;			
			this.eventTrigger = targetTracker as EventTrigger;
			if (this.eventTrigger != null)
			{
				this.collider = this.eventTrigger.coll;
				this.collider2D = this.eventTrigger.coll2D;
			}
			
            this.fireController = null;
        }

        // Init by copy
        public Target(Target otherTarget)
        {
            this.gameObject = otherTarget.gameObject;
            this.transform = otherTarget.transform;
            this.targetable = otherTarget.targetable;

            this.targetTracker = otherTarget.targetTracker;
            this.fireController = otherTarget.fireController;
            this.eventTrigger = otherTarget.eventTrigger;

			this.collider = otherTarget.collider;
			this.collider2D = otherTarget.collider2D;
		}

        public static bool operator ==(Target tA, Target tB)
        {
            return tA.gameObject == tB.gameObject;
        }

        public static bool operator !=(Target tA, Target tB)
        {
            return tA.gameObject != tB.gameObject;
        }

        // These are required to shut the cimpiler up when == or != is overriden
        // This are implimented as recomended by the msdn documentation.
        public override int GetHashCode(){ return base.GetHashCode(); }
        public override bool Equals(System.Object other) 
        {
            if (other == null) return false;
            return this == (Target)other; // Uses overriden ==
        }

        /// <summary>
        /// Returns true if the target is in a spawned state. Spawned means the target 
        /// is not null (not destroyed) and it IS enabled (not despawned by an instance 
        /// pooling system like PoolManager)
        /// </summary>
        public bool isSpawned 
        { 
            get 
            {
                // Null means destroyed so false, if pooled, disabled is false
                return this.gameObject == null ? false : this.gameObject.activeInHierarchy;
            } 
        }

        /// <summary>
        /// For internal use only.
        /// The default for IComparable. Will test if this target is equal to another
        /// by using the GameObject reference equality test
        /// </summary>
        public int CompareTo(Target obj)
        {
            return this.gameObject == obj.gameObject ? 1 : 0;
        }

    }

    /// <summary>
    /// A custom List implimentation to allow for user-friendly usage and ToString() 
    /// representation as well as an extra level of abstraction for future 
    /// extensibility
    /// </summary>
    public class TargetList : List<Target>
    {
        // Impliment both constructors to enable the 1 arg copy-style constructor
        public TargetList() : base() { }
        public TargetList(TargetList targetList) : base(targetList) { }
        public TargetList(Area area) : base(area) { }

        public override string ToString()
        {
            string[] names = new string[base.Count];
            int i = 0;
			Target target;
			IEnumerator<Target> enumerator = base.GetEnumerator();
			while (enumerator.MoveNext())
			{
				target = enumerator.Current;
	            if (target.transform == null)
	                continue;

	            names[i] = target.transform.name;
	            i++;
			}

            return string.Join(", ", names);
        }
    }

    /// <summary>
    /// Used to pass information to a target when it is hit
    /// </summary>
    public struct EventInfo
    {
        public string name;
        public float value;
        public float duration;
        public float hitTime;

        /// <summary>
        /// Copy constructor to populate a new struct with an old
        /// </summary>
		/// <param name="eventInfo"></param>
        public EventInfo(EventInfo eventInfo)
        {
            this.name = eventInfo.name;
            this.value = eventInfo.value;
            this.duration = eventInfo.duration;
            this.hitTime = eventInfo.hitTime;
        }

        /// <summary>
        /// This returns how much of the duration is left based on the current time
        /// and the hitTime (time stamped by Targetable OnHit)
        /// </summary>
        public float deltaDurationTime
        {
            get 
            {
                // If smaller than 0, return 0.
                return Mathf.Max((hitTime + duration) - Time.time, 0);
            }
        }

        public override string ToString()
        {
            return string.Format
            (
                "(name '{0}', value {1}, duration {2}, hitTime {3}, deltaDurationTime {4})",
                this.name, 
                this.value,
                this.duration,
                this.hitTime,
                this.deltaDurationTime
            );
        }
    }

	/// <summary>
	/// The base class for all Modifiers. This class is abstract so it can only be 
	/// inherited and implimented, not used directly. However it can still be used for 
	/// Type detection when trying to figure out if a component is a Modifier.
	/// Requested by user.
	/// </summary>
	public abstract class TriggerEventProModifier : MonoBehaviour { }
	
    /// <summary>
    /// A custom List implimentation to allow for user-friendly usage and ToString() 
    /// representation as well as an extra later of abstraction for futre 
    /// extensibility
    /// </summary>
    public class EventInfoList : List<EventInfo>
    {
        // Impliment both constructors to enable the 1 arg copy-style initilizer
        public EventInfoList() : base() { }
		public EventInfoList(EventInfoList eventInfoList) : base(eventInfoList) { }

        /// <summary>
        /// Print a nice message showing the contents of this list.
        /// </summary>
        public override string ToString()
        {
            string[] infoStrings = new string[base.Count];
            int i = 0;
			EventInfo info;
			IEnumerator<EventInfo> enumerator = base.GetEnumerator();
			while (enumerator.MoveNext())
			{
				info = enumerator.Current;
				infoStrings[i] = info.ToString();				
				i++;
			}

            return System.String.Join(", ", infoStrings);
        }


        /// <summary>
        /// Get a copy of this list with effectInfo.hitTime set to 'now'.
        /// </summary>
		/// <returns>EventInfoList</returns>
        public EventInfoList CopyWithHitTime()
        {
            var newlist = new EventInfoList();
			EventInfo info;
			IEnumerator<EventInfo> enumerator = base.GetEnumerator();
			while (enumerator.MoveNext())
			{
				info = enumerator.Current;
				info.hitTime = Time.time;
				newlist.Add(info);
			}

            return newlist;
        }
    }


    /// <summary>
    /// Used by the Editor custom Inspector to get user input
    /// </summary>
    [System.Serializable]
    public class EventInfoListGUIBacker
    {
        public string name = "<Event Name>";
        public float value = 0;
        public float duration;

        /// <summary>
		/// Create a new EventInfoListGUIBacker with default values
        /// </summary>
        /// <returns></returns>
        public EventInfoListGUIBacker() { }

        /// <summary>
		/// Create a new EventInfoListGUIBacker from a HitEffect struct
        /// </summary>
        /// <returns></returns>
        public EventInfoListGUIBacker(EventInfo info)
        {
            this.name = info.name;
            this.value = info.value;
            this.duration = info.duration;
        }

        /// <summary>
		/// Return an EventInfo struct.
        /// </summary>
        /// <returns></returns>
        public EventInfo GetEventInfo()
        {
            return new EventInfo
            {
                name = this.name,
                value = this.value,
                duration = this.duration,
            };
        }
    }

    #endregion Classes, Structs, Interfaces
}