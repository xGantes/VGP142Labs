using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VGP142.PlayerInputs
{
    public class BgMusic : MonoBehaviour
    {
        public static BgMusic instance;

        public Sound[] sounds;

        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
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
        void Start()
        {
            Play("MenuBG");
        }

        private void Update()
        {
            // Create a temporary reference to the current scene.
            Scene currentScene = SceneManager.GetActiveScene();

            // Retrieve the name of this scene.
            string sceneName = currentScene.name;

            if (sceneName == "Playground")
            {
                Play("Mysterious");
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

