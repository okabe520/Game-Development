using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public Transform player;
    public Transform[] battleTriggerPoints;
    public float battleTriggerRadius = 1f;

    void Update()
    {
        CheckBattleTriggers();
    }

    void CheckBattleTriggers()
    {
        foreach (Transform triggerPoint in battleTriggerPoints)
        {
            if (Vector2.Distance(player.position, triggerPoint.position) < battleTriggerRadius)
            {
                StartBattle();
                break;
            }
        }
    }

    void StartBattle()
    {
        // 可以在这里保存玩家位置
        PlayerPrefs.SetFloat("PlayerPosX", player.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", player.position.y);

        SceneManager.LoadScene("BattleScene");
    }
}
