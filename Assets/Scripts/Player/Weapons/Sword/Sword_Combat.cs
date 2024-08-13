using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Combat : WeaponBase
{
    public bool isSlashUnlocked = false;
    public bool isWhirlwindUnlocked = false;

    public override void Attack(bool isLightAttack)
    {
        if (isLightAttack)
        {
            if (isSlashUnlocked)
            {
                PerformSlash();
            }
            else
            {
                PerformBasicAttack();
            }
        }
        else
        {
            if (isWhirlwindUnlocked)
            {
                PerformWhirlwind();
            }
            else
            {
                PerformHeavyAttack();
            }
        }
    }

    private void PerformBasicAttack()
    {
        Debug.Log("Basic Sword Attack");
    }

    private void PerformSlash()
    {
        Debug.Log("Sword Slash Attack");
    }

    private void PerformHeavyAttack()
    {
        Debug.Log("Heavy Sword Attack");
    }

    private void PerformWhirlwind()
    {
        Debug.Log("Sword Whirlwind Attack");
    }

    // Methoden zum Freischalten von Fähigkeiten
    public void UnlockSlash()
    {
        isSlashUnlocked = true;
    }

    public void UnlockWhirlwind()
    {
        isWhirlwindUnlocked = true;
    }
}
