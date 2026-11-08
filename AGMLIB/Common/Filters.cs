namespace Lib
{
    public static class Filters
    {
        public static bool CheckComponent(HullPartResourceConnected hullComponent, List<string> Whitelist, List<string> Blacklist, bool defaultvalue)
        {

            if (hullComponent is not WeaponComponent weaponComponent)
            {
                return defaultvalue;
                //Debug.Log("skiping non weapon" + hullComponent.SaveKey);


            }


            //Debug.Log("checking weapon" + hullComponent.SaveKey + weaponComponent.Category);



            MunitionTags[] _compatibleAmmoTags = weaponComponent?.Internals().CompatibleAmmoTags;
            IEnumerable<string> taglist = _compatibleAmmoTags.ToList().ConvertAll(tag => tag.Subclass);
            string _statGroupSubtype = weaponComponent is ContinuousWeaponComponent continuous ? continuous.Internals().StatGroupSubtype : null;
            if (Blacklist.Intersect(taglist).Any())
                return false;
            if (Whitelist.Intersect(taglist).Any())
                return true;
            taglist = _compatibleAmmoTags.ToList().ConvertAll(tag => tag.Class);
            if (Blacklist.Intersect(taglist).Any())
                return false;
            if (Whitelist.Intersect(taglist).Any())
                return true;
            if (Blacklist.Contains(weaponComponent.Category))
                return false;
            if (Whitelist.Contains(weaponComponent.Category))
                return true;
            if (Blacklist.Contains(_statGroupSubtype))
                return false;
            if (Whitelist.Contains(_statGroupSubtype))
                return true;
            if (Blacklist.Contains(weaponComponent.CostBreakdownClass.ToString()))
                return false;
            if (Whitelist.Contains(weaponComponent.CostBreakdownClass.ToString()))
                return true;
            if (Blacklist.Contains(weaponComponent.CompoundingCostClass))
                return false;
            if (Whitelist.Contains(weaponComponent.CompoundingCostClass))
                return true;
            return defaultvalue;
        }
    }
}
