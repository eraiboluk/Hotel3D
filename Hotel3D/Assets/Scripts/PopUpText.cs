using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUpText : MonoBehaviour
{
    public TextMeshProUGUI text;

    void Start()
    {
        HideText();
    }

    public void Animate(string a){
        text.text = a;
        gameObject.SetActive(true);
        Invoke("HideText", 2f);
    }

    private void HideText()
    {
        gameObject.SetActive(false);
    }
}  
