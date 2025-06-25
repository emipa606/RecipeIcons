using HarmonyLib;
using UnityEngine;
using Verse;

namespace RecipeIcons.Patch;

[HarmonyPatch(typeof(FloatMenuOption), nameof(FloatMenuOption.DoGUI))]
internal static class FloatMenuOption_DoGUI
{
    private static readonly RecipeTooltip tooltip = new();

    private static void Postfix(FloatMenuOption __instance, Rect rect)
    {
        if (!RecipeIcons.Settings.enableTooltip)
        {
            return;
        }

        if (!RecipeTooltip.IsRecipe(__instance))
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