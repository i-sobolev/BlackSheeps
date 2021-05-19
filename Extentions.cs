using UnityEngine;

public static class Extentions
{
    public static float EaseInOutQuad(this float x) => x < 0.5 ? 2 * x * x : 1 - (-2 * x + 2) * (-2 * x + 2) / 2;
    public static float EaseOutQuint(this float x) => 1f - Mathf.Pow(1 - x, 5);
    public static float EaseInQuint(this float x) => Mathf.Pow(x, 4);
}
