using UnityEngine;
using UnityEngine.UI;

public class WeaponWheelController : MonoBehaviour
{
    public Animator anim;
    public Sprite noImage;
    public Sprite Fist;
    public Sprite Sword;
    public Sprite Bow;
    public Sprite Katana;
    public Sprite Scythe;
    public Sprite Dagger;
    public static int weaponID;
    public Image selectedWeapon;
    private bool weaponWheelSelected = false;

    // Referenz zum CombatSystem
    public CombatSystem combatSystem;

    // Referenzen zu den Waffen-Skripten
    public WeaponBase fistWeapon;
    public WeaponBase swordWeapon;
    public WeaponBase bowWeapon;
    public WeaponBase katanaWeapon;
    public WeaponBase scytheWeapon;
    public WeaponBase daggerWeapon;

    private void Start()
    {
        // Initialisiere mit Faust-Waffe
        SwitchWeapon(fistWeapon);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            weaponWheelSelected = !weaponWheelSelected;
        }

        if (weaponWheelSelected)
        {
            anim.SetBool("OpenWeaponWheel", true);
        }
        else
        {
            anim.SetBool("OpenWeaponWheel", false);
        }

        switch (weaponID)
        {
            case 0:
                selectedWeapon.sprite = noImage;
                break;
            case 1:
                selectedWeapon.sprite = Fist;
                SwitchWeapon(fistWeapon);
                break;
            case 2:
                selectedWeapon.sprite = Sword;
                SwitchWeapon(swordWeapon);
                break;
            case 3:
                selectedWeapon.sprite = Bow;
                SwitchWeapon(bowWeapon);
                break;
            case 4:
                selectedWeapon.sprite = Katana;
                SwitchWeapon(katanaWeapon);
                break;
            case 5:
                selectedWeapon.sprite = Scythe;
                SwitchWeapon(scytheWeapon);
                break;
            case 6:
                selectedWeapon.sprite = Dagger;
                SwitchWeapon(daggerWeapon);
                break;
        }
    }

    private void SwitchWeapon(WeaponBase newWeapon)
    {
        if (combatSystem == null || newWeapon == null)
        {
            Debug.LogError("CombatSystem oder Waffen-Skript nicht gesetzt!");
            return;
        }

        if (combatSystem.currentWeapon == newWeapon)
        {
            return;
        }

        if (combatSystem.currentWeapon != null)
        {
            combatSystem.currentWeapon.enabled = false;
        }

        newWeapon.enabled = true;
        combatSystem.SwitchWeapon(newWeapon);
    }
}
