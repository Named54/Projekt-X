using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    private PlayerLevelUpSystem playerLevelSystem;

    private void Start()
    {
        playerLevelSystem = FindFirstObjectByType<PlayerLevelUpSystem>();
        playerLevelSystem.OnLevelUp += HandleLevelUp;
    }

    private void HandleLevelUp(int newLevel)
    {
        Debug.Log($"Congratulations! You've reached level {newLevel}!");
        // Hier k�nnen Sie UI-Elemente aktualisieren, Belohnungen anzeigen, etc.
    }
}
