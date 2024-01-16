using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour
{
    const float speed = 10.0f, PLATFORME_WIDTH = 3.5f;
    bool move = false;
    int dir = 0;

    public GameObject piece;
    Transform gen_parent;

    float timer = 10.0f;

    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gen_parent = GameObject.Find("gen_parent").transform;
    }

    void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        else
        {
            Instantiate(piece, transform.position + new Vector3(-1.5f, 0, 0), piece.transform.rotation, gen_parent);
            timer = Random.Range(3.5f, 7.5f);
        }

        transform.position = new Vector3(12, transform.position.y, transform.position.z);

        Debug.DrawRay(new Vector3(transform.position.x, 0.8f, transform.position.z), new Vector3(1, 0, 0), Color.red, Mathf.Infinity);
        Debug.DrawRay(new Vector3(transform.position.x, 0.8f, transform.position.z + 0.6f), new Vector3(1, 0, 0), Color.red, Mathf.Infinity);
        Debug.DrawRay(new Vector3(transform.position.x, 0.8f, transform.position.z - 0.6f), new Vector3(1, 0, 0), Color.red, Mathf.Infinity);

        RaycastHit hit;
        if (((Physics.Raycast(new Ray(transform.position + new Vector3(0, 0, +0.6f), new Vector3(1, 0, 0)), out hit) && hit.collider.tag == "obstacle") ||
            (Physics.Raycast(new Ray(transform.position + new Vector3(0, 0, -0.6f), new Vector3(1, 0, 0)), out hit) && hit.collider.tag == "obstacle") ||
            Physics.Raycast(new Ray(transform.position, new Vector3(1, 0, 0)), out hit) && hit.collider.tag == "obstacle") &&
            Vector3.Distance(transform.position, hit.collider.transform.position) < 20) //gauche droite : détection d'obstacles
        {
            Debug.Log("hited");

            if (!move)
            {
                if (transform.position.z > 0)
                    dir = -1;
                else
                    dir = 1;

                move = true;
            }
        }
        if (!(((Physics.Raycast(new Ray(transform.position + new Vector3(0, 0, +0.6f), new Vector3(1, 0, 0)), out hit) && hit.collider.tag == "obstacle") &&
            (Physics.Raycast(new Ray(transform.position + new Vector3(0, 0, -0.6f), new Vector3(1, 0, 0)), out hit) && hit.collider.tag == "obstacle") &&
            (Physics.Raycast(new Ray(transform.position, new Vector3(1, 0, 0)), out hit) && hit.collider.tag == "obstacle")) &&
            Vector3.Distance(transform.position, hit.transform.position) < 20))
            move = false;
        if (move)
        {
            transform.Translate(new Vector3(0, 0, dir) * speed * Time.deltaTime);
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
                rb.velocity += new Vector3(0, 0.8f, 0);
            }
        }

        if (Physics.Raycast(new Ray(transform.position, new Vector3(2, 1, 0)), out hit) && hit.transform.tag == "obstacle") //Accroupissement
        {
            StartCoroutine(Crouch());
        }
    }

    IEnumerator Crouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);
        yield return new WaitForSeconds(1.5f);
        transform.localScale = new Vector3(transform.localScale.x, 1.0f, transform.localScale.z);
    }
}
