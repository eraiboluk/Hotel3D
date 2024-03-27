using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase.Database;
using TMPro;
using Firebase;

[Serializable]
public class dataToSave {
    public int totalMoney;
    public int[] crrLevels;
    public int[] buyAreaFees;
    public int playerLevel;
    public int levelBar;
}
public class DataBaseSaver : MonoBehaviour
{
    public dataToSave dts;
    public string userId;
    DatabaseReference dbRef;
    public bool loaded = false;

    public string remoteConfigTag = "Remote";
    RemoteConfigScript remote = null;

    private void Awake()
    {
        userId = GetAndroidID();
        Time.timeScale = 0;
        GameObject gameobject = GameObject.FindGameObjectWithTag(remoteConfigTag);
        remote =gameobject.GetComponent<RemoteConfigScript>();
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        LoadDataFn();
    }

    public void SaveDataFn() 
    {
        string json = JsonUtility.ToJson(dts);
        dbRef.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }

    public void LoadDataFn()
    {
        StartCoroutine(LoadDataEnum());
    }

    IEnumerator LoadDataEnum() 
    {
        var serverData = dbRef.Child("users").Child(userId).GetValueAsync();

        yield return new WaitUntil(() => serverData.IsCompleted);

        if (serverData.Exception != null) {
            // Handle error
            Debug.LogError("Failed to load data: " + serverData.Exception);
            yield break;
        }

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if (!string.IsNullOrEmpty(jsonData)) {

            Debug.Log("Server data found");
            dts = JsonUtility.FromJson<dataToSave>(jsonData);

        } else {
            // If no data found, wait for remote data fetch to complete
            yield return new WaitUntil(() => remote.fetchComplete);

            if (remote.allConfigData != null && remote.allConfigData.buyAreaFees != null) {
                dts.buyAreaFees = (int[])remote.allConfigData.buyAreaFees.Clone();
                Debug.Log("No data found, using remote data");
            } else {
                Debug.LogError("Failed to load remote data");
                yield break;
            }
        }

    yield return new WaitForEndOfFrame();
    loaded = true;
    }

    string GetAndroidID()
    {
        try
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaClass androidUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = androidUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
                AndroidJavaObject secure = new AndroidJavaObject("android.provider.Settings$Secure");
                string androidId = secure.CallStatic<string>("getString", contentResolver, "android_id");

                return androidId;
            }
            else
            {
                return "Not Android Platform!";
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error getting Android ID: " + ex.Message);
            return "Error";
        }
    }
}
