using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Masquerade/Character")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public enum CharacterClass { Nobility, Business, Celebrity }
    public CharacterClass correctClass;
    public GameObject characterPrefab;
    public Color characterColor = Color.white;
    public bool isEnemy; 
    [TextArea] public string whatIsWrong; // For your own notes on the "tell" 
}