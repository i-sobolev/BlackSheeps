using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extentions
{
    private static bool _logging = true;

    public static void LogValue(this MonoBehaviour sender, string message)
    {
        if (_logging)
            Debug.Log($"[{sender.GetType()}]: {message}"); 
    }

    public static Coroutine Start(this IEnumerator x, MonoBehaviour sender) => sender.StartCoroutine(x);

    public static float EaseInOutQuad(this float x) => x < 0.5 ? 2 * x * x : 1 - (-2 * x + 2) * (-2 * x + 2) / 2;
    public static float EaseOutQuint(this float x) => 1f - Mathf.Pow(1 - x, 5);
    public static float EaseInQuint(this float x) => Mathf.Pow(x, 4);
}
