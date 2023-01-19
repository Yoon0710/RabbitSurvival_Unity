using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameUtil
{
    public enum enColor
    {
        Red = 0,
        Green,
        Blue,
        White
    }

    public static Color GetColor(enColor color)
    {
        switch (color)
        {
            case enColor.Red: return Color.red;
            case enColor.Blue: return new Color(160f / 255f, 160f / 255f, 255f / 255f, 255f / 255f);
            case enColor.Green: return Color.green;
            case enColor.White: return Color.white;
            default: return Color.red;
        }
    }
}
