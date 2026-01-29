using UnityEngine;
using UnityEngine.UIElements;

public class MaskUIController : MonoBehaviour
{
    public MasqueradeManager manager;
    private VisualElement root;
    private VisualElement maskTray;
    
    // We store the mask being dragged
    private VisualElement draggedMask;
    private bool isDragging = false;

    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        
        // Set up the 3 masks from your UXML (ensure names match your UXML)
        SetupMask("NobilityMask", CharacterData.CharacterClass.Nobility);
        SetupMask("BusinessMask", CharacterData.CharacterClass.Business);
        SetupMask("CelebrityMask", CharacterData.CharacterClass.Celebrity);
    }

    void SetupMask(string elementName, CharacterData.CharacterClass maskClass)
    {
        VisualElement mask = root.Q<VisualElement>(elementName);
        Vector2 startPointerPos = Vector2.zero;

        mask.RegisterCallback<PointerDownEvent>(evt => {
            isDragging = true;
            draggedMask = mask;
            
            // Record the starting mouse position to calculate the offset
            startPointerPos = evt.localPosition;
            
            mask.CapturePointer(evt.pointerId);
        });

        mask.RegisterCallback<PointerMoveEvent>(evt => {
            if (isDragging && draggedMask == mask)
            {
                // Calculate how far the mouse has moved from the start
                Vector2 delta = (Vector2)evt.localPosition - startPointerPos;

                // NEW API: Use style.translate instead of transform.position
                // This moves the element relative to its original position
                mask.style.translate = new Translate(
                    mask.style.translate.value.x.value + delta.x, 
                    mask.style.translate.value.y.value + delta.y, 
                    0);
            }
        });

        mask.RegisterCallback<PointerUpEvent>(evt => {
            if (!isDragging) return;
            
            isDragging = false;
            mask.ReleasePointer(evt.pointerId);
            
            // Raycast logic to check if dropped on character
            // Note: UI Toolkit coordinates have (0,0) at Top-Left, 
            // while Screen coordinates usually have (0,0) at Bottom-Left.
            Vector2 screenPos = new Vector2(evt.position.x, Screen.height - evt.position.y);
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.CompareTag("Guest"))
            {
                manager.CheckMask(maskClass);
            }
            
            // Reset the mask back to its original position in the tray
            mask.style.translate = new Translate(0, 0, 0);
        });
    }
}