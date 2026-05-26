using UnityEngine;

public class BattleBGM : MonoBehaviour
{
    public AudioClip bgm;
    private AudioSource bgmSource;

    void Start()
    {
        bgmSource = GetComponent<AudioSource>();
        if (bgmSource == null) return;
        bgmSource.clip = bgm;
        bgmSource.loop = true;
        bgmSource.volume = 1.0f;
        bgmSource.Play();
    }

    void Update()
    {
        if (bgmSource == null) return;
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (bgmSource.isPlaying)
                bgmSource.Pause();
            else
                bgmSource.UnPause();
        }
    }
}

