using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataType : ScriptableObject
{
    [SerializeField] private string id;

    public Guid ID
    {
        get => string.IsNullOrEmpty(id) ? Guid.Empty : Guid.Parse(id);
        set => id = value.ToString();
    }

    protected void MarkDirty()
    {
        DatatService.instance.MarkDirty(this);
    }

    public virtual void ResetScriptableObject()
    {

    }

    /*#if UNITY_EDITOR

        protected virtual void OnEnable()
        {
            DataServiceEditor.Register(this);
        }

        protected virtual void OnValidate()
        {
            // si se duplica, se autoregistra con otro ID
            if (!string.IsNullOrEmpty(id))
            {
                var existing = DataServiceEditor.Get(ID);
                if (existing == null || existing != this)
                {
                    DataServiceEditor.Register(this);
                }
            }
            else
            {
                Debug.Log($"register {name}");
                DataServiceEditor.Register(this);
            }
        }
    #endif*/
}
