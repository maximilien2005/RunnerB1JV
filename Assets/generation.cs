using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Obsolete]
public class generation : MonoBehaviour
{
    float SPEED = 10, tscale = 1;
    const float CAM_SPEED = 0.1f;
    float distance = 0;
    public float targetCamPos = 0;

    public GameObject[] gens, obstacles, powerup;
    public Slider[] ss;
    public Text[] txtxt;
    GameObject Player, cam; public GameObject endGameScreen, PauseScreen, ParaScreen;
    Transform gen_parent;
    Text txt;

    joueur Joueur;

    int indice;
    float x_gen = 100;

    public List<float> gen_poss = new List<float>();

    string LanguageText;

    AudioSource gameMusic;

    void Start()
    {
        gameMusic = GetComponent<AudioSource>();
        gameMusic.volume = PlayerPrefs.GetFloat("volume_music");
        gameMusic.Play();

        Joueur = GameObject.Find("Player").GetComponent<joueur>();

        gen_poss.Add(0);

        indice = indiceGen(0);

        Player = GameObject.Find("Player");
        gen_parent = GameObject.Find("gen_parent").transform; //là où seront en enfant les platformes gen;
        txt = GameObject.Find("Text").GetComponent<Text>();
        cam = GameObject.Find("Camera");
        txt.text = "NULL";

        InvokeRepeating("DestroyGen", 0.0f, 2.0f);

        LanguageCheck();
    }

    public void LanguageChange()
    {
        if (PlayerPrefs.GetInt("Langue") == 0)
            PlayerPrefs.SetInt("Langue", 1);
        else
            PlayerPrefs.SetInt("Langue", 0);

        LanguageCheck();
    }

    void LanguageCheck()
    {
        if (PlayerPrefs.GetInt("Langue") == 0) //Anglais
        {
            LanguageText = "coins";

            txtxt[0].text = "Settings";
            txtxt[1].text = "Music";
            txtxt[2].text = "Song";
            txtxt[3].text = "Français";
        }
        else
        {
            LanguageText = "pièces";

            txtxt[0].text = "Paramètres ";
            txtxt[1].text = "Musique";
            txtxt[2].text = "Sons";
            txtxt[3].text = "English";
        }
    }

    void Update()
    {
        if (!PauseScreen.active)
        {
            tscale += Time.deltaTime * (distance / 8000);
            Time.timeScale = tscale;
        }
        else
            Time.timeScale = 0;

        //Distance & affichage
        //       +=         (valeur) simplification en 0.0
        distance += Time.deltaTime * SPEED / 5;
        txt.text = (Mathf.Round(distance * 10) / 10) + "m\n" + Joueur.gold + " " + LanguageText;

        GameOverTest();

        gen_parent.transform.Translate(Time.deltaTime * new Vector3(-SPEED, 0, 0));

        if (gen_parent.transform.position.x <= x_gen)
        {
            for (int i = 0; i < gen_poss.Count; i++)
            {
                int iGen = indiceGen(gen_poss[i]);

                //déplacer le prochain endroit de la génération à 0.5 * la largeur de la platforme qui spawn + 0.5 * la largeur de la prochaine qui va spawn
                Transform gen = Instantiate(gens[iGen], new Vector3(100, 0, gen_poss[i]), gens[indice].transform.rotation, gen_parent).transform;
                //x_gen -= gen.transform.GetChild(0).transform.localScale.x / 2;


                if (iGen == 2)
                {
                    gen_poss.Add(gen_poss[i] - 8);
                    gen_poss.Add(gen_poss[i] + 8);
                    break;
                }
                if (iGen == 4 || iGen == 3)
                    gen_poss.RemoveAt(i);

                //Placer les obstacles sur la platforme
                float randPowerUp = Random.Range(0.0f, 10.0f);
                int side = Random.Range(-1, 2);
                if (randPowerUp < 0.4f) //powerup
                {
                    float rObj = Random.Range(0.0f, 10.0f);
                    int po = 0;

                    if (rObj < 6)
                        po = 0;
                    else
                        po = 1;
                    Instantiate(powerup[po], gen.transform.position + new Vector3(Random.Range(-5, 5), 1.0f, 1.5f * side), powerup[po].transform.rotation, gen);
                }
                else //obstacle
                {
                    float rObj = Random.Range(0.0f, 10.0f);
                    int objstacle = 0;
                    if (rObj > 8.0f)
                        objstacle = 1;
                    else if (rObj < 1.0f)
                        objstacle = 2;

                    Instantiate(obstacles[objstacle], gen.transform.position + new Vector3(Random.Range(-5, 5), 1.0f, 1.5f * side), obstacles[objstacle].transform.rotation, gen);
                }
                //indice = indiceGen(gen.position.z);

               // x_gen -= gens[indice].transform.GetChild(0).transform.localScale.x / 2 - 2;
            }
            x_gen -= 19;
        }

        //déaplcer la caméra en fonction des platformes choisies
        if (Vector3.Distance(cam.transform.position, new Vector3(cam.transform.position.x, cam.transform.position.y, targetCamPos)) > 0.2f)
        {
            if (cam.transform.position.z > targetCamPos)
                cam.transform.Translate(-new Vector3(0, 0, CAM_SPEED), Space.World);
            //déplacer la caméra dans le bon sens jusqu'au target pos
            else if (cam.transform.position.z < targetCamPos)
                cam.transform.Translate(new Vector3(0, 0, CAM_SPEED), Space.World);
        }

        if (ParaScreen.active)
        {
            PlayerPrefs.SetFloat("volume_music", 0.35f * ss[0].value);
            PlayerPrefs.SetFloat("volume_sounds", ss[1].value);

            gameMusic.volume = PlayerPrefs.GetFloat("volume_music");
        }
    }

