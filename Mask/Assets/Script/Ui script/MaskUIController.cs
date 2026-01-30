using UnityEngine;
using UnityEngine.UIElements;

public class MaskUIController : MonoBehaviour
{
    public MasqueradeManager manager;
    private VisualElement root;
    private VisualElement logbookOverlay;
    private VisualElement invitationOverlay;
    private VisualElement protocolDetail;
    private Label detailTitle;
    private Label detailContent;

    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        // Overlay References
        logbookOverlay = root.Q<VisualElement>("LogbookOverlay");
        invitationOverlay = root.Q<VisualElement>("InvitationOverlay");

        // Bottom Tool Buttons
        root.Q<Button>("LogbookBtn").clicked += () => ShowOverlay(logbookOverlay);
        root.Q<Button>("InvitationBtn").clicked += () => ShowOverlay(invitationOverlay);
        
        // Close Buttons
        root.Q<Button>("CloseLogbook").clicked += () => HideOverlay(logbookOverlay);
        root.Q<Button>("CloseInvite").clicked += () => HideOverlay(invitationOverlay);

        // Kick Button
        root.Q<Button>("KickButton").clicked += () => manager.KickCharacter();

        // Setup Dragging for masks (Existing Logic)
        SetupMaskDrag("NobilityMask", CharacterData.CharacterClass.Nobility);
        SetupMaskDrag("BusinessMask", CharacterData.CharacterClass.Business);
        SetupMaskDrag("CelebrityMask", CharacterData.CharacterClass.Celebrity);

        protocolDetail = root.Q<VisualElement>("ProtocolDetailView");
        detailTitle = root.Q<Label>("DetailTitle");
        detailContent = root.Q<Label>("DetailContent");

        // Link the individual protocol letters
        root.Q<Button>("ProtocolHouseA").clicked += () => 
            ShowProtocol("Nobility House A", "- Crest: Silver Eagle\n- Seal: Blue Wax\n- Language: High Silens");
        
        root.Q<Button>("ProtocolHouseB").clicked += () => 
            ShowProtocol("Nobility House B", "- Crest: Golden Lion\n- Seal: Red Wax\n- Language: Archaic Latin");

        root.Q<Button>("ProtocolGeneral").clicked += () => 
            ShowProtocol("General Invitation", "- Format: Standard Parchment\n- Valid for: Common Guests & Merchants");

        root.Q<Button>("CloseProtocol").clicked += () => protocolDetail.style.display = DisplayStyle.None;
    }

    void ShowOverlay(VisualElement element)
    {
        element.style.display = DisplayStyle.Flex;
        Debug.Log($"Displaying {element.name}");
    }

    void ShowProtocol(string title, string content)
    {
        detailTitle.text = title;
        detailContent.text = content;
        protocolDetail.style.display = DisplayStyle.Flex;
    }

    void HideOverlay(VisualElement element)
    {
        element.style.display = DisplayStyle.None;
    }

    // Use the drag logic from previous turns here...
    void SetupMaskDrag(string id, CharacterData.CharacterClass maskClass) { /* ... */ }
}