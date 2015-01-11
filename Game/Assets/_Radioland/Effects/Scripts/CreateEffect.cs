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
    [Tooltip("Calls StartEvent on an EffectManager on the created object at the end.")]
    [SerializeField] private bool chainEffectsOnEnd = false;
    [Range(0.0f, 1.0f)] [SerializeField] private float chanceToSpawn = 1.0f;

    private static GameObject createEffectParent;

    private GameObject createdObject;

    protected override void Awake() {
        base.Awake();

        if (referenceObjectTag.Length > 0 && !referenceObject) {
            referenceObject = GameObject.FindGameObjectWithTag(referenceObjectTag);
        }

        if (!createEffectParent) {
            createEffectParent = new GameObject();
            createEffectParent.name = "CreateEffectParent";
            createEffectParent.transform.position = Vector3.zero;
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

            if (referenceObject && parentToReference) {
                createdObject.transform.parent = referenceObject.transform;
                createdObject.transform.localPosition = localPosition;
            } else {
                createdObject.transform.parent = createEffectParent.transform;
                if (referenceObject) {
                    createdObject.transform.position = referenceObject.transform.position;
                    createdObject.transform.Translate(localPosition);
                } else {
                    createdObject.transform.position = localPosition;
                }
            }

            createdObject.transform.localEulerAngles = localEulerAngles;

            // Add a backup destruction method, in case another object is created.
            // TODO: make chainEffectsOnEnd also work in this case.
            //       use effects system? or keep a list of objects in this class?
            if (destroyOnEnd) {
                MaxLifetime maxLifetime = createdObject.AddComponent<MaxLifetime>();
                maxLifetime.maxTimeToLive = timing.duration;
            }
        }
    }

    public override void EndEffect() {
        base.EndEffect();

        if (chainEffectsOnEnd && createdObject) {
            EffectManager effectManager = createdObject.GetComponent<EffectManager>();
            if (effectManager) {
                effectManager.StartEvent();
            }
        }
        if (destroyOnEnd && createdObject) {
            Destroy(createdObject);
        }
    }
}
