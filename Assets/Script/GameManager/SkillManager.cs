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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
		print (player.name);
        arrSkillInfo[0].skill = player.GetComponent<Skill1ShoutingOut>();
        arrSkillInfo[1].skill = player.GetComponent<Skill2GusleMaker>();
        arrSkillInfo[2].skill = player.GetComponent<Skill3HungryAttacking>();
        arrSkillInfo[3].skill = player.GetComponent<Skill4AngryAttacking>();
        arrSkillInfo[4].skill = player.GetComponent<Skill5PlazmaMaker>();
        arrSkillInfo[5].skill = player.GetComponent<Skill6LaserRotation>();

        for (int i = 0; i < arrSkillInfo.Length; i++)
        {
            arrSkillInfo[i].skillCalculator.skillInfo = arrSkillInfo[i];
        }
        AllSKillEventRegiste();

        InGameManager.Instance.EventTimeLimitBegin += StartUseAllSkill;
        InGameManager.Instance.EventTimeLimitEnd += StopUseAllSKill;
    }

    public void StartUseAllSkill()
    {
        for (int i = 0; i < arrSkillInfo.Length; i++)
        {
            if(arrSkillInfo[i].isActive)
                OnUseSkill(arrSkillInfo[i]);
        }
    }

    public void StopUseAllSKill()
    {
        for (int i = 0; i < arrSkillInfo.Length; i++)
        {
            if(arrSkillInfo[i].isActive)
                OnStopSkill(arrSkillInfo[i]);
        }
    }

    //스킬 이벤트 등록
    public void AllSKillEventRegiste()
    {
        for (int i = 0; i < arrSkillInfo.Length; i++)
        {
            SkillInfo info = arrSkillInfo[i];
            if (info.skill != null)
                info.isActive = true;
            info.skillCalculator.EventCoolTimeDone -= OnUseSkill;
            if(info.isActive)
            {
                info.skillCalculator.EventCoolTimeDone += OnUseSkill;
                info.imgSkillSlot.sprite = info.sprSkillActiveImage;
            }
            else
            {
                info.imgSkillSlot.sprite = info.sprSkillDeActiveImage;
            }
        }
    }

    private void OnUseSkill(SkillInfo info)
    {
        //위치 리셋
        info.skillCalculator.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -20f);
        info.imgSkillSlot.transform.DOPunchScale(new Vector3(0.4f,0.4f,0.4f), 0.2f, 0);

        //스킬 사용
        info.skill.UseSkill();

        //다시 쿨타임 계산
        info.skillCalculator.BeginCalculateCoolTime();
    }

    void OnStopSkill(SkillInfo info)
    {
        info.skillCalculator.StopCalculateCoolTime();
    }

    // Update is called once per frame
    void Update () {
	
	}
}
