using UnityEngine;

[CreateAssetMenu(fileName = "GameplaySettings", menuName = "[APP]/Settings/Gameplay Settings")]
public class GameplaySettings : ScriptableObject
{
    [SerializeField] private int initialAllowance;
    public int InitialAllowance => initialAllowance;
    [SerializeField] private int initialHappiness;
    public int InitialHappiness => initialHappiness;
    [SerializeField] private string apiUrl;
    public string ApiUrl => apiUrl;
}
