using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete]
public class Runner : MonoBehaviour
{
    const float speed = 10.0f, PLATFORME_WIDTH = 3.5f;
    bool move = false;
    int dir = 0;

    bool moving = false;

    public GameObject piece;
    Transform gen_parent;

    float timer = 10.0f;

    Rigidbody rb;
    Animator rAnim;
    joueur jScript;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gen_parent = GameObject.Find("gen_parent").transform;
        rAnim = transform.GetChild(0).GetComponent<Animator>();
        jScript = GameObject.Find("Player").GetComponent<joueur>();
    }
    
    bool L = false, R = false;
    bool Autogolded = false;

    public IEnumerator AutoGold()
    {
        Autogolded = true;
        yield return new WaitForSeconds(4.0f);
        Autogolded = false;
    }

    void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        else
        {
            if (!Autogolded)
            {
                Instantiate(piece, transform.position + new Vector3(-1.5f, 0, 0), piece.transform.rotation, gen_parent);
            }
            else
                jScript.gold++;
            timer = Random.Range(3.5f, 7.5f);
        }

        transform.position = new Vector3(4 + 4.5f * jScript.lifes, transform.position.y, transform.position.z);
        if (transform.position.y < 0.0f)
            transform.position = new Vector3(transform.position.x, 1.04f, transform.position.z);
        
        RaycastHit hit;
        if (Physics.Raycast(new Ray(new Vector3(transform.position.x, 1.0f, transform.position.z + 0.3f), new Vector3(1, 0, 0)), out hit) && !R && Vector3.Distance(transform.position, hit.collider.transform.position) < 5)
        {
            L = true;
        }
        else
            L = false;
        if (Physics.Raycast(new Ray(new Vector3(transform.position.x, 1.0f, transform.position.z - 0.3f), new Vector3(1, 0, 0)), out hit) && !L && Vector3.Distance(transform.position, hit.collider.transform.position) < 5)
        {
            R = true;
        }
        else
            R = false;

        if (L)
        {
            if (Mathf.Abs(transform.position.z) < 1.5f)
                transform.Translate(new Vector3(0, 0, -1) * speed * Time.deltaTime);
            else
                transform.Translate(new Vector3(0, 0, 1) * speed * Time.deltaTime);
        }
        else if (R)
        {
            if (Mathf.Abs(transform.position.z) < 1.5f)
                transform.Translate(new Vector3(0, 0, 1) * speed * Time.deltaTime);
            else
                transform.Translate(new Vector3(0, 0, -1) * speed * Time.deltaTime);
        }

        //Faire en sorte que lennemie n'entre pas en collision avec les obstacles

        //sécurité
        if (transform.position.z > PLATFORME_WIDTH / 2)
            transform.position = new Vector3(transform.position.x, transform.position.y, PLATFORME_WIDTH / 2);
        else if (transform.position.z < -PLATFORME_WIDTH / 2)
            transform.position = new Vector3(transform.position.x, transform.position.y, -PLATFORME_WIDTH / 2);

        if (transform.position.y < 1.5f) //jump
        {
            if (!Physics.Raycast(new Ray(transform.position, new Vector3(2, -1, 0)))) //détecter un trou
            {
                Debug.Log("a");
                rAnim.SetInteger("ra", 1);
                rb.velocity += new Vector3(0, 1.0f, 0);
            }
        }

        if (Physics.Raycast(new Ray(transform.position, new Vector3(2, 1, 0)), out hit) && hit.transform.tag == "obstacle") //Accroupissement
        {
            StartCoroutine(Crouch());
        }

        if (transform.position.y < 1.5f && transform.localScale.y == 1)
            rAnim.SetInteger("ra", 0);
    }

    IEnumerator Crouch()
    {
        rAnim.SetInteger("ra", -1);
        transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);
        yield return new WaitForSeconds(1.5f);
        transform.localScale = new Vector3(transform.localScale.x, 1.0f, transform.localScale.z);
        rAnim.SetInteger("ra", 0);
    }
}
