using UnityEngine;

namespace PathologicalGames
{

public class DemoEnemy : MonoBehaviour
{
    public int life = 100;
    public ParticleSystem explosion;
    
    protected Color startingColor;
    protected bool isDead = false;

    protected void Awake()
    {
        this.startingColor = this.GetComponent<Renderer>().material.color;

        var targetable = this.GetComponent<Targetable>();

        targetable.AddOnDetectedDelegate(this.MakeMeBig);
        targetable.AddOnDetectedDelegate(this.MakeMeGreen);

        targetable.AddOnNotDetectedDelegate(this.MakeMeNormal);
        targetable.AddOnNotDetectedDelegate(this.ResetColor);

        targetable.AddOnHitDelegate(this.OnHit);
    }

    protected void OnHit(EventInfoList infoList, Target target)
    {
        if (this.isDead) return;
				
        foreach (EventInfo info in infoList)
        {
			Debug.Log("@@@" + info.name + ": " + info.value);
            switch (info.name)
            {
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

    protected void MakeMeGreen(TargetTracker source)
    {
        if (this.isDead) return;
        this.GetComponent<Renderer>().material.color = Color.green;
    }

    protected void ResetColor(TargetTracker source)
    {
        if (this.isDead) return;
        this.GetComponent<Renderer>().material.color = this.startingColor;
    }

    protected void MakeMeBig(TargetTracker source)
    {
        if (this.isDead) return;
        this.transform.localScale = new Vector3(2, 2, 2);
    }

    protected void MakeMeNormal(TargetTracker source)
    {
        if (this.isDead) return;
        this.transform.localScale = Vector3.one;
    }
}

}