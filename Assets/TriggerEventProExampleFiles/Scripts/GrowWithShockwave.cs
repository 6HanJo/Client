using UnityEngine;
using System.Collections;

namespace PathologicalGames
{

[RequireComponent(typeof(EventTrigger))]
public class GrowWithShockwave : MonoBehaviour
{
    public EventTrigger eventTrigger;

    void Update()
    {
        Vector3 scl = this.eventTrigger.range * 2.1f; // Let geo move ahead of real range
        scl.y *= 0.2f; // More cenematic hieght.
        this.transform.localScale = scl;

        // Blend the alpha channel of the color off as the range expands.
        Color col = this.GetComponent<Renderer>().material.GetColor("_TintColor");
        col.a = Mathf.Lerp(0.7f, 0, this.eventTrigger.range.x / this.eventTrigger.endRange.x);
        this.GetComponent<Renderer>().material.SetColor("_TintColor", col);
    }
}
	
}