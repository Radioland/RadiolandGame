using UnityEngine;
using System.Collections;

public class UnparentToWorld : MonoBehaviour {

    // Use this for initialization
    private void Start() {
        transform.parent = null;
    }

}
