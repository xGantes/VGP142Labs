using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace VGP142.PlayerInputs
{
    public class EnemyHealthBar : MonoBehaviour
    {
        public Enemy enemyHealth;
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

            float fillValue = enemyHealth.currentHealth;
            slider.value = fillValue;
        }
    }
}

