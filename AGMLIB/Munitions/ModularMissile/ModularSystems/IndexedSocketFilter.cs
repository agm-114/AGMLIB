public class IndexedSocketFilter : MonoBehaviour, IFilterIndexed
{
    [SerializeField]
    protected List<string> _whitelist = new();
    [SerializeField]
    protected bool _whitelisteverything = false;
    [SerializeField]
    protected List<string> _blacklist = new();
    [SerializeField]
    protected bool _blacklisteverything = false;
    public List<string> Whitelist => _whitelist;
    public bool Whitelisteverything => _whitelisteverything;
    public List<string> Blacklist => _blacklist;
    public bool Blacklisteverything => _blacklisteverything;
    [SerializeField]
    protected bool _allowIllegal = false;
    public bool AllowIllegal => _allowIllegal;
    [SerializeField]
    protected bool _bypassFactionRestrictions = true;
    public bool BypassFactionRestrictions => _bypassFactionRestrictions;

    [SerializeField]
    protected int _index = 0;
    [SerializeField]
    protected bool _allindexes = false;
    public int Index => _index;
    public bool AllIndexes => _allindexes;

}
