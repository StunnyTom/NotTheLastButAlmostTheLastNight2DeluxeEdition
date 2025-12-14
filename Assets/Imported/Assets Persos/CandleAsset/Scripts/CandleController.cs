using UnityEngine;

public class CandleController : MonoBehaviour
{
    [SerializeField] GameObject flameVfx;
    [SerializeField] Light flameLight;

    bool isLit = true;
    public bool IsLit => isLit;

    void Start()
    {
        ApplyState();
    }

    public void LightOn()
    {
        isLit = true;
        ApplyState();
    }

    public void LightOff()
    {
        isLit = false;
        ApplyState();
    }

    public void Toggle()
    {
        isLit = !isLit;
        ApplyState();
    }

    public void SetLit(bool lit)
    {
        isLit = lit;
        ApplyState();
    }

    void ApplyState()
    {
        if (flameVfx) flameVfx.SetActive(isLit);
        if (flameLight) flameLight.enabled = isLit;
    }
}
