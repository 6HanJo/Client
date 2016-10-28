using UnityEngine;

namespace PathologicalGames
{

/// <summary>
///	Uses a Rigidbody to make this projectile seek a target.
/// </summary>
[RequireComponent(typeof(EventTrigger))]
[RequireComponent(typeof(SmoothLookAtConstraint))]
public class DemoSeeker : MonoBehaviour
{
    public float maxVelocity = 500;
    public float acceleration = 75;

    // protected Cache...
    protected EventTrigger projectile;
	protected Rigidbody rbd;

#if (!UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2)
    protected Rigidbody2D rbd2D;
#endif	
		
    protected float minDrag = 10;
    protected float drag = 40;

    protected void Awake()
    {
		this.rbd = this.GetComponent<Rigidbody>();

#if (!UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2)
		this.rbd2D = this.GetComponent<Rigidbody2D>();
#endif	
        this.projectile = this.GetComponent<EventTrigger>();
        this.projectile.AddOnListenStartDelegate(this.OnLaunched);
        this.projectile.AddOnListenUpdateDelegate(this.OnLaunchedUpdate);
        this.projectile.AddOnFireDelegate(this.OnEventTriggerFire);
    }

    /// <summary>
    /// Runs when launched.
    /// </summary>
    protected void OnLaunched()
    {
        // Reset the rigidbody state when launced. This is only needed when pooling.
        if (this.rbd != null)
			this.rbd.drag = this.drag;
#if (!UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2)
        else if (this.rbd2D != null)
			this.rbd2D.drag = this.drag;
#endif			
        // This is a great place to start a fire trail effect. Try a UnityConstraint
        //  Transform constraint to attach it by making this.transform its target.
    }

    /// <summary>
    /// Runs each frame while the projectile is live
    /// </summary>
    protected void OnLaunchedUpdate()
    {			
        // If the target is active, fly to it, otherwise, continue straight 
        //   The constraint should be set to do nothing when there is no target
        // If there is no target, hit anything in the target layers.
        if (!this.projectile.target.isSpawned)  // Despawned
        {
            this.projectile.hitMode = EventTrigger.HIT_MODES.HitLayers;
        }
        else
        {
            this.projectile.hitMode = EventTrigger.HIT_MODES.TargetOnly;
        }

        if (this.rbd != null)
		{
	        // Simulate acceleration by starting with a high drag and reducing it 
	        //		until it reaches the target drag. Init drag in start().
	        if (this.rbd.drag > this.minDrag)
	            this.rbd.drag -= (this.acceleration * 0.01f);
	
	        // Fly straight, constraint will handle rotation
	        this.rbd.AddForce(this.transform.forward * this.maxVelocity);
			
		}
#if (!UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2)
        else if (this.rbd2D != null)
		{
	        // Simulate acceleration by starting with a high drag and reducing it 
	        //		until it reaches the target drag. Init drag in start().
	        if (this.rbd2D.drag > this.minDrag)
	            this.rbd2D.drag -= (this.acceleration * 0.01f);
	
	        // Fly straight, constraint will handle rotation. 
			// 	 2D uses "up" which is the Y axis to keep Z facing camera.
	        this.rbd2D.AddForce(this.transform.up * this.maxVelocity);
		}
#endif			
    }


    /// <summary>
    /// A delegate run by the EventTrigger component OnFire
    /// </summary>
    protected void OnEventTriggerFire(TargetList targets)
    {
       // A great place for an explosion effect to be triggered!
    }
}
}