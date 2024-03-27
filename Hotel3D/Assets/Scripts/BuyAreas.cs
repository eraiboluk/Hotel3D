using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyAreas : MonoBehaviour
{
    public int buyAreaNumber;

    public int remainingFee;
    public TextMeshPro textFee;
    private PlayerStats player;
    public int MoneyReductionCoefficient = 1;
    private float delayTime = 1f;

    public Rooms roomControl;

    public string dataSaveTag = "Data";
    DataBaseSaver loader = null;

    void Start()
    {     
        StartCoroutine(FeeValue());
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.GetComponent<PlayerStats>();

            if (player == null)
            {
                Debug.LogError("Oyuncu objesinde PlayerStats bileşeni bulunamadı!");
            }
        }
        else
        {
            Debug.LogError("Oyuncu objesi bulunamadı!");
        }
        
    }
    private void OnTriggerEnter(Collider other){
        if (other.CompareTag("Player"))
            delayTime = 1f;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(remainingFee > 0){

                if (delayTime <= 0)
                {
                    MoneyReductionCoefficient = (int)(player.moneyCount/100 + 1);
                    if((int)(remainingFee/100 + 1) <= MoneyReductionCoefficient)
                        MoneyReductionCoefficient = (int)(remainingFee/100 + 1);

                    if(remainingFee > 0 && player.moneyCount > 0){

                        if(!FindObjectOfType<AudioManager>().isPlay("CoinDrop"))
                            FindObjectOfType<AudioManager>().Toggle("CoinDrop",1);

                        remainingFee -= MoneyReductionCoefficient;
                        textFee.text=remainingFee.ToString();
                        player.SetMoney(-MoneyReductionCoefficient);
                    }
                    else
                        FindObjectOfType<AudioManager>().Toggle("CoinDrop",0);
                }
                else
                    delayTime -= Time.deltaTime;
            }
            else{
                gameObject.GetComponent<CapsuleCollider>().enabled = false; 
                gameObject.GetComponent<CapsuleCollider>().isTrigger = false;

                Debug.Log("31");
                FindObjectOfType<AudioManager>().Toggle("CoinDrop",0);
                FindObjectOfType<AudioManager>().Toggle("Purchase",1);

                loader.dts.buyAreaFees[buyAreaNumber] = remainingFee;
                player.SetLevel(20);

                loader.SaveDataFn();

                roomControl.Purchased();
                Destroy(gameObject);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<AudioManager>().Toggle("CoinDrop",0);
            loader.dts.buyAreaFees[buyAreaNumber] = remainingFee;
            loader.SaveDataFn();
        }
    }
    IEnumerator FeeValue() 
    {
        GameObject gameobject = GameObject.FindGameObjectWithTag(dataSaveTag);
        loader =gameobject.GetComponent<DataBaseSaver>();
        yield return new WaitUntil(() => loader.loaded == true && loader.dts.buyAreaFees != null);
        remainingFee = loader.dts.buyAreaFees[buyAreaNumber];
        textFee.text=remainingFee.ToString();
    }
}