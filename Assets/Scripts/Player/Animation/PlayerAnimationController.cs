using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    [Header("Animation")]
    public float idleTime;
    public Vector2 attackDirection;
    public Vector2 lookDirection = Vector2.zero;

    [Header("Frame")]
    public float frameRate;

    [Header("Book of Animation")]
    public List<List<Sprite>> IdleSpriteBook;
    public List<List<Sprite>> WalkSpriteBook;
    public List<List<Sprite>> RunSpriteBook;
    public List<List<Sprite>> HurtSpriteBook;
    public List<List<Sprite>> DieSpriteBook;
    public List<List<Sprite>> LightAttackSpriteBook_1;
    public List<List<Sprite>> LightAttackSpriteBook_2;
    public List<List<Sprite>> LightAttackSpriteBook_3;
    public List<List<Sprite>> HeavyAttackSpriteBook_1;
    public List<List<Sprite>> HeavyAttackSpriteBook_2;
    public List<List<Sprite>> HeavyAttackSpriteBook_3;

    private List<List<Sprite>> selectedSpriteBook;

    [Header("Idle Animation")]
    public List<Sprite> nIdle;
    public List<Sprite> neIdle;
    public List<Sprite> eIdle;
    public List<Sprite> seIdle;
    public List<Sprite> sIdle;

    [Header("Walk Animation")]
    public List<Sprite> nWalk;
    public List<Sprite> neWalk;
    public List<Sprite> eWalk;
    public List<Sprite> seWalk;
    public List<Sprite> sWalk;

    [Header("Run Animation")]
    public List<Sprite> nRun;
    public List<Sprite> neRun;
    public List<Sprite> eRun;
    public List<Sprite> seRun;
    public List<Sprite> sRun;

    [Header("Hurt Animation")]
    public List<Sprite> nHurt;
    public List<Sprite> neHurt;
    public List<Sprite> eHurt;
    public List<Sprite> seHurt;
    public List<Sprite> sHurt;

    [Header("Die Animation")]
    public List<Sprite> eDie;

    [Header("Light Attack Animation (1)")]
    public List<Sprite> nLightAttack_1;
    public List<Sprite> neLightAttack_1;
    public List<Sprite> eLightAttack_1;
    public List<Sprite> seLightAttack_1;
    public List<Sprite> sLightAttack_1;

    [Header("Light Attack Animation (2)")]
    public List<Sprite> nLightAttack_2;
    public List<Sprite> neLightAttack_2;
    public List<Sprite> eLightAttack_2;
    public List<Sprite> seLightAttack_2;
    public List<Sprite> sLightAttack_2;

    [Header("Light Attack Animation (3)")]
    public List<Sprite> nLightAttack_3;
    public List<Sprite> neLightAttack_3;
    public List<Sprite> eLightAttack_3;
    public List<Sprite> seLightAttack_3;
    public List<Sprite> sLightAttack_3;

    [Header("Light Attack Animation (4)")]
    public List<Sprite> nLightAttack_4;
    public List<Sprite> neLightAttack_4;
    public List<Sprite> eLightAttack_4;
    public List<Sprite> seLightAttack_4;
    public List<Sprite> sLightAttack_4;

    [Header("Light Attack Animation (5)")]
    public List<Sprite> nLightAttack_5;
    public List<Sprite> neLightAttack_5;
    public List<Sprite> eLightAttack_5;
    public List<Sprite> seLightAttack_5;
    public List<Sprite> sLightAttack_5;

    [Header("Heavy Attack Animation (1)")]
    public List<Sprite> nHeavyAttack_1;
    public List<Sprite> neHeavyAttack_1;
    public List<Sprite> eHeavyAttack_1;
    public List<Sprite> seHeavyAttack_1;
    public List<Sprite> sHeavyAttack_1;

    [Header("Heavy Attack Animation (2)")]
    public List<Sprite> nHeavyAttack_2;
    public List<Sprite> neHeavyAttack_2;
    public List<Sprite> eHeavyAttack_2;
    public List<Sprite> seHeavyAttack_2;
    public List<Sprite> sHeavyAttack_2;

    [Header("Heavy Attack Animation (3)")]
    public List<Sprite> nHeavyAttack_3;
    public List<Sprite> neHeavyAttack_3;
    public List<Sprite> eHeavyAttack_3;
    public List<Sprite> seHeavyAttack_3;
    public List<Sprite> sHeavyAttack_3;

    [Header("Heavy Attack Animation (4)")]
    public List<Sprite> nHeavyAttack_4;
    public List<Sprite> neHeavyAttack_4;
    public List<Sprite> eHeavyAttack_4;
    public List<Sprite> seHeavyAttack_4;
    public List<Sprite> sHeavyAttack_4;

    [Header("Heavy Attack Animation (5)")]
    public List<Sprite> nHeavyAttack_5;
    public List<Sprite> neHeavyAttack_5;
    public List<Sprite> eHeavyAttack_5;
    public List<Sprite> seHeavyAttack_5;
    public List<Sprite> sHeavyAttack_5;

    protected virtual void Start()
    {
        IinitSpriteBooks();
        spriteRenderer = GetComponent<SpriteRenderer>();
        selectedSpriteBook = IdleSpriteBook;
    }
    protected virtual void Update()
    {
        SetSprite();
        HandleSpriteFlip();
    }

    public void SetSprite()
    {
        List<Sprite> directionSprites = GetSpriteDirection();

        if (directionSprites != null && directionSprites.Count > 0)
        {
            float playTime = Time.time - idleTime;
            int totalFrames = (int)(playTime * frameRate);
            int frame = totalFrames % directionSprites.Count;

            // Stellen Sie sicher, dass der Index innerhalb des gültigen Bereichs liegt
            if (frame < 0)
                frame += directionSprites.Count;

            spriteRenderer.sprite = directionSprites[frame];
        }
        else
        {
            idleTime = Time.time;
        }
    }
    public List<Sprite> GetSpriteDirection()
    {
        if (lookDirection.magnitude > 0)
        {
            float angle = Vector2.SignedAngle(Vector2.up, lookDirection);
            int octant = Mathf.FloorToInt((angle + 22.5f) / 45f);
            octant = (octant + 8) % 8; // range [0, 7]

            switch (octant)
            {
                case 0: return selectedSpriteBook[0];
                case 1: return selectedSpriteBook[1];
                case 2: return selectedSpriteBook[2];
                case 3: return selectedSpriteBook[3];
                case 4: return selectedSpriteBook[4];
                case 5: return selectedSpriteBook[3];
                case 6: return selectedSpriteBook[2];
                case 7: return selectedSpriteBook[1];
            }
        }
        return IdleSpriteBook[2];
    }

    public void HandleSpriteFlip()
    {
        if (lookDirection.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (lookDirection.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }
    public void SetAttackDirection(Vector3 targetPosition)
    {
        // Setzen Sie die Angriffsrichtung basierend auf der Mausposition relativ zur Spielerposition
        Vector2 attackDirection = ((Vector2)targetPosition - (Vector2)transform.position).normalized;
        lookDirection = attackDirection;
    }
    public void SetSelectedSpriteBook(List<List<Sprite>> newSpriteBook)
    {
        selectedSpriteBook = newSpriteBook;
    }
    public void IinitSpriteBooks()
    {
        IdleSpriteBook = new List<List<Sprite>>()
        { nIdle, neIdle, eIdle, seIdle, sIdle};

        WalkSpriteBook = new List<List<Sprite>>()
        { nWalk, neWalk, eWalk, seWalk, sWalk};

        RunSpriteBook = new List<List<Sprite>>()
        { nRun, neRun, eRun, seRun, sRun};

        HurtSpriteBook = new List<List<Sprite>>()
        { nHurt, neHurt, eHurt, seHurt, sHurt};

        DieSpriteBook = new List<List<Sprite>>()
        {eDie};

        LightAttackSpriteBook_1 = new List<List<Sprite>>()
        {nLightAttack_1, neLightAttack_1, eLightAttack_1, seLightAttack_1, sLightAttack_1};

        LightAttackSpriteBook_2 = new List<List<Sprite>>()
        {nLightAttack_2, neLightAttack_2, eLightAttack_2, seLightAttack_2, sLightAttack_2};

        LightAttackSpriteBook_3 = new List<List<Sprite>>()
        {nLightAttack_3, neLightAttack_3, eLightAttack_3, seLightAttack_3, sLightAttack_3};

        HeavyAttackSpriteBook_1 = new List<List<Sprite>>()
        {nHeavyAttack_1, neHeavyAttack_1, eHeavyAttack_1, seHeavyAttack_1, sHeavyAttack_1};

        HeavyAttackSpriteBook_2 = new List<List<Sprite>>()
        {nHeavyAttack_2, neHeavyAttack_2, eHeavyAttack_2, seHeavyAttack_2, sHeavyAttack_2};

        HeavyAttackSpriteBook_3 = new List<List<Sprite>>()
        {nHeavyAttack_3, neHeavyAttack_3, eHeavyAttack_3, seHeavyAttack_3, sHeavyAttack_3};
    }
}
