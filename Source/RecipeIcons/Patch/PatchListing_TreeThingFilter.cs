namespace RecipeIcons.Patch;

//[HarmonyPatch(typeof(Listing_TreeThingFilter), "DoThingDef")]
//internal class PatchListing_TreeThingFilter
//{
//    private static void Prefix(Listing_TreeThingFilter __instance, float ___curY, ThingDef tDef, ref int nestLevel)
//    {
//        nestLevel++;

//        var icon = Icon.getIcon(tDef);
//        if (icon == Icon.missing)
//        {
//            return;
//        }

//        var rect = new Rect((__instance.nestIndentWidth * nestLevel) - 6f, ___curY, 20f, 20f);
//        icon.Draw2D(rect);
//        GUI.color = Color.white;
//    }
//}