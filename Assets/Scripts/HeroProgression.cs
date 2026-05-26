using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "HeroProgression", menuName = "Game/Hero Progression")]
public class HeroProgression : ScriptableObject
{
    [System.Serializable]
    public class LevelData
    {
        public int level;
        public int xpRequired;        // 升到该级需要的累积经验
        public float healthBonus;
        public float manaBonus;
        public float attackBonus;
        public float defenseBonus;
    }

    [System.Serializable]
    public class SkillUnlockData
    {
        public string skillName;
        public int unlockLevel;
    }

    [System.Serializable]
    public class ItemRewardData
    {
        public string itemName;
        public int quantity;
        public int requiredBossesDefeated;
    }

    [Header("升级曲线")]
    public LevelData[] levelCurve = new LevelData[]
    {
        new LevelData { level = 1, xpRequired = 0,    healthBonus = 0,   manaBonus = 0,   attackBonus = 0,  defenseBonus = 0 },
        new LevelData { level = 2, xpRequired = 100,  healthBonus = 20,  manaBonus = 15,  attackBonus = 3,  defenseBonus = 2 },
        new LevelData { level = 3, xpRequired = 300,  healthBonus = 20,  manaBonus = 15,  attackBonus = 3,  defenseBonus = 2 },
        new LevelData { level = 4, xpRequired = 600,  healthBonus = 20,  manaBonus = 15,  attackBonus = 3,  defenseBonus = 2 },
        new LevelData { level = 5, xpRequired = 1000, healthBonus = 20,  manaBonus = 15,  attackBonus = 3,  defenseBonus = 2 },
        new LevelData { level = 6, xpRequired = 1500, healthBonus = 20,  manaBonus = 15,  attackBonus = 3,  defenseBonus = 2 },
    };

    [Header("技能解锁（等级要求）")]
    public SkillUnlockData[] skillUnlocks = new SkillUnlockData[]
    {
        new SkillUnlockData { skillName = "火球术",   unlockLevel = 1 },
        new SkillUnlockData { skillName = "冰锥",     unlockLevel = 2 },
        new SkillUnlockData { skillName = "虚弱",     unlockLevel = 3 },
        new SkillUnlockData { skillName = "雷击",     unlockLevel = 4 },
        new SkillUnlockData { skillName = "治疗术",   unlockLevel = 5 },
        new SkillUnlockData { skillName = "灵魂激流", unlockLevel = 6 },
    };

    [Header("Boss XP 奖励")]
    public int[] bossXPRewards = new int[] { 0, 100, 200, 300, 500, 800 };
    // bossXPRewards[1] = 第1个Boss, 以此类推

    [Header("道具奖励（击败N个Boss后解锁）")]
    public ItemRewardData[] itemRewards = new ItemRewardData[]
    {
        new ItemRewardData { itemName = "治疗药水", quantity = 2, requiredBossesDefeated = 1 },
        new ItemRewardData { itemName = "魔力药水", quantity = 2, requiredBossesDefeated = 2 },
        new ItemRewardData { itemName = "怒气石",   quantity = 2, requiredBossesDefeated = 3 },
        new ItemRewardData { itemName = "治疗药水", quantity = 3, requiredBossesDefeated = 4 },
        new ItemRewardData { itemName = "魔力药水", quantity = 3, requiredBossesDefeated = 5 },
    };

    public int GetXPForBoss(int bossIndex)
    {
        if (bossIndex >= 0 && bossIndex < bossXPRewards.Length)
            return bossXPRewards[bossIndex];
        return 0;
    }

    public int GetXPRequiredForLevel(int level)
    {
        foreach (var ld in levelCurve)
        {
            if (ld.level == level) return ld.xpRequired;
        }
        return int.MaxValue;
    }

    public LevelData GetLevelData(int level)
    {
        foreach (var ld in levelCurve)
        {
            if (ld.level == level) return ld;
        }
        return levelCurve[levelCurve.Length - 1];
    }

    public int GetLevelForXP(int xp)
    {
        int currentLevel = 1;
        foreach (var ld in levelCurve)
        {
            if (xp >= ld.xpRequired)
                currentLevel = ld.level;
            else
                break;
        }
        return currentLevel;
    }

    public List<string> GetSkillsToUnlockAtLevel(int level)
    {
        List<string> result = new List<string>();
        foreach (var su in skillUnlocks)
        {
            if (su.unlockLevel == level)
                result.Add(su.skillName);
        }
        return result;
    }

    public List<ItemRewardData> GetItemRewardsForBossesDefeated(int bossesDefeated)
    {
        List<ItemRewardData> result = new List<ItemRewardData>();
        foreach (var ir in itemRewards)
        {
            if (ir.requiredBossesDefeated == bossesDefeated)
                result.Add(ir);
        }
        return result;
    }
}
