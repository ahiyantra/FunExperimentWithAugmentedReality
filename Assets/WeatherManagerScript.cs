using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro; // ???

using UnityEngine.Networking;
using SimpleJSON;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class WeatherManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    public string apiKey; // = "0625e609e90b87ff4e702e42fca68faf";
    public string currentWeatherApi = "api.openweathermap.org/data/2.5/weather?";
    [Header("UI")]
    public TextMeshProUGUI statusText; // TMPro.TMP_Text 
    public TextMeshProUGUI location; // TMPro.TMP_Text 
    public TextMeshProUGUI mainWeather; // TMPro.TMP_Text 
    public TextMeshProUGUI description; // TMPro.TMP_Text 
    public TextMeshProUGUI temp; // TMPro.TMP_Text 
    public TextMeshProUGUI feels_like; // TMPro.TMP_Text 
    public TextMeshProUGUI temp_min; // TMPro.TMP_Text 
    public TextMeshProUGUI temp_max; // TMPro.TMP_Text 
    public TextMeshProUGUI pressure; // TMPro.TMP_Text 
    public TextMeshProUGUI humidity; // TMPro.TMP_Text 
    public TextMeshProUGUI windspeed; // TMPro.TMP_Text 

    private LocationInfo lastLocation;

    GameObject dialog = null;

    void Start()
    {

        #if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            dialog = new GameObject();
            }
        #endif

        StartCoroutine(FetchLocationData());

    }

    void Update()
    {
        
        //StartCoroutine(FetchLocationData());
        
    }

    private IEnumerator FetchLocationData()
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield break;
        // Start service before querying location
        Input.location.Start();
        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            statusText.text = "Location Timed out";
            yield break;
        }
        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            statusText.text = "Unable to determine device location";
            yield break;
        }
        else
        {
            lastLocation = Input.location.lastData;
            UpdateWeatherData();
        }
        Input.location.Stop();
    }

    private void UpdateWeatherData()
    {
        StartCoroutine(FetchWeatherDataFromApi(lastLocation.latitude.ToString(), lastLocation.longitude.ToString()));
    }

    private IEnumerator FetchWeatherDataFromApi(string latitude, string longitude)
    {
        string url = currentWeatherApi + "lat=" + latitude + "&lon=" + longitude + "&appid=" + apiKey + "&units=metric";
        UnityWebRequest fetchWeatherRequest = UnityWebRequest.Get(url);
        yield return fetchWeatherRequest.SendWebRequest();
        if (fetchWeatherRequest.isNetworkError || fetchWeatherRequest.isHttpError)
        {
            //Check and print error
            statusText.text = fetchWeatherRequest.error;
        }
        else
        {
            Debug.Log(fetchWeatherRequest.downloadHandler.text);
            var response = JSON.Parse(fetchWeatherRequest.downloadHandler.text);

            statusText.text = "Status";

            location.text = "Your location is " + response["name"];
            mainWeather.text = "The weather is " + response["weather"][0]["main"];
            description.text = "(" + response["weather"][0]["description"] + ")";
            temp.text = "Temp is " + response["main"]["temp"] + " C";
            feels_like.text = "Feels like " + response["main"]["feels_like"] + " C";
            temp_min.text = "Min temp is " + response["main"]["temp_min"] + " C";
            temp_max.text = "Max temp is " + response["main"]["temp_max"] + " C";
            pressure.text = "Pressure is "  + response["main"]["pressure"] +  " Pa";
            humidity.text = "Humidity is "  + response["main"]["humidity"] + " %";
            windspeed.text = "Windspeed is " + response["wind"]["speed"] + " Km/h";
        }
    }

}
