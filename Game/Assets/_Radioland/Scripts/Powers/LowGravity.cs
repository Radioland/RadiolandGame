using UnityEngine;
using System.Collections;

public class LowGravity : JumpPowerup
{
    [SerializeField] private float gravity = 2.5f;
    [SerializeField] private float newSmoothDampTimes = 1.0f;
    [SerializeField] private Vector3 launchDirection = new Vector3(0f, 0.5f, 1.0f);
    [SerializeField] private float launchForce = 5.0f;

    private int longJumpHash;

    public override void Awake() {
        base.Awake();

        longJumpHash = Animator.StringToHash("LongJump");
    }

    public override void Start() {
        base.Start();
    }

    public override void Update() {
        base.Update();

        if (primed && !inUse && characterMovement.falling) {
            characterMovement.SetGravity(gravity);
            inUse = true;
        }
    }

    public override void UsePowerup() {
        base.UsePowerup();

        animator.SetBool(longJumpHash, true);

        characterMovement.SetAirSmoothDampTime(newSmoothDampTimes);
        characterMovement.SetGroundSmoothDampTime(newSmoothDampTimes);

        Vector3 launchDirectionRelative = (transform.forward * launchDirection.z +
                                            transform.right * launchDirection.x +
                                            transform.up * launchDirection.y).normalized;
        characterMovement.AddVelocity(launchDirectionRelative * launchForce);
    }

    // Called via SendMessage in CharacterMovement.
    protected override void JumpStarted() {
        base.JumpStarted();

        if (inUse) {
            characterMovement.SetGravity(gravity);
        }
    }

    public override void EndPowerup() {
        base.EndPowerup();

        characterMovement.ResetGravity();
        characterMovement.ResetAirSmoothDampTime();
        characterMovement.ResetGroundSmoothDampTime();

        if (animator && animator.gameObject.activeInHierarchy) {
            animator.SetBool(longJumpHash, false);
        }
    }
}
