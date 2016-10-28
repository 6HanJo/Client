using UnityEngine;
using System.Collections;
using PathologicalGames;


public class DemoAreaTargetTrackerOnNotDetected : MonoBehaviour {

	protected TargetTracker tracker;
	
	protected void Awake()
	{
		this.tracker = this.GetComponent<TargetTracker>();
	}
	
	// OnEnable and OnDisable add the check box in the inspector too.
	protected void OnEnable()
	{
		if (this.tracker != null)
			this.tracker.AddOnNotDetectedDelegate(this.OnNotDetected);
	}
	
	protected void OnDisable()
	{
		if (this.tracker != null)
			this.tracker.RemoveOnNotDetectedDelegate(this.OnNotDetected);
		
	}

	protected void OnNotDetected(TargetTracker source, Target target)
	{
		Debug.Log
		(
			string.Format("OnNotDetected triggered for tracker '{0}' by target '{1}'.", source, target)
		);
	}

}