    void GameOverTest()
    {
        //endgame si une condition de mort est activé

        if (Player.transform.position.y < -2.0f)
        {
            EndGame();
        }
    }
    public void EndGame()
    {
        if (!endGameScreen.active)
        {
            PlayerPrefs.SetInt("Ccoins", PlayerPrefs.GetInt("Ccoins") + Joueur.gold); //augmenter la valeur totale de gold
            PlayerPrefs.SetInt("save_dist", 0);
            endGameScreen.active = true;
            Time.timeScale = 0;
        }
    }

    void DestroyGen()
    {
        for (int i = 0; i < gen_parent.childCount; i++)
        {
            if (gen_parent.GetChild(i).transform.position.x < -50)
                Destroy(gen_parent.GetChild(i).gameObject);
        }
    }


    int indiceGen(float ActualZ)
    {
        //random valeur pour l'index correspondant à la plaftorme

        float rand = Random.Range(0.0f, 10.0f);

        if (rand < 5)
            return 0; //platforme I
        else if (rand < 8)
            return 1; //Platforme ]
        else
        {
            if (ActualZ == 0 &&
                 (!gen_poss.Contains(-8) && !gen_poss.Contains(8)))
            {
                return 2; //platforme W
            }
            else if (ActualZ != 0)
            {
                if (ActualZ > 0)
                    return 3;
                else
                    return 4;
            }
            else return 0;
        }
    }
    public void OtherRoadCam(GameObject other)
    {
        //déplacer la position target de la camera et détruire les triggers

        if (other.tag == "left")
            targetCamPos += 8;

        else if (other.tag == "right")
            targetCamPos -= 8;

        Destroy(other.transform.parent.gameObject);
    }
    public void PauseUnpause()
    {
        if (!endGameScreen.active)
        {
            //dépauser
            PauseScreen.active = !PauseScreen.active;
        }
    }
    public void ButMenu()
    {
        gameMusic.Stop();
        SceneManager.LoadScene(0);
    }

    public void SlowDown()
    {
        if (tscale < 6)
            tscale = 1;
        else
            tscale -= 5;
    }
    public void SlowUp()
    {
        tscale += 0.2f;
    }

    public void ParaSon()
    {
        ParaScreen.active = !ParaScreen.active;
    }
}
