using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CopyPasteIdeaInactiveConditions :  EditorWindow
{
    [MenuItem("Tools/Transfer Inactive Conditionals")]
    public static void TransferData()
    {
        // Busca todos los GeneratorActionController en el proyecto (en prefabs, escenas, etc.)
        GeneratorActionController[] controllers = Resources.FindObjectsOfTypeAll<GeneratorActionController>();

        int modified = 0;

        foreach (var controller in controllers)
        {
            if (controller == null) continue;

            // Obtenemos el ActionToAdd
            StateEnum state = controller.GetActionToAdd();
            if (state == null) continue;

            // Obtenemos las InactiveConditionals del controller
            List<ConditionalClass> list = controller.GetInactiveConditions();
            if (list == null || list.Count == 0) continue;

            // Aplicamos la lista al ScriptableObject
            state.SetInactiveConditional(new List<ConditionalClass>(list));

            // Marcamos como modificado para que Unity lo guarde
            EditorUtility.SetDirty(state);
            modified++;
        }

        // Guardamos cambios
        AssetDatabase.SaveAssets();
        Debug.Log($"Transferencia completada. Se actualizaron {modified} StateEnums.");
    }
}
