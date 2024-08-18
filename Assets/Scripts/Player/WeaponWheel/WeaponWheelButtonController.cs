using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponWheelButtonController : MonoBehaviour
{
    public int ID;
    public Sprite icon;
    private Animator anim;
    public string weaponName;
    public Image selectedWeapon;
    public bool selected = false;
    public TextMeshProUGUI weaponText;
    private WeaponWheelController weaponWheelController;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        weaponWheelController = gameObject.GetComponentInParent<WeaponWheelController>();

    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            selectedWeapon.sprite = icon;
            weaponText.text = weaponName;
        }
    }
    public void Selected()
    {
        selected = true;
        weaponWheelController.weaponID = ID;
        weaponWheelController.SetWeaponWheelOpen(false);
    }
    public void Deselected()
    {
        selected = false;
        weaponWheelController.weaponID = 0;
        weaponWheelController.SetWeaponWheelOpen(false);
    }

    public void HoverEnter()
    {
        anim.SetBool("Hover", true);
        weaponText.text = weaponName;
    }

    public void HoverExit()
    {
        anim.SetBool("Hover", false);
        weaponText.text = "";
    }
}
