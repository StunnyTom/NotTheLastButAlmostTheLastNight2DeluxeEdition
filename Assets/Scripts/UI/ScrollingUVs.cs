using UnityEngine;
using UnityEngine.UI;

public class ScrollingUVs : MonoBehaviour
{
    public float speedX = 0.1f;
    public float speedY = 0.1f;

    private RawImage rawImage;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        if (rawImage != null)
        {
            Rect uvRect = rawImage.uvRect;
            uvRect.x += speedX * Time.deltaTime;
            uvRect.y += speedY * Time.deltaTime;
            rawImage.uvRect = uvRect;
        }
    }
}
