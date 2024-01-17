using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete]
public class joueur : MonoBehaviour
{
    const float PLATFORM_WIDTH = 3.5f, MOV_SPEED = 0.01f;
    float x = Screen.width / 2;
    public int gold = 0;
    Rigidbody rb;

    Transform cam;
    public GameObject pauseScreen;

    generation genScript;

    void Start()
    {
        Time.timeScale = 1;

        rb = GetComponent<Rigidbody>();
        genScript = GameObject.Find("gen_parent").GetComponent<generation>();
        cam = GameObject.Find("Camera").transform;
    }
    void Jump()
    {
        if (transform.position.y < 1.5f)
        {
            rb.velocity += new Vector3(0, 2.0f, 0);
            Debug.Log("jump");
        }
    }
    void Update()
    {
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

            float posX = cam.transform.position.z - PLATFORM_WIDTH * ((x - Screen.width / 2) / (Screen.width / 2)); //convertir la position du click à l'écran en un position z en jeu

            if (Mathf.Abs(transform.position.z - posX) < 0.1f)
            { //ce que je veux faire marche po :(
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, (transform.position.z - posX) * MOV_SPEED);
            }

            transform.position = new Vector3(transform.position.x, transform.position.y, posX);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "obstacle")
        {
            genScript.EndGame();
        }

        if (other.transform.tag == "left") //déplacer la caméra d'un espace de 8 jusqu'à l'autre côté
            genScript.OtherRoadCam(other.gameObject);
        if (other.transform.tag == "right")
            genScript.OtherRoadCam(other.gameObject);

        Debug.Log(other.transform.tag);

        if (other.transform.tag == "gold")
        {
            gold++;
            Destroy(other.gameObject);
        }
    }

    IEnumerator Crouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);
        yield return new WaitForSeconds(2.0f); //changer la taille du joueur temportairement pour le crouch
        transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);
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

            if (velocityY > 400)
                Jump();
            if (velocityY < -400)
            {
                if (transform.position.y > 2)
                    rb.velocity -= new Vector3(0, 1.0f, 0);
                else
                    StartCoroutine(Crouch());
            }
        }
    }
}
