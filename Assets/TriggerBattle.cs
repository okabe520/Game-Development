using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerBattle : MonoBehaviour
{
    public string battleSceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(battleSceneName);
        }
    }
}
