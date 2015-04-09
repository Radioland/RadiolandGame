using System;
using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour
{
    public Sprite image;
    public string type;

    private void Awake() {

    }

    private void Start() {

    }

    private void Update() {

    }

    protected void Collect() {
        Messenger.Broadcast("CollectObject", type);
    }
}
