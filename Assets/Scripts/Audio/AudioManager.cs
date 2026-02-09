/*****************************************************************************
// File Name : AudioManager.cs
// Author : Brandon Koederitz
// Creation Date : 10/3/2025
// Last Modified : 10/3/2025
//
// Brief Description : Allows for the mixing an playing of audio
*****************************************************************************/
using System;
using UnityEngine;
using UnityEngine.Audio;
using static Unity.VisualScripting.Member;

namespace FoodFlight
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private Sound[] sounds;
        [SerializeField] private RandomizedSound[] randomizedSounds;

        private static AudioManager instance;

        #region Nested
        [System.Serializable]
        public class Sound : SoundBase
        {
            [Header("Clip")]
            [SerializeField] protected AudioClip audioClip;

            protected override void Setup(AudioSource source)
            {
                base.Setup(source);
                source.clip = audioClip;
            }

            /// <summary>
            /// Plays the set audio clip of this sound.
            /// </summary>
            public override void Play()
            {
                //source.clip = audioClip;
                source.Play();
            }
        }
        // Plays a random sound from an array of sounds.
        [System.Serializable]
        private class RandomizedSound : SoundBase
        {
            [Header("Clip")]
            [SerializeField] internal AudioClip[] audioClips;

            /// <summary>
            /// Gets a random audio clip from the array and plays it.
            /// </summary>
            public override void Play()
            {
                AudioClip clip = audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
                source.clip = clip;
                source.Play();
            }
        }
        #endregion

        private void Awake()
        {

            // If an AudioManager already exists, then we should destroy this AudioManager and prevent any setup.
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                // Make the AudioManager DontDestroyOnLoad so that sounds can persist across scenes.
                DontDestroyOnLoad(gameObject);
                instance = this;
            }

            SoundBase.SetupSounds(sounds, gameObject);
            SoundBase.SetupSounds(randomizedSounds, gameObject);

            AudioRelay.RelayPlayEvent = Play;
            AudioRelay.RelayPlayRandomizedEvent = PlayRandom;
            AudioRelay.RelayStopEvent = Stop;
            AudioRelay.RelayStopAllEvent = StopAll;

        }

        /// <summary>
        /// When the Audio Manager is destroyed, reset all relay actions.
        /// </summary>
        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
                AudioRelay.RelayPlayEvent = null;
                AudioRelay.RelayPlayRandomizedEvent = null;
                AudioRelay.RelayStopEvent = null;
                AudioRelay.RelayStopAllEvent = null;
            }
        }

        /// <summary>
        /// Plays a given sound.
        /// </summary>
        /// <param name="soundName">The sound to play.</param>
        public void Play(string soundName)
        {
            SoundBase toPlay = Array.Find(sounds, item => item.Name == soundName);
            if (toPlay != null)
            {
                toPlay.Play();
            }
            else
            {
                Debug.LogWarning("There is no sound effect with the name " + soundName +
                    " registered in the AudioManager.");
            }
        }

        /// <summary>
        /// Plays a given randomized sound.
        /// </summary>
        /// <param name="soundName">The sound to play.</param>
        public void PlayRandom(string soundName)
        {
            SoundBase toPlay = Array.Find(randomizedSounds, item => item.Name == soundName);
            if (toPlay != null)
            {
                toPlay.Play();
            }
            else
            {
                Debug.LogWarning("There is no sound effect with the name " + soundName +
                    " registered in the AudioManager.");
            }
        }

        /// <summary>
        /// Stops a given sound.
        /// </summary>
        /// <param name="soundName">The sound to stop.</param>
        public void Stop(string soundName)
        {
            // Check both normal and randomized sounds.
            SoundBase toStop = Array.Find(sounds, item => item.Name == soundName);
            if (toStop != null)
            {
                toStop.Stop();
                return;
            }

            toStop = Array.Find(randomizedSounds, item => item.Name == soundName);
            if (toStop != null)
            {
                toStop.Stop();
                return;
            }
        }

        /// <summary>
        /// Stops all playing sounds.
        /// </summary>
        public void StopAll()
        {
            foreach (var sound in sounds)
            {
                sound.Stop();
            }
        }
    }
}
