using UnityEngine;
using Verse;

namespace RecipeIcons;

public class Settings : ModSettings
{
    public bool disableSmallMenus = true;
    public bool enableTooltip = true;

    public bool showMod = true;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref enableTooltip, "enableTooltip");
        Scribe_Values.Look(ref disableSmallMenus, "disableSmallMenus");
        Scribe_Values.Look(ref showMod, "showMod");
    }

    public void DoSettingsWindowContents(Rect inRect)
    {
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(inRect);
        listing_Standard.CheckboxLabeled("RecipeIconsEnableTooltipName".Translate(), ref enableTooltip,
            "RecipeIconsEnableTooltipDesc".Translate());
        listing_Standard.CheckboxLabeled("RecipeIconsDisableSmallMenusName".Translate(), ref disableSmallMenus,
            "RecipeIconsDisableSmallMenusDesc".Translate());
        listing_Standard.CheckboxLabeled("RecipeIconsShowModName".Translate(), ref showMod,
            "RecipeIconsShowModDesc".Translate());
        listing_Standard.End();
    }
}