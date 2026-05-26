using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemSelectionUI : MonoBehaviour
{
    public GameObject itemButtonPrefab;
    public Transform itemButtonContainer;
    public BattleUI battleUI;

    private List<Button> itemButtons = new List<Button>();

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

        // 初始时隐藏道具选择面板
        gameObject.SetActive(false);
    }

    public void ShowItemSelection(List<string> items)
    {
        ClearItemButtons();

        foreach (string item in items)
        {
            CreateItemButton(item);
        }

        // 显示道具选择面板
        gameObject.SetActive(true);
    }

    private void CreateItemButton(string item)
    {
        GameObject buttonObj = Instantiate(itemButtonPrefab, itemButtonContainer);
        Button button = buttonObj.GetComponent<Button>();
        if (button == null) return;
        Text buttonText = buttonObj.GetComponentInChildren<Text>();

        if (buttonText != null)
        {
            buttonText.text = item;
        }
        else
        {
            Debug.LogWarning("Text component not found in item button prefab");
        }

        button.onClick.AddListener(() => OnItemSelected(item));

        itemButtons.Add(button);
    }

    void OnDestroy()
    {
        ClearItemButtons();
    }

    private void OnItemSelected(string item)
    {
        if (battleUI != null)
        {
            battleUI.OnItemSelected(item);
            gameObject.SetActive(false); // 选择道具后隐藏面板
        }
        else
        {
            Debug.LogError("BattleUI reference is null in ItemSelectionUI");
        }
    }

    private void ClearItemButtons()
    {
        foreach (Button button in itemButtons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }
        itemButtons.Clear();
    }

    public void HideItemSelection()
    {
        gameObject.SetActive(false);
    }
}