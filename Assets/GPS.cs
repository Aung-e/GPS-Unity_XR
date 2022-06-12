using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GPS : MonoBehaviour
{
    public float latitude;
    public float longitude;
    public float alt;
    public TMP_Text outputText;
    public Text lat1Text;
    public Text lat2Text;
    public Text long1Text;
    public Text long2Text;
    public Text compareOutputText;
    int requests =0;
    int GPSRequests =0;
    float[] coordArr = {0, 0, 0, 0};
    float[] altitude = { 0.0f, 0.0f };

    public GameObject markerPrefab;
    public GameObject playSpace;

    public void Start(){
        StartCoroutine(StartLocationService());
        StartCoroutine(CreateMarkers());
    }

    public IEnumerator CreateMarkers()
    {
        yield return new WaitForSeconds(5.0f);
        //28.14958, -81.85098
        //28.15014, -81.85066
        //28.15035,-81.85003
        
        
        Vector3 p1 = FromGPS(latitude,longitude, 28.14958f, -81.85098f);
        Vector3 p2 = FromGPS(latitude, longitude, 28.15014f, -81.85066f);
        Vector3 p3 = FromGPS(latitude, longitude, 28.15035f, -81.85003f);
        if (markerPrefab != null)
        {
            GameObject g1 = GameObject.Instantiate(markerPrefab, Camera.main.transform.position + p1, Quaternion.identity);
            GameObject g2 = GameObject.Instantiate(markerPrefab, Camera.main.transform.position + p2, Quaternion.identity);
            GameObject g3 = GameObject.Instantiate(markerPrefab, Camera.main.transform.position + p3, Quaternion.identity);
            g1.transform.SetParent(playSpace.transform);
            g2.transform.SetParent(playSpace.transform);
            g3.transform.SetParent(playSpace.transform);
            outputText.text = "Marker 1 located at " + g1.transform.position;


        }


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


    Vector3 RawMetersFromGPS(float lat1, float lon1)
    {
        float dlat = (lat1) * 111.32f;
        float dlon = (lon1) * 40075 * Mathf.Cos(lat1 * Mathf.PI / 180);
        return new Vector3(dlon, 0.0f, dlat);
    }


    Vector3 FromGPS(float lat1, float lon1, float lat2, float lon2)
    {

        //Length in meters of 1° of latitude = always 111.32 km
        //Length in meters of 1° of longitude = 40075 km* cos(latitude ) / 360
        float dLat = ((lat2  - lat1)*111.32f)*1000;
        float dLon =( 
            ( (lon2) * 111.32f * Mathf.Cos(lat2*Mathf.PI/180) )  //Don't know where the number 40075 came from.
            -((lon1)* 111.32f * Mathf.Cos(lat1*Mathf.PI/180))
            )*1000;
        Vector3 result = new Vector3 (dLon,0.0f, dLat);
        return result;
    }
    //To GPS function
    public void SetArray()
    {
        altitude[0] = altitude[1];
        coordArr[0] = coordArr[2];
        coordArr[1] = coordArr[3];
        coordArr[2] = Input.location.lastData.latitude;
        coordArr[3] = Input.location.lastData.longitude;
        altitude[1] = alt;
        lat1Text.text = coordArr[0].ToString();
        long1Text.text = coordArr[1].ToString();
        lat2Text.text = coordArr[2].ToString();
        long2Text.text = coordArr[3].ToString();

        

    }

    public void CoordCompare()
    {
        //compareOutputText.text = measure(coordArr[0], coordArr[1], coordArr[2], coordArr[3]).ToString();
        compareOutputText.text = "My current unity position " + Camera.main.transform.position+", My Current GPS-to Unity position "
            +(Camera.main.transform.position+FromGPS(coordArr[0], coordArr[1], coordArr[2], coordArr[3]));
        Vector3 result = Camera.main.transform.position + FromGPS(coordArr[0], coordArr[1], coordArr[2], coordArr[3]);
        if (markerPrefab != null)
        {
            GameObject.Instantiate(markerPrefab, result, Quaternion.identity);
        }
    }

    private void Update(){
        latitude = Input.location.lastData.latitude; //This should be a moving average

        longitude = Input.location.lastData.longitude;  //This should be a moving average.

        alt = Input.location.lastData.altitude;

        outputText.text = Input.location.status +  "   "  + "lat "  +latitude + "\n long" + longitude +  "\n GPS Requests" + requests;  
        Debug.Log("Latitude: " + latitude + " Longitude: " + longitude);

    }

    private IEnumerator StartLocationService(){

        while (true)
        {
            requests += 1;

            if (!Input.location.isEnabledByUser)
            {
                Debug.Log("User does not have GPS");
                outputText.text = "This app requires GPS location data " + requests;
                yield break;
            }

            Input.location.Start(1f, 1f);

            int maxWait = 20;

            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            if (maxWait <= 0)
            {
                outputText.text = "Timed out";
                Debug.Log("Timed Out");
                yield break;
            }
            yield return new WaitForSeconds(1);
        }
        //Input.location.Stop();
        //yield break;
        
    }
}
