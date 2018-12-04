using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    // Positive is behind scene, negative in front
    [Range(-1,1)]
    public float Depth;

    private Vector3 lastCamPos;

    // Start is called before the first frame update
    void Start()
    {
        lastCamPos = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 camMovement = Camera.main.transform.position - lastCamPos;
        transform.Translate(camMovement.x * Depth, 0, 0);
        lastCamPos = Camera.main.transform.position;
    }
}
