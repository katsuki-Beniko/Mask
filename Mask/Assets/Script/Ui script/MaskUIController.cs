using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class MaskUIController : MonoBehaviour
{
    public MasqueradeManager manager;
    public AudioSource audioSource; 
    public AudioClip hoverSound;   
    private VisualElement root;
    private VisualElement logbookOverlay;
    private VisualElement invitationOverlay;
    private VisualElement protocolDetail;
    private Label detailTitle;
    private VisualElement resultScreen;
    private VisualElement pauseMenu;
    private bool isPaused = false; 
    private VisualElement crestIcon; // Add this reference
    public Texture2D crestHouseA;    // Drag Assets/Images/CrestA here
    public Texture2D crestHouseB;    // Drag Assets/Images/CrestB here
    public Texture2D crestGeneral;   // Drag Assets/Images/CrestGeneral here

    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        // --- Existing Button Logic ---
        SetupButtonHover("ResumeBtn");
        SetupButtonHover("RestartGameBtn");
        SetupButtonHover("MainMenuBtn");
        SetupButtonHover("LogbookBtn");
        SetupButtonHover("InvitationBtn");
        SetupButtonHover("KickButton");
        
        // Protocol Buttons
        SetupButtonHover("ProtocolHouseA");
        SetupButtonHover("ProtocolHouseB");
        SetupButtonHover("ProtocolGeneral");

        // Overlay References
        logbookOverlay = root.Q<VisualElement>("LogbookOverlay");
        invitationOverlay = root.Q<VisualElement>("InvitationOverlay");
        crestIcon = root.Q<VisualElement>("CrestIcon");

        // Bottom Tool Buttons
        root.Q<Button>("LogbookBtn").clicked += () => ShowOverlay(logbookOverlay);
        root.Q<Button>("InvitationBtn").clicked += () => ShowOverlay(invitationOverlay);
        
        // Close Buttons
        root.Q<Button>("CloseLogbook").clicked += () => HideOverlay(logbookOverlay);
        root.Q<Button>("CloseInvite").clicked += () => HideOverlay(invitationOverlay);

        // Kick Button
        root.Q<Button>("KickButton").clicked += () => manager.KickCharacter();

        root.Q<Button>("ProtocolHouseA").clicked += () => ShowProtocol("Nobility House A", crestHouseA);
        root.Q<Button>("ProtocolHouseB").clicked += () => ShowProtocol("Nobility House B", crestHouseB);
        root.Q<Button>("ProtocolGeneral").clicked += () => ShowProtocol("General Invitation", crestGeneral);

        // Setup Dragging for masks (Existing Logic)
        SetupMaskDrag("NobilityMask", CharacterData.CharacterClass.Nobility);
        SetupMaskDrag("BusinessMask", CharacterData.CharacterClass.Business);
        SetupMaskDrag("CelebrityMask", CharacterData.CharacterClass.Celebrity);

        protocolDetail = root.Q<VisualElement>("ProtocolDetailView");
        detailTitle = root.Q<Label>("DetailTitle");

        root.Q<Button>("CloseProtocol").clicked += () => protocolDetail.style.display = DisplayStyle.None;

        resultScreen = root.Q<VisualElement>("ResultScreen");
        root.Q<Button>("RestartBtn").clicked += () => UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        pauseMenu = root.Q<VisualElement>("PauseMenu");
        if (pauseMenu != null)
        {
            root.Q<Button>("ResumeBtn").clicked += TogglePause;

            // RESTART: Reloads the current scene
            root.Q<Button>("RestartGameBtn").clicked += () => {
                Time.timeScale = 1; // Unfreeze time before reloading!
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            };

            // MAIN MENU: Loads scene index 0 (assuming your menu is index 0)
            root.Q<Button>("MainMenuBtn").clicked += () => {
                Time.timeScale = 1;
                UnityEngine.SceneManagement.SceneManager.LoadScene(0); 
            };
        }
    }

    void ShowOverlay(VisualElement element)
    {
        element.style.display = DisplayStyle.Flex;
        Debug.Log($"Displaying {element.name}");
    }

    void ShowProtocol(string factionName, Texture2D crestImage)
    {
        detailTitle.text = factionName;
        if (crestImage != null)
            crestIcon.style.backgroundImage = new StyleBackground(crestImage);
        else
            crestIcon.style.backgroundImage = null;

        protocolDetail.style.display = DisplayStyle.Flex;
    }

    public void ShowResultScreen()
    {
        // Update raw stats
        root.Q<Label>("CorrectRejectsVal").text = manager.correctRejects.ToString();
        root.Q<Label>("IncorrectRejectsVal").text = manager.incorrectRejects.ToString();
        root.Q<Label>("CorrectMasksVal").text = manager.correctMasks.ToString();
        root.Q<Label>("IncorrectMasksVal").text = manager.incorrectMasks.ToString();

        // Get and show grade
        string finalGrade = manager.CalculateGrade();
        Label gradeLabel = root.Q<Label>("GradeValue");
        gradeLabel.text = finalGrade;

        // Apply basic color coding for testing
        if (finalGrade == "PERFECT") gradeLabel.style.color = new Color(0, 1, 0); // Green
        else if (finalGrade == "FAILURE") gradeLabel.style.color = new Color(1, 0, 0); // Red
        else gradeLabel.style.color = new Color(1, 1, 1); // White

        resultScreen.style.display = DisplayStyle.Flex;
    }

    void HideOverlay(VisualElement element)
    {
        element.style.display = DisplayStyle.None;
    }

    void SetupMaskDrag(string id, CharacterData.CharacterClass maskClass)
    {
        VisualElement mask = root.Q<VisualElement>(id);
        Vector2 offset = Vector2.zero;
        bool isDragging = false;

        mask.RegisterCallback<PointerDownEvent>(evt => {
            isDragging = true;
            
            // Calculate the distance between the mouse and the top-left of the mask
            offset = evt.localPosition; 
            
            mask.CapturePointer(evt.pointerId);
            mask.BringToFront(); 
        });

        mask.RegisterCallback<PointerMoveEvent>(evt => {
            if (!isDragging) return;

            // Use world position (evt.position) instead of local position
            // This stops the 'jumping' when dragging across the screen
            mask.style.left = evt.position.x - mask.parent.worldBound.x - offset.x;
            mask.style.top = evt.position.y - mask.parent.worldBound.y - offset.y;
        });

        mask.RegisterCallback<PointerUpEvent>(evt => {
            if (!isDragging) return;
            isDragging = false;
            mask.ReleasePointer(evt.pointerId);

            // Standard hit detection for the guest
            Vector2 screenPos = new Vector2(evt.position.x, Screen.height - evt.position.y);
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.CompareTag("Guest"))
            {
                manager.CheckMask(maskClass);
            }

            // Snap back to its home slot
            mask.style.left = StyleKeyword.Null;
            mask.style.top = StyleKeyword.Null;
        });
    }
    void Update()
    {
        // Use the New Input System check
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.style.display = isPaused ? DisplayStyle.Flex : DisplayStyle.None;
        
        // Freeze or unfreeze the game time
        Time.timeScale = isPaused ? 0 : 1;
    }

    // New helper method to attach hover sounds
    void SetupButtonHover(string buttonName)
    {
        Button btn = root.Q<Button>(buttonName);
        if (btn != null)
        {
            btn.RegisterCallback<PointerEnterEvent>(evt => PlayHoverSound());
        }
    }

    void PlayHoverSound()
    {
        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

}