using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class menuScript : MonoBehaviour
{
    Text Ctxt;

    void Start()
    {
        int length = 0;
        for (; length < 6; length++)
        {
            if (PlayerPrefs.GetInt("Ccoins") > Mathf.Pow(10, length))
                length++;
            else
                break;
        } //mettre 0020 au lieu de 20 ou 00001441 au lieu de 1441


        Ctxt = GameObject.Find("PieceText").GetComponent<Text>();
        Ctxt.text = "";

        for (int i = 0; i < 6 - length; i++)
            Ctxt.text += "0"; //Set text emptys
        Ctxt.text += "" + PlayerPrefs.GetInt("Ccoins"); //Récupérer la valeur de COINS !
    }

    public void ButStart()
    {
        SceneManager.LoadScene(1);
    }
}
