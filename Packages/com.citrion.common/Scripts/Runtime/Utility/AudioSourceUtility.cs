using System.Collections.Generic;
using UnityEngine;

namespace CitrioN.Common
{
  public static class AudioSourceUtility
  {
    private static List<AudioSource> playingAudioSources;
    private static List<float> playingAudioSourcesTimes;
    private static float cachedTime = -1;

    public static List<AudioSource> PlayingAudioSources
    {
      get
      {
        if (playingAudioSources == null)
        {
          playingAudioSources = new List<AudioSource>();
        }
        return playingAudioSources;
      }
    }

    public static List<float> PlayingAudioSourcesTimes
    {
      get
      {
        if (playingAudioSourcesTimes == null)
        {
          playingAudioSourcesTimes = new List<float>();
        }
        return playingAudioSourcesTimes;
      }
    }

    public static void PausePlayingAudioSources()
    {
      if (Time.time == cachedTime) { return; }
      cachedTime = Time.time;
      PlayingAudioSources.Clear();
      PlayingAudioSourcesTimes.Clear();

      AudioSource[] audioSources =
#if UNITY_2023_1_OR_NEWER
        Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
#else
        Object.FindObjectsOfType<AudioSource>();
#endif

      foreach (AudioSource audioSource in audioSources)
      {
        if (audioSource.isPlaying)
        {
          audioSource.Pause();
          PlayingAudioSources.Add(audioSource);
          PlayingAudioSourcesTimes.Add(audioSource.time);
        }
      }
    }

    public static void UnpausePlayingAudioSources()
    {
      var count = PlayingAudioSources.Count;
      for (int i = 0; i < count; i++)
      {
        var audioSource = PlayingAudioSources[i];
        audioSource.Play();
        audioSource.time = PlayingAudioSourcesTimes[i];
      }
    }
  }
}