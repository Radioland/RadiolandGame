using UnityEngine;
using System.Collections;

public class LowGravity : JumpPowerup
{
    [SerializeField] private float gravity = 4.0f;
    
    public override void Awake() {
        base.Awake();
    }
    
    public override void Start() {
        base.Start();
    }
    
    public override void Update() {
        base.Update();

        // Debug usage, potentially glitchy.
        if (Input.GetKeyDown(KeyCode.G)) {
            UsePowerup();
        }
    }

    public override void UsePowerup() {
        base.UsePowerup();

        Debug.Log("Used Low Gravity.");
        
        characterMovement.SetGravity(gravity);

        if (characterMovement.jumping) {
            StartJump();
        }
    }
    
    public override void EndPowerup() {
        base.EndPowerup();
        
        characterMovement.ResetGravity();
    }
}
