using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class LoadingController : MonoBehaviour
{
    private int _step = 0;
    private float _timer = 0f;
    [SerializeField] private float timePerStep = 0.05f;

    [Header("UI Element")]
    [SerializeField] private VisualElement ui;
    private List<Image> _loadingLabels = new List<Image>();


    private string nextScene;
    private int maxLabel = 6;


    void OnEnable()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
        nextScene = LoadingBridge.NextSceneName;

        int totalLabel = _loadingLabels.Count;
        for (int i = 1; i <= maxLabel; i++)
        {
            Image lbl = ui.Q<Image>($"label-{i}");
            
            if (lbl != null)
            {
                _loadingLabels.Add(lbl);
                lbl.AddToClassList("hide");
                lbl.RemoveFromClassList("appear");
            }

        }

        if (!string.IsNullOrEmpty(nextScene))
        {
            StartCoroutine(LoadInBackground(nextScene));
        }
        else
        {
            Debug.LogError("SneneName is Empty");
        }
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= timePerStep)
        {
            _timer = 0f;
            _step++;

            if (_step > maxLabel)
            {
                _step = 1;
                ResetAllToHide();
            }

            ShowUpTo(_step);
        }
    }

    void ShowUpTo(int currentStep)
    {
        for(int i = 0;  i < currentStep; i++)
        {
            _loadingLabels[i].RemoveFromClassList("hide");
            _loadingLabels[i].AddToClassList("appear");
        }
    }

    void ResetAllToHide()
    {
        foreach (var lbl in _loadingLabels)
        {
            lbl.RemoveFromClassList("appear");
            lbl.AddToClassList("hide");
        }
    }

    IEnumerator LoadInBackground(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            yield return null;
        }

        op.allowSceneActivation = true;
    }
}
