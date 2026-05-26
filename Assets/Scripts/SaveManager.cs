using UnityEngine;
using System.IO;
using System;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private static string SAVE_FOLDER;
    private const string SAVE_FILE_PREFIX = "save_";
    private const string SAVE_EXTENSION = ".json";
    private const int MAX_SAVE_SLOTS = 3;

    public SaveData CurrentSave { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SAVE_FOLDER = Application.persistentDataPath + "/Saves/";
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NewGame()
    {
        CurrentSave = new SaveData();
        CurrentSave.saveSlotName = "新游戏";
        CurrentSave.saveTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // 默认英雄属性
        CurrentSave.maxHealth = 200f;
        CurrentSave.health = 200f;
        CurrentSave.maxMana = 150f;
        CurrentSave.mana = 150f;
        CurrentSave.attackPower = 15f;
        CurrentSave.defense = 5f;
        CurrentSave.speed = 5f;
        CurrentSave.rage = 0f;
        CurrentSave.maxRage = 100f;
        CurrentSave.element = "Fire";
        CurrentSave.level = 1;
        CurrentSave.experience = 0;

        // 初始只解锁 火球术
        CurrentSave.unlockedSkills = new System.Collections.Generic.Dictionary<string, bool>
        {
            { "火球术", true },
            { "冰锥", false },
            { "雷击", false },
            { "虚弱", false },
            { "治疗术", false },
            { "灵魂激流", false }
        };

        // 初始道具
        CurrentSave.inventory = new System.Collections.Generic.Dictionary<string, int>
        {
            { "治疗药水", 3 },
            { "魔力药水", 3 },
            { "怒气石", 1 }
        };

        CurrentSave.bossesDefeated = 0;
        CurrentSave.mapPosX = 0f;
        CurrentSave.mapPosY = 0f;
    }

    public bool SaveGame(int slot)
    {
        if (CurrentSave == null) return false;
        if (slot < 0 || slot >= MAX_SAVE_SLOTS) return false;

        try
        {
            if (!Directory.Exists(SAVE_FOLDER))
                Directory.CreateDirectory(SAVE_FOLDER);

            CurrentSave.saveTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            CurrentSave.saveSlotName = $"存档槽 {slot + 1}";

            string path = GetSavePath(slot);
            string json = JsonUtility.ToJson(CurrentSave, true);
            File.WriteAllText(path, json);

            Debug.Log($"游戏已保存到槽位 {slot}: {path}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"保存失败: {e.Message}");
            return false;
        }
    }

    public bool LoadGame(int slot)
    {
        string path = GetSavePath(slot);
        if (!File.Exists(path)) return false;

        try
        {
            string json = File.ReadAllText(path);
            CurrentSave = JsonUtility.FromJson<SaveData>(json);
            Debug.Log($"游戏已从槽位 {slot} 加载");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"加载失败: {e.Message}");
            return false;
        }
    }

    public bool HasSaveInSlot(int slot)
    {
        return File.Exists(GetSavePath(slot));
    }

    public SaveData GetSaveInfo(int slot)
    {
        string path = GetSavePath(slot);
        if (!File.Exists(path)) return null;

        try
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch
        {
            return null;
        }
    }

    public bool DeleteSave(int slot)
    {
        string path = GetSavePath(slot);
        if (File.Exists(path))
        {
            File.Delete(path);
            return true;
        }
        return false;
    }

    public void ApplySaveToHero(Hero hero)
    {
        if (CurrentSave == null || hero == null) return;

        hero.health = CurrentSave.health;
        hero.maxHealth = CurrentSave.maxHealth;
        hero.mana = CurrentSave.mana;
        hero.maxMana = CurrentSave.maxMana;
        hero.attackPower = CurrentSave.attackPower;
        hero.defense = CurrentSave.defense;
        hero.speed = CurrentSave.speed;
        hero.rage = CurrentSave.rage;
        hero.maxRage = CurrentSave.maxRage;
        hero.element = CurrentSave.element;
        hero.level = CurrentSave.level;
        hero.experience = CurrentSave.experience;

        // 同步技能解锁状态
        foreach (var kvp in CurrentSave.unlockedSkills)
        {
            hero.UnlockSkill(kvp.Key, kvp.Value);
        }

        // 同步道具
        foreach (var kvp in CurrentSave.inventory)
        {
            hero.SetItemQuantity(kvp.Key, kvp.Value);
        }

        // 同步 Boss 击败列表
        hero.bossesDefeated = CurrentSave.bossesDefeated;
        hero.defeatedBossNames = new System.Collections.Generic.List<string>(CurrentSave.defeatedBossNames);
    }

    public void UpdateSaveFromHero(Hero hero)
    {
        if (CurrentSave == null || hero == null) return;

        CurrentSave.health = hero.health;
        CurrentSave.maxHealth = hero.maxHealth;
        CurrentSave.mana = hero.mana;
        CurrentSave.maxMana = hero.maxMana;
        CurrentSave.attackPower = hero.attackPower;
        CurrentSave.defense = hero.defense;
        CurrentSave.speed = hero.speed;
        CurrentSave.rage = hero.rage;
        CurrentSave.maxRage = hero.maxRage;
        CurrentSave.element = hero.element;
        CurrentSave.level = hero.level;
        CurrentSave.experience = hero.experience;

        // 同步技能
        CurrentSave.unlockedSkills.Clear();
        foreach (var skill in hero.skills)
        {
            CurrentSave.unlockedSkills[skill.name] = skill.isUnlocked;
        }

        // 同步道具
        CurrentSave.inventory.Clear();
        foreach (var item in hero.GetAllItems())
        {
            CurrentSave.inventory[item.Key] = item.Value;
        }

        CurrentSave.bossesDefeated = hero.bossesDefeated;
        CurrentSave.defeatedBossNames = new System.Collections.Generic.List<string>(hero.defeatedBossNames);
    }

    private string GetSavePath(int slot)
    {
        return SAVE_FOLDER + SAVE_FILE_PREFIX + slot + SAVE_EXTENSION;
    }
}
