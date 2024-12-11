using UnityEngine.UI;
using UnityEngine;

public class ElementWheelController : MonoBehaviour
{
    public Animator anim;
    public Sprite noElementSelected;
    public Sprite fire;
    public Sprite water;
    public Sprite ice;
    public Sprite thunder;
    public Sprite lighting;
    public Sprite darkness;
    public int elementID;
    public Image selectedElement;

    private bool elementWheelSelected = false;
    private ElementBase newElement = null;

    public ElementSystem elementSystem;

    public ElementBase iceElement;
    public ElementBase noneElement;
    public ElementBase fireElement;
    public ElementBase waterElement;
    public ElementBase lightElement;
    public ElementBase thunderElement;
    public ElementBase darknessElement;

    private void Start()
    {
        // Initialisiere mit Faust-Waffe
        SwitchElement(noneElement);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleElementWheelOpen();
        }
        //Direkter Waffenwechsel, wenn Spieler eine Taste dr�ckt
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectElement(1); // Faust-Waffe
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectElement(2); // Schwert
        }
        if (IsElementWheelOpen())
        {
            anim.SetBool("OpenWeaponWheel", true);
        }
        else
        {
            anim.SetBool("OpenWeaponWheel", false);
            //canAttack = true;
        }
        UpdateSelectedElementSprite();
        switch (elementID)
        {
            case 0:
                selectedElement.sprite = noElementSelected;
                break;
            case 1:
                selectedElement.sprite = fire;
                SwitchElement(fireElement);
                break;
            case 2:
                selectedElement.sprite = ice;
                SwitchElement(iceElement);
                break;
            case 3:
                selectedElement.sprite = water;
                SwitchElement(waterElement);
                break;
            case 4:
                selectedElement.sprite = thunder;
                SwitchElement(thunderElement);
                break;
            case 5:
                selectedElement.sprite = lighting;
                SwitchElement(lightElement);
                break;
            case 6:
                selectedElement.sprite = darkness;
                SwitchElement(darknessElement);
                break;
            default:
                break;
        }
    }
    private void UpdateSelectedElementSprite()
    {
        selectedElement.sprite = elementID switch
        {
            0 => noElementSelected,
            1 => fire,
            2 => water,
            3 => ice,
            4 => thunder,
            5 => lighting,
            6 => darkness,
            _ => noElementSelected
        };
    }
    private void SwitchElement(ElementBase newElement)
    {
        if (elementSystem == null || newElement == null)
        {
            Debug.LogError("ElementSystem oder Element-Skript nicht gesetzt!");
            return;
        }

        if (elementSystem.currentElement == newElement)
        {
            return;
        }

        if (elementSystem.currentElement != null)
        {
            elementSystem.currentElement.enabled = false;
        }

        newElement.enabled = true;
        elementSystem.currentElement = newElement;
    }

    public void SelectElement(int elementId)
    {
        elementID = elementId;
        SwitchElement(GetElementByID(elementId));
    }

    private ElementBase GetElementByID(int elementId)
    {
        return elementId switch
        {
            0 => noneElement,
            1 => fireElement,
            2 => waterElement,
            3 => iceElement,
            4 => thunderElement,
            5 => lightElement,
            6 => darknessElement,
            _ => null
        };
    }

    public void SetElementWheelOpen(bool open)
    {
        elementWheelSelected = open;
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(open);
        gameObject.SetActive(true);

        if (open)
            InputManager.PushEnterUIMode();
        else
            InputManager.PopEnterUIMode();
    }

    public void ToggleElementWheelOpen()
    {
        SetElementWheelOpen(!IsElementWheelOpen());
    }

    public bool IsElementWheelOpen()
    {
        return elementWheelSelected;
    }
}
