/// <summary>
/// Legacy defensive replacement for the missing-resource tooltip formatter.
///
/// Native <see cref="HullPartResourceConnected.GetMissingResources"/> enumerates
/// <c>_requiredResources</c> and reads <c>ResourceValue.Resource.Name</c> for
/// every unmet requirement. Modded components have historically reached this
/// UI path with incomplete or unregistered resource metadata, causing tooltip
/// generation to throw. This replacement preserves the native bullet-list
/// format while skipping entries that cannot be evaluated or named.
///
/// This does not provide resources, change demand, or restore component
/// functionality. It only prevents malformed resource metadata from breaking
/// the component-status UI. Because it replaces the method globally and hides
/// bad data, retain it only while affected content still requires the fallback.
/// </summary>
[HarmonyPatch(typeof(HullPartResourceConnected), "GetMissingResources")]
internal static class HullPartResourceConnectedGetMissingResourcesPatch
{
    private static bool Prefix(ResourceValue[] ____requiredResources, ref string __result)
    {
        Common.LogPatch();
        string missingResources = "";

        if (____requiredResources == null)
        {
            __result = missingResources;
            return false;
        }

        foreach (ResourceValue resourceValue in ____requiredResources)
        {
            try
            {
                if (resourceValue.HasAll)
                    continue;

                try
                {
                    missingResources += $"  - {resourceValue.Resource.Name}\n";
                }
                catch
                {
                    // Preserve the native result replacement even when a resource has incomplete metadata.
                }
            }
            catch
            {
                // Preserve the native result replacement when a resource entry cannot be evaluated.
            }
        }

        __result = missingResources;
        return false;
    }
}
