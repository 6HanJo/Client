﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

[Serializable]
public class SkillInfo {
    public int idx;
    public bool isActive = false;
    public Sprite sprSkillActiveImage;
    public Sprite sprSkillDeActiveImage;
    public Image imgSkillSlot;
    public int coolTime;
    public SkillCalculator skillCalculator;
    public ISkill skill;
}
