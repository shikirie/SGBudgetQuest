using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;
using System.Collections;

public class MainMenuService : IInitializable, IDisposable
{
    // [Inject] private readonly ProjectSavingSystem SavingSystem;

    private readonly MainMenuView view;

    public MainMenuService(MainMenuView view)
    {
        this.view = view;
    }

    void IInitializable.Initialize()
    {
        if (view != null)
        {
            view.OnButtonPlayClicked += HandleButtonPlayClicked;
        }
    }

    void IDisposable.Dispose()
    {
        if (view != null)
        {
            view.OnButtonPlayClicked -= HandleButtonPlayClicked;
        }
    }

    private IEnumerator LoadSceneGame()
    {
        var async = SceneManager.LoadSceneAsync("Gameplay");
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            yield return null;

            if (async.progress >= 0.9f)
            {
                async.allowSceneActivation = true;
            }
        }

        float end = Time.realtimeSinceStartup + 1f;
        while (Time.realtimeSinceStartup < end)
        {
            yield return null;
        }
    }

    private void HandleButtonPlayClicked()
    {
        if (view == null) return;

        view.StartCoroutine(LoadSceneGame());
    }
}