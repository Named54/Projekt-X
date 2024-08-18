using UnityEngine;
using System.Collections;

public class Bow_Combat : WeaponBase
{
    [System.Serializable]
    public class BowCombatStats
    {
        public float minArrowDamage = 10f;
        public float maxArrowDamage = 30f;
        public float maxChargeTime = 2f;
        public float minArrowSpeed = 10f;
        public float maxArrowSpeed = 30f;
        public GameObject arrowPrefab;
        public float desiredRotation = 90f;
    }
    public BowCombatStats bowStats;

    private bool isChargingShot = false;
    private float chargeStartTime;

    private WeaponWheelController weaponWheel;

    private void Start()
    {
        weaponWheel = FindFirstObjectByType<WeaponWheelController>();
    }
    private void Update()
    {
        if (IsCurrentWeapon())
        {
            AimAndShoot();
        }
    }
    private bool IsCurrentWeapon()
    {
        return weaponWheel.weaponID == 3; // Assuming 3 is the ID for the bow
    }

    private void AimAndShoot()
    {
        if (!enabled) return;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 shootingDirection = (mousePosition - transform.position).normalized;

        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            isChargingShot = true;
            chargeStartTime = Time.time;
        }

        if ((Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse1)) && isChargingShot)
        {
            float chargeDuration = Time.time - chargeStartTime;
            float arrowDamage = Mathf.Lerp(bowStats.minArrowDamage, bowStats.maxArrowDamage, chargeDuration / bowStats.maxChargeTime);
            float arrowSpeed = Mathf.Lerp(bowStats.minArrowSpeed, bowStats.maxArrowSpeed, chargeDuration / bowStats.maxChargeTime);
            ShootArrow(shootingDirection, arrowDamage, arrowSpeed);
            isChargingShot = false;
        }

        if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
        {
            // Hier können Sie visuelles Feedback für das Aufladen des Schusses hinzufügen
        }
    }

    private void ShootArrow(Vector2 shootingDirection, float arrowDamage, float arrowSpeed)
    {
        GameObject arrow = Instantiate(bowStats.arrowPrefab, transform.position, Quaternion.identity);
        AdvancedArrow advancedArrow = arrow.GetComponent<AdvancedArrow>();
        if (advancedArrow != null)
        {
            advancedArrow.Initialize(shootingDirection, arrowDamage, arrowSpeed, bowStats.desiredRotation);
        }
    }

    public override void Attack(bool isLightAttack)
    {
        // This method is not used for the bow, as we're using AimAndShoot instead
    }

    public override void UpdateCooldowns()
    {
        base.UpdateCooldowns();
    }
}