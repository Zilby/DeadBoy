using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleTop : MonoBehaviour
{
    public List<GameObject> ledges;
    public Rigidbody2D rb;
    [Range(-1,1)]
    public float settleDepth;
    [Range(0,3)]
    public float bouyancy;

    [Range(0,50)]
    public float rightingForce = 21.6f ;

    private BoxCollider2D waterCollider;
    private List<PlayerController> pcs = new List<PlayerController>();

    // Start is called before the first frame update
    void Start()
    {
        if (rb == null) {
            rb = gameObject.GetComponent<Rigidbody2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (waterCollider != null && pcs.Count > 0) {
            // float surface = waterCollider.transform.position.y + waterCollider.size.y * waterCollider.transform.lossyScale.y / 2;
            // float height = this.gameObject.transform.position.y - settleDepth;
            // rb.AddForce(new Vector3(0, (surface - height ) * bouyancy, 0), ForceMode2D.Impulse);
            float f = (((450 - transform.eulerAngles.z) % 180) - 90)/rightingForce;
//            transform.eulerAngles = transform.eulerAngles.ZAdd(f);
            rb.AddTorque(f,  ForceMode2D.Impulse);
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController p = collision.gameObject.GetComponent<PlayerController>();
        if (p!=null) {pcs.Add(p);}    
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        pcs.Remove(collision.gameObject.GetComponent<PlayerController>());
    }

    void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
		{
			waterCollider = collision.gameObject.GetComponent<BoxCollider2D>();
            foreach(GameObject l in ledges) {
                l.SetActive(true);
            } 
		}
    }

    void OnTriggerExit2D(Collider2D collision)
	{
        Debug.Log("OUT");
		if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
		{
        Debug.Log("OUTWATER");
			waterCollider = null;
            foreach(GameObject l in ledges) {
                l.SetActive(false);
            } 
		}
        
    }
}
