using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LoadingScreen : MonoBehaviour
{
    public GameObject panelToDestroy;

    void Update()
    {
        if (Time.timeScale == 1)
        {
            Destroy(gameObject);
        }
    }
}
