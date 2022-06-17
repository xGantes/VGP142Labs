using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGP142.PlayerInputs
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance;

        public Sound[] sounds;

        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }

            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.outputAudioMixerGroup = s.mixer;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }
        }

        public void Play(string sound)
        {
            Sound s = Array.Find(sounds, item => item.name == sound);
            s.source.Play();
        }
        public void Stop(string sound)
        {
            Sound s = Array.Find(sounds, item => item.name == sound);
            s.source.Stop();
        }
    }
}

