using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Credits : MonoBehaviour
{
    public VisualElement ui;
    private Label _tips;


    private VisualElement _page1, _page2, _page3, _page4;
    private Button _prevBtn, _nextBtn;

    private int currentPage = 0;

    private void OnEnable()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;

        _tips = ui.Q<Label>("tips");
        _prevBtn = ui.Q<Button>("prev_btn");
        _nextBtn = ui.Q<Button>("next_btn");
        _page1 = ui.Q<VisualElement>("page-1");
        _page2 = ui.Q<VisualElement>("page-2");
        _page3 = ui.Q<VisualElement>("page-3");
        _page4 = ui.Q<VisualElement>("page-4");

        _prevBtn.clicked += () => ChangePage(-1);
        _nextBtn.clicked += () => ChangePage(1);
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

    private void ChangePage(int index)
    {
        currentPage = Mathf.Clamp(currentPage + index, 1, 4);
        UpdatePageUI();
    }

    private void UpdatePageUI()
    {
        // 1. Hide All
        _page1.RemoveFromClassList("appear"); _page1.AddToClassList("hide");
        _page2.RemoveFromClassList("appear"); _page2.AddToClassList("hide");
        _page3.RemoveFromClassList("appear"); _page3.AddToClassList("hide");
        _page4.RemoveFromClassList("appear"); _page4.AddToClassList("hide");

        // 2. Appear Current
        if (currentPage == 1) { _page1.RemoveFromClassList("hide"); _page1.AddToClassList("appear"); }
        if (currentPage == 2) { _page2.RemoveFromClassList("hide"); _page2.AddToClassList("appear"); }
        if (currentPage == 3) { _page3.RemoveFromClassList("hide"); _page3.AddToClassList("appear"); }
        if (currentPage == 4) { _page4.RemoveFromClassList("hide"); _page4.AddToClassList("appear"); }

        ToggleButtons();
    }

    private void ToggleButtons()
    {
        // Prev Button
        if (currentPage == 1) { _prevBtn.AddToClassList("hide"); _prevBtn.RemoveFromClassList("appear"); }
        else { _prevBtn.RemoveFromClassList("hide"); _prevBtn.AddToClassList("appear"); }

        // Next Button
        if (currentPage == 4) { _nextBtn.AddToClassList("hide"); _nextBtn.RemoveFromClassList("appear"); }
        else { _nextBtn.RemoveFromClassList("hide"); _nextBtn.AddToClassList("appear"); }
    }
}
