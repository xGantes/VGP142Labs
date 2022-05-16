using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VGP142.PlayerInputs
{
    public class HealthSlider : MonoBehaviour
    {
        public Player playerHealth;
        public Image fillImage;
        private Slider slider;

        private void Awake()
        {
            slider = GetComponent<Slider>();
        }

        private void Update()
        {
            if (slider.value <= slider.minValue)
            {
                fillImage.enabled = false;
            }
            if (slider.value > slider.minValue && !fillImage.enabled)
            {
                fillImage.enabled = true;
            }
            float fillValue = playerHealth.currentHealth;
            slider.value = fillValue;
        }
    }
}
