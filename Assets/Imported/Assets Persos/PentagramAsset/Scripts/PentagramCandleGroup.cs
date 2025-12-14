using System;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class PentagramCandleGroup : MonoBehaviour
{
    [Header("Candles (size must be 5)")]
    [SerializeField] private CandleController[] candles = new CandleController[5];

    [Header("Events")]
    public UnityEvent OnAllLit;
    public UnityEvent OnAllExtinguished;

    public int Count => candles?.Length ?? 0;

    /// <summary>Returns candle by index [0..4], or null if invalid/missing.</summary>
    public CandleController GetCandle(int index)
    {
        if (candles == null || index < 0 || index >= candles.Length) return null;
        return candles[index];
    }

    /// <summary>Link/replace a candle at index [0..4].</summary>
    public void SetCandle(int index, CandleController candle)
    {
        if (candles == null || index < 0 || index >= candles.Length) return;
        candles[index] = candle;
    }

    /// <summary>Turn all candles on.</summary>
    public void LightAll()
    {
        ForEachCandle(static c => c.LightOn());
        OnAllLit?.Invoke();
    }

    /// <summary>Turn all candles off.</summary>
    public void ExtinguishAll()
    {
        ForEachCandle(static c => c.LightOff());
        OnAllExtinguished?.Invoke();
    }

    /// <summary>Toggle all candles (each one toggles individually).</summary>
    public void ToggleAll()
    {
        ForEachCandle(static c => c.Toggle());
    }

    /// <summary>Turn a single candle on/off by index.</summary>
    public void SetLit(int index, bool lit)
    {
        var c = GetCandle(index);
        if (c == null) return;
        c.SetLit(lit);
    }

    /// <summary>Toggle a single candle by index.</summary>
    public void Toggle(int index)
    {
        var c = GetCandle(index);
        if (c == null) return;
        c.Toggle();
    }

    /// <summary>
    /// Apply a 5-bit pattern (indices 0..4). Example: [true,false,true,false,true]
    /// </summary>
    public void SetPattern(bool[] pattern)
    {
        if (pattern == null) return;

        int n = Mathf.Min(pattern.Length, candles.Length);
        for (int i = 0; i < n; i++)
        {
            var c = candles[i];
            if (c == null) continue;
            c.SetLit(pattern[i]);
        }
    }

    /// <summary>
    /// Apply a bitmask (bit0->candle0 ... bit4->candle4). Example: mask 0b10101 lights 0,2,4
    /// </summary>
    public void SetMask(int mask)
    {
        for (int i = 0; i < candles.Length; i++)
        {
            var c = candles[i];
            if (c == null) continue;

            bool lit = (mask & (1 << i)) != 0;
            c.SetLit(lit);
        }
    }

    /// <summary>Returns a bitmask of current state (if CandleController exposes IsLit).</summary>
    public int GetMask()
    {
        int mask = 0;
        for (int i = 0; i < candles.Length; i++)
        {
            var c = candles[i];
            if (c == null) continue;

            if (c.IsLit) mask |= (1 << i);
        }
        return mask;
    }

    private void ForEachCandle(Action<CandleController> action)
    {
        if (candles == null || action == null) return;

        for (int i = 0; i < candles.Length; i++)
        {
            var c = candles[i];
            if (c == null) continue;
            action(c);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Enforce size = 5 for a pentagram pack
        if (candles == null || candles.Length != 5)
            Array.Resize(ref candles, 5);
    }
#endif
}
