using UnityEngine;
using System.Collections;
using Anima2D;


/// <summary>
/// A simple class that can be inherited to enable FadeIn / FadeOut functionality for a SpriteMesh object.
/// Requires a reference to a sprite renderer.
/// </summary>
public class FadeableSpriteMesh : Fadeable
{
    /// <summary>
    /// The sprite renderer that will be Faded In/Out
    /// </summary>
    [SerializeField]
    protected SpriteMeshInstance rend;

    public SpriteMeshInstance Rend
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
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, value);
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
        rend = GetComponent<SpriteMeshInstance>();

    }
}