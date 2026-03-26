#nullable enable
using PrimeTween;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    UIDocument _uiDocument;

    private VisualElement _container;

    private void Awake()
    {
        if (!_uiDocument)
        {
            Debug.LogWarning("UI Document not assigned to main menu script!");
            return;
        }

        var root = _uiDocument.rootVisualElement;

        var container = root.Q("container");
        var playButton = root.Q("play-button");
        var quitButton = root.Q("quit-button");

        if (container == null || playButton == null || quitButton == null)
        {
            Debug.Log("Could not find container, or play button, or quit button!");
            return;
        }

        _container = container;

        playButton.RegisterCallback<MouseUpEvent>(HandlePlayButtonPressed);
        quitButton.RegisterCallback<MouseUpEvent>(HandleQuitButtonPressed);

        if (Application.platform == RuntimePlatform.WebGLPlayer)
            quitButton.style.display = DisplayStyle.None;

        Events.OnShowMenu.AddListener(() => ShowMenu());
    }

    private void HandlePlayButtonPressed(MouseUpEvent evt)
    {
        Events.OnGameStart.Invoke();

        HideMenu();
    }

    private void HandleQuitButtonPressed(MouseUpEvent evt)
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private Tween HideMenu()
    {
        return Tween.Custom(1f, 0f, 0.5f, (val) => _container.style.opacity = val);
    }

    private Tween ShowMenu()
    {
        return Tween.Custom(0f, 1f, 0.5f, (val) => _container.style.opacity = val);
    }
}
