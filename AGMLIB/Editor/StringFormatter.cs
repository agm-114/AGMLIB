using static Utility.GameColors;
public enum BasicTag
{
    None,
    Bold = 'b',
    Italic = 'i',
    StrikeThrough = 's',
    Underline = 'u',
    NoBreak,
    AlignLeft,
    AlignCenter,
    AlignRight,
    AlignJustified,
    AlignFlushed,
    Align,
    CapsUpper,
    CapsLower,
    CapsSmallcaps,
    Subscript,
    Superscript
}

public enum CustomColor
{
    Red,
    Orange,
    Yellow,
    Green,
    LightBlue,
    DarkBlue,
    Purple,
    Gray,
    White,
    LightGray,
    RedTextColor,
    OrangeTextColor,
    YellowTextColor,
    GreenTextColor,
    LightBlueTextColor,
    DarkBlueTextColor,
    FlavorTextColor
}

public class StringFormatter : MonoBehaviour
{
    static protected BasicTag[] _defaultlist = { BasicTag.Bold, BasicTag.Italic };
    [SerializeField]
    protected List<BasicTag> _taglist = new();
    [SerializeField]
    protected bool _colortext = false;
    [SerializeField]
    protected CustomColor _textcolor = CustomColor.White;
    public CustomColor Color { get => _textcolor; set { _textcolor = value; _colortext = true; } }
    public string Text { get => _text; set => _text = value; }

    [SerializeField]
    protected List<StringFormatter> _prefixes = new();
    [TextArea(5, 10)]
    [SerializeField]
    protected string _text = "";//https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/manual/RichText.html
    [SerializeField]
    protected List<StringFormatter> _postfixes = new();
    [SerializeField] protected TMP_FontAsset? _font;
    public TMP_FontAsset? Font => _font;
    public static string GetCoreTag(BasicTag tag) => tag switch
    {
        BasicTag.None => "",
        BasicTag.NoBreak => "nobr",
        BasicTag.Align => "align",
        BasicTag.AlignLeft => "align",
        BasicTag.AlignCenter => "align",
        BasicTag.AlignRight => "align",
        BasicTag.AlignJustified => "align",
        BasicTag.AlignFlushed => "align",
        BasicTag.CapsUpper => "lowercase",
        BasicTag.CapsLower => "uppercase",
        BasicTag.CapsSmallcaps => "smallcaps",
        BasicTag.Subscript => "sub",
        BasicTag.Superscript => "sup",
        BasicTag.Bold => "b",
        BasicTag.Italic => "i",
        BasicTag.StrikeThrough => "s",
        BasicTag.Underline => "u",
        _ => ""
    };
    public static string GetEntryTag(BasicTag tag, string value = "1px") => "<" + GetCoreTag(tag) + "=" + value + '>';

    public static string GetEntryTag(BasicTag tag) => tag switch
    {
        BasicTag.AlignLeft => GetEntryTag(BasicTag.Align, "\"left\""),
        BasicTag.AlignCenter => GetEntryTag(BasicTag.Align, "\"center\""),
        BasicTag.AlignRight => GetEntryTag(BasicTag.Align, "\"right\""),
        BasicTag.AlignJustified => GetEntryTag(BasicTag.Align, "\"justified\""),
        BasicTag.AlignFlushed => GetEntryTag(BasicTag.Align, "\"flush\""),
        _ => "<" + GetCoreTag(tag) + '>',
    };
    public static string GetExitTag(BasicTag tag) => "</" + GetCoreTag(tag) + '>';

    public static string MergeStrings(List<StringFormatter> strings)
    {
        string returnstring = string.Empty;
        foreach (StringFormatter sb in strings)
            returnstring += sb.ToString();
        return returnstring;
    }

    public static string GetColorTag(CustomColor color)
    {
        var textColor = color switch
        {
            CustomColor.Red => GameColors.GetTextColor(ColorName.Red),
            CustomColor.Orange => GameColors.GetTextColor(ColorName.Orange),
            CustomColor.Yellow => GameColors.GetTextColor(ColorName.Yellow),
            CustomColor.Green => GameColors.GetTextColor(ColorName.Green),
            CustomColor.LightBlue => GameColors.GetTextColor(ColorName.LightBlue),
            CustomColor.DarkBlue => GameColors.GetTextColor(ColorName.DarkBlue),
            CustomColor.Purple => GameColors.GetTextColor(ColorName.Purple),
            CustomColor.Gray => GameColors.GetTextColor(ColorName.Gray),
            CustomColor.LightGray => GameColors.GetTextColor(ColorName.LightGray),
            CustomColor.White => GameColors.GetTextColor(ColorName.White),
            CustomColor.RedTextColor => GameColors.RedTextColor,
            CustomColor.OrangeTextColor => GameColors.OrangeTextColor,
            CustomColor.YellowTextColor => GameColors.YellowTextColor,
            CustomColor.GreenTextColor => GameColors.GreenTextColor,
            CustomColor.LightBlueTextColor => GameColors.LightBlueTextColor,
            CustomColor.DarkBlueTextColor => GameColors.DarkBlueTextColor,
            CustomColor.FlavorTextColor => GameColors.FlavorTextColor,
            _ => GameColors.GetTextColor(ColorName.White),
        };
        return "<color=" + textColor + ">";
    }
    public override string ToString()
    {
        string returnstring = string.Empty;
        if (_colortext || _textcolor != CustomColor.White)
            returnstring += GetColorTag(_textcolor);
        foreach (BasicTag tag in _taglist)
            returnstring += GetEntryTag(tag);
        returnstring += MergeStrings(_prefixes);
        returnstring += _text;
        returnstring += MergeStrings(_postfixes);
        foreach (BasicTag tag in _taglist)
            returnstring += GetExitTag(tag);
        if (_colortext)
            returnstring += "</color>";
        return returnstring;
    }

}
