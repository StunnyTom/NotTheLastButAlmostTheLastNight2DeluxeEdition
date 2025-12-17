using UnityEngine;

public class SimpleProgressBar : MonoBehaviour
{
    [SerializeField] RectTransform fill;
    [SerializeField] float maxWidth = 200f;

    /// progress entre 0 et 1
    public void SetProgress(float progress01)
    {
        progress01 = Mathf.Clamp01(progress01);

        Vector2 size = fill.sizeDelta;
        size.x = maxWidth * progress01;
        fill.sizeDelta = size;
    }

    public void ResetBar()
    {
        SetProgress(0f);
    }

    public void Full()
    {
        SetProgress(1f);
    }
}
