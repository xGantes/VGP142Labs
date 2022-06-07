using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace VGP142.PlayerInputs
{
    public class CanvasManager : MonoBehaviour
    {
        private MainPlayerInputs input;
        private Player player;

        [Header("Buttons")]
        public Button startButton;
        public Button exitButton;
        public Button settingButton;
        public Button returnButton;
        public Button returnToGame;
        public Button returnToMenu;
        public Button tryAgainButton;

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
            //if (volSlide && sliderText)
            //{
            //    volSlide.onValueChanged.AddListener((value) => OnSliderValueChange(value));
            //    sliderText.text = volSlide.value.ToString();
            //}
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

            //resumeMenu.SetActive(true);
            //if (resumeMenu)
            //{
            //    if (input.pause)
            //    {
            //        Debug.Log("Pause");
            //        resumeMenu.gameObject.SetActive(true);

            //        resumeMenu.SetActive(!resumeMenu.activeSelf);
            //        if (resumeMenu.activeSelf)
            //        {
            //            resumeMenu.SetActive(true);
            //            Time.timeScale = 0f;
            //        }
            //        else
            //        {
            //            Time.timeScale = 1f;
            //        }
            //    }
            //}

        private void resumeGame()
        {
            resumeMenu.gameObject.SetActive(false);
            //Time.timeScale = 1.0f;
        }
    }
}