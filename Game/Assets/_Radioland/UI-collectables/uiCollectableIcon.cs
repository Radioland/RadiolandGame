using UnityEngine;
using System.Collections;

public class uiCollectableIcon : MonoBehaviour
{
    public bool isUnlocked;
    public Material unlockedMaterial;
    private bool materialSet;

    private Vector3 rotateDegreesPerSecond;
    // public bool ignoreTimescale;
    private float m_LastRealTime;

    private Quaternion initialRotation;

    private void Start()
    {
        m_LastRealTime = Time.realtimeSinceStartup;
        initialRotation = transform.rotation;
        materialSet = false;
    }

    // Update is called once per frame
    private void Update()
    {
        float deltaTime = Time.deltaTime;
        // if (ignoreTimescale)
        // {
            deltaTime = (Time.realtimeSinceStartup - m_LastRealTime);
            m_LastRealTime = Time.realtimeSinceStartup;
        // }
        transform.Rotate(rotateDegreesPerSecond * deltaTime);

        if (isUnlocked && !materialSet) {
            GetComponent<Renderer>().material = unlockedMaterial;
            materialSet = true;
        }
    }

    public void startRotate() {
        rotateDegreesPerSecond.y = 30f;
    }

    public void stopRotate() {
        rotateDegreesPerSecond.y = 0;
        transform.rotation = initialRotation;
    }
}
