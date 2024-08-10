using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Utility.FastNameplateBaker;

class Common
{
    public static BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    public static BindingFlags ValFlags = Flags | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.GetProperty;
    public static BindingFlags FunFlags = Flags;
    public static T GetVal<T>(object instance, string fieldName, Type type = null)
    {
        if (instance == null)
            return default;
        if (type == null)
            type = instance.GetType();
        FieldInfo field = type.GetField(fieldName, ValFlags);
        if (field != null)
            return (T)field.GetValue(instance);
        else if (type.BaseType != null)
            return GetVal<T>(instance, fieldName, type.BaseType);
        return default;
    }
    public static T SetVal<T>(object instance, string fieldName, T value) => (T)SetVal(instance, fieldName, value, null);
    public static object SetVal(object instance, string fieldName, object value = null, Type type = null)
    {
        if (type == null)
            type = instance.GetType();
        FieldInfo field = type.GetField(fieldName, ValFlags);
        if (field != null)
            field.SetValue(instance, value);
        else if (type.BaseType != null)
            SetVal(instance, fieldName, value, type.BaseType);
        return value;
    }
    
    public static void RunFunc(object instance, string functionname, object[] paramters = null, Type type = null)
    {
        MethodInfo protectedMethod = instance.GetType().GetMethod(functionname, FunFlags);

        protectedMethod.Invoke(instance, paramters);

    }
}