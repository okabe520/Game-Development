using UnityEngine;
using UnityEngine.UI;

public class MonsterStatusBar : MonoBehaviour
{
    public Slider healthSlider;
    public Text healthText;
    public Monster monster;

    private void Start()
    {
        if (monster == null)
        {
            monster = FindObjectOfType<Monster>();
            if (monster == null)
            {
                Debug.LogError("Monster not found in the scene!");
                return;
            }
        }

        // 延迟初始化，确保Monster的Start方法已经执行
        Invoke("InitializeHealth", 0.1f);
    }

    public void InitializeHealth()
    {
        if (monster != null)
        {
            healthSlider.maxValue = monster.maxHealth;
            UpdateStatusBar();
        }
        else
        {
            Debug.LogError("Monster reference is null in MonsterStatusBar!");
        }
    }

    public void UpdateStatusBar()
    {
        if (monster == null)
        {
            Debug.LogWarning("Monster reference is null in MonsterStatusBar!");
            return;
        }

        healthSlider.maxValue = monster.maxHealth;
        healthSlider.value = Mathf.Clamp(monster.health, 0, monster.maxHealth);

        if (healthText != null)
        {
            healthText.text = $"{monster.health:F0}/{monster.maxHealth:F0}";
        }

    }
}