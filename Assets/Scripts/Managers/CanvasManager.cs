using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

namespace VGP142.PlayerInputs
{
    public class CanvasManager : MonoBehaviour
    {
        private MainPlayerInputs input;
        private Player player;

        [Header("Buttons")]
        public Button startButton;
        public Button returnButton;
        public Button tryAgainButton;
        public Button settingButton;
        public Button returnToGame;

        [Header("Menus")]
        public GameObject mainMenu;
        public GameObject settingMenu;

        [Header("Text")]
        public Text liveText;
        public Text healthText;
        public Text staminaText;
        public Text sliderText;

        [Header("Resume HUD")]
        public GameObject resumeMenu;
        private bool togglePanel;
        //public GameManager playerHealth;

        [Header("Volume")]
        public Slider volSlider;
        public AudioMixer mixer;
        private float value;

        private void Awake()
        {
            input = GetComponent<MainPlayerInputs>();

        }

        void Start()
        {
            if (settingButton)
            {
                settingButton.onClick.AddListener(() => showSetMenu());
            }
            if (returnButton)
            {
                returnButton.onClick.AddListener(() => showMainMenu());
            }
            if (startButton)
            {
                startButton.onClick.AddListener(() => startGame());
            }
            if (tryAgainButton)
            {
                tryAgainButton.onClick.AddListener(() => tryAgain());
            }
            if (returnToGame)
            {
                returnToGame.onClick.AddListener(() => resumeGame());
            }

            mixer.GetFloat("volume", out value);
            volSlider.value = value;
        }

        public void showMainMenu()
        {
            settingMenu.SetActive(false);
            mainMenu.SetActive(true);
        }

        public void showSetMenu()
        {
            settingMenu.SetActive(true);
            mainMenu.SetActive(false);
        }
        public void startGame()
        {
            SceneManager.LoadScene("Playground");
        }
        public void tryAgain()
        {
            SceneManager.LoadScene("MainMenuScene");
        }
        void OnSliderValueChange(float value)
        {
            sliderText.text = value.ToString();
        }

        public void resumeGame()
        {
            //Debug.Log("resume");
            resumeMenu.SetActive(false);
            Time.timeScale = 1.0f;
        }

        public void SetVolume()
        {
            mixer.SetFloat("volume", volSlider.value);
        }
    }
}