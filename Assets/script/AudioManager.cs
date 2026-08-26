using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Background Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private float musicVolume = 0.5f;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip boxDestroyClip;
    [SerializeField] private AudioClip boxRespawnClip;
    [SerializeField] private AudioClip battleWinClip;
    [SerializeField] private AudioClip battleLoseClip;
    [SerializeField] private AudioClip playerDeathClip;
    [SerializeField] private AudioClip buttonClickClip;

    [Header("SFX Settings")]
    [SerializeField] private float sfxVolume = 1f;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }

    void Start()
    {
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (backgroundMusic == null) return;
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayBoxDestroy() => PlaySFX(boxDestroyClip);
    public void PlayBoxRespawn() => PlaySFX(boxRespawnClip);
    public void PlayBattleWin() => PlaySFX(battleWinClip);
    public void PlayBattleLose() => PlaySFX(battleLoseClip);
    public void PlayPlayerDeath() => PlaySFX(playerDeathClip);
    public void PlayButtonClick() => PlaySFX(buttonClickClip);
}