using UnityEngine;
using System.Collections;

public class HighJump : Powerup
{
    [SerializeField] private float jumpHeight = 4.0f;

    public override void Awake() {
        base.Awake();
    }
    
    public override void Start() {
        base.Start();
    }
    
    public override void Update() {
        base.Update();

        if (Input.GetKeyDown(KeyCode.H)) {
            UsePowerup();
        }
    }
    
    public override void UsePowerup() {
        base.UsePowerup();

        Debug.Log ("High Jump");

        characterMovement.SetJumpHeight(jumpHeight);
    }
    
    public override void EndPowerup() {
        base.EndPowerup();

        characterMovement.ResetJumpHeight();
    }
}
