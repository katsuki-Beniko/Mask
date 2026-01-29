using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Masquerade/Character")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public enum CharacterClass { Nobility, Business, Celebrity }
    public CharacterClass correctClass;
    public GameObject characterPrefab;
    
    // NEW: Add this to change color in Inspector
    public Color characterColor = Color.white; 
}