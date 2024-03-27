using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public int moneyCount = 0;
    public TextMeshProUGUI MoneyText;

    public int barLevel = 0;
    public ProgressBar bar;
    public int playerLevel = 1;
    public TextMeshProUGUI playerLevelText;

    public PopUpText popUpText;

    public string dataSaveTag = "Data";
    DataBaseSaver loader = null;

    void Start(){
        GameObject gameobject = GameObject.FindGameObjectWithTag(dataSaveTag);
        loader =gameobject.GetComponent<DataBaseSaver>();
        StartCoroutine(Values());
    }

    public void SetMoney(float moneyValue){
        moneyCount += (int)moneyValue;
        loader.dts.totalMoney = moneyCount;
        MoneyText.text = moneyCount.ToString();
    }

    public void SetLevel(int amount){
        if(amount + barLevel > 100){
            barLevel = (amount + barLevel)-100;
            bar.BarValue = barLevel;
            playerLevel++;
            playerLevelText.text = playerLevel.ToString();

            popUpText.Animate("Level "+playerLevel+"!!!");
            FindObjectOfType<AudioManager>().Toggle("NewLevel",1);

            loader.dts.levelBar = barLevel;
            loader.dts.playerLevel = playerLevel;
        }
        else{
            barLevel += amount;
            bar.BarValue = barLevel;
            playerLevelText.text = playerLevel.ToString();

            loader.dts.levelBar = barLevel;
            loader.dts.playerLevel = playerLevel;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Money"))
        {
            SetMoney((other.transform.localScale.z / other.transform.localScale.y) * 50f);
            loader.SaveDataFn();
            FindObjectOfType<AudioManager>().Toggle("PickUp",1);
            Destroy(other.gameObject);
        }
    }
    IEnumerator Values() 
    {
        yield return new WaitUntil(() => loader.loaded == true);

        SetMoney(loader.dts.totalMoney);

        barLevel = loader.dts.levelBar;
        playerLevel = loader.dts.playerLevel;

        SetLevel(0);
    }
}
