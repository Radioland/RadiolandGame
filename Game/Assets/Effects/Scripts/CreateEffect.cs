using UnityEngine;
using System.Collections;

public class CreateEffect : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private GameObject prefabToCreate;
    [SerializeField] private GameObject referenceObject;
    [Tooltip("Alternative to setting referenceObject.")]
    [SerializeField] private string referenceObjectTag;
    [SerializeField] private bool parentToReference = true;
    [SerializeField] private Vector3 localPosition;
    [SerializeField] private Vector3 localEulerAngles;
    [SerializeField] private bool destroyOnEnd = false;
    [Range(0.0f, 1.0f)] [SerializeField] private float chanceToSpawn = 1.0f;

    private GameObject createdObject;
    
    protected override void Awake() {
        base.Awake();

        if (referenceObjectTag.Length > 0 && !referenceObject) {
            referenceObject = GameObject.FindGameObjectWithTag(referenceObjectTag);
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
            if (referenceObject) {
                if (parentToReference) {
                    createdObject.transform.parent = referenceObject.transform;
                    createdObject.transform.localPosition = localPosition;
                } else {
                    createdObject.transform.position = referenceObject.transform.position;
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
