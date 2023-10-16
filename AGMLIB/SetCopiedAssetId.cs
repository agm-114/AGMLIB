using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Utility
{
    public class SetCopiedAssetId : MonoBehaviour
    {
        public string copiedAssetId;

        [ContextMenu("Set copiedAssetId value")]
        void SetCopiedAssetIdValue()
        {
            NetworkPoolable networkPoolable = GetComponent<NetworkPoolable>();
            SetPrivateField(networkPoolable, "__copiedAssetId", this.copiedAssetId);
        }

        public static void SetPrivateField(object instance, string fieldName, object value)
        {
            static void SetPrivateFieldInternal(object instance, string fieldName, object value, System.Type type)
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
}