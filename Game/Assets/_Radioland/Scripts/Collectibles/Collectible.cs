using System;
using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour
{
    public Sprite image;
    public string type;
    public bool playAnim;

    private void Awake() {

    }

    private void Start() {

    }

    private void Update() {

    }

    public void Collect() {
        Messenger.Broadcast("CollectObject", type, playAnim);
    }
}
