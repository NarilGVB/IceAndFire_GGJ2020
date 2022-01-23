using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PRUEBA : MonoBehaviour
{

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Jump"))
        {
            print("PUM");
            rb.AddForce(Vector2.right * 5, ForceMode2D.Impulse);
        }

        transform.Translate(Mathf.Sin(Time.time)*Time.deltaTime,0,0);
        
    }
}
