using UnityEngine;

public abstract class GameEvent : MonoBehaviour {
    public bool IsDestroyed { get; protected set; }
    
    public abstract void StartTurn();
    
    public abstract void EndTurn();
}
