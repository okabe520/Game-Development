using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("主面板")]
    public GameObject mainPanel;
    public Button newGameButton;
    public Button continueButton;
    public Button loadGameButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("存档面板")]
    public GameObject loadPanel;
    public Button[] slotButtons;
    public Text[] slotLabels;
    public Button loadPanelBackButton;

    [Header("设置面板")]
    public GameObject settingsPanel;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Button settingsBackButton;

    void Start()
    {
        // 初始：显示主面板
        ShowMainPanel();

        // 主面板按钮
        if (newGameButton != null) newGameButton.onClick.AddListener(OnNewGame);
        if (continueButton != null) continueButton.onClick.AddListener(OnContinue);
        if (loadGameButton != null) loadGameButton.onClick.AddListener(ShowLoadPanel);
        if (settingsButton != null) settingsButton.onClick.AddListener(ShowSettingsPanel);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuit);

        // 存档面板
        if (loadPanelBackButton != null)
            loadPanelBackButton.onClick.AddListener(ShowMainPanel);
        if (slotButtons != null)
        {
            for (int i = 0; i < slotButtons.Length; i++)
            {
                int slot = i;
                if (slotButtons[i] != null)
                    slotButtons[i].onClick.AddListener(() => OnLoadSlot(slot));
            }
        }

        // 设置面板
        if (settingsBackButton != null)
            settingsBackButton.onClick.AddListener(ShowMainPanel);
        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
            bgmSlider.value = 1f;
        }
        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            sfxSlider.value = 1f;
        }

        // 刷新存档列表
        RefreshSlotDisplay();
    }

    void ShowMainPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (loadPanel != null) loadPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        bool hasAnySave = false;
        for (int i = 0; i < 3; i++)
        {
            if (SaveManager.Instance != null && SaveManager.Instance.HasSaveInSlot(i))
            {
                hasAnySave = true;
                break;
            }
        }
        if (continueButton != null) continueButton.interactable = hasAnySave;
    }

    void ShowLoadPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (loadPanel != null) loadPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        RefreshSlotDisplay();
    }

    void ShowSettingsPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (loadPanel != null) loadPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    void OnNewGame()
    {
        Debug.Log("[MainMenu] 新游戏按钮被点击");
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.NewGame();
            Debug.Log("[MainMenu] SaveManager.NewGame() 已调用");
        }
        else
        {
            Debug.LogWarning("[MainMenu] SaveManager.Instance 为 null，跳过存档初始化");
        }
        SceneManager.LoadScene("MapScene");
    }

    void OnContinue()
    {
        // 加载最新的存档
        for (int i = 2; i >= 0; i--)
        {
            if (SaveManager.Instance != null && SaveManager.Instance.HasSaveInSlot(i))
            {
                SaveManager.Instance.LoadGame(i);
                SceneManager.LoadScene("MapScene");
                return;
            }
        }
    }

    void OnLoadSlot(int slot)
    {
        if (SaveManager.Instance != null && SaveManager.Instance.HasSaveInSlot(slot))
        {
            SaveManager.Instance.LoadGame(slot);
            SceneManager.LoadScene("MapScene");
        }
    }

    void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void OnBGMVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetBGMVolume(value);
    }

    void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
    }

    void RefreshSlotDisplay()
    {
        if (SaveManager.Instance == null) return;
        if (slotLabels == null || slotButtons == null) return;

        for (int i = 0; i < slotLabels.Length && i < slotButtons.Length && i < 3; i++)
        {
            if (slotLabels[i] == null || slotButtons[i] == null) continue;

            if (SaveManager.Instance.HasSaveInSlot(i))
            {
                var info = SaveManager.Instance.GetSaveInfo(i);
                if (info != null)
                {
                    slotLabels[i].text = $"存档 {i + 1}\nLv.{info.level}  Boss×{info.bossesDefeated}\n{info.saveTimestamp}";
                    slotButtons[i].interactable = true;
                }
            }
            else
            {
                slotLabels[i].text = $"存档 {i + 1}\n— 空 —";
                slotButtons[i].interactable = false;
            }
        }
    }
}
