using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SkillCalculator : MonoBehaviour {
    public event CallbackUseSkill EventCoolTimeDone;
    public SkillInfo skillInfo;
    RectTransform rectTransform;

    // Use this for initialization
    void Start() {
        rectTransform = GetComponent<RectTransform>();
    }

    public void BeginCalculateCoolTime()
    {
        StartCoroutine(CoCalculateCoolTime());
    }

    IEnumerator CoCalculateCoolTime()
    {
        rectTransform.DOAnchorPos(new Vector2(0f, -175f), skillInfo.coolTime).SetEase(Ease.Linear);
        yield return new WaitForSeconds(skillInfo.coolTime);
        EventCoolTimeDone(skillInfo);
    }
}
