using UnityEngine;
using System.Collections;


/// <summary>
/// A simple class that can be inherited to enable FadeIn / FadeOut functionality for a Sprite object.
/// Requires a reference to a sprite renderer.
/// </summary>
public class FadeableSprite : Fadeable
{
    /// <summary>
    /// The sprite renderer that will be Faded In/Out
    /// </summary>
    [SerializeField]
    protected SpriteRenderer rend;

    public SpriteRenderer Rend
    {
        get
        {
            return rend;
        }
    }

    public override float Alpha
    {
        get
        {
            return rend.color.a;
        }

        set
        {
			rend.color = rend.color.A(value);
        }
    }

    public override bool BlocksRaycasts
    {
        set
        {
			Collider2D col = rend.GetComponent<Collider2D>();
			if (col != null)
			{
				rend.GetComponent<Collider2D>().enabled = value;
			}
        }
    }

    public override bool Active
    {
        set
        {
            rend.gameObject.SetActive(value);
        }
    }

    protected virtual void Reset()
    {
        rend = GetComponent<SpriteRenderer>();
        if (rend == null)
        {
            rend = gameObject.AddComponent<SpriteRenderer>();
        }

    }
}