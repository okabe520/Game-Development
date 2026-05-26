using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattleLog : MonoBehaviour
{
    public Text logText;
    public float messageDisplayTime = 1f;
    public int maxCharacters = 100;

    private Queue<string> messageQueue = new Queue<string>();
    private bool isDisplaying = false;
    public BattleUI battleUI;

    private void Start()
    {
        if (logText == null)
        {
            Debug.LogError("Text component not assigned in BattleLog!");
            return;
        }
        logText.text = "";

        logText.horizontalOverflow = HorizontalWrapMode.Wrap;
        logText.verticalOverflow = VerticalWrapMode.Overflow;

        // 确保 BattleUI 被正确引用
        if (battleUI == null)
        {
            battleUI = FindObjectOfType<BattleUI>();
            if (battleUI == null)
            {
                Debug.LogError("BattleUI not found in the scene!");
            }
        }
    }

    public void AddMessage(string message)
    {
        if (maxCharacters > 0 && message.Length > maxCharacters)
        {
            message = message.Substring(0, maxCharacters) + "...";
        }

        messageQueue.Enqueue(message);
        if (!isDisplaying)
        {
            DisplayNextMessage();
        }
    }

    private void DisplayNextMessage()
    {
        if (messageQueue.Count > 0)
        {
            isDisplaying = true;
            if (battleUI != null) battleUI.DisableButtons();

            string message = messageQueue.Dequeue();
            logText.text = message;

            StartCoroutine(WaitAndDisplayNext());
        }
        else
        {
            isDisplaying = false;
            if (battleUI != null) battleUI.EnableButtons();
        }
    }

    private IEnumerator WaitAndDisplayNext()
    {
        yield return new WaitForSeconds(messageDisplayTime);
        DisplayNextMessage();
    }

    public void ClearLog()
    {
        logText.text = "";
        messageQueue.Clear();
        isDisplaying = false;
    }
}