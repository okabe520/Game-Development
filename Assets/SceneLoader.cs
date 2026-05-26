using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "InitScene")
        {
            // 在初始场景中不执行任何操作
            return;
        }

        if (scene.name == "BattleScene")
        {
            BattleManager.Instance.InitializeBattle();
        }
        else if (scene.name == "MapScene")
        {
            // 可以在这里初始化地图场景
            Debug.Log("Map scene loaded");
        }
    }
}
