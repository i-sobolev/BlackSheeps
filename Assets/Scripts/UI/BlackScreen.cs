using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BlackScreen : MonoBehaviour
{
    public Image BlackScreenImage = null;
    public static BlackScreen Instance { private set; get; }
    public bool IsAwakeOnStart;

    private void Awake()
    {
        Instance = this;

    }

    private void Start()
    {
        if (IsAwakeOnStart)
        {
            BlackScreenImage.color = new Color(0, 0, 0, 1);
            Hide();
        }
    }

    public void ShowAndLoadMainMenu()
    {
        StartCoroutine(ScreenAnimation(0, 1, "Menu"));
    }

    public void ShowAndPlayAnimatic()
    {
        StartCoroutine(ScreenAnimation(0, 1, "End"));
    }

    public void ShowAndLoadMainScene()
    {
        StartCoroutine(ScreenAnimation(0, 1, "Main"));
    }

    public void Hide()
    {
        StartCoroutine(ScreenAnimation(1, 0));
    }

    public void GameEnd()
    {
        StartCoroutine(ScreenAnimation(1, 1, "Menu"));
    }

    private IEnumerator ScreenAnimation(float startAlpha, float endAlpha, string sceneName = null)
    {
        float lerp = 0;

        BlackScreenImage.color = new Color(0, 0, 0, startAlpha);

        while (lerp < 1)
        {
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, lerp.EaseInOutQuad());
            
            BlackScreenImage.color = new Color(0, 0, 0, newAlpha);
            lerp += Time.deltaTime / 2;

            yield return null;
        }

        BlackScreenImage.color = new Color(0, 0, 0, endAlpha);

        if (sceneName != null)
            SceneManager.LoadScene(sceneName);
    }
}
