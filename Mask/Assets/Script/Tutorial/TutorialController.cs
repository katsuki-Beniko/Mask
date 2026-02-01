using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TutorialController : MonoBehaviour
{
    public VisualElement ui;
    private Label _tips;

    private void OnEnable()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;

        _tips = ui.Q<Label>("tips");
    }

    // Update is called once per frame
    void Update()
    {
        if (_tips != null)
        {
            float pulseOpacity = (Mathf.Sin(Time.time * 2f) + 1f) / 2f;
            _tips.style.opacity = pulseOpacity;
        }

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            LoadNewPage("MainMenu");
        }
    }

    public void LoadNewPage(string targetName)
    {
        LoadingBridge.NextSceneName = targetName;
        SceneManager.LoadScene("Loading");
    }
}
