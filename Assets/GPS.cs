using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GPS : MonoBehaviour
{
    public float latitude;
    public float longitude;
    public TMP_Text outputText;
    int requests =0;

    public void Start(){
        StartCoroutine(StartLocationService());
    }


    static double measure(float lat1, float lon1, float lat2, float lon2){  // generally used geo measurement function
        var R = 6378.137; // Radius of earth in KM
        var dLat = lat2 * Mathf.PI / 180 - lat1 * Mathf.PI / 180;
        var dLon = lon2 * Mathf.PI / 180 - lon1 * Mathf.PI / 180;
        var a = Mathf.Sin(dLat/2) * Mathf.Sin(dLat/2) +
        Mathf.Cos(lat1 * Mathf.PI / 180) * Mathf.Cos(lat2 * Mathf.PI / 180) *
        Mathf.Sin(dLon/2) * Mathf.Sin(dLon/2);
        var c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1-a));
        var d = R * c;
        return d * 1000; // meters
    }

    private IEnumerator StartLocationService(){
        if(!Input.location.isEnabledByUser){
            Debug.Log("User does not have GPS");
             outputText.text ="This app requires GPS location data " + requests;
             requests +=1;
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;

        while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0){
            yield return new WaitForSeconds(1);
            maxWait --;
        }

        if(maxWait <=0 ){
            outputText.text = "Timed out";
            Debug.Log("Timed Out");
        }
        
        latitude = Input.location.lastData.latitude;

        longitude = Input.location.lastData.longitude;
        outputText.text = "lat "+latitude + "\n long" + longitude;  
        Debug.Log("Latitude: " + latitude + " Longitude: " + longitude);
        yield break;

    }
}
