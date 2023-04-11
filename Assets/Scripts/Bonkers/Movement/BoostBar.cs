using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bonkers.Movement
{
    public class BoostBar : MonoBehaviour
    {
        public Slider slider;
        public Gradient gradient;
        public Image fill;

        public void SetBoost(float boost)
        {
            slider.value = boost;
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }

        public void SetMaxBoost(float boost)
        {
            slider.maxValue = boost;
            slider.value = boost;

            gradient.Evaluate(1f);
            fill.color = gradient.Evaluate(1f);
        }
    }

}
