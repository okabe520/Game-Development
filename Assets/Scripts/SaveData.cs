using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    // 英雄属性
    public float health;
    public float maxHealth;
    public float mana;
    public float maxMana;
    public float attackPower;
    public float defense;
    public float speed;
    public float rage;
    public float maxRage;
    public string element;

    // 等级与经验
    public int level;
    public int experience;
    public int skillPoints;

    // 技能解锁状态（技能名 → 是否解锁）
    public Dictionary<string, bool> unlockedSkills;

    // 道具库存（道具名 → 数量）
    public Dictionary<string, int> inventory;

    // 游戏进度
    public int bossesDefeated;       // 已击杀 Boss 数
    public List<string> defeatedBossNames;

    // 地图位置
    public float mapPosX;
    public float mapPosY;

    // 元数据
    public string saveTimestamp;
    public string saveSlotName;
    public int playTimeSeconds;

    public SaveData()
    {
        unlockedSkills = new Dictionary<string, bool>();
        inventory = new Dictionary<string, int>();
        defeatedBossNames = new List<string>();
        bossesDefeated = 0;
        level = 1;
        experience = 0;
        skillPoints = 0;
    }
}
