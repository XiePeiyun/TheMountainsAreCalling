using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    [Header("Skyboxes")]
    public Material nightSkybox;
    public Material sunnySkybox;
    public Material cloudSkybox;
    public Material windSkybox;
    public Material rainSkybox;
    public Material snowSkybox;
    public Material lightingSkybox;

    public void SetNightSky()
    {
        if (nightSkybox == null) return;
        RenderSettings.skybox = nightSkybox;
        Debug.Log("Skybox ¡ú Night");
    }

    public void SetWeatherSky(string weather)
    {
        if (string.IsNullOrEmpty(weather))
        {
            Debug.LogWarning("SetWeatherSky called with empty weather");
            return;
        }

        switch (weather)
        {
            case "Sunny":
                if (sunnySkybox != null) RenderSettings.skybox = sunnySkybox;
                break;
            case "Cloud":
                if (cloudSkybox != null) RenderSettings.skybox = cloudSkybox;
                break;
            case "Wind":
                if (windSkybox != null) RenderSettings.skybox = windSkybox;
                break;
            case "Rain":
                if (rainSkybox != null) RenderSettings.skybox = rainSkybox;
                break;
            case "Snow":
                if (snowSkybox != null) RenderSettings.skybox = snowSkybox;
                break;
            case "Lighting":
                if (lightingSkybox != null) RenderSettings.skybox = lightingSkybox;
                break;
            default:
                Debug.LogWarning("Unknown weather: " + weather);
                return;
        }

        Debug.Log("Skybox ¡ú " + weather);
    }
}
