using HarmonyLib;
using UnityEngine;
using Verse;

namespace RecipeIcons.Patch;

[HarmonyPatch(typeof(Widgets), "DefIcon", typeof(Rect), typeof(Def), typeof(ThingDef), typeof(float),
    typeof(ThingStyleDef), typeof(bool), typeof(Color?), typeof(Material), typeof(int?))]
internal class PatchWidgetsDefIcon
{
    private static readonly float targetWidth = 28f;

    private static bool ShiftIsHeld => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

    private static bool Prefix(Rect rect, Def def)
    {
        // don't do anything for larger icons in architect UI etc.
        if (rect.width != rect.height || rect.width > targetWidth)
        {
            return true;
        }

        var icon = Icon.getIcon(def);
        if (icon == Icon.missing)
        {
            return true;
        }

        var widthDiff = targetWidth - rect.width;
        rect = new Rect(rect.x - (widthDiff / 2), rect.y - (widthDiff / 2), targetWidth, targetWidth);

        var color = GUI.color;
        var shouldRunOriginalFunc = !icon.Draw(rect);
        GUI.color = color;

        return shouldRunOriginalFunc;
    }
}