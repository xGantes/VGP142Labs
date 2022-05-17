using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace VGP142.PlayerInputs
{
    public class CanvasManager : MonoBehaviour
    {
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
        public GameObject pauseMenu;
        public GameObject settingMenu;

        [Header("Text")]
        public Text liveText;
        public Text healthText;
        public Text staminaText;
        public Text sliderText;

        [Header("Slider")]
        public Slider volSlide;
        public Slider healthSlider;
        public Slider staminaSlider;

        public GameManager playerHealth;
        public Image imageFill;

        private void Awake()
        {
            healthSlider = GetComponent<Slider>();
            staminaSlider = GetComponent<Slider>();
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
            if (volSlide && sliderText)
            {
                volSlide.onValueChanged.AddListener((value) => OnSliderValueChange(value));
                sliderText.text = volSlide.value.ToString();
            }
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

        void Update()
        {
            if (pauseMenu)
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    pauseMenu.SetActive(!pauseMenu.activeSelf);
                    if (pauseMenu.activeSelf)
                    {
                        Time.timeScale = 0f;
                    }
                    else
                    {
                        Time.timeScale = 1f;
                    }
                }
            }
        }
    }
}