using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Hero : MonoBehaviour
{
    // 英雄属性
    public float health = 200f;
    public float maxHealth = 200f;
    public float mana = 150f;
    public float maxMana = 150f;
    public float attackPower = 15f;
    public float defense = 5f;
    public float speed = 5f;
    public float rage = 0f;
    public float maxRage = 100f;
    public string element = "Fire";

    // 等级与经验
    public int level = 1;
    public int experience = 0;

    // Boss 击败计数
    public int bossesDefeated = 0;
    public List<string> defeatedBossNames = new List<string>();

    // 初始属性值常量
    private const float INITIAL_HEALTH = 200f;
    private const float INITIAL_MANA = 150f;
    private const float INITIAL_DEFENSE = 5f;
    private const float INITIAL_ATTACKPOWER = 15f;

    // 道具
    public int healingPotions = 3;
    public int manaPotions = 3;
    public int rageStones = 3;

    // 防御状态
    public bool isDefending = false;
    private bool wasDefending = false;

    public List<Skill> skills = new List<Skill>();

    [SerializeField] private Animator heroAnimator;
    [SerializeField] private Animator monsterAnimator;

    // 成长配置（Inspector 中拖入 HeroProgression.asset）
    [SerializeField] private HeroProgression progression;

    // 等级提升事件（供 UI 监听）
    public System.Action<int, List<string>> OnLevelUp;
    public System.Action<int> OnExperienceChanged;

    private Dictionary<string, int> inventory = new Dictionary<string, int>();

    void Start()
    {
        InitializeSkills();

        // 检查 SaveManager 中是否有存档数据需要恢复
        if (SaveManager.Instance != null && SaveManager.Instance.CurrentSave != null
            && SaveManager.Instance.CurrentSave.bossesDefeated >= 0)
        {
            SaveManager.Instance.ApplySaveToHero(this);
        }
        else
        {
            health = maxHealth = INITIAL_HEALTH;
            mana = maxMana = INITIAL_MANA;

            inventory["治疗药水"] = 3;
            inventory["魔力药水"] = 3;
            inventory["怒气石"] = 1;
        }

        // 应用已击败 Boss 的道具奖励
        if (progression != null)
            ApplyBossItemRewards();
    }

    public void Attack(Monster target)
    {
        AddRage(20f);
    }

    public bool UseSkill(Skill skill, Monster target)
    {
        if (!skill.isUnlocked)
        {
            Debug.LogWarning($"技能 {skill.name} 尚未解锁，无法使用");
            return false;
        }

        if (skill.isUltimate && rage < maxRage) return false;
        if (!skill.isUltimate && mana < skill.manaCost) return false;

        if (!skill.isUltimate)
        {
            mana -= skill.manaCost;
            AddRage(skill.manaCost / 2f);
        }
        else
        {
            rage = 0f;
        }

        return true;
    }

    public bool UseItem(string itemName)
    {
        if (inventory.ContainsKey(itemName) && inventory[itemName] > 0)
        {
            switch (itemName)
            {
                case "治疗药水":
                    health = Mathf.Clamp(health + 50, 0f, maxHealth);
                    break;
                case "魔力药水":
                    mana = Mathf.Clamp(mana + 60, 0f, maxMana);
                    break;
                case "怒气石":
                    AddRage(50);
                    break;
                default:
                    return false;
            }
            inventory[itemName]--;
            return true;
        }
        return false;
    }

    public List<string> GetAvailableItems()
    {
        List<string> availableItems = new List<string>();
        foreach (var item in inventory)
        {
            if (item.Value > 0)
                availableItems.Add(item.Key);
        }
        return availableItems;
    }

    public void Defend()
    {
        isDefending = true;
        wasDefending = true;
        defense = INITIAL_DEFENSE * 2f;
    }

    public void TakeDamage(float damage)
    {
        float actualDamage = damage - defense;
        if (actualDamage < 0) actualDamage = 0;

        health -= actualDamage;
        if (heroAnimator != null) heroAnimator.SetTrigger("Get Hit");
        if (monsterAnimator != null) monsterAnimator.SetTrigger("Attack");
        health = Mathf.Clamp(health, 0f, maxHealth);

        AddRage(actualDamage / 2f);
    }

    public void ResetState()
    {
        health = maxHealth;
        mana = maxMana;
        rage = 0f;
        defense = INITIAL_DEFENSE;
        attackPower = INITIAL_ATTACKPOWER;
        isDefending = false;
    }

    public void EndTurn()
    {
    }

    private void InitializeSkills()
    {
        skills.Add(new Skill("火球术",   20f, 30f, false, true));
        skills.Add(new Skill("冰锥",     15f, 25f, false, false));
        skills.Add(new Skill("雷击",     25f, 40f, false, false));
        skills.Add(new Skill("虚弱",     30f, 0f,  false, false));
        skills.Add(new Skill("治疗术",   30f, 0f,  false, false));
        skills.Add(new Skill("灵魂激流", 0f,  0f,  true,  false));
    }

    private void AddRage(float amount)
    {
        rage = Mathf.Clamp(rage + amount, 0f, maxRage);
    }

    public void StartTurn()
    {
    }

    public void ResetDefense()
    {
        if (isDefending)
        {
            isDefending = false;
            defense = INITIAL_DEFENSE;
        }
    }

    public bool CheckAndResetDefendingStatus()
    {
        bool status = wasDefending;
        wasDefending = false;
        return status;
    }

    public bool CanUseMagicSurge()
    {
        return rage >= maxRage;
    }

    public bool HasEnoughMana(float manaCost)
    {
        return mana >= manaCost;
    }

    public void ResetRage()
    {
        rage = 0f;
    }

    public void UnlockSkill(string skillName)
    {
        Skill skill = skills.Find(s => s.name == skillName);
        if (skill != null)
        {
            skill.isUnlocked = true;
            Debug.Log($"技能 {skillName} 已解锁！");
        }
        else
        {
            Debug.LogWarning($"未找到名为 {skillName} 的技能");
        }
    }

    public void UnlockSkill(string skillName, bool unlocked)
    {
        Skill skill = skills.Find(s => s.name == skillName);
        if (skill != null)
            skill.isUnlocked = unlocked;
    }

    public List<Skill> GetUnlockedSkills()
    {
        return skills.Where(s => s.isUnlocked).ToList();
    }

    public void AdjustItemQuantity(string itemName, int amount)
    {
        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName] += amount;
            if (inventory[itemName] < 0)
                inventory[itemName] = 0;
        }
        else if (amount > 0)
        {
            inventory[itemName] = amount;
        }

        if (inventory.ContainsKey(itemName))
            Debug.Log($"道具 {itemName} 的数量调整为: {inventory[itemName]}");
    }

    public int GetItemQuantity(string itemName)
    {
        return inventory.ContainsKey(itemName) ? inventory[itemName] : 0;
    }

    public void SetItemQuantity(string itemName, int quantity)
    {
        inventory[itemName] = quantity;
    }

    public Dictionary<string, int> GetAllItems()
    {
        return new Dictionary<string, int>(inventory);
    }

    // ========== 成长系统 ==========

    public void GainExperience(int amount)
    {
        if (progression == null)
        {
            Debug.LogWarning("HeroProgression 未配置，无法获得经验");
            return;
        }

        experience += amount;
        OnExperienceChanged?.Invoke(experience);

        int newLevel = progression.GetLevelForXP(experience);
        while (newLevel > level)
        {
            level++;
            ApplyLevelUp(level);
        }

        OnExperienceChanged?.Invoke(experience);
    }

    private void ApplyLevelUp(int newLevel)
    {
        var levelData = progression.GetLevelData(newLevel);

        maxHealth += levelData.healthBonus;
        health = maxHealth;
        maxMana += levelData.manaBonus;
        mana = maxMana;
        attackPower += levelData.attackBonus;
        defense += levelData.defenseBonus;

        // 解锁该等级的技能
        List<string> newSkills = new List<string>();
        var skillsToUnlock = progression.GetSkillsToUnlockAtLevel(newLevel);
        foreach (var skillName in skillsToUnlock)
        {
            var skill = skills.Find(s => s.name == skillName);
            if (skill != null && !skill.isUnlocked)
            {
                skill.isUnlocked = true;
                newSkills.Add(skillName);
                Debug.Log($"升级解锁技能: {skillName}！");
            }
        }

        OnLevelUp?.Invoke(newLevel, newSkills);
        Debug.Log($"英雄升级至 Lv.{newLevel}！HP+{levelData.healthBonus} MP+{levelData.manaBonus} ATK+{levelData.attackBonus} DEF+{levelData.defenseBonus}");
    }

    /// <summary> 击败一个 Boss 后调用 </summary>
    public void OnBossDefeated(string bossName, int bossIndex)
    {
        if (defeatedBossNames.Contains(bossName)) return;

        defeatedBossNames.Add(bossName);
        bossesDefeated++;

        // 给予经验
        if (progression != null)
        {
            int xp = progression.GetXPForBoss(bossIndex);
            GainExperience(xp);
        }

        // 给予道具奖励
        if (progression != null)
        {
            var rewards = progression.GetItemRewardsForBossesDefeated(bossesDefeated);
            foreach (var reward in rewards)
            {
                AdjustItemQuantity(reward.itemName, reward.quantity);
                Debug.Log($"获得道具奖励: {reward.itemName} ×{reward.quantity}");
            }
        }

        // 每次击败 Boss 自动存档
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.UpdateSaveFromHero(this);
            SaveManager.Instance.SaveGame(0); // 自动存档到槽位 0
        }
    }

    /// <summary> 应用已有击败数的道具奖励（加载存档时调用） </summary>
    private void ApplyBossItemRewards()
    {
        for (int i = 1; i <= bossesDefeated; i++)
        {
            var rewards = progression.GetItemRewardsForBossesDefeated(i);
            foreach (var reward in rewards)
            {
                AdjustItemQuantity(reward.itemName, reward.quantity);
            }
        }
    }
}
