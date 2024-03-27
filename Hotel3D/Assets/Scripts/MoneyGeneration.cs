using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyGeneration : MonoBehaviour
{
    public GameObject moneyPrefab;
    private float defaultSize = 100f;
    public PlayerStats player;

    public void SetMoney(float moneyValue, Vector3 spawnPosition)
    {
        float zRatio = moneyValue / 50f;

        GameObject money = Instantiate(moneyPrefab, spawnPosition, Quaternion.identity);
        money.transform.localScale = new Vector3(defaultSize ,defaultSize ,defaultSize * zRatio);
        money.transform.Rotate(-90f, 0f, 0f);
    }
}
