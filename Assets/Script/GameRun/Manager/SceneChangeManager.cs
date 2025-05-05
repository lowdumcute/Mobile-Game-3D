using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager Instance;
    public GameObject loadingScreen;
    public Animator loadingAnimator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        loadingScreen.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingScreen.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        float loadStartTime = Time.time;
        bool isSceneReady = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                isSceneReady = true;
            }

            if (isSceneReady && Time.time - loadStartTime >= 2f) // Cho hiệu ứng ngắn hơn 2s
            {
                asyncLoad.allowSceneActivation = true;
                yield return new WaitForSeconds(0.5f);

                DynamicGI.UpdateEnvironment();
                loadingAnimator.SetTrigger("End");
                yield return new WaitForSeconds(1.0f);

                HideLoadingScreen();
            }

            yield return null;
        }
    }

    public void HideLoadingScreen()
    {
        loadingScreen.SetActive(false);
    }
}
