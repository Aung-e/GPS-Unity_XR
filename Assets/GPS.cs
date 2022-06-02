using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GPS : MonoBehaviour
{
    public float latitude;
    public float longitude;
    public TMP_Text outputText;
    

    public void Start(){
        StartCoroutine(StartLocationService());
    }

    private IEnumerator StartLocationService(){
        if(!Input.location.isEnabledByUser){
            Debug.Log("User does not have GPS");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;

        while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0){
            yield return new WaitForSeconds(1);
            maxWait --;
        }

        if(maxWait <=0 ){
            Debug.Log("Timed Out");
        }
        
        latitude = Input.location.lastData.latitude;

        longitude = Input.location.lastData.longitude;
        outputText.text = "lat"+latitude + "\n long" + longitude;  
        Debug.Log("Latitude: " + latitude + " Longitude: " + longitude);
        yield break;

    }
}
