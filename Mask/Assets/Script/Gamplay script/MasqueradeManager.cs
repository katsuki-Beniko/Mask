using UnityEngine;
using System.Collections.Generic;

public class MasqueradeManager : MonoBehaviour
{
    public Transform spawnPoint;
    public List<CharacterData> characterQueue;
    
    // Stats Tracking
    [HideInInspector] public int correctRejects = 0;
    [HideInInspector] public int incorrectRejects = 0;
    [HideInInspector] public int correctMasks = 0;
    [HideInInspector] public int incorrectMasks = 0;

    public MaskUIController uiController; // Reference to show the final screen
    private GameObject currentCharacterInstance;
    private int currentIdx = 0;

    void Start() => SpawnNextCharacter();

    public void SpawnNextCharacter()
    {
        if (currentCharacterInstance != null) Destroy(currentCharacterInstance);

        if (currentIdx < characterQueue.Count)
        {
            CharacterData data = characterQueue[currentIdx];
            currentCharacterInstance = Instantiate(data.characterPrefab, spawnPoint.position, Quaternion.identity);
            currentCharacterInstance.transform.SetParent(spawnPoint);
            bool isNobility = (data.correctClass == CharacterData.CharacterClass.Nobility);
            uiController.UpdateNobilityUI(isNobility, data.familyCrest);
            SpriteRenderer renderer = currentCharacterInstance.GetComponentInChildren<SpriteRenderer>();
            if (renderer != null) renderer.color = data.characterColor;

            currentIdx++;
        }
        else
        {
            // Trigger the result screen when no characters are left
            uiController.ShowResultScreen();
        }
    }

    public void CheckMask(CharacterData.CharacterClass givenClass)
    {
        CharacterData data = characterQueue[currentIdx - 1];
        if (givenClass == data.correctClass && !data.isEnemy) correctMasks++;
        else incorrectMasks++;
        
        SpawnNextCharacter();
    }

    public void KickCharacter()
    {
        CharacterData data = characterQueue[currentIdx - 1];
        if (data.isEnemy) correctRejects++;
        else incorrectRejects++;

        SpawnNextCharacter();
    }

    public string CalculateGrade()
    {
        int totalGuests = characterQueue.Count;
        int totalCorrect = correctMasks + correctRejects;
        
        // Calculate percentage of correct actions
        float scorePercentage = (float)totalCorrect / totalGuests;

        if (totalCorrect == totalGuests) return "PERFECT";
        if (totalCorrect == 0) return "FAILURE";
        
        if (scorePercentage >= 0.8f) return "GOOD";    // 80% or higher
        if (scorePercentage >= 0.5f) return "NORMAL";  // 50% to 79%
        return "BAD";                                  // Below 50%
    }
}