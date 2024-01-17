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
    public Slider[] Ssliders;

    AudioSource musicMenu;

    void Sound()
    {
        musicMenu.volume = PlayerPrefs.GetFloat("volume_music");
    }
    void Language()
    {
        if (PlayerPrefs.GetInt("Langue") == 0) //anglais
        {
            Textexts[0].text = "Start";
            Textexts[1].text = "Goals";
            Textexts[2].text = "Statistics";
            Textexts[3].text = "Settings";
            Textexts[4].text = "Français";
            Textexts[5].text = "Music";
            Textexts[6].text = "Sounds";
        }
        else //français
        {
            Textexts[0].text = "Démarrer";
            Textexts[1].text = "Objectifs";
            Textexts[2].text = "Statistiques";
            Textexts[3].text = "Paramètres";
            Textexts[4].text = "English";
            Textexts[5].text = "Musique";
            Textexts[6].text = "Sons";
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
        musicMenu = GetComponent<AudioSource>();
        musicMenu.Play();

        Language();
        Sound();

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

    void Update()
    {
        if (MenuParts[0].active)
        {
            PlayerPrefs.SetFloat("volume_music", 0.35f * Ssliders[0].value);
            Sound();

            PlayerPrefs.SetFloat("volume_sounds", Ssliders[1].value);
        }
    }

    public void ButStart()
    {
        musicMenu.Stop();
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
