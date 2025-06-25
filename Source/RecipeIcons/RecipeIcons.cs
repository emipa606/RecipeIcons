using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace RecipeIcons;

public class RecipeIcons : Mod
{
    public static Settings Settings;

    public RecipeIcons(ModContentPack pack) : base(pack)
    {
        var harmony = new Harmony("com.github.automatic1111.recipeicons");
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        Settings = GetSettings<Settings>();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        Settings.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "RecipeIconsTitle".Translate();
    }
}