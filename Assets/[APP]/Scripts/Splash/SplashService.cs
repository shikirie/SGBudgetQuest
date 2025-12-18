using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

public class SplashService : IStartable
{
    void IStartable.Start()
    {
        Application.targetFrameRate = 60;
        string targetScene = "MainMenu";

        CoroutineRunner.Run(LoadScene(targetScene));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            yield return null;

            if (async.progress >= 0.9f)
            {
                async.allowSceneActivation = true;
            }
        }

        yield return new WaitForSeconds(1);
    }
}
