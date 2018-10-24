using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple class that can be inherited to enable FadeIn / FadeOut functionality.
/// </summary>
public abstract class Fadeable : MonoBehaviour
{
    protected const float FADE_IN_DUR = 0.3f;
    protected const float FADE_OUT_DUR = 0.2f;

    /// <summary>
    /// Allows the FadeableUI to tween without being affected by TimeScale.
    /// NOTE: This can potentially cause frames to be skipped.
    /// </summary>
    [System.NonSerialized]
    public bool useUnscaledDeltaTimeForUI = true;

	public bool fadeInOnEnable = false;
    /// <summary>
    /// A reference to an active fade coroutine.
    /// </summary>
    protected Coroutine fadeCoroutine;
    protected bool isFading = false;

    public bool IsVisible { get; protected set; }

    /// <summary>
    /// Gets or sets the alpha value.
    /// </summary>
    public abstract float Alpha
    {
        get;
        set;
    }

    /// <summary>
    /// Sets whether or not the fading object is active. 
    /// </summary>
    public abstract bool Active
    {
        set;
    }

    /// <summary>
    /// Sets whether or not the fading object blocks raycasts. 
    /// </summary>
    public virtual bool BlocksRaycasts
    {
        set { }
    }

    public bool IsFading
    {
        get
        {
            return isFading;
        }
    }

    protected virtual void Awake()
    {
        IsVisible = Alpha > 0;
    }

	public virtual void OnEnable() 
	{
		if(fadeInOnEnable) 
		{
			SelfFadeIn();
		}
	}

	/// <summary>
	/// Immediately displays the sprite.
	/// </summary>
	public virtual void Show()
    {
        IsVisible = true;
        Alpha = 1;
        Active = true;
        BlocksRaycasts = true;
    }


    /// <summary>
    /// Immediately hides the sprite.
    /// </summary>
    public virtual void Hide()
    {
        IsVisible = false;
        Alpha = 0;
        Active = false;
        BlocksRaycasts = false;
    }


    /// <summary>
    /// Fades in the Canvas group over the defined time.
    /// Interaction is disabled until the animation has finished.
    /// </summary>
    public virtual IEnumerator FadeIn(float startAlpha = 0, float endAlpha = 1, float dur = FADE_IN_DUR)
    {
        IsVisible = true;
        isFading = true;
        Active = true;
        BlocksRaycasts = false;
        Alpha = startAlpha;

        float duration = dur;
        float timeElapsed = duration * Alpha;

        while (timeElapsed < duration)
        {
            Alpha = Mathf.Lerp(startAlpha, endAlpha, timeElapsed / duration);
            yield return null;
            timeElapsed += useUnscaledDeltaTimeForUI ? Time.unscaledDeltaTime : Time.deltaTime;
        }
        Alpha = endAlpha;
        BlocksRaycasts = true;
        isFading = false;
        yield break;
    }


    /// <summary>
    /// Fades out the Canvas group over the defined time.
    /// Interaction is becomes disabled immediately.
    /// </summary>
    public virtual IEnumerator FadeOut(float endAlpha = 0, float dur = FADE_OUT_DUR)
    {
        IsVisible = false;
        isFading = true;
        float duration = dur;
        float timeElapsed = duration * (1f - Alpha);
        BlocksRaycasts = false;

        while (timeElapsed < duration)
        {
            Alpha = Mathf.Lerp(1, endAlpha, timeElapsed / duration);
            yield return null;
            timeElapsed += useUnscaledDeltaTimeForUI ? Time.unscaledDeltaTime : Time.deltaTime;
        }
        Alpha = endAlpha;
        Active = Alpha != 0;
        isFading = false;
        yield break;
    }


    /// <summary>
    /// Starts the FadeIn coroutine inside this script.
    /// </summary>
    /// <param name="force">If true, any previously running fade animation will be cancelled</param>
    public virtual void SelfFadeIn(bool force = true, float startAlpha = 0, float endAlpha = 1, float dur = FADE_IN_DUR)
    {
        if (!force && isFading)
        {
            return;
        }
        // Make the self active incase disabled, coroutine cant run otherwise.
        this.gameObject.SetActive(true);
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeIn(startAlpha, endAlpha, dur));
    }


    /// <summary>
    /// Starts the FadeOut coroutine inside this script. 
    /// </summary>
    /// <param name="force">If true, any previously running fade animation will be cancelled</param>
    public virtual void SelfFadeOut(bool force = true, float endAlpha = 0, float dur = FADE_OUT_DUR)
    {
        if (!force && isFading)
        {
            return;
        }
        // Make the self active incase disabled, coroutine cant run otherwise.
        this.gameObject.SetActive(true);
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOut(endAlpha, dur));
    }
}
