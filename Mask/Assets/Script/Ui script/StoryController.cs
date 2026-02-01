using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class StoryController : MonoBehaviour
{
    [System.Serializable]
    public struct StoryMoment {
        public string elementName; 
        [TextArea] public string dialogue;
    }

    public List<StoryMoment> storySequence;
    private int currentStep = -1;
    private Label storyText;
    private VisualElement root;
    
    // NEW: Boolean to prevent clicking during fades
    private bool isFading = false;

    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        storyText = root.Q<Label>("StoryText");

        // Pre-hide all moments
        foreach (var moment in storySequence) {
            VisualElement ve = root.Q<VisualElement>(moment.elementName);
            if (ve != null) ve.style.opacity = 0;
        }

        root.RegisterCallback<PointerDownEvent>(evt => {
            // Only allow click if we are NOT currently fading
            if (!isFading) NextStep();
        });
    }

    void NextStep()
    {
        currentStep++;

        if (currentStep < storySequence.Count) {
            StartCoroutine(AnimateTextAndImage());
        }
        else {
            SceneManager.LoadScene(4); 
        }
    }

    System.Collections.IEnumerator AnimateTextAndImage()
    {
        isFading = true; // Lock the input

        // 1. Fade OUT the current text
        storyText.style.opacity = 0;
        yield return new WaitForSeconds(0.5f); // Wait for the 1s transition duration

        // 2. Update and fade IN the new image
        VisualElement ve = root.Q<VisualElement>(storySequence[currentStep].elementName);
        if (ve != null) {
            ve.style.opacity = 1;
        }

        // 3. Update text content and fade it back IN
        storyText.text = storySequence[currentStep].dialogue;
        storyText.style.opacity = 1;

        // Wait for the fade-in to complete before unlocking
        yield return new WaitForSeconds(1f); 
        
        isFading = false; // Unlock the input so player can click again
    }
}