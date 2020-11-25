using UnityEngine;

[CreateAssetMenu(fileName = "Bool Array", menuName = "Value/Bool Array")]
[System.Serializable]

public class BoolArrayValue : ScriptableObject, ISerializationCallbackReceiver
{
    public bool[] initialValue;

    [HideInInspector]
    public bool[] RuntimeValue;

    public void OnAfterDeserialize()
    {
        RuntimeValue = initialValue;
    }

    public void OnBeforeSerialize()
    {

    }
}
