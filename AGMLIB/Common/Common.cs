using System.Reflection;

public static class Common
{
    public static bool SkipFunction => false;
    public static bool RunFunction => true;

    public static bool InventoryDebug => false;

    public static string Cat = "≽^•⩊•^≼";

    public static BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    public static BindingFlags ValFlags = Flags | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.GetProperty;
    public static BindingFlags FunFlags = Flags;
    public static T GetVal<T>(object instance, string fieldName, Type type = null)
    {
        if (instance == null)
            return default;
        if (type == null)
            type = instance.GetType();

        if (type.GetField(fieldName, ValFlags) is FieldInfo field)
            return (T)field.GetValue(instance);
        else if (type.GetProperty(fieldName, ValFlags) is PropertyInfo property)
            return (T)property.GetValue(instance);
        else if (type.BaseType != null)
            return GetVal<T>(instance, fieldName, type.BaseType);
        return default;
    }
    public static T SetVal<T>(object instance, string fieldName, T value) => (T)SetVal(instance, fieldName, value, null);
    public static object SetVal(object instance, string fieldName, object value = null, Type type = null)
    {
        if (type == null)
            type = instance.GetType();

        if (type.GetField(fieldName, ValFlags) is FieldInfo field)
            field.SetValue(instance, value);
        else if (type.GetProperty(fieldName, Flags) is PropertyInfo property)
            property.SetValue(instance, value);
        else if (type.BaseType != null)
            SetVal(instance, fieldName, value, type.BaseType);
        return value;
    }

    public static void RunFunc(object instance, string functionname, object[] paramters = null, Type type = null)
    {
        MethodInfo protectedMethod = instance.GetType().GetMethod(functionname, FunFlags);

        protectedMethod.Invoke(instance, paramters);

    }

    public static void Trace(object pobject, object error) => SilentPrint(pobject, error);
    public static void Trace(object message) => SilentPrint(message, "");
    public static void Hint(object pobject, object error) => Print(pobject, error, CustomColor.LightBlueTextColor);
    public static void Hint(object message) => Print(message, CustomColor.LightBlueTextColor);

    public static void SilentPrint(object pobject, object error)
    {
        string name = string.Empty;
        if (pobject is HullComponent hullComponent)
        {
            pobject = hullComponent.gameObject;
        }
        else if (pobject is GameObject go)
        {

            name += go.GetComponentInChildren<HullComponent>()?.ComponentName ?? "";
            name += go.name;

        }
        else
        {
            name = pobject.ToString();
        }


        Debug.Log(name + " " + error.ToString());

    }
    public static void Print(object pobject, object error, CustomColor color)
    {
        string name = string.Empty;
        if (pobject is HullComponent hullComponent)
        {
            pobject = hullComponent.gameObject;
        }
        else if (pobject is GameObject go)
        {

            name += go.GetComponentInChildren<HullComponent>()?.ComponentName ?? "";
            name += go.name;

        }

        Print(name + " " + error.ToString(), color);
    }

    public static void Print(object message, CustomColor color)
    {
        StringFormatter formatter = new StringFormatter();
        formatter.Color = CustomColor.LightBlueTextColor;
        formatter.Text = message.ToString();
        Debug.LogError("" + formatter);
    }


    public static void LogPatch(object? message = null)
    {
        return;
        if (message == null)
            message = "patch triggered";
        System.Diagnostics.StackFrame frame = new System.Diagnostics.StackFrame(1);
        string className = frame.GetMethod().DeclaringType.Name;
        string methodName = frame.GetMethod().Name;

        StringFormatter formatter = new StringFormatter();
        formatter.Color = CustomColor.OrangeTextColor;
        formatter.Text = $"{className}.{methodName}: {message}";

        Debug.LogError("" + formatter);
    }

    //    list: List<T> to resize
    //    size: desired new size
    // element: default value to insert

    public static Vector3 Closest(this Vector3 target,  Vector3 a, Vector3 b)
    {
        float distanceToA = Vector3.Distance(a, target);
        float distanceToB = Vector3.Distance(b, target);

        return distanceToA < distanceToB ? a : b;
    }
    public static Vector3 Furthest(this Vector3 target, Vector3 a, Vector3 b)
    {
        float distanceToA = Vector3.Distance(a, target);
        float distanceToB = Vector3.Distance(b, target);

        return distanceToA > distanceToB ? a : b;
    }

}

public static class CommonNonStatic
{
    public static void Resize<T>(this List<T> list, int size, T element = default(T))
    {
        int count = list.Count;

        if (size < count)
        {
            list.RemoveRange(size, count - size);
        }
        else if (size > count)
        {
            if (size > list.Capacity)   // Optimization
                list.Capacity = size;

            list.AddRange(Enumerable.Repeat(element, size - count));
        }
    }

    public static void Desize<T>(this List<T> list, int size)
    {
        int count = list.Count;

        if (size < count)
        {
            list.RemoveRange(size, count - size);
        }

    }

    public static IEnumerable<TSource> WhereNotNull<TSource>(this IEnumerable<TSource> source)
    {
        return source.Where(item => item != null);
    }
}

public class MonoComponent : MonoBehaviour
{
    public void Trace(object message)
    {
        //Debug.LogError(message);
        Common.Trace(this, message);
    }

}