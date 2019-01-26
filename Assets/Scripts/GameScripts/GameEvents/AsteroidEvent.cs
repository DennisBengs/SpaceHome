using UnityEngine;

public sealed class AsteroidEvent : GameEvent {
    public SpriteRenderer Border;
    public override void StartTurn() {

    }
    
    public override void EndTurn() {
    }

    private void Update() {
        float alpha = Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup * 2 * Mathf.PI * 0.3f));
        Border.color = new Color(1, 0, 0, alpha);
    }
}
