using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EInputTargetMode
{
    Game,
    UserInterface
}

public static class InputManager
{
    private static uint userInterfaceInputCounter = 0;

    public static void PushEnterUIMode()
    {
        Debug.Log("pushed ui mode");
        userInterfaceInputCounter++;
    }

    public static void PopEnterUIMode()
    {
        if (!IsInUIMode())
            throw new Exception("Input Manager is not in UI mode and thus cannot pop UI mode.");

        Debug.Log("popped ui mode");
        userInterfaceInputCounter--;
    }

    public static bool IsInUIMode()
    {
        return userInterfaceInputCounter > 0;
    }

    public static void ResetUIMode()
    {
        Debug.Log("reset ui mode");
        userInterfaceInputCounter = 0;
    }

    public static bool GetKeyDown(KeyCode key, EInputTargetMode mode = EInputTargetMode.Game)
    {
        if (mode == EInputTargetMode.UserInterface && !IsInUIMode())
            return false;

        if (mode == EInputTargetMode.Game && IsInUIMode())
            return false;

        return Input.GetKeyDown(key);
    }

    public static bool GetKeyUp(KeyCode key, EInputTargetMode mode = EInputTargetMode.Game)
    {
        if (mode == EInputTargetMode.UserInterface && !IsInUIMode())
            return false;

        if (mode == EInputTargetMode.Game && IsInUIMode())
            return false;

        return Input.GetKeyUp(key);
    }

    public static bool GetKey(KeyCode key, EInputTargetMode mode = EInputTargetMode.Game)
    {
        if (mode == EInputTargetMode.UserInterface && !IsInUIMode())
            return false;

        if (mode == EInputTargetMode.Game && IsInUIMode())
            return false;

        return Input.GetKey(key);
    }

    public static bool GetMouseButtonDown(int button, EInputTargetMode mode = EInputTargetMode.Game)
    {
        if (mode == EInputTargetMode.UserInterface && !IsInUIMode())
            return false;

        if (mode == EInputTargetMode.Game && IsInUIMode())
            return false;

        return Input.GetMouseButtonDown(button);
    }

    public static bool GetMouseButtonUp(int button, EInputTargetMode mode = EInputTargetMode.Game)
    {
        if (mode == EInputTargetMode.UserInterface && !IsInUIMode())
            return false;

        if (mode == EInputTargetMode.Game && IsInUIMode())
            return false;

        return Input.GetMouseButtonUp(button);
    }

    public static bool GetMouseButton(int button, EInputTargetMode mode = EInputTargetMode.Game)
    {
        if (mode == EInputTargetMode.UserInterface && !IsInUIMode())
            return false;

        if (mode == EInputTargetMode.Game && IsInUIMode())
            return false;

        return Input.GetMouseButton(button);
    }

}
