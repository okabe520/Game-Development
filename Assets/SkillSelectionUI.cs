using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillSelectionUI : MonoBehaviour
{
    public GameObject skillButtonPrefab;
    public Transform skillButtonContainer;
    public BattleUI battleUI;

    private List<Button> skillButtons = new List<Button>();

    void Start()
    {
        if (battleUI == null)
        {
            battleUI = FindObjectOfType<BattleUI>();
            if (battleUI == null)
            {
                Debug.LogError("BattleUI not found in the scene!");
            }
        }

        // 初始时隐藏技能选择面板
        gameObject.SetActive(false);
    }

    public void ShowSkillSelection(List<Skill> skills, bool canUseUltimate)
    {
        ClearSkillButtons();

        foreach (Skill skill in skills)
        {
            if (skill.isUnlocked)
            {
                CreateSkillButton(skill);
            }
        }

        // 显示技能选择面板
        gameObject.SetActive(true);
    }

    private void CreateSkillButton(Skill skill)
    {
        GameObject buttonObj = Instantiate(skillButtonPrefab, skillButtonContainer);
        Button button = buttonObj.GetComponent<Button>();
        if (button == null) return;
        Text buttonText = buttonObj.GetComponentInChildren<Text>();

        if (buttonText != null)
        {
            buttonText.text = skill.isUltimate ? $"{skill.name} (终结技)" : $"{skill.name} (MP: {skill.manaCost})";
        }
        else
        {
            Debug.LogWarning("Text component not found in skill button prefab");
        }

        button.onClick.AddListener(() => OnSkillSelected(skill));

        skillButtons.Add(button);
    }

    void OnDestroy()
    {
        ClearSkillButtons();
    }

    private void OnSkillSelected(Skill skill)
    {
        if (battleUI != null)
        {
            battleUI.OnSkillSelected(skill);
            gameObject.SetActive(false); // 选择技能后隐藏面板
        }
        else
        {
            Debug.LogError("BattleUI reference is null in SkillSelectionUI");
        }
    }

    private void ClearSkillButtons()
    {
        foreach (Button button in skillButtons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }
        skillButtons.Clear();
    }

    public void HideSkillSelection()
    {
        gameObject.SetActive(false);
    }

    public void UpdateSkillButtons(List<Skill> unlockedSkills)
    {
        ClearSkillButtons();

        // 只为已解锁的技能创建按钮
        foreach (Skill skill in unlockedSkills)
        {
            CreateSkillButton(skill);
        }
    }
}