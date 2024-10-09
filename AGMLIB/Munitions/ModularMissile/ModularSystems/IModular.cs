public enum WeaponRole
{
    Unlocked = -1,
    Offensive = 0,
    Defensive = 1
}
public enum LaunchType
{
    Unlocked = -1,
    Hot = 1,
    Cold = 0
}
public enum TargetLost
{
    Unlocked = -1,
    Resume = 0,
    SelfDestruct = 1
}
public enum Terminal
{
    Unlocked = -1,
    None = 0,
    Weave = 1,
    Corkscrew = 2
}
public enum Guidance
{
    Unlocked = -1,
    MinimumAngle = 0,
    FreeApproach = 1
}
public interface IModular
{
    public List<ScriptableObject> Modules { get; }
}

public interface ILimited
{
    public Dictionary<string, int> RestrictedOptions { get; }
    //public Dictionary<string, bool> LockedOptions { get; }
}

public class Modular
{
    public static IFilterIndexed FindIndexedFilter(List<IFilterIndexed> filters, int index = -1)
    {
        if (filters == null || filters.Count <= 0)
            return null;
        List<IFilterIndexed> testlist = filters.FindAll(filter => filter.Index == index);
        if (testlist.Count > 0)
            return testlist.First();
        testlist = filters.FindAll(filter => filter.AllIndexes);
        return testlist.Count > 0 ? testlist.First() : null;

    }

    public static IFilterIndexed FindIndexedFilter(IEnumerable<IFilterIndexed> filters, int index = -1) => FindIndexedFilter(filters.ToList(), index);

    public static IFilterIndexed Default => ScriptableObject.CreateInstance<ScriptableFilter>();
}


/*

SequenceOption _disabledOption = new SequenceOption
{
    Text = "HOT",
    TextColor = GameColors.ColorName.Red
};
Common.SetVal(avionicssettings2, "_disabledOption", _disabledOption);
//_launchButton.SetDisabledText("HOT");
_launchButton.SetEnabled(enabled: false);
*/
//, MissileComponentDescriptor component, SequentialButton ____launchButton
//____launchButton.SetEnabled(enabled: false);
//SequentialButton _launchButton = Common.GetVal<SequentialButton>(avionicssettings, "_launchButton");
/*
SequenceOption[] _options = Common.GetVal<SequenceOption[]>(avionicssettings, "_options");
if (_options == null)
{
    //Debug.LogError("Null Option ");

}
else
{
    SequenceOption _disabledOption = _options[1];
    Common.SetVal(avionicssettings, "_disabledOption", _disabledOption);
}*/

//_launchButton.SetOptionWithoutNotify(1, mixed: false);
//_launchButton.SetEnabled(false);
//_launchButton.SetDisabledText("HOT");
//_launchButton.SetEnabled(enabled: false);

