﻿public enum AdvancedTag
{
    Alpha,
    Indentation,
    Height,
    Color,
    Font,
    FontWeight,
    Size,
    Margin,
    MarginRight,
    MarginLeft,
    Mark,
    Monospace,
    Pos,
    Rotate,
    Width,
    Align
}
public class ExperimentalStringFormatter : StringFormatter
{
    [SerializeField]
    protected List<AdvancedTag> _advancedtaglist = new();
    [SerializeField]
    protected List<String> _advancedtaglistvalues = new();

    public IEnumerable<KeyValuePair<AdvancedTag, string>> AdvancedTagList => _advancedtaglist.Zip(_advancedtaglistvalues, (key, value) => new KeyValuePair<AdvancedTag, string>(key, value));

    public static string GetCoreTag(AdvancedTag tag) => tag switch
    {
        AdvancedTag.Alpha => "alpha",
        AdvancedTag.Indentation => "indent",
        AdvancedTag.Height => "line-height",
        AdvancedTag.Color => "color",
        AdvancedTag.Font => "font",
        AdvancedTag.FontWeight => "font-weight",
        AdvancedTag.Size => "size",
        AdvancedTag.Margin => "margin",
        AdvancedTag.MarginRight => "margin-right",
        AdvancedTag.MarginLeft => "margin-left",
        AdvancedTag.Mark => "mark",
        AdvancedTag.Monospace => "mspace",
        AdvancedTag.Pos => "pos",
        AdvancedTag.Rotate => "rotate",
        AdvancedTag.Width => "width",
        AdvancedTag.Align => "align",
        _ => "",
    };
    public static string GetEntryTag(AdvancedTag tag, string value = "1px") => "<" + GetCoreTag(tag) + "=" + value + '>';
    public static string GetExitTag(AdvancedTag tag, string _ = "") => "</" + GetCoreTag(tag) + '>';

    public override string ToString()
    {
        base.ToString();
        string returnstring = string.Empty;
        if (_colortext)
            returnstring += GetColorTag(_textcolor);
        foreach (BasicTag tag in _taglist)
            returnstring += GetEntryTag(tag);
        foreach (KeyValuePair<AdvancedTag, string> tag in AdvancedTagList)
            returnstring += GetEntryTag(tag.Key, tag.Value);
        returnstring += MergeStrings(_prefixes);
        returnstring += _text;
        returnstring += MergeStrings(_postfixes);
        foreach (BasicTag tag in _taglist)
            returnstring += GetExitTag(tag);
        foreach (KeyValuePair<AdvancedTag, string> tag in AdvancedTagList)
            returnstring += GetExitTag(tag.Key, tag.Value);
        if (_colortext)
            returnstring += "</color>";
        return returnstring;
    }
}
