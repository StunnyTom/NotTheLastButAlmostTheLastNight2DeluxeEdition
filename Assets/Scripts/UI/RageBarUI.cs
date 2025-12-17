using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RageBarUI : MonoBehaviour
    {
        [Header("UI Components")]
        public Image fillImage;
        public Gradient colorGradient; // Optional: Change color as it fills

        public void UpdateBar(float current, float max)
        {
            if (fillImage != null)
            {
                float ratio = Mathf.Clamp01(current / max);
                fillImage.fillAmount = ratio;

                if (colorGradient != null)
                {
                    fillImage.color = colorGradient.Evaluate(ratio);
                }
            }
        }
    }
}
