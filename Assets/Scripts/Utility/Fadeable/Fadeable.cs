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

    [Header("Fade On Enable")]
    /// <summary>
    /// Delay after activation before fading in
    /// </summary>
	public bool fadeOnEnable = false;
    [ConditionalHideAttribute("fadeOnEnable", true, false)]
	public bool fadeIn = true; //false for out
    
    [Header("Delayed Fade")]
    public float fadeDelay = 0;
	public float fadeDuration = FADE_IN_DUR;
    
    [Header("Other Options")]
    /// <summary>
    /// Disable game object after fading out
    /// </summary>
    public bool disableOnFadeOut = false;
    /// <summary>
    /// Whether the object blocks raycasts during the fade
    /// </summary>
    public bool hitboxDuringFade = false;


    /// <summary>
    /// A reference to an active fade coroutine.
    /// </summary>
	protected Coroutine fadeCoroutine;
    protected bool isFading = false;

    public bool IsVisible { get; protected set; }
    public bool VisibleOrFading { get{ return IsVisible || isFading; } }

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
		if(fadeOnEnable) 
		{
			StartCoroutine(fadeIn ? DelayedFadeIn() : DelayedFadeOut());
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

	public virtual void SelfDelayedFadeIn(float delay = 1f) {
		fadeDelay = delay;
		StartCoroutine(DelayedFadeIn());
	}

	public virtual void SelfDelayedFadeOut(float delay = 1f)
	{
		fadeDelay = delay;
		StartCoroutine(DelayedFadeOut());
	}

	public virtual IEnumerator DelayedFadeIn() {
		IsVisible = false;
		Alpha = 0;
        BlocksRaycasts = false;
		yield return new WaitForSeconds(fadeDelay);
		SelfFadeIn(dur:fadeDuration);
	}

    public virtual IEnumerator DelayedFadeOut() {
		IsVisible = true;
		Alpha = 1;
		yield return new WaitForSeconds(fadeDelay);
		SelfFadeOut(dur:fadeDuration);
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
        BlocksRaycasts = hitboxDuringFade;
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
        BlocksRaycasts = hitboxDuringFade;

        while (timeElapsed < duration)
        {
            Alpha = Mathf.Lerp(1, endAlpha, timeElapsed / duration);
            yield return null;
            timeElapsed += useUnscaledDeltaTimeForUI ? Time.unscaledDeltaTime : Time.deltaTime;
        }
        Alpha = endAlpha;
        isFading = false;
        if (disableOnFadeOut) {
			Active = Alpha != 0;
		}
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
