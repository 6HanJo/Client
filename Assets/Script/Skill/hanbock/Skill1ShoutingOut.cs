using UnityEngine;
using System.Collections;

public class Skill1ShoutingOut : MonoBehaviour, ISkill {

    public status skillset;
    public float ShoutDelay;

    public Shout[] shouts;

    void Start()
    {
        //StartCoroutine("Delay");
        UseSkill();
    }

    public void SetSkill(float damage, float range, float reload, float ShoutDelay)
    {
        skillset.damage = damage;
        skillset.distance = range;
        skillset.reload = reload;
        this.ShoutDelay = ShoutDelay;
    }

    public void UseSkill()
    {
        StartCoroutine("Shout");
    }

    IEnumerator Shout() {
        shouts[0].dmg = skillset.damage;
        shouts[0].transform.localScale = new Vector2(skillset.distance, skillset.distance);
        shouts[0].gameObject.SetActive(true);
        yield return new WaitForSeconds(ShoutDelay);
        shouts[0].gameObject.SetActive(false);
        shouts[1].dmg = skillset.damage;
        shouts[1].transform.localScale = new Vector2(skillset.distance, skillset.distance);
        shouts[1].gameObject.SetActive(true);
        shouts[2].dmg = skillset.damage;
        shouts[2].transform.localScale = new Vector2(skillset.distance, skillset.distance);
        shouts[2].gameObject.SetActive(true);
        yield return new WaitForSeconds(ShoutDelay);
        shouts[1].gameObject.SetActive(false);
        shouts[2].gameObject.SetActive(false);
        shouts[3].dmg = skillset.damage;
        shouts[3].transform.localScale = new Vector2(skillset.distance, skillset.distance);
        shouts[3].gameObject.SetActive(true);
        shouts[4].dmg = skillset.damage;
        shouts[4].transform.localScale = new Vector2(skillset.distance, skillset.distance);
        shouts[4].gameObject.SetActive(true);
        yield return new WaitForSeconds(ShoutDelay);
        shouts[3].gameObject.SetActive(false);
        shouts[4].gameObject.SetActive(false);
        shouts[5].dmg = skillset.damage;
        shouts[5].transform.localScale = new Vector2(skillset.distance, skillset.distance);
        shouts[5].gameObject.SetActive(true);
        shouts[6].dmg = skillset.damage;
        shouts[6].transform.localScale = new Vector2(skillset.distance, skillset.distance);
        shouts[6].gameObject.SetActive(true);
        yield return new WaitForSeconds(ShoutDelay);
        shouts[5].gameObject.SetActive(false);
        shouts[6].gameObject.SetActive(false);
        shouts[7].dmg = skillset.damage;
        shouts[7].transform.localScale = new Vector2(skillset.distance, skillset.distance);
        shouts[7].gameObject.SetActive(true);
        yield return new WaitForSeconds(ShoutDelay);
        shouts[7].gameObject.SetActive(false);
    }



    IEnumerator Delay()
    {
        while (true)
        {
            yield return new WaitForSeconds(skillset.reload);
            UseSkill();
        }
    }


}
