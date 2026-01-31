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
    private VisualElement invitationBtn;
    private VisualElement crestLogoLayer;
    private VisualElement invitationCrestLarge; // Reference to a large icon in the overlay
    public Texture2D defaultCrestBase; // Assign 'crest based.png' here
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

        // Overlay References
        logbookOverlay = root.Q<VisualElement>("LogbookOverlay");
        invitationOverlay = root.Q<VisualElement>("InvitationOverlay");
        crestIcon = root.Q<VisualElement>("CrestIcon");
        invitationBtn = root.Q<Button>("InvitationBtn");
        crestLogoLayer = root.Q<VisualElement>("CrestLogoLayer");
        invitationCrestLarge = invitationOverlay.Q<VisualElement>("CrestIconLarge");
        root = GetComponent<UIDocument>().rootVisualElement;
        VisualElement screenWrapper = root.Q<VisualElement>("ScreenWrapper");
        
        screenWrapper.RegisterCallback<PointerDownEvent>(evt => {
            VisualElement targetElement = evt.target as VisualElement;

            if (targetElement != null)
            {
                // Close overlays if clicking background, curtains, desk, racks, or ceiling
                if (targetElement == screenWrapper || 
                    targetElement.name.Contains("Curtain") || 
                    targetElement.name == "BottomDesk" ||
                    targetElement.name == "LeftRack" ||
                    targetElement.name == "RightProtocol" ||
                    targetElement.name == "Ceiling") 
                {
                    CloseAllOverlays();
                }
            }
        }, TrickleDown.NoTrickleDown);

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

        // Setup Dragging for masks (Existing Logic)
        SetupMaskDrag("NobilityMask", CharacterData.CharacterClass.Nobility);
        SetupMaskDrag("BusinessMask", CharacterData.CharacterClass.Business);
        SetupMaskDrag("CelebrityMask", CharacterData.CharacterClass.Celebrity);

        protocolDetail = root.Q<VisualElement>("ProtocolDetailView");
        detailTitle = root.Q<Label>("DetailTitle");

        root.Q<Button>("CloseProtocol").clicked += () => protocolDetail.style.display = DisplayStyle.None;

        resultScreen = root.Q<VisualElement>("ResultScreen");
        Button finishBtn = root.Q<Button>("FinishBtn");
        if (finishBtn != null)
        {
            finishBtn.clicked += () => {
                Time.timeScale = 1; // Ensure time is unfrozen
                UnityEngine.SceneManagement.SceneManager.LoadScene(0); 
            };
            // Also add the hover sound for consistency
            SetupButtonHover("FinishBtn");
        }
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

    public void UpdateNobilityUI(bool isNobility, Texture2D logoTexture)
    {
        if (isNobility && logoTexture != null)
        {
            invitationBtn.style.display = DisplayStyle.Flex;
            crestLogoLayer.style.backgroundColor = Color.clear;
            crestLogoLayer.style.backgroundImage = new StyleBackground(logoTexture);
            
            // Also update the large image in the overlay for when they click it
            if (invitationCrestLarge != null)
                invitationCrestLarge.style.backgroundColor = Color.clear;
                invitationCrestLarge.style.backgroundImage = new StyleBackground(logoTexture);
        }
        else
        {
            invitationBtn.style.display = DisplayStyle.None;
        }
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

        // 1. Get the center of the mask in screen pixels
        Vector2 maskCenter = mask.worldBound.center;
        
        // 2. Convert UI coordinates to Screen coordinates
        // UI Toolkit (0,0) is top-left, but Screen (0,0) is bottom-left. 
        // This is often why the "drop" feels 'way different'.
        Vector2 screenPos = new Vector2(maskCenter.x, Screen.height - maskCenter.y);
        
        // 3. Convert that to a World Ray
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        // --- DEBUG SECTION ---
        // This draws a line in the SCENE view for 2 seconds
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 2f);
        
        Debug.Log($"[UI Pos]: {maskCenter} | [Converted Screen Pos]: {screenPos}");
        
        if (hit.collider != null)
        {
            Debug.Log($"[Hit Success]: Hit {hit.collider.name} with tag {hit.collider.tag}");
            if (hit.collider.CompareTag("Guest"))
            {
                manager.CheckMask(maskClass);
            }
        }
        else
        {
            Debug.Log("[Hit Failed]: The raycast didn't hit anything. Check your BoxCollider2D!");
        }
        // --- END DEBUG ---

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
    void CloseAllOverlays()
    {
        logbookOverlay.style.display = DisplayStyle.None;
        invitationOverlay.style.display = DisplayStyle.None;
        protocolDetail.style.display = DisplayStyle.None;
    }

}