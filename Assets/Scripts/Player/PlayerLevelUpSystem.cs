using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelUpSystem : MonoBehaviour
{
    [System.Serializable]
    public class PlayerAttributes
    {
        public int maxHealth;
        public int maxMana;
        public int maxStamina;
        public int damage;
        public int parryDamage;
        // Add new attributes here
        // public int newAttribute;
    }

    public Image xpImage;
    public TMP_Text levelText;
    public TMP_Text levelUpText;
    public PlayerAttributes playerAttributes;

    public int maxLevel = 100;
    public int currentLevel = 1;
    public int currentExperience = 0;
    public int baseXpRequired = 100; 
    public float xpMultiplier = 1.2f;

    private WeaponBase weapon;
    private Player_health playerHealth;
    private PlayerMovement playerMovement;
    private Parry_Block_System parrySystem;

    public event Action<int> OnLevelUp;
    public event Action<int> OnExperienceGained;

    private void Start()
    {
        playerHealth = GetComponent<Player_health>();
        weapon = GetComponentInChildren<WeaponBase>();
        parrySystem = GetComponent<Parry_Block_System>();
        playerMovement = GetComponent<PlayerMovement>();

        UpdateUI();
    }

    public void GainExperience(int amount)
    {
        int xpToNextLevel = GetExperienceToNextLevel();
        currentExperience += amount;

        while (currentExperience >= xpToNextLevel && currentLevel < maxLevel)
        {
            int excessXP = currentExperience - xpToNextLevel;
            LevelUp();
            currentExperience = excessXP;
            xpToNextLevel = GetExperienceToNextLevel();
        }

        currentExperience = Mathf.Clamp(currentExperience, 0, xpToNextLevel);

        OnExperienceGained?.Invoke(amount);
        UpdateUI();
    }

    private void LevelUp()
    {
        currentLevel++;
        UpdatePlayerStats();
        OnLevelUp?.Invoke(currentLevel);

        if (levelUpText != null)
        {
            levelUpText.text = $"Level Up! You are now level {currentLevel}";
            levelUpText.gameObject.SetActive(true);
            Invoke("HideLevelUpText", 3f);
        }

        Debug.Log($"Level Up! You are now level {currentLevel}");
    }

    private void UpdatePlayerStats()
    {
        playerAttributes.maxHealth += UnityEngine.Random.Range(1, 4);
        playerAttributes.maxMana += UnityEngine.Random.Range(1, 3);
        playerAttributes.maxStamina += UnityEngine.Random.Range(2, 6);
        playerAttributes.damage += UnityEngine.Random.Range(5, 10);
        playerAttributes.parryDamage += UnityEngine.Random.Range(2, 7);

        float levelProgress = (float)currentLevel / maxLevel;
        // Aktualisierung der Spielerkomponenten
        if (playerHealth != null)
        {
            playerHealth.maxHealth = playerAttributes.maxHealth;
            playerHealth.Heal(playerAttributes.maxHealth); // Vollständige Heilung beim Level-Up
        }

        if (playerMovement != null)
        {
            playerMovement.maxStamina = playerAttributes.maxStamina;
            playerMovement.currentStamina = playerAttributes.maxStamina;
        }

        if (weapon != null)
        {
            weapon.weaponStats.lightAttackDamage += playerAttributes.damage;
            weapon.weaponStats.heavyAttackDamage += playerAttributes.damage;
        }

        if (parrySystem != null)
        {
            parrySystem.parryDamage = playerAttributes.parryDamage;
        }

        // Update new attributes here
        // int newAttributeIncrease = Mathf.RoundToInt(levelData.newAttributeCurve.Evaluate(levelProgress));
        // playerAttributes.newAttribute += newAttributeIncrease;
        // Update corresponding system here
    }

    public int GetExperienceToNextLevel()
    {
        if (currentLevel >= maxLevel)
        {
            return int.MaxValue; // Maximales Level erreicht
        }
        return Mathf.RoundToInt(baseXpRequired * Mathf.Pow(xpMultiplier, currentLevel - 1));
    }

    public float GetLevelProgress()
    {
        if (currentLevel >= maxLevel)
        {
            return 1f; // Maximales Level erreicht
        }
        return (float)currentExperience / GetExperienceToNextLevel();
    }

    private void UpdateUI()
    {
        if (xpImage != null)
        {
            xpImage.fillAmount = GetLevelProgress();
        }

        if (levelText != null)
        {
            levelText.text = $"Level: {currentLevel}";
        }
    }

    private void HideLevelUpText()
    {
        if (levelUpText != null)
        {
            levelUpText.gameObject.SetActive(false);
        }
    }
}