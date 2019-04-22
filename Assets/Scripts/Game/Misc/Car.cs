using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public Chargable ch;
    public Rigidbody2D rbody;
    [Range(0, 10)]
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        if (ch == null) {
            ch = gameObject.GetComponent<Chargable>();
        }
        if (rbody == null) {
            rbody = gameObject.GetComponent<Rigidbody2D>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ch != null && ch.charged) {
            rbody.AddForce(new Vector2(speed * 1000, 0));// mostly controlled by drag
        }
    }
}
