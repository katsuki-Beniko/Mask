using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class MainMenuController : MonoBehaviour
{

    [Header("Audio Settings")]
    [SerializeField] private float fadeDuration = 5f;
    private float _fadeTimer = 0f;
    private float _targetMaxVolume;
    [SerializeField] private AudioSource _audioSource;


    [Header("UI Elements")]
    public VisualElement ui;
    private Label musicName;
    private Image musicNote;
    private Button startBtn;
    private Button settingBtn;
    private Button tutoBtn;
    private Button creditBtn;
    private VisualElement _settings;
    private SliderInt _volSlider;
    private Label _volNumLabel;
    private Button _backBtn;
    private VisualElement _exit;
    private Button _noBtn;
    private Button _yesBtn;

    private float _timer = 0f;
    //private float _cumulativeAngle = 0f;

    void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        musicName = ui.Q<Label>("music_name");
        musicNote = ui.Q<Image>("music_note");
        startBtn = ui.Q<Button>("Start");
        settingBtn = ui.Q<Button>("Settings");
        tutoBtn = ui.Q<Button>("Tutorial");
        creditBtn = ui.Q<Button>("Credits");

        _settings = ui.Q<VisualElement>("settings_container");
        _volSlider = ui.Q<SliderInt>("vol");
        _volNumLabel = ui.Q<Label>("vol_num");
        _backBtn = ui.Q<Button>("back_btn");

        _exit = ui.Q<VisualElement>("exit_container");
        _noBtn = ui.Q<Button>("no_btn");
        _yesBtn = ui.Q<Button>("yes_btn");

        startBtn.clicked += OnStartButtonClicked;
        settingBtn.clicked += ShowSettings;
        tutoBtn.clicked += OnTutoButtonClicked;
        creditBtn.clicked += OnCreditButtonClicked;
        _backBtn.clicked += HideSettings;
        _noBtn.clicked += HideExit;
        _yesBtn.clicked += QuitGame;

        int savedVol = PlayerPrefs.GetInt("MasterVolume", 100);
        _volSlider.value = savedVol;
        _volNumLabel.text = savedVol.ToString();

        _volSlider.RegisterValueChangedCallback(evt =>
        {
            _volNumLabel.text = evt.newValue.ToString();
            PlayerPrefs.SetInt("MasterVolume", evt.newValue);

            AudioListener.volume = evt.newValue / 100f;
        });

        if(_audioSource != null && _audioSource.clip  != null)
        {
            _targetMaxVolume = _audioSource.volume;
            _audioSource.volume = 0f;
            _audioSource.Play();

            UpdateSongTitle();
        }
    }

    public void LoadNewPage(string targetName)
    {
        LoadingBridge.NextSceneName = targetName;
        SceneManager.LoadScene("Loading");
    }

    void Update()
    {
        float dt = Time.deltaTime;

        if(_audioSource != null && _fadeTimer < fadeDuration)
        {
            _fadeTimer += dt;
            float fadePercentage = Mathf.Clamp01(_fadeTimer / fadeDuration);
            _audioSource.volume = fadePercentage * _targetMaxVolume;

            UpdateSongTitle();
        }

        if (musicName != null)
        {
            float pulseOpacity = (Mathf.Sin(Time.time * 2f) + 1f) / 2f;
            musicName.style.opacity = pulseOpacity;
        }

        _timer += dt;

        if (musicNote != null)
        {
            float verticalOffset = Mathf.Sin(_timer * 2f) * 10f;
            musicNote.style.translate = new Translate(0, verticalOffset);
        }

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if(_settings.ClassListContains("appear"))
            {
                return;
            }
            else
            {
                ShowExit();
            }
        }
    }

    private void UpdateSongTitle()
    {
        if (musicName != null)
        {
            musicName.text = _audioSource.clip.name;
        }
    }

    private void OnStartButtonClicked()
    {
        Debug.Log("Start");
    }

    private void ShowSettings()
    {
        _settings.RemoveFromClassList("hide");
        _settings.AddToClassList("appear");
    }

    private void HideSettings()
    {
        _settings.RemoveFromClassList("appear");
        _settings.AddToClassList("hide");
    }

    private void ShowExit()
    {
        _exit.RemoveFromClassList("hide");
        _exit.AddToClassList("appear");
    }

    private void HideExit()
    {
        _exit.RemoveFromClassList("appear");
        _exit.AddToClassList("hide");
    }

    private void OnTutoButtonClicked()
    {
        LoadNewPage("tutorial");
    }

    private void OnCreditButtonClicked()
    {
        //SceneManager.SceneLoaded();
        LoadNewPage("Credits");
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
