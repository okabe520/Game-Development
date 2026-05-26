using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    public static GameInitializer Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeGame()
    {
        // 初始化游戏状态、加载配置等
        Debug.Log("Initializing game...");

        // 创建并初始化 SceneLoader
        GameObject sceneLoaderObj = new GameObject("SceneLoader");
        sceneLoaderObj.AddComponent<SceneLoader>();
        DontDestroyOnLoad(sceneLoaderObj);

        // 加载主菜单或地图场景
        SceneManager.LoadScene("MapScene");
    }
}
