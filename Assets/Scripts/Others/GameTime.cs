using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime : MonoBehaviour
{
   // public float gameTime;
    public FloatValue savedGameTime;

    // Start is called before the first frame update
    void Start()
    {
    //    gameTime = savedGameTime.RuntimeValue;

        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        savedGameTime.RuntimeValue += Time.deltaTime;
    }
}
