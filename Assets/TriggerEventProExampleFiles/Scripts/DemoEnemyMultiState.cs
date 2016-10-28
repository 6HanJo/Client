using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PathologicalGames
{

/// <summary>
/// This enemy will change size and color depending on one of three states: 
///   1. Not detected: red (or original color)
///   2. Detected by a TargetTracker Area: yellow
///   3. The active target of a TargetTracker determined by sort and number: green
/// </summary>
public class DemoEnemyMultiState : MonoBehaviour
{
    public int life = 100;
    public ParticleSystem explosion;
			
	// States
	protected enum STATES { Dead, NotDetected, Detected, ActiveTarget }	
	protected STATES currentState = STATES.NotDetected;
	protected bool isUpdateWhileTrackedRunning = false;
		
	// Cache...
	protected Vector3 activeTargetScale = new Vector3(2, 2, 2);
	protected Color startingColor;
	protected Targetable targetable;
	protected List<TargetTracker> detectingTrackers = new List<TargetTracker>();
		
    protected void Awake()
    {
        this.startingColor = this.GetComponent<Renderer>().material.color;
			
        this.targetable = this.GetComponent<Targetable>();
        this.targetable.AddOnDetectedDelegate(this.OnDetected);
        this.targetable.AddOnNotDetectedDelegate(this.OnNotDetected);

        this.targetable.AddOnHitDelegate(this.OnHit);
    }

    protected void OnHit(EventInfoList infoList, Target target)
    {
		// If dead, then all done! Will also interupt the co-routine.
        if (this.currentState == STATES.Dead) return;  

		if (target.collider != null)
			Debug.Log(this.name + " was hit by 3D collider on " + target.collider.name);

#if (!UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2)  // Unity 4.3+ only...
		if (target.collider2D != null)
			Debug.Log(this.name + " was hit by 2D collider on " + target.collider2D.name);
#endif
		foreach (EventInfo info in infoList)
        {
            switch (info.name)
            {
				case "Damage":
                	this.life -= (int)info.value;
					break;
            }
        }

        if (this.life <= 0)
        {
            this.SetState(STATES.Dead);
				
            Instantiate
            (
                this.explosion.gameObject,
                this.transform.position,
                this.transform.rotation
            );
			
            Destroy(this.gameObject);
        }
    }
		
    protected void OnDetected(TargetTracker source)
    {
		this.detectingTrackers.Add(source);	
				
		// Start a co-routine for each TargetTracker that detects this
		if (!this.isUpdateWhileTrackedRunning)				
			this.StartCoroutine(this.UpdateWhileTracked());
    }

    protected void OnNotDetected(TargetTracker source)
    {
		this.detectingTrackers.Remove(source);	
	}
	
	protected void SetState(STATES state)
	{
		// Only process state changes.
		// Once dead, never process state changes again.
		if (this.currentState == state || this.currentState == STATES.Dead)
			return;
						
		switch (state)
		{
			case STATES.Dead:
				// Just here to be explicit. Once Dead, this won't run again. See above.
				break;

			case STATES.NotDetected:
		        this.transform.localScale = Vector3.one;
		        this.GetComponent<Renderer>().material.color = this.startingColor;
				break;

			case STATES.Detected:
				this.GetComponent<Renderer>().material.color = Color.yellow;
				this.transform.localScale = this.activeTargetScale * 0.75f;
				break;
			
			case STATES.ActiveTarget:
				this.GetComponent<Renderer>().material.color = Color.green;
				this.transform.localScale = this.activeTargetScale;
				break;
		}

		this.currentState = state;
	}
    
	protected IEnumerator UpdateWhileTracked()
    {
		this.isUpdateWhileTrackedRunning = true;
		
		// Track the state change to figure out when not detected
		bool switchedToActive;  
			
		// Quit if the targetable is no longer being tracked
        while (this.detectingTrackers.Count > 0)
        {
            if (this.currentState == STATES.Dead) yield break;  // If dead, then all done!
			
			// If 1 TargetTracker is focused on this it is enough to 
			//	 trigger the active state and break the loop.
			switchedToActive = false;
			foreach (TargetTracker tracker in this.detectingTrackers)
			{
	            if (tracker.targets.Contains(new Target(this.targetable, tracker)))
	            {
					this.SetState(STATES.ActiveTarget);	
					switchedToActive = true;
					break;
	            } 
			}
				
			if (!switchedToActive)
				this.SetState(STATES.Detected);
			 
            yield return null;
        }
		
		this.SetState(STATES.NotDetected);

		this.isUpdateWhileTrackedRunning = false;
	}

}
}