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
	/// A TargetTracker which uses an Area, a child game object generated at runtime 
	/// used to detect and manage Targetables in range.
	/// </summary>
    [AddComponentMenu("Path-o-logical/TriggerEventPRO/TriggerEventPRO Area TargetTracker")]
    public class AreaTargetTracker : TargetTracker
    {

        #region Parameters

        #region Inspector Fields
        /// <summary>
        /// The range in which targets will be found.
        /// The size from the center to the edge of the Area in x, y and z for any 
        /// shape. Depending on the shape some values may be ignored. E.g. Spheres only 
        /// use X for radius
        /// </summary>
        public Vector3 range
        {
            get { return this._range; }
            set
            {
                // Store the passed Vector3 then process the range per collider type
                this._range = value;

                if (this.area != null) 
					this.UpdateAreaRange();
            }
        }
        [SerializeField]  // protected backing fields must be SerializeField. For instances.
        protected Vector3 _range = Vector3.one;

        /// <summary>
        /// Area shape options
        /// </summary>
		public enum AREA_SHAPES { Capsule, Box, Sphere, Box2D, Circle2D }
		
		/// <summary>
		/// Returns true if the Area shape in use is one of the 2D collider types.
		/// </summary>
		/// <value>
		public bool is2D
		{
			get 
			{
				int shapes = (int)(AREA_SHAPES.Box2D | AREA_SHAPES.Circle2D);
				return System.Enum.IsDefined(typeof(AREA_SHAPES), shapes);
			}
		}			
		
        /// <summary>
        /// The shape of the Area used to detect targets in range
        /// </summary>
        public AREA_SHAPES areaShape
        {
            get { return this._areaShape; }
            set
            {
                this._areaShape = value;

                // Just in case this is called before Awake runs.
                if (this.area == null) 
					return;
                
            	this.StartCoroutine(this.UpdateAreaShape());  
            }
        }
        [SerializeField]  // protected backing fields must be SerializeField. For instances.
        protected AREA_SHAPES _areaShape = AREA_SHAPES.Sphere;

        /// <summary>
        /// An optional position offset for the Area. 
        /// For example, if you have an object resting on the ground which has a range of 
        /// 4, a position offset of Vector3(0, 4, 0) will place your Area so it is 
        /// also sitting on the ground
        /// </summary>
        public Vector3 areaPositionOffset
        {
            get { return this._areaPositionOffset; }
            set
            {
                this._areaPositionOffset = value;

                // Just in case this is called before Awake runs.
                if (this.area == null) 
					return;
				
                this.area.transform.localPosition = value;
            }
        }
        [SerializeField]  // protected backing fields must be SerializeField. For instances.
        protected Vector3 _areaPositionOffset = Vector3.zero;

        /// <summary>
        /// An optional rotational offset for the Area.
        /// </summary>
        public Vector3 areaRotationOffset
        {
            get { return this._areaRotationOffset; }
            set
            {
                this._areaRotationOffset = value;

                // Just in case this is called before Awake runs.
                if (this.area == null) 
					return;
                
				// TODO: Does this work for 2D circles?
				
				this.area.transform.localRotation = Quaternion.Euler(value);
            }
        }
        [SerializeField]  // protected backing fields must be SerializeField. For instances.
        protected Vector3 _areaRotationOffset = Vector3.zero;


        /// <summary>
        /// The layer to put the Area in. 0 is the default layer.
        /// </summary>
        public int areaLayer
        {
            get { return this._areaLayer; }
            set
            {
                this._areaLayer = value;

                // Just in case this is called before Awake runs.
                if (this.area == null) return;
                this.area.gameObject.layer = value;

            }
        }
        [SerializeField]  // protected backing fields must be SerializeField. For instances.
        protected int _areaLayer = 0;
        #endregion Inspector Fields

        /// <summary>
        /// Access to the Area component/list
        /// </summary>
        public override Area area { get; protected set; }
		
		/// <summary>
		/// Used by this and derived AreaTargetTrackers to determine if their Area should detect 
		/// Targets right away. Set this to false in a derived type's Awake() method just before 
		/// calling base.Awake() to override the state. EventTriggers do this, for exmaple, so they 
		/// don't detect targets unless in AreaHit mode and only when they are triggered.
		/// </summary>
		protected bool areaColliderEnabledAtStart = true;
        #endregion Perameters


        #region Cache and Setup
        protected override void Awake()
        {
            base.Awake();
			
            #region Build Perimter
            // PERINETER GAME OBJECT
            // Create the Area object at run-time. The name is really just for debugging
            var areaGO = new GameObject(this.name + "_Area");
            areaGO.transform.parent = this.transform;
            //areaGO.hideFlags = HideFlags.HideInHierarchy;
            areaGO.SetActive(false);
            areaGO.SetActive(true);

            // These are set by properties but need to be synced when first generated
            areaGO.transform.localPosition = this.areaPositionOffset;
            areaGO.transform.localRotation = Quaternion.Euler(this.areaRotationOffset);
            areaGO.layer = this.areaLayer;
						
			// PERINETER PHYSICS...
			// Adds collider and rigidbody. Needed by Area Awake below. so keep up here.
            this.StartCoroutine(this.UpdateAreaShape(areaGO)); // Co-routine for after Awake()
			
			// Set collider state before adding Area component
			//   'areaColliderEnabledAtStart' may be set by derived types before this Awake() runs.
			this.setAreaColliderEnabled(areaGO, this.areaColliderEnabledAtStart);

            // AREA COMPONENT...
            // Add the Area script and return it
            this.area = areaGO.AddComponent<Area>();  // Will also start "disabled"
            this.area.targetTracker = this;

			// INIT OTHER...
            this.UpdateAreaRange();  // Sets the collider's size
            #endregion Build Perimter

        }
		
		
        /// <summary>
        /// Update the Area range buy removing the old collider and making a new one.
        /// This is a coroutine but it always runs right away unless the rigidbody has to 
        /// be destroyed to change from 2D to 3D or back, in which case it needs a frame 
        /// to process the destroy request.
        /// </summary>
        protected IEnumerator UpdateAreaShape()
        {
			this.StartCoroutine(this.UpdateAreaShape(this.area.go));
			yield break;
		}
		
        protected IEnumerator UpdateAreaShape(GameObject areaGO)
        {
            // Remove the old collider, or quit if already the right shape
            Collider oldCol = areaGO.GetComponent<Collider>();
            Collider2D oldCol2D = areaGO.GetComponent<Collider2D>();
            switch (this.areaShape)
            {
                case AREA_SHAPES.Sphere:
                    if (oldCol is SphereCollider) yield break;
                    break;

                case AREA_SHAPES.Box:
                    if (oldCol is BoxCollider) yield break;
                    break;

                case AREA_SHAPES.Capsule:
                    if (oldCol is CapsuleCollider) yield break;
                    break;
                case AREA_SHAPES.Box2D:
                    if (oldCol2D is BoxCollider2D) yield break;
                    break;
				
                case AREA_SHAPES.Circle2D:
                    if (oldCol2D is CircleCollider2D) yield break;
                    break;
				default:
	                throw new System.Exception("Unsupported collider type.");
            }

            if (oldCol != null)   
				Destroy(oldCol);

			if (oldCol2D != null) 
				Destroy(oldCol2D);

			// If the shape is changing from a 2D shape to a 3D shape, swap RigidBodies too
			// If there is no rigidbody, add it (usually only needed during initilization.
			switch (this.areaShape)
            {
                case AREA_SHAPES.Sphere:
                case AREA_SHAPES.Box:
                case AREA_SHAPES.Capsule:
					if (areaGO.GetComponent<Rigidbody2D>() != null)
					{
						Destroy(areaGO.GetComponent<Rigidbody2D>());
						yield return null;
					}
				
					if (areaGO.GetComponent<Rigidbody>() == null)
					{
						var rbd = areaGO.AddComponent<Rigidbody>();
						rbd.isKinematic = true;
					}
				
					break;

                case AREA_SHAPES.Box2D:
                case AREA_SHAPES.Circle2D:
					if (areaGO.GetComponent<Rigidbody>() != null)
					{
						Destroy(areaGO.GetComponent<Rigidbody>());
						yield return null;
					}
						
					if (areaGO.GetComponent<Rigidbody2D>() == null)
					{
						var rbd2D = areaGO.AddComponent<Rigidbody2D>();
						rbd2D.isKinematic = true; // BUG looks solved: http://issuetracker.unity3d.com/issues/rigidbody2d-with-kinematic-rigidbody-will-not-cause-ontriggerenter2d
						//rbd2D.gravityScale = 0;  // hack work-around for the above bug
					}

					break;
			}	

			
            // Add the new collider
            Collider coll = null;
			Collider2D coll2D = null;
			switch (this.areaShape)
            {
                case AREA_SHAPES.Sphere:
					coll = areaGO.AddComponent<SphereCollider>();
					coll.isTrigger = true;
                    break;

                case AREA_SHAPES.Box:
                    coll = areaGO.AddComponent<BoxCollider>();
					coll.isTrigger = true;
                    break;

                case AREA_SHAPES.Capsule:
                    coll = areaGO.AddComponent<CapsuleCollider>();
					coll.isTrigger = true;
                    break;
                case AREA_SHAPES.Box2D:
                    coll2D = areaGO.AddComponent<BoxCollider2D>();
					coll2D.isTrigger = true;
                    break;
				
                case AREA_SHAPES.Circle2D:
                    coll2D = areaGO.AddComponent<CircleCollider2D>();
					coll2D.isTrigger = true;
                    break;
				default:
	                throw new System.Exception("Unsupported collider type.");
			}

			// No collisions. Trigger only
			if (coll != null)
			{
	            coll.isTrigger = true;

				// Update area cached reference
				if (this.area != null)
					this.area.coll = coll;
			}
			else if (coll2D != null)
			{
				coll2D.isTrigger = true;

				// Update area cached reference
				if (this.area != null)
					this.area.coll2D = coll2D;
			}

			this.UpdateAreaRange(coll, coll2D);

			yield break;
        }

        /// <summary>
        /// Sets the range based on this._range and the collider type.
        /// </summary>
        protected void UpdateAreaRange()
        {			
            Collider col = this.area.coll;
			Collider2D col2D = this.area.coll2D;
			this.UpdateAreaRange(col, col2D);	
		}

		/// <summary>
		/// Overload to skip the need for an Area to exist so this can be used during setup.
		/// </summary>
		protected void UpdateAreaRange(Collider col, Component col2D)  // Component is for 4.0 compatability
		{
			Vector3 normRange = this.GetNormalizedRange();

			if (col is SphereCollider)
            {
                var collider = (SphereCollider)col;
                collider.radius = normRange.x;
            }
            else if (col is BoxCollider)
            {
                var collider = (BoxCollider)col;
                collider.size = normRange;
            }
            else if (col is CapsuleCollider)
            {
                var collider = (CapsuleCollider)col;
                collider.radius = normRange.x;
                collider.height = normRange.y;
            }
            else if (col2D is CircleCollider2D)
            {
                var collider = (CircleCollider2D)col2D;
                collider.radius = normRange.x;
            }
            else if (col2D is BoxCollider2D)
            {
                var collider = (BoxCollider2D)col2D;
                collider.size = new Vector2(normRange.x, normRange.y);
            }
			else
            {
				string colType;
				string colName;
				if (col != null)
				{
					colType = col.GetType().Name;
					colName = col.name;
				}
				else if (col2D != null)
				{
					colType = col2D.GetType().Name;
					colName = col2D.name;
				}
				else
				{
					throw new System.NullReferenceException("No Collider Found!");
				}

				Debug.LogWarning
				(
					string.Format
					(
						"Unsupported collider type '{0}' for collider '{1}'.",
						colType,
						colName
					)
				);
            }
        }

        /// <summary>
        /// Calculate the range based on the collider shape. The user input creates
        /// different results depending on the shape used. A sphere radius is different
        /// than a box width. This function creates a mapping which normalizes the result.
        /// The shape is a string so this can be used before the collider exits
        /// </summary>
        /// <returns>Vector3</returns>
        public Vector3 GetNormalizedRange()
        {
            Vector3 normRange = Vector3.zero;
            switch (this.areaShape)
            {
				case AREA_SHAPES.Circle2D: 	// Fallthrough
                    normRange = new Vector3
                    (
                        this._range.x,
                        this._range.x,
                        0
                    );
					break;
                case AREA_SHAPES.Sphere:
                    normRange = new Vector3
                    (
                        this._range.x,
                        this._range.x,
                        this._range.x
                    );
                    break;
				case AREA_SHAPES.Box2D: 
                    normRange = new Vector3
                    (
                        this._range.x * 2,
                        this._range.y * 2,
                        0
                    );
                    break;
                case AREA_SHAPES.Box:
                    normRange = new Vector3
                    (
                        this._range.x * 2,
                        this._range.y,      // Not x2 because measured from base to top, nothing "below-ground"
                        this._range.z * 2
                    );
                    break;

                case AREA_SHAPES.Capsule:
                    normRange = new Vector3
                    (
                        this._range.x,
                        this._range.y * 2,  // Capsules work this way
                        this._range.x
                    );
                    break;
            }

            return normRange;
        }

		/// <summary>
		/// Read-only convienince property to find out of the Tracker's Area is currently 
		/// able to detect targets based on the state of its collider. Use 
		/// <see cref="PathologicalGames.AreaTargetTracker.setAreaColliderEnabled"/> to set 
		/// this state.
		/// </summary>
		/// <value><c>true</c> if area collider enabled; otherwise, <c>false</c>.</value>
		public bool areaColliderEnabled 
		{
			get
			{
				if (this.area != null)
				{
					if (this.area.coll != null)
					{
						return this.area.coll.enabled;
					}
					else if (this.area.coll2D != null)
					{
						return this.area.coll2D.enabled;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Sets the area collider enabled/disabled. When disabled, no Targets are detected.
		/// </summary>
		/// <param name="enable">If set to <c>true</c> enable.</param>
		public void setAreaColliderEnabled(bool enable)
		{
			this.setAreaColliderEnabled(this.area.go, enable);
		}

		/// <summary>
		/// Sets the area collider enabled/disabled. When disabled, no Targets are detected. 
		/// This overload used internally by this AreaTargetTracker, or derived classes, to set 
		/// the state of the Area GameObject's collider before the area component is added. If 
		/// the GameObject does already have an Area, however, the area cached components are 
		/// used directly.
		/// </summary>
		/// <param name="areaGO">Area GameObject to check for colliders</param>
		/// <param name="enable">If set to <c>true</c> enable.</param>
		protected void setAreaColliderEnabled(GameObject areaGO, bool enable)
		{
			// Old way. It is more work to toggle the collider instead, but it allows for normal 
			//   operation of the gameObject while preventing premature target detection.
			//areaGO.SetActive(enable);

			Collider coll;
			if (this.area != null)  // Indirect logic. use this area or fallback to GO.
			{
				coll = this.area.coll;
			}
		    else
			{
				coll = areaGO.GetComponent<Collider>();
			}
			
			if (coll != null)
			{
				coll.enabled = enable;
				return;
			}
			else
			{
			    Collider2D coll2D;
				if (this.area != null)
				{
					coll2D = this.area.coll2D;
				}
				else
				{
					coll2D = areaGO.GetComponent<Collider2D>();
				}

				if (coll2D != null)
				{
					coll2D.enabled = enable;
					return;
				}
			}
			throw new System.Exception("Unexpected Error: No area collider found.");
		}


        protected override void OnEnable()
        {
			base.OnEnable();
			
			if (this.area != null) 
				this.area.go.SetActive(true);
		}

        protected virtual void OnDisable()
        {
            // Needed to avoid error when stoping the game or if the Area was destroyed 
            //   for some reason.
            if (this.area == null) return;

            this.area.Clear();
			this.area.go.SetActive(false);
        }

        #endregion Cache and Setup

    }

}