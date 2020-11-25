using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "String", menuName = "Value/String")]

public class StringValue : ScriptableObject, ISerializationCallbackReceiver
{
    public string initialValue;
    public string RuntimeValue;
    private StringValue saveWorldName;

    public StringValue(StringValue value)
    {
        initialValue = value.initialValue;
        RuntimeValue = value.initialValue;
    }

    public void OnAfterDeserialize()
    {
        initialValue = RuntimeValue;
    }

    public void OnBeforeSerialize()
    {

    }
}
