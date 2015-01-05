using UnityEngine;
using System.Collections;

public class HighJump : JumpPowerup
{
    [SerializeField] private float jumpHeight = 4.0f;

    private int highJumpHash;

    public override void Awake() {
        base.Awake();

        highJumpHash = Animator.StringToHash("HighJump");
    }

    public override void Start() {
        base.Start();
    }

    public override void Update() {
        base.Update();
    }

    public override bool CanUsePowerup() {
        return base.CanUsePowerup() && characterMovement.grounded;
    }

    public override void UsePowerup() {
        base.UsePowerup();

        characterMovement.SetJumpHeight(jumpHeight);
        animator.SetBool(highJumpHash, true);
    }

    public override void EndPowerup() {
        base.EndPowerup();

        characterMovement.ResetJumpHeight();
        animator.SetBool(highJumpHash, false);
    }
}
