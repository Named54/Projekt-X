using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPopup : MonoBehaviour
{
    public static TextPopup Create(Vector3 position, int damageAmount, bool isCriticalHit, Vector2 direction)
    {
        GameObject damagePopupGameObject = Instantiate(Resources.Load<GameObject>("pfDamagePopup"), position, Quaternion.identity);
        TextPopup damagePopup = damagePopupGameObject.GetComponent<TextPopup>();
        damagePopup.Setup(damageAmount, isCriticalHit, direction);

        return damagePopup;
    }
    private static int sortingOrder;

    private const float DISAPPEAR_TIMER_MAX = 1f;

    private Vector3 moveVector;
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
        textTransfrom = GetComponent<RectTransform>();
        textMeshPro = GetComponent<TextMeshProUGUI>();
        startColor = textMeshPro.color;
    }

    public void Setup(int damageAmout, bool isCriticalHit, Vector2 direction)
    {
        textMesh.SetText(damageAmout.ToString());
        if (!isCriticalHit)
        {
            textMesh.fontSize = 8;
            textColor = Color.red;
        }
        else
        {
            textMesh.fontSize = 10;
            textColor = Color.blue;
        }

        textMesh.color = textColor;
        disappearTimer = DISAPPEAR_TIMER_MAX;

        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;

        moveVector = direction * 20f;
    }

    private void Update()
    {
        damagePopup();
        ColorFade();
    }
    private void damagePopup()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;

        if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f)
        {
            float increaseScaleAmount = 1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            float decreaseScaleAmount = 1f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    //Pixels per seconds
    public Vector3 moveSpeed = new Vector3(0, 80, 0);
    public float timeToFade = 1f;

    RectTransform textTransfrom;
    TextMeshProUGUI textMeshPro;

    private float timeElapsed = 0f;
    private Color startColor;
    // Update is called once per frame
    private void ColorFade()
    {
        textTransfrom.position += moveSpeed * Time.deltaTime;

        timeElapsed += Time.deltaTime;

        if (timeElapsed < timeToFade)
        {
            float fadeAlpha = startColor.a * (1 - (timeElapsed / timeToFade));
            fadeAlpha = Mathf.Max(0f, fadeAlpha); // Ensure fadeAlpha doesn't go below 0
            textMeshPro.color = new Color(startColor.r, startColor.g, startColor.b, fadeAlpha);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
