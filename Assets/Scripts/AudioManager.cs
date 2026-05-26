using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("BGM")]
    [SerializeField] private AudioClip battleBGM;
    [SerializeField] private AudioClip victoryFanfare;
    [SerializeField] private AudioClip defeatFanfare;

    [Header("SFX")]
    [SerializeField] private AudioClip attackSFX;
    [SerializeField] private AudioClip skillSFX;
    [SerializeField] private AudioClip hitSFX;
    [SerializeField] private AudioClip defendSFX;
    [SerializeField] private AudioClip healSFX;
    [SerializeField] private AudioClip itemSFX;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeAudioSources()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = bgmVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource.clip == clip) return;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PauseBGM()
    {
        bgmSource.Pause();
    }

    public void ResumeBGM()
    {
        bgmSource.UnPause();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayVictory()
    {
        StopBGM();
        PlaySFX(victoryFanfare);
    }

    public void PlayDefeat()
    {
        StopBGM();
        PlaySFX(defeatFanfare);
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }

}
