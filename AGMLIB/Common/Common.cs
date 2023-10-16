using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

class Common
{

    public static T GetVal<T>(object instance, string fieldName)
    {
        static object GetPrivateFieldInternal(object instance, string fieldName, Type type)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                return (T)field.GetValue(instance);
            }
            else if (type.BaseType != null)
            {
                return (T)GetPrivateFieldInternal(instance, fieldName, type.BaseType);
            }
            else
            {
                return null;
            }
        }

        return (T)GetPrivateFieldInternal(instance, fieldName, instance.GetType());
    }

    public static object GetPrivateField(object instance, string fieldName)
    {
        static object GetPrivateFieldInternal(object instance, string fieldName, Type type)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                return field.GetValue(instance);
            }
            else if (type.BaseType != null)
            {
                return GetPrivateFieldInternal(instance, fieldName, type.BaseType);
            }
            else
            {
                return null;
            }
        }

        return GetPrivateFieldInternal(instance, fieldName, instance.GetType());
    }
    public static void SetVal(System.Object instance, string fieldName, System.Object value)
    {
        static void SetPrivateFieldInternal(System.Object instance, string fieldName, System.Object value, Type type)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                field.SetValue(instance, value);
                return;
            }
            else if (type.BaseType != null)
            {
                SetPrivateFieldInternal(instance, fieldName, value, type.BaseType);
                return;
            }
        }

        SetPrivateFieldInternal(instance, fieldName, value, instance.GetType());
    }
    public static void SetPrivateField(System.Object instance, string fieldName, System.Object value)
    {
        static void SetPrivateFieldInternal(System.Object instance, string fieldName, System.Object value, Type type)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                field.SetValue(instance, value);
                return;
            }
            else if (type.BaseType != null)
            {
                SetPrivateFieldInternal(instance, fieldName, value, type.BaseType);
                return;
            }
        }

        SetPrivateFieldInternal(instance, fieldName, value, instance.GetType());
    }
}