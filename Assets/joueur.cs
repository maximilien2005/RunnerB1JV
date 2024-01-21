using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete]
public class joueur : MonoBehaviour
{
    const float PLATFORM_WIDTH = 3.5f, MOV_SPEED = 0.01f;
    float x = Screen.width / 2;
    public int gold = 0, lifes = 0;
    bool invincible = false;
    Rigidbody rb;
    int cost = 2;

    Transform cam;
    public GameObject pauseScreen;

    generation genScript; Runner rScript;
    Animator jAnim;

    float RoueTimer = 0;
    public void ArgentRoue()
    {
        if (RoueTimer <= 0)
        {
            if (gold >= cost)
            {
                int rand = Random.Range(0, 3);

                if (rand == 0)
                    genScript.SlowDown();
                else if (rand == 1)
                    genScript.SlowUp();
                else if (rand == 2)
                {
                    if (lifes > 0)
                        lifes--;
                }
                else
                    rScript.AutoGold();

                gold -= cost;
                cost *= 2; //cout

                Debug.Log("You got id:" + rand + " !");
            }
            RoueTimer = 4.0f; // COOLDOWN
        }
    }

    public void LifesAdd()
    {
        if (lifes > 0)
            lifes--;
    }

    void Start()
    {

        Time.timeScale = 1;

        rb = GetComponent<Rigidbody>();
        genScript = GameObject.Find("gen_parent").GetComponent<generation>();
        rScript = GameObject.Find("pasGentil").GetComponent<Runner>();
        cam = GameObject.Find("Camera").transform;
        jAnim = transform.GetChild(0).GetComponent<Animator>();
    }
    void Jump()
    {
        if (transform.position.y < 1.5f)
        {
            rb.velocity += new Vector3(0, 1.0f, 0);
            jAnim.SetInteger("ja", 1);
        }
    }
    void Update()
    {
        if (RoueTimer > 0)
            RoueTimer -= Time.deltaTime;


        if (transform.position.y < 1.5f && transform.localScale.y == 1)
            jAnim.SetInteger("ja", 0);

        if (!pauseScreen.active)
        {
            //joueur
            if (Input.GetKey(KeyCode.Mouse1))
            { // dans l'éditeur unity avec la souris
                if (Input.mousePosition.y < Screen.height * 0.8f)
                {
                    x = Input.mousePosition.x;
                    StartCoroutine(MouseMov());
                }
            }
            else if (Input.touchCount > 0) //détecter un appui sur l'écran
            { //en jeu : click sur 'lécran
                if (Input.touches[0].position.y < Screen.height * 0.8f)
                {
                    x = Input.touches[0].position.x;
                    StartCoroutine(MouseMov());
                }
            }

            float posX = cam.transform.position.z - 0.4f * PLATFORM_WIDTH * ((x - Screen.width / 2) / (Screen.width / 2)); //convertir la position du click à l'écran en un position z en jeu

            if (Mathf.Abs(transform.position.z - posX) < 0.1f)
            { //ce que je veux faire marche po :(
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, (transform.position.z - posX) * MOV_SPEED);
            }

            transform.position = new Vector3(transform.position.x, transform.position.y, posX);
        }
    }

    IEnumerator HIT()
    {
        invincible = true;

        if (lifes < 3)
            lifes++;
        else
            genScript.EndGame();

        yield return new WaitForSeconds(1.0f);
        invincible = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "obstacle" && !invincible)
        {
            StartCoroutine(HIT());
        }
        if (other.transform.tag == "obstacleB" && !invincible)
            genScript.EndGame();

        if (other.transform.tag == "left") //déplacer la caméra d'un espace de 8 jusqu'à l'autre côté
            genScript.OtherRoadCam(other.gameObject);
        if (other.transform.tag == "right")
            genScript.OtherRoadCam(other.gameObject);

        if (other.transform.tag == "gold")
        {
            gold++;
            Destroy(other.gameObject);
        }

        if (other.transform.tag == "pu_slow")
        {
            genScript.SlowDown();
            Destroy(other.gameObject);
        }
        else if (other.transform.tag == "pu_accelerate")
        {
            genScript.SlowUp();
            Destroy(other.gameObject);
        }
    }

    IEnumerator Crouch()
    {
        jAnim.SetInteger("ja", -1);
        transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);
        yield return new WaitForSeconds(2.0f); //changer la taille du joueur temportairement pour le crouch
        transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);
        jAnim.SetInteger("ja", 0);
    }

    IEnumerator MouseMov()
    {
        float a = 0, b = 0;

        if (Input.GetKey(KeyCode.Mouse1))
            a = Input.mousePosition.y;
        else if (Input.touchCount > 0)
            a = Input.touches[0].position.y;

        yield return new WaitForSeconds(0.1f);

        if (Input.GetKey(KeyCode.Mouse1))
            b = Input.mousePosition.y;
        else if (Input.touchCount > 0)
            b = Input.touches[0].position.y;

        float velocityY = b - a;

        if (Input.GetKey(KeyCode.Mouse1) || Input.touchCount > 0)
        {

            if (velocityY > 200)
                Jump();
            if (velocityY < -200)
            {
                if (transform.position.y > 2)
                    rb.velocity -= new Vector3(0, 1.0f, 0);
                else
                    StartCoroutine(Crouch());
            }
        }
    }
}
