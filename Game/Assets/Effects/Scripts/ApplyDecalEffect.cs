using UnityEngine;
using System.Collections;

public class ApplyDecalEffect : Effect
{
    public GameObject decalPrefab;
    public GameObject positionObject;
    public Vector3 offsetFromObject;

    private static GameObject decalParent;

    protected override void Awake() {
        base.Awake();

        if (!decalParent) {
            decalParent = new GameObject("DecalParent");
            decalParent.transform.position = Vector3.zero;
        }
    }
    
    protected override void Start() {
        base.Start();
    }
    
    protected override void Update() {
        base.Update();
    }
    
    public override void TriggerEffect() {
        base.TriggerEffect();
    }
    
    public override void StartEffect() {
        base.StartEffect();

        GameObject decal = (GameObject) Instantiate(decalPrefab);
        
        Vector3 position = positionObject.transform.position;
        // Orient and offset in the x direction
        position.x += offsetFromObject.x * positionObject.transform.forward.x;
        // Simply offset in the y and z directions
        position.y += offsetFromObject.y;
        position.z += offsetFromObject.z;
        decal.transform.position = position;
        
        decal.transform.parent = decalParent.transform;
    }
    
    public override void EndEffect() {
        base.EndEffect();
    }
}
