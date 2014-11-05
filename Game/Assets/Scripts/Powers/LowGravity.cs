using UnityEngine;
using System.Collections;

public class LowGravity : JumpPowerup
{
    [SerializeField] private float gravity = 4.0f;

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
    }

    // Called via SendMessage in CharacterMovement.
    protected override void StartJump() {
        base.StartJump();

        if (inUse) {
            characterMovement.SetGravity(gravity);
        }
    }
    
    public override void EndPowerup() {
        base.EndPowerup();
        
        characterMovement.ResetGravity();
        animator.SetBool(longJumpHash, false);
    }
}
