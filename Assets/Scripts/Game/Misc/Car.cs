using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
	[Range(0, 10)]
	public float speed;

	private Chargable ch;
	private Rigidbody2D rbody;

	// Start is called before the first frame update
	void Start()
	{
		ch = gameObject.GetComponent<Chargable>();
		rbody = gameObject.GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (ch.Charged)
		{
			rbody.AddForce(new Vector2(speed * 1000, 0));// mostly controlled by drag
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("NotPlayer"))
		{
			ch.Charged = false;
			Destroy(ch);
		}
	}
}
