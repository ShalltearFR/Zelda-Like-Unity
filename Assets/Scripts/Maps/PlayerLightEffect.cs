using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class PlayerLightEffect : MonoBehaviour
{
    private new UnityEngine.Experimental.Rendering.Universal.Light2D light;
    public float value = 7.5f;

    private void Start()
    {
        light = gameObject.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
        StartCoroutine(LightUp());
    }
    // Update is called once per frame
    void Update()
    {
        light.pointLightOuterRadius = value;
    }

    IEnumerator LightUp()
    {
        // Lumiere qui <s'agrandit>
        int i;
        for (i = 0; i < 5; i++)
        {
            value += 0.05f;
            yield return new WaitForSeconds(0.02f);
        }
        StartCoroutine(LightDown());
        yield return null;
    }

    IEnumerator LightDown()
    {
        // Lumiere qui se <rétrécit>
        int i;
        for (i = 0; i < 5; i++)
        {
            value -= 0.05f;
            yield return new WaitForSeconds(0.02f);
        }
        StartCoroutine(LightUp());
        yield return null;
    }
}
