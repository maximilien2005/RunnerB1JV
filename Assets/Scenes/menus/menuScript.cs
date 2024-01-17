using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Obsolete]
public class menuScript : MonoBehaviour
{
    Text Ctxt;

    public Text[] Textexts;
    public GameObject[] MenuParts;

    void Language()
    {
        if (PlayerPrefs.GetInt("Langue") == 0) //anglais
        {
            Textexts[0].text = "Start";
            Textexts[1].text = "Goals";
            Textexts[2].text = "Statistics";
            Textexts[3].text = "Settings";
            Textexts[4].text = "Français";
        }
        else //français
        {
            Textexts[0].text = "Démarrer";
            Textexts[1].text = "Objectifs";
            Textexts[2].text = "Statistiques";
            Textexts[3].text = "Paramètres";
            Textexts[4].text = "English";
        }
    }

    public void ButLangue()
    {
        if (PlayerPrefs.GetInt("Langue") == 0)
            PlayerPrefs.SetInt("Langue", 1);
        else
            PlayerPrefs.SetInt("Langue", 0);

        Language();
    }

    void Start()
    {
        Language();

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

    public void ButLanguageUI()
    {
        MenuParts[0].active = true;
    }

    public void ButBack()
    {
        for (int i = 0; i < MenuParts.Length; i++)
            MenuParts[i].active = false;
    }
}
