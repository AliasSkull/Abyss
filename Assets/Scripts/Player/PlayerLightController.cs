using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerLightController : MonoBehaviour
{
    public Light2D playerLight;

    public float maxLightIntensity = 7f;
    public float lightDecayTime;

    public Slider lightSlider;

    // Start is called before the first frame update
    void Start()
    {
        playerLight.intensity = maxLightIntensity;
        lightSlider.maxValue = maxLightIntensity;
        lightSlider.minValue = 0;
      
    }

    // Update is called once per frame
    void Update()
    {
        
        lightSlider.value = playerLight.intensity;

        if (playerLight.intensity > 0)
        {   
            playerLight.intensity -= lightDecayTime * Time.deltaTime;

        }
        if (playerLight.intensity == 0)
        {
            playerLight.intensity = 0;
        }



    }

 
}
