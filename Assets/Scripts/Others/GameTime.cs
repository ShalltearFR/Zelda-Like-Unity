using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime : MonoBehaviour
{
   // public float gameTime;
    public FloatValue savedGameTime;

    void Start()
    {
    //  gameTime = savedGameTime.RuntimeValue;
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        savedGameTime.RuntimeValue += Time.deltaTime;
    }
}
