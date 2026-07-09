using System.Globalization;
using System.Text;

public static class RectTransformDebug
{
    public static string Dump(RectTransform rect, string? label = null, bool includeWorldCorners = true)
    {
        if (rect == null)
        {
            return $"{label ?? "RectTransform"}: <null>";
        }

        StringBuilder builder = new();
        AppendRect(builder, rect, label ?? rect.name);
        if (includeWorldCorners)
        {
            AppendWorldCorners(builder, rect, label ?? rect.name);
        }

        return builder.ToString();
    }

    public static string DumpHierarchy(RectTransform rect, string? label = null, bool includeWorldCorners = true)
    {
        if (rect == null)
        {
            return $"{label ?? "RectTransform"}: <null>";
        }

        StringBuilder builder = new();
        AppendRect(builder, rect, label ?? rect.name);
        if (includeWorldCorners)
        {
            AppendWorldCorners(builder, rect, label ?? rect.name);
        }

        Transform current = rect.parent;
        while (current != null)
        {
            RectTransform parentRect = current as RectTransform;
            if (parentRect == null)
            {
                builder.AppendLine(string.Format(
                    CultureInfo.InvariantCulture,
                    "Parent: name='{0}' rect=<not RectTransform> scale={1}",
                    current.name,
                    FormatVector3(current.lossyScale)));
            }
            else
            {
                AppendRect(builder, parentRect, "Parent");
            }

            current = current.parent;
        }

        return builder.ToString();
    }

    public static void Log(RectTransform rect, string header = "RectTransform dump", bool includeWorldCorners = true)
    {
        Debug.Log($"{header}\n{Dump(rect, rect == null ? null : rect.name, includeWorldCorners)}");
    }

    public static void LogHierarchy(RectTransform rect, string header = "RectTransform hierarchy dump", bool includeWorldCorners = true)
    {
        Debug.Log($"{header}\n{DumpHierarchy(rect, rect == null ? null : rect.name, includeWorldCorners)}");
    }

    private static void AppendRect(StringBuilder builder, RectTransform rect, string label)
    {
        builder.AppendLine(string.Format(
            CultureInfo.InvariantCulture,
            "{0}: name='{1}' rect={2} anchors={3}-{4} pivot={5} offsetMin={6} offsetMax={7} anchored={8} scale={9}",
            label,
            rect.name,
            FormatVector2(rect.rect.size),
            FormatVector2(rect.anchorMin),
            FormatVector2(rect.anchorMax),
            FormatVector2(rect.pivot),
            FormatVector2(rect.offsetMin),
            FormatVector2(rect.offsetMax),
            FormatVector2(rect.anchoredPosition),
            FormatVector3(rect.lossyScale)));
    }

    private static void AppendWorldCorners(StringBuilder builder, RectTransform rect, string label)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        builder.AppendLine(string.Format(
            CultureInfo.InvariantCulture,
            "{0} world corners: bl={1} tl={2} tr={3} br={4}",
            label,
            FormatVector3(corners[0]),
            FormatVector3(corners[1]),
            FormatVector3(corners[2]),
            FormatVector3(corners[3])));
    }

    private static string FormatVector2(Vector2 value)
    {
        return string.Format(CultureInfo.InvariantCulture, "({0:0.###},{1:0.###})", value.x, value.y);
    }

    private static string FormatVector3(Vector3 value)
    {
        return string.Format(CultureInfo.InvariantCulture, "({0:0.###},{1:0.###},{2:0.###})", value.x, value.y, value.z);
    }
}
