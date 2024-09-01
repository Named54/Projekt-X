using System;
using UnityEngine;

public class CharacterEvents : MonoBehaviour
{
    //Character damaged and damage value
    public event Action<GameObject, int> characterDamaged;

    //Character healed and amount value
    public event Action<GameObject, int> characterHealed;

    public void OnCharacterDamaged(GameObject character, int damage)
    {
        characterDamaged?.Invoke(character, damage);
    }

    public void OnCharacterHealed(GameObject character, int health)
    {
        characterHealed?.Invoke(character, health);
    }
}
