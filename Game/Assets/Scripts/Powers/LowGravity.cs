using UnityEngine;
using System.Collections;

public class LowGravity : Powerup
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

        if (Input.GetKeyDown(KeyCode.G)) {
            UsePowerup();
        }
    }
    
    public override void UsePowerup() {
        base.UsePowerup();

        Debug.Log("Used Low Gravity.");
        
        characterMovement.SetGravity(gravity);
    }
    
    public override void EndPowerup() {
        base.EndPowerup();
        
        characterMovement.ResetGravity();
    }
}
