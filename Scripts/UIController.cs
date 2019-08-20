using UnityEngine;
 
public class UIController: MonoBehaviour {
 
    private RectTransform myRectTfm;
 
    void Start() {
        myRectTfm = GetComponent<RectTransform>();
    }
 
    void Update() {
        // 自身の向きをカメラに向ける
        myRectTfm.LookAt(Camera.main.transform);
    }
}