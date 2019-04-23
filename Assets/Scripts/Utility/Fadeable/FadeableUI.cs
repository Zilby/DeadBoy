using UnityEngine;
using System.Collections;


/// <summary>
/// A simple class that can be inherited to enable FadeIn / FadeOut functionality for a UI object.
/// Requires a reference to a canvasGroup.
/// </summary>
public class FadeableUI : Fadeable
{
    /// <summary>
    /// The canvas group that will be Faded In/Out
    /// </summary>
    [SerializeField]
    protected CanvasGroup canvasGroup;

    public CanvasGroup CanvasGroup
    {
        get
        {
            return canvasGroup;
        }
    }


    public override float Alpha
    {
        get
        {
            return canvasGroup.alpha;
        }

        set
        {
            canvasGroup.alpha = value;
        }
    }

    public override bool Active
    {
        set
        {
            canvasGroup.gameObject.SetActive(value);
        }
    }

    public override bool BlocksRaycasts
    {
        set
        {
            canvasGroup.blocksRaycasts = value;
			canvasGroup.interactable = value;
        }
    }

    protected virtual void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

    }


}