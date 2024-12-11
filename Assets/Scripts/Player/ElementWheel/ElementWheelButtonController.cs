using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElementWheelButtonController : MonoBehaviour
{
    public int ID;
    public Sprite icon;
    private Animator anim;
    public string elementName;
    public Image selectedElement;
    public bool selected = false;
    public TextMeshProUGUI elementText;
    private ElementWheelController elementWheelController;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        elementWheelController = gameObject.GetComponentInParent<ElementWheelController>();

    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            selectedElement.sprite = icon;
            elementText.text = elementName;
        }
    }
    public void Selected()
    {
        selected = true;
        elementWheelController.elementID = ID;
      //  elementWheelController.SetElementWheelOpen(false);
    }
    public void Deselected()
    {
        selected = false;
        elementWheelController.elementID = 0;
       // elementWheelController.SetElementWheelOpen(false);
    }

    public void HoverEnter()
    {
        anim.SetBool("Hover", true);
        elementText.text = elementName;
    }

    public void HoverExit()
    {
        anim.SetBool("Hover", false);
        elementText.text = "";
    }
}
