using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private Button buttonPlay;

    public Action OnButtonPlayClicked;

    private void Awake()
    {
        buttonPlay.onClick.AddListener(OnPlayButtonClicked);
    }

    private void OnDestroy()
    {
        buttonPlay.onClick.RemoveListener(OnPlayButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        OnButtonPlayClicked?.Invoke();
    }
}
