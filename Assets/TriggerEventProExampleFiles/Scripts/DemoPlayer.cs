using UnityEngine;

namespace PathologicalGames
{
	
[RequireComponent(typeof(Targetable))]
public class DemoPlayer : MonoBehaviour
{
    public int life = 100;
    public ParticleSystem explosion;

	protected bool isDead = false;
	
    protected void Awake()
    {

        var targetable = this.GetComponent<Targetable>();
        targetable.AddOnHitDelegate(this.OnHit);
    }

    protected void OnHit(EventInfoList infoList, Target target)
    {
        if (this.isDead) return;

        foreach (EventInfo info in infoList)
        {
            switch (info.name)
            {
				case "Life":
                	this.life += (int)info.value;
					break;
					
				case "Damage":
                	this.life -= (int)info.value;
					break;
            }
        }
			
        if (this.life <= 0)
        {
            this.isDead = true;
            Instantiate
            (
                this.explosion.gameObject,
                this.transform.position,
                this.transform.rotation
            );

            this.gameObject.SetActive(false);
        }			
	}
		
	protected void OnGUI()
	{
        GUI.Label(new Rect(10, 10, 100, 20), "LIFE: " + this.life);
	}
}

}