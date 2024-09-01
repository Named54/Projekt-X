using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public GameObject healthTextPrefab;

    public Canvas gameCanvas;
    public CharacterEvents characterEvents;

    private void Awake()
    {
        gameCanvas = Object.FindAnyObjectByType<Canvas>();
    }
    private void OnEnable()
    {
        if (characterEvents == null)
        {
            characterEvents = FindAnyObjectByType<CharacterEvents>();
        }

        characterEvents.characterDamaged += CharacterTookDamage;
        characterEvents.characterHealed += CharacterHealed;
    }

    private void OnDisable()
    {
        characterEvents.characterDamaged -= CharacterTookDamage;
        characterEvents.characterHealed -= CharacterHealed;
    }

    //Enemy take Damage
    public void CharacterTookDamage(GameObject character, int damageReceived)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = damageReceived.ToString();
    }

    //Player get Health
    public void CharacterHealed(GameObject character, int healthRestored)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = healthRestored.ToString();
    }
}
