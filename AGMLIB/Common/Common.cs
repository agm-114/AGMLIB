using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

class Common
{

    public static T GetVal<T>(object instance, string fieldName, Type type = null)
    {
        if (type == null)
            type = instance.GetType();
        FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
            return (T)field.GetValue(instance);
        else if (type.BaseType != null)
            return (T)GetVal<T>(instance, fieldName, type.BaseType);
        return default(T);
    }
    public static void SetVal(System.Object instance, string fieldName, System.Object value, Type type = null)
    {
        if (type == null)
            type = instance.GetType();
        FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
            field.SetValue(instance, value);
        else if (type.BaseType != null)
            SetVal(instance, fieldName, value, type.BaseType);
    }

}