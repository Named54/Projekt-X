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
    public int weaponID;
    public Image selectedWeapon;
    
    private bool weaponWheelSelected = false;
    private bool weaponSwitchRequested = false;
    private WeaponBase newWeapon = null;
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
            ToggleWeaponWheelOpen();
        }
        /* Direkter Waffenwechsel, wenn Spieler eine Taste drückt
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectWeapon(1); // Faust-Waffe
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectWeapon(2); // Schwert
        }*/
        if (IsWeaponWheelOpen())
        {
            anim.SetBool("OpenWeaponWheel", true);
        }
        else
        {
            anim.SetBool("OpenWeaponWheel", false);
            //canAttack = true;
        }
        if (weaponSwitchRequested)
        {
            SwitchWeapon(newWeapon);
            weaponSwitchRequested = false;
            newWeapon = null;
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
            default:
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
    public void SelectWeapon(int weaponId)
    {
        weaponID = weaponId;
        SwitchWeapon(GetWeaponByID(weaponId));
    }
    private WeaponBase GetWeaponByID(int weaponId)
    {
        switch (weaponId)
        {
            case 1: return fistWeapon;
            case 2: return swordWeapon;
            case 3: return bowWeapon;
            case 4: return katanaWeapon;
            case 5: return scytheWeapon;
            case 6: return daggerWeapon;
            default: return null;
        }
    }
    public void SetWeaponWheelOpen(bool open)
    {
        weaponWheelSelected = open;
        for (int i = 0; i < gameObject.transform.childCount; i++)
            gameObject.transform.GetChild(i).gameObject.SetActive(open);
        gameObject.SetActive(true);

        if (open)
            InputManager.PushEnterUIMode();
        else
            InputManager.PopEnterUIMode();
    }

    public void ToggleWeaponWheelOpen()
    {
        SetWeaponWheelOpen(!IsWeaponWheelOpen());
    }

    public bool IsWeaponWheelOpen()
    {
        return weaponWheelSelected;
    }
}

