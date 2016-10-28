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
    ///	Adds Line-of-Sight (LOS) filtering to TargetPRO components. Line of sight means 
    ///	events are based on whether or not a target can be "seen". This visibility test 
    ///	is done by ray casting against a given layer. If the ray is broken before hitting 
    ///	the target, the target is not in LOS.
    ///	
    /// If added to the same GameObject as a TargetTracker it can filter out any targets 
    /// which are not currently in LOS.
    /// 
    /// If added to the same GameObject as a FireController it can prevent firing on any 
    /// targets which are not currently in LOS.
    /// </summary>
    [AddComponentMenu("Path-o-logical/TriggerEventPRO/TriggerEventPRO Modifier - Line of Sight")]
    public class LineOfSightModifier : TriggerEventProModifier
    {
        #region Parameters
        public LayerMask targetTrackerLayerMask;
        public LayerMask fireControllerLayerMask;

        public enum TEST_MODE { SinglePoint, BoundingBox }
        public TEST_MODE testMode = TEST_MODE.SinglePoint;
        public float radius = 1.0f;

        public DEBUG_LEVELS debugLevel = DEBUG_LEVELS.Off;
        #endregion Parameters


        #region Cache
        // Public for reference and Inspector logic
        public TargetTracker tracker;
        public EventFireController fireCtrl;
        #endregion Cache


        protected void Awake()
        {
            this.tracker = this.GetComponent<TargetTracker>();
			this.fireCtrl = this.GetComponent<EventFireController>();
        }

		// OnEnable and OnDisable add the check box in the inspector too.
		protected void OnEnable()
		{
            if (this.tracker != null)
				this.tracker.AddOnPostSortDelegate(this.FilterTrackerTargetList);

			if (this.fireCtrl != null)
                this.fireCtrl.AddOnPreFireDelegate(this.FilterFireTargetList);

		}
		
		protected void OnDisable()
		{
            if (this.tracker != null)
				this.tracker.RemoveOnPostSortDelegate(this.FilterTrackerTargetList);

			if (this.fireCtrl != null)
                this.fireCtrl.RemoveOnPreFireDelegate(this.FilterFireTargetList);

		}

        protected void FilterTrackerTargetList(TargetTracker source, TargetList targets)
        {
            // Quit if the mask is set to nothing == OFF
            if (this.targetTrackerLayerMask.value == 0)
                return;
			
			Vector3 fromPos;
			if (this.tracker.area != null)
            	fromPos = this.tracker.area.transform.position;
			else
				fromPos = this.tracker.transform.position;
			
            LayerMask mask = this.targetTrackerLayerMask;
            this.FilterTargetList(targets, mask, fromPos, Color.red);
        }

        protected void FilterFireTargetList(TargetList targets)
        {
            // Quit if the mask is set to nothing == OFF
            if (this.fireControllerLayerMask.value == 0)
                return;

            Vector3 fromPos;
            if (this.fireCtrl.spawnEventTriggerAtTransform != null)
                fromPos = this.fireCtrl.spawnEventTriggerAtTransform.position;
            else
                fromPos = this.fireCtrl.transform.position;

            LayerMask mask = this.fireControllerLayerMask;
            this.FilterTargetList(targets, mask, fromPos, Color.yellow);
        }


        protected void FilterTargetList(TargetList targets, LayerMask mask, Vector3 fromPos,
                                      Color debugLineColor)
        {
#if UNITY_EDITOR
            var debugRemoveNames = new List<string>();
#endif

            Vector3 toPos;
            bool isNotLOS;
            var iterTargets = new List<Target>(targets);

			Collider2D targetColl2D;
			Collider targetColl;
			Vector3 ext;
			bool use2d;
            foreach (Target target in iterTargets)
            {
				use2d = target.targetable.coll2D != null;
				isNotLOS = false;

                if (this.testMode == TEST_MODE.BoundingBox)
                {
					if (use2d)
					{
						targetColl2D = target.targetable.coll2D;			    
						ext = targetColl2D.bounds.extents * 0.5f;
					}
					else
					{
						targetColl = target.targetable.coll;
						ext = targetColl.bounds.extents * 0.5f;
					}

					// This solution works with rotation pretty well
				    Matrix4x4 mtx = target.targetable.transform.localToWorldMatrix;
				   
					var bboxPnts = new Vector3[8];
				    bboxPnts[0] = mtx.MultiplyPoint3x4(ext);
				    bboxPnts[1] = mtx.MultiplyPoint3x4(new Vector3(-ext.x, ext.y, ext.z));
				    bboxPnts[2] = mtx.MultiplyPoint3x4(new Vector3(ext.x, ext.y, -ext.z));
				    bboxPnts[3] = mtx.MultiplyPoint3x4(new Vector3(-ext.x, ext.y, -ext.z));
				    bboxPnts[4] = mtx.MultiplyPoint3x4(new Vector3(ext.x, -ext.y, ext.z));
				    bboxPnts[5] = mtx.MultiplyPoint3x4(new Vector3(-ext.x, -ext.y, ext.z));
				    bboxPnts[6] = mtx.MultiplyPoint3x4(new Vector3(ext.x, -ext.y, -ext.z));
				    bboxPnts[7] = mtx.MultiplyPoint3x4(-ext);

					for (int i = 0; i < bboxPnts.Length; i++)
					{
						if (use2d)
							isNotLOS = Physics2D.Linecast(fromPos, bboxPnts[i], mask);
						else
							isNotLOS = Physics.Linecast(fromPos, bboxPnts[i], mask);

                        // Quit loop at first positive test
                        if (isNotLOS)
						{
#if UNITY_EDITOR
	                        if (this.debugLevel > DEBUG_LEVELS.Off)
	                            Debug.DrawLine(fromPos, bboxPnts[i], debugLineColor, 0.01f);
#endif							
                            continue;
						}
                        else
                            break;
                    }
                }
                else
                {
                	toPos = target.targetable.transform.position;
					if (use2d)
						isNotLOS = Physics2D.Linecast(fromPos, toPos, mask);
					else
						isNotLOS = Physics.Linecast(fromPos, toPos, mask);

#if UNITY_EDITOR
                    if (isNotLOS && this.debugLevel > DEBUG_LEVELS.Off)
                        Debug.DrawLine(fromPos, toPos, debugLineColor, 0.01f);
#endif
                }

                if (isNotLOS)
                {
                    targets.Remove(target);

#if UNITY_EDITOR
                    debugRemoveNames.Add(target.targetable.name);
#endif
                }
            }

#if UNITY_EDITOR
            if (this.debugLevel == DEBUG_LEVELS.High && debugRemoveNames.Count > 0)
                Debug.Log("Holding fire for LOS: " +
                          string.Join(",", debugRemoveNames.ToArray()));
#endif

        }

	}
}