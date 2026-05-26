using UnityEngine;
using UnityEngine.UI;

public class HeroStatusBar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider manaSlider;
    public Slider rageSlider;
    public Slider xpSlider;

    public Text healthText;
    public Text manaText;
    public Text rageText;
    public Text levelText;
    public Text xpText;

    public Hero hero;

    private void Start()
    {
        if (hero == null)
        {
            hero = FindObjectOfType<Hero>();
            if (hero == null)
            {
                Debug.LogError("Hero not found in the scene!");
                return;
            }
        }

        hero.OnExperienceChanged += OnXPChanged;
        hero.OnLevelUp += OnLevelChanged;
        Invoke("InitializeStatusBars", 0.1f);
    }

    void OnDestroy()
    {
        if (hero != null)
        {
            hero.OnExperienceChanged -= OnXPChanged;
            hero.OnLevelUp -= OnLevelChanged;
        }
    }

    public void InitializeStatusBars()
    {
        if (hero != null)
        {
            healthSlider.maxValue = hero.maxHealth;
            manaSlider.maxValue = hero.maxMana;
            rageSlider.maxValue = hero.maxRage;
            UpdateStatusBars();
        }
    }

    public void UpdateStatusBars()
    {
        if (hero == null) return;

        healthSlider.maxValue = hero.maxHealth;
        manaSlider.maxValue = hero.maxMana;
        rageSlider.maxValue = hero.maxRage;

        healthSlider.value = hero.health;
        manaSlider.value = hero.mana;
        rageSlider.value = hero.rage;

        if (healthText != null)
            healthText.text = $"{hero.health:F0}/{hero.maxHealth:F0}";
        if (manaText != null)
            manaText.text = $"{hero.mana:F0}/{hero.maxMana:F0}";
        if (rageText != null)
            rageText.text = $"{hero.rage:F0}/{hero.maxRage:F0}";
        if (levelText != null)
            levelText.text = $"Lv.{hero.level}";
    }

    private void OnXPChanged(int xp)
    {
        if (xpSlider != null && xpText != null)
        {
            // HeroProgression 在运行时可获取
            var progression = Resources.Load<HeroProgression>("HeroProgression");
            int currentLevelXp = 0;
            int nextLevelXp = 100;
            if (progression != null)
            {
                currentLevelXp = progression.GetXPRequiredForLevel(hero.level);
                nextLevelXp = progression.GetXPRequiredForLevel(hero.level + 1);
            }

            xpSlider.minValue = currentLevelXp;
            xpSlider.maxValue = nextLevelXp;
            xpSlider.value = hero.experience;
            xpText.text = $"XP: {hero.experience}/{nextLevelXp}";
        }
    }

    private void OnLevelChanged(int newLevel, System.Collections.Generic.List<string> newSkills)
    {
        UpdateStatusBars();
        OnXPChanged(hero.experience);
    }
}
