using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquishController : PlayerController
{
    /// <summary>
	/// Percentage of max speed when squished.
	/// </summary>
	[Range(0, 1)]
    public float squishSpeedMultipler; 

    /// <summary>
	/// Speed of passing through a grate (dist/sec).
	/// </summary>
	[Range(0, 2)]
    public float grateSpeed; 

    /// <summary>
    /// Multiplier for jump height when in blob mode
    /// <summary>
    [Range(1,2)]
    public float blobJumpMultiplier;

    /// <summary>
	/// Time before coming up through a grate before falling again.
	/// </summary>
	[Range(0, 9)]
    public float grateProtectionTime;

    public override bool AcceptingMoveInput
	{
		get
		{
            return !passingGrate && base.AcceptingMoveInput;
        }
    }

    // high level state
    private bool blobMode;
    private bool transitioning; //to from squish mode
    private bool passingGrate;

    // low level state
    private Collider2D lastGrate;
    private bool onGrate;
    private float grateProt;
    private bool passingGrateDown; // only used for animation

	public override int SORT_VALUE
	{
		get { return 3; }
	}

	public override string Name
	{
		get { return "Squish"; }
	}

    public override float GetJumpHeight {
        get {
            if (blobMode) {
                return jumpHeight * blobJumpMultiplier;
            } else {
                return jumpHeight;
            }
        }
    }

	protected override void Update()
	{
		base.Update();
        grateProt -= Time.deltaTime;

        if (DBInputManager.GetInput(this, PlayerInput.Power, InputType.Pressed) && !transitioning) 
        {
            StartCoroutine(ToggleSquish());
        }
        if (blobMode && onGrate && !passingGrate 
            && (grateProt < 0 || DBInputManager.GetInput(this, PlayerInput.Down, InputType.Pressed)) 
            && transform.position.x - lastGrate.transform.position.x < 0.5)
        {
            c = StartCoroutine(GrateCoroutine(transform.position.y > lastGrate.transform.position.y));
        }
	}

    public IEnumerator ToggleSquish() 
    {
            transitioning = true;
            blobMode = !blobMode;
            //Loop over time
            {
                speed = speed * (blobMode ? squishSpeedMultipler : 1/squishSpeedMultipler);
                transform.localScale = transform.localScale.YMul(blobMode ? 0.5f : 2);
            //    yield return null;
            }
            transitioning = false;
            yield return null;
    }

    protected override void SetAnimationState()
	{
        base.SetAnimationState();
//TODO
        anim.SetBool("BlobMode", blobMode);
        anim.SetBool("PassingGrate", passingGrate);
        anim.SetBool("PassingGrateDown", passingGrateDown);
		
    }

    #region GrateTriggers

    private Coroutine c;

    protected override void EnterGrate(Collider2D trigger)
	{
        onGrate = true;
        lastGrate = trigger;
	}

	protected override void ExitGrate(Collider2D trigger)
	{
        onGrate = false;
        grateProt = 0;
	}

    protected IEnumerator GrateCoroutine(bool down) {
        passingGrate = true;
        passingGrateDown = down;
        rBody.isKinematic = true;
        rBody.velocity = Vector3.zero; 
        float direction = down ? -1 : 1;
        while (onGrate) {
            transform.Translate(Vector3.zero.Y(grateSpeed*direction*Time.deltaTime));
            yield return null;
        }
        
        /*if (!down) 
        {
            if (DBInputManager.GetInput(this, PlayerInput.Left, InputType.Held, false)) {
                direction = -1;
            } else if (DBInputManager.GetInput(this, PlayerInput.Right, InputType.Held, false)) {
                direction = 1;
                } else {
                direction = transform.position.x > lastGrate.transform.position.x ? 1 : -1;
                
            }
            rBody.velocity = Vector3.zero.X(direction*0.00001f);
            
            while(cCollider.bounds.min.x < lastGrate.bounds.max.x
                && cCollider.bounds.max.x > lastGrate.bounds.min.x) {
                transform.Translate(Vector3.zero.X(grateSpeed*direction*Time.deltaTime), Space.World);
                Debug.Log(transform.position.x + "        " + direction + "       "  + grateSpeed*direction*Time.deltaTime);
                yield return null;
            } 
        }*/

        if (!down) {
            grateProt = grateProtectionTime;
        }
        passingGrate = false;
        rBody.isKinematic = false;
        yield return null;
    }

    #endregion
}