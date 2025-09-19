using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class EnableDisableComponent : MonoBehaviour
{
    public void SetComponentEnabled<T>(bool x) where T : Component
    {
        var component = GetComponent<T>();
        if (component == null)
        {
            Debug.LogWarning($"{typeof(T).Name} no encontrado en {gameObject.name}");
            return;
        }

        // Intentar encontrar propiedad "enabled"
        var prop = typeof(T).GetProperty("enabled", BindingFlags.Public | BindingFlags.Instance);
        if (prop != null && prop.PropertyType == typeof(bool))
        {
            prop.SetValue(component, x);
        }
        else
        {
            Debug.LogWarning($"{typeof(T).Name} no tiene propiedad 'enabled'");
        }
    }

}
