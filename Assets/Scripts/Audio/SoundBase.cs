/*****************************************************************************
// File Name : SoundBase.cs
// Author : Brandon Koederitz
// Creation Date : 2/4/2026
// Last Modified : 2/4/2026
//
// Brief Description : Base class for all sound representing classes.
*****************************************************************************/
using UnityEngine;
using UnityEngine.Audio;

namespace FoodFlight
{
    [System.Serializable]
    public abstract class SoundBase
    {
        [SerializeField] protected string name;
        [SerializeField] protected AudioMixerGroup mixerGroup;
        [SerializeField, Range(0f, 1f)] protected float volume = 1f;
        [SerializeField, Range(-3f, 3f)] protected float pitch = 1f;
        [SerializeField] protected bool loop;

        protected AudioSource source;

        #region Properties
        public string Name => name;
        #endregion

        public abstract void Play();

        public virtual void Stop()
        {
            source.Stop();
        }

        /// <summary>
        /// Sets up this sound with an audio source.
        /// </summary>
        /// <param name="source">The audio source for this sound.</param>
        protected virtual void Setup(AudioSource source)
        {
            source.outputAudioMixerGroup = mixerGroup;
            source.volume = volume;
            source.pitch = pitch;
            source.loop = loop;
            source.playOnAwake = false;
        }


        /// <summary>
        /// Sets up all of the sounds on this audio manager with an audio source.
        /// </summary>
        public static void SetupSounds(SoundBase[] sounds, GameObject managerObject)
        {
            foreach (var sound in sounds)
            {
                //sound.source = managerObject.AddComponent<AudioSource>();

                //sound.source.outputAudioMixerGroup = sound.mixerGroup;
                //sound.source.volume = sound.volume;
                //sound.source.pitch = sound.pitch;
                //sound.source.loop = sound.loop;
                //sound.source.playOnAwake = false;
                sound.source = managerObject.AddComponent<AudioSource>();
                sound.Setup(sound.source);
            }
        }
    }
}
