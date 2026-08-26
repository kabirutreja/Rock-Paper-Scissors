using UnityEngine;

namespace QuantumTek.SimpleMenu
{
    /// <summary> A script that will play audio clips using the given audio source. </summary>
    [AddComponentMenu("Quantum Tek/Simple Menu/Audio")]
    [DisallowMultipleComponent]
    public class SM_Audio : MonoBehaviour
    {
        [Header("Object References")]
        [Tooltip("A reference to the audio source that will play the sounds.")]
        [SerializeField] protected AudioSource source;
        [Tooltip("A reference to the default audio clip that will be played.")]
        [SerializeField] protected AudioClip clip;

        /// <summary> Plays the default audio. </summary>
        public void PlayAudio()
        { if (source && clip) source.PlayOneShot(clip); }
        /// <summary> Plays the given audio. </summary>
        /// <param name="pClip">The clip to play.</param>
        public void PlayAudio(AudioClip pClip)
        { if (source && pClip) source.PlayOneShot(pClip); }
    }
}