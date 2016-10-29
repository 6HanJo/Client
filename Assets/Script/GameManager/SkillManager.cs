using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SkillManager : MonoBehaviour {

    static SkillManager instance;
    public static SkillManager Instance
    {
        get { return instance; }
    }

    public SkillInfo[] arrSkillInfo;

    void Awake()
    {
        if(instance == null)
            instance = this;
    }

    // Use this for initialization
    void Start () {

        for (int i = 0; i < arrSkillInfo.Length; i++)
        {
            arrSkillInfo[i].skillCalculator.skillInfo = arrSkillInfo[i];
        }
        AllSKillEventRegiste();
        StartCoroutine(test());
    }

    IEnumerator test()
    {
        yield return new WaitForSeconds(1f);
        BeginUseAllSkill();
    }

    public void BeginUseAllSkill()
    {
        for (int i = 0; i < arrSkillInfo.Length; i++)
        {
            OnUseSkill(arrSkillInfo[i]);
        }
    }

    //스킬 이벤트 등록
    public void AllSKillEventRegiste()
    {
        for (int i = 0; i < arrSkillInfo.Length; i++)
        {
            SkillInfo info = arrSkillInfo[i];
            info.skillCalculator.EventCoolTimeDone -= OnUseSkill;
            if(info.isActive)
            {
                info.skillCalculator.EventCoolTimeDone += OnUseSkill;
                info.imgSkillSlot.sprite = info.sprSkillImage;
            }
        }
    }

    private void OnUseSkill(SkillInfo info)
    {
        //위치 리셋
        info.skillCalculator.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -20f);
        info.imgSkillSlot.transform.DOPunchScale(new Vector3(0.4f,0.4f,0.4f), 0.2f, 0);

        //스킬 사용


        //다시 쿨타임 계산
        info.skillCalculator.BeginCalculateCoolTime();
    }

    // Update is called once per frame
    void Update () {
	
	}
}
