using UnityEngine;
using System.Collections;

public class LoadLevelEffect : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private bool loadNext = true;
    [SerializeField] private int levelToLoad;
    [SerializeField] private GameObject loadingScreen;

    protected override void Awake() {
        base.Awake();
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

        if (loadNext) { levelToLoad = Application.loadedLevel + 1; }
        if (Application.levelCount > levelToLoad || levelToLoad < 0) {
            GameObject loadingScreenObject = (GameObject) Instantiate(loadingScreen);
            Loading loading = loadingScreenObject.GetComponent<Loading>();
            if (loading) {
                loading.LoadLevel(levelToLoad);
            } else {
                Application.LoadLevel(levelToLoad);
            }
        } else {
            Debug.LogWarning("Could not load level " + levelToLoad + ", out of range.");
        }
    }

    public override void EndEffect() {
        base.EndEffect();
    }
}
