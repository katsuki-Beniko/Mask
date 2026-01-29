using UnityEngine;
using System.Collections.Generic;

public class MasqueradeManager : MonoBehaviour
{
    public Transform spawnPoint;
    public List<CharacterData> characterQueue;
    
    private GameObject currentCharacterInstance;
    private int currentIdx = 0;

    void Start()
    {
        SpawnNextCharacter();
    }

    public void SpawnNextCharacter()
    {
        if (currentCharacterInstance != null) Destroy(currentCharacterInstance);

        if (currentIdx < characterQueue.Count && characterQueue[currentIdx] != null)
        {
            CharacterData data = characterQueue[currentIdx];
            
            currentCharacterInstance = Instantiate(data.characterPrefab, spawnPoint.position, Quaternion.identity);
            currentCharacterInstance.transform.SetParent(spawnPoint);

            // Change color for testing placeholders
            SpriteRenderer renderer = currentCharacterInstance.GetComponent<SpriteRenderer>();
            if (renderer == null) renderer = currentCharacterInstance.GetComponentInChildren<SpriteRenderer>();
            
            if (renderer != null)
            {
                renderer.color = data.characterColor;
            }

            // TEST LOG: Tells you who just walked in
            Debug.Log($"<color=cyan>TESTING:</color> {data.characterName} has arrived. Expected Mask: {data.correctClass}");

            currentIdx++;
        }
        else
        {
            Debug.Log("No more guests in the queue.");
        }
    }

    public void CheckMask(CharacterData.CharacterClass givenClass)
    {
        if (currentIdx == 0) return;

        CharacterData data = characterQueue[currentIdx - 1];
        
        if (givenClass == data.correctClass)
        {
            Debug.Log($"<color=green>SUCCESS:</color> You correctly identified {data.characterName} as {data.correctClass}.");
        }
        else
        {
            Debug.Log($"<color=red>SCANDAL:</color> You gave a {givenClass} mask to a {data.correctClass}!");
        }
        
        SpawnNextCharacter();
    }

    public void KickCharacter()
    {
        if (currentIdx == 0) return;

        CharacterData data = characterQueue[currentIdx - 1];

        if (data.isEnemy)
        {
            Debug.Log("<color=blue>BANNED:</color> Good eye! That was an impostor.");
            // Add "Security Score" points here
        }
        else
        {
            Debug.Log("<color=red>MISTAKE:</color> You just kicked out a real guest!");
            // Lower your Stability Meter
        }

        SpawnNextCharacter();
    }
}