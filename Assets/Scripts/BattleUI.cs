using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BattleUI : MonoBehaviour
{
    public Hero hero; // 添加对英雄的引用
    public Monster monster;         
    public Text statusText;        
    public BattleManager battleManager;
    public SkillSelectionUI skillSelectionUI;
    public BattleLog battleLog;
    public HeroStatusBar heroStatusBar;
    public MonsterStatusBar monsterStatusBar;

    public Button attackButton;     
    public Button skillButton;
    public Button itemButton;       
    public Button defendButton;     

    public ItemSelectionUI itemSelectionUI;

    void Start()
    {
        Debug.Log("BattleUI Start method called");
        if (battleManager == null)
        {
            battleManager = FindObjectOfType<BattleManager>();
            if (battleManager == null)
            {
                Debug.LogError("BattleManager not found in the scene!");
                return;
            }
        }

        // 设置 BattleManager 中的 BattleUI 引用
        battleManager.SetBattleUI(this);

        // Add button click listeners
        attackButton.onClick.AddListener(OnAttackButtonClicked);
        skillButton.onClick.AddListener(OnSkillButtonClicked);
        itemButton.onClick.AddListener(OnItemButtonClicked);
        defendButton.onClick.AddListener(OnDefendButtonClicked);

        if (battleLog == null)
        {
            battleLog = FindObjectOfType<BattleLog>();
            if (battleLog == null) Debug.LogError("BattleLog not found in the scene!");
        }

        if (heroStatusBar == null)
        {
            heroStatusBar = FindObjectOfType<HeroStatusBar>();
            if (heroStatusBar == null) Debug.LogError("HeroStatusBar not found in the scene!");
        }
        if (monsterStatusBar == null)
        {
            monsterStatusBar = FindObjectOfType<MonsterStatusBar>();
            if (monsterStatusBar == null) Debug.LogError("MonsterStatusBar not found in the scene!");
        }

        if (hero == null)
        {
            hero = FindObjectOfType<Hero>();
            if (hero == null) Debug.LogError("Hero not found in the scene!");
        }

        // Initialize status text
        UpdateUI();

        // 监听英雄升级事件
        if (hero != null)
            hero.OnLevelUp += OnHeroLevelUp;
    }

    void OnDestroy()
    {
        if (attackButton != null) attackButton.onClick.RemoveListener(OnAttackButtonClicked);
        if (skillButton != null) skillButton.onClick.RemoveListener(OnSkillButtonClicked);
        if (itemButton != null) itemButton.onClick.RemoveListener(OnItemButtonClicked);
        if (defendButton != null) defendButton.onClick.RemoveListener(OnDefendButtonClicked);
        if (hero != null) hero.OnLevelUp -= OnHeroLevelUp;
    }

    private void OnHeroLevelUp(int newLevel, System.Collections.Generic.List<string> newSkills)
    {
        string skillsStr = newSkills.Count > 0
            ? "\n新技能解锁: " + string.Join(", ", newSkills)
            : "";
        LogMessage($"英雄升级至 Lv.{newLevel}！{skillsStr}");
    }

    // Ordinary attack button click event
    void OnAttackButtonClicked()
    {
        CloseSkillSelection();
        CloseItemSelection(); // 添加这行
        Debug.Log("Attack button clicked");
        if (battleManager.isHeroTurn)
        {
            battleManager.HeroTurn("Attack");
            UpdateUI();
        }
    }

    // Skill button click event
    void OnSkillButtonClicked()
    {
        CloseItemSelection(); // 添加这行
        if (skillSelectionUI.gameObject.activeSelf)
        {
            CloseSkillSelection();
        }
        else
        {
            OpenSkillSelection();
        }
    }

    // Use item button click event
    void OnItemButtonClicked()
    {
        CloseSkillSelection();
        if (battleManager.isHeroTurn)
        {
            if (itemSelectionUI.gameObject.activeSelf)
            {
                CloseItemSelection();
            }
            else
            {
                OpenItemSelection();
            }
        }
    }

    void OpenItemSelection()
    {
        if (itemSelectionUI == null || hero == null) return;
        List<string> availableItems = hero.GetAvailableItems();
        itemSelectionUI.ShowItemSelection(availableItems);
    }

    void CloseItemSelection()
    {
        if (itemSelectionUI != null)
        {
            itemSelectionUI.HideItemSelection();
        }
    }

    // Defend button click event
    void OnDefendButtonClicked()
    {
        CloseSkillSelection();
        CloseItemSelection(); // 添加这行
        if (battleManager.isHeroTurn)
        {
            battleManager.HeroTurn("Defend");
            UpdateUI();
        }
    }

    // Handle skill selection
    public void OnSkillSelected(Skill skill)
    {
        CloseSkillSelection();
        CloseItemSelection(); // 添加这行
        if (battleManager.isHeroTurn)
        {
            battleManager.HeroTurn("Skill", skill);
            UpdateUI();
        }
    }

    // Handle Magic Surge selection
    public void OnMagicSurgeSelected()
    {
        if (battleManager.isHeroTurn)
        {
            battleManager.HeroTurn("MagicSurge");
            UpdateUI();
        }
    }

    // Update UI method
    public void UpdateUI()
    {
        if (heroStatusBar != null)
        {
            heroStatusBar.UpdateStatusBars();
        }
        if (monsterStatusBar != null)
        {
            monsterStatusBar.UpdateStatusBar();
        }
    }

    // 添加新方法来记录消息
    public void LogMessage(string message)
    {
        if (battleLog != null)
        {
            battleLog.AddMessage(message);
        }
        else
        {
            Debug.LogWarning("BattleLog is null, cannot log message: " + message);
        }
    }

    public void DisableButtons()
    {
        attackButton.interactable = false;
        skillButton.interactable = false;
        itemButton.interactable = false;
        defendButton.interactable = false;
    }

    public void EnableButtons()
    {
        attackButton.interactable = true;
        skillButton.interactable = true;
        itemButton.interactable = true;
        defendButton.interactable = true;
    }

    void OpenSkillSelection()
    {
        if (skillSelectionUI == null || hero == null) return;
        bool canUseUltimate = hero.rage >= hero.maxRage;
        skillSelectionUI.ShowSkillSelection(hero.skills, canUseUltimate);
    }

    void CloseSkillSelection()
    {
        if (skillSelectionUI != null)
            skillSelectionUI.HideSkillSelection();
    }

    // 添加这个方法到 BattleUI 类中
    public void SetBattleManager(BattleManager manager)
    {
        battleManager = manager;
    }

    public void UpdateSkillButtons(List<Skill> unlockedSkills)
    {
        if (skillSelectionUI != null)
        {
            skillSelectionUI.UpdateSkillButtons(unlockedSkills);
        }
        else
        {
            Debug.LogWarning("SkillSelectionUI is null in BattleUI");
        }
    }

    public void OnItemSelected(string item)
    {
        CloseItemSelection();
        if (battleManager.isHeroTurn)
        {
            battleManager.HeroTurn("Item", null, item);
            UpdateUI();
        }
    }

    public void UpdateHeroStatus()
    {
        if (heroStatusBar != null)
        {
            heroStatusBar.UpdateStatusBars();
        }
        else
        {
            Debug.LogWarning("HeroStatusBar is null in BattleUI");
        }
    }
}