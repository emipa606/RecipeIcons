using HarmonyLib;
using UnityEngine;
using Verse;

namespace RecipeIcons.Patch;

[HarmonyPatch(typeof(FloatMenuOption), "DoGUI")]
internal class PatchFloatMenuOptionDoGUI
{
    private static readonly RecipeTooltip tooltip = new RecipeTooltip();

    private static void Postfix(FloatMenuOption __instance, Rect rect)
    {
        if (!RecipeIcons.settings.enableTooltip)
        {
            return;
        }

        if (!Mouse.IsOver(rect.TopPartPixels(rect.height - 1)))
        {
            return;
        }

        tooltip.ShowAt(
            __instance,
            Find.WindowStack.currentlyDrawnWindow.windowRect.x + rect.x + rect.width + 5,
            Find.WindowStack.currentlyDrawnWindow.windowRect.y + rect.y
        );
    }
}