using UnityEngine;

public sealed class CameraRotator : MonoBehaviour {
    private float angle = -90.0f;
    
    private void Update() {
        angle = Mathf.Lerp(angle, 0.0f, 0.05f);
        Camera.main.transform.eulerAngles = new Vector3(angle, 0.0f, Mathf.Cos(Time.realtimeSinceStartup * 2.0f * Mathf.PI * 0.05f) * 0.75f);
    }
}
