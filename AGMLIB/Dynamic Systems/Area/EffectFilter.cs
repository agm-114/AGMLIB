using Munitions.ModularMissiles;

namespace AGMLIB.DynamicSystems.Area
{
    public class EffectFilter : ShipState
    {
        public bool AffectFriendlies = true;
        public bool AffectEnemies = true;
        public bool AffectSelf = true;
        public List<string> Whitelist = new();
        public List<string> Blacklist = new();
        public bool Default = true;


        public bool ValidListTarget(Ship target)
        {
            if (target == null || Blacklist.Contains(target.Hull.SaveKey))
                return false;

            if (Whitelist.Contains(target.Hull.SaveKey))
                return true;
            return Default;
        }
        public bool ValidIFFTarget(Ship target)
        {
            if (target == Ship)
                return AffectSelf;
            return ValidIFFTarget(target?.Controller?.OwnedBy);

        }
        public bool ValidIFFTarget(ModularMissile target) => ValidIFFTarget(target?.OwnedBy);
        public bool ValidIFFTarget(IPlayer owner)
        {
            Game.IFF IFFstatus = ShipController?.GetIFF(owner) ?? Game.IFF.None;
            if (!AffectFriendlies && IFFstatus != Game.IFF.Enemy)
                return false;
            else if (!AffectEnemies && IFFstatus == Game.IFF.Enemy)
                return false;
            return true;
        }
        public bool ValidTarget(Ship target)
        {
            if (target == null)
                return false;
            return ValidListTarget(target) && ValidIFFTarget(target);
        }
    }
}
