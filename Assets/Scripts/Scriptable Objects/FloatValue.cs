using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Float", menuName = "Value/Float")]
[System.Serializable]

public class FloatValue : ScriptableObject
{
    public float initialValue;
    public float RuntimeValue;
}
