using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]

public class StringValue : ScriptableObject, ISerializationCallbackReceiver
{
    public string initialValue;

    [HideInInspector]
    public string RuntimeValue;

    public void OnAfterDeserialize()
    {
        RuntimeValue = initialValue;
    }

    public void OnBeforeSerialize()
    {

    }
}
