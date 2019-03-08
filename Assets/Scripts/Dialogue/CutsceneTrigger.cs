using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    public Cutscene cutscene;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            StartCoroutine(cutscene.ExecuteCutscene());
            Destroy(gameObject);
        }
    }
}
