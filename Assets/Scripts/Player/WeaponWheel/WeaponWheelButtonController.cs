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

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

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
        WeaponWheelController.weaponID = ID;
    }
    public void Deselected()
    {
        selected = false;
        WeaponWheelController.weaponID = 0;
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
