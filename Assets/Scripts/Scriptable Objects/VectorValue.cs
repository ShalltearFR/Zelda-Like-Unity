using UnityEngine;

[CreateAssetMenu(fileName = "Vector", menuName = "Value/Vector")]

public class VectorValue : ScriptableObject, ISerializationCallbackReceiver
{
    public Vector2 initialValue;
    public Vector2 defaultValue;
    public Vector2 teleporationValue;

    public void OnAfterDeserialize()
    {
        initialValue = defaultValue;
    }

    public void OnBeforeSerialize()
    {
        
    }
}
