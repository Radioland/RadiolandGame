using UnityEngine;
using System.Collections;

public class CreateEffect : Effect
{
    public GameObject prefabToCreate;
    public GameObject parentObject;
    public string parentObjectTag;
    public bool parentToParent = true;
    public Vector3 localPosition;
    public Vector3 localEulerAngles;
    public bool destroyOnEnd = false;
    [Range(0.0f, 1.0f)] public float chanceToSpawn = 1.0f;

    private GameObject createdObject;
    
    protected override void Awake() {
        base.Awake();

        if (parentObjectTag.Length > 0 && !parentObject) {
            parentObject = GameObject.FindGameObjectWithTag(parentObjectTag);
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

        if (Mathf.Approximately(chanceToSpawn, 1.0f) || Random.Range(0.0f, 1.0f) < chanceToSpawn) {
            createdObject = (GameObject) Instantiate(prefabToCreate);
            if (parentObject) {
                if (parentToParent) {
                    createdObject.transform.parent = parentObject.transform;
                    createdObject.transform.localPosition = localPosition;
                } else {
                    createdObject.transform.position = parentObject.transform.position;
                    createdObject.transform.Translate(localPosition);
                }
            } else {
                createdObject.transform.position = localPosition;
            }
            createdObject.transform.localEulerAngles = localEulerAngles;
        }
    }
    
    public override void EndEffect() {
        base.EndEffect();
        
        if (destroyOnEnd && createdObject) {
            Destroy(createdObject);
        }
    }
}
