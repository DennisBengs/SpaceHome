using UnityEngine;
using System.Collections.Generic;

public sealed class EventFactory : MonoBehaviour {
    public static EventFactory Instance = null; 

    public List<GameEvent> Events;
    public GameEvent ElevatorEvent;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public GameEvent CreateElevatorEvent() {
        return Instantiate(ElevatorEvent);
    }
    
    public GameEvent CreateRandomEvent() {
        return Instantiate(Events[Random.Range(0, Events.Count - 1)]);
    }
}
