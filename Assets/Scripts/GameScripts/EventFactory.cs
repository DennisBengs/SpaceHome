using UnityEngine;
using System.Collections.Generic;

public sealed class EventFactory : MonoBehaviour {
    public List<GameEvent> Events;
    public GameEvent ElevatorEvent;

    public GameEvent CreateEvent() {
        return Instantiate(ElevatorEvent);
    }
    
    public GameEvent CreateRandomEvent() {
        return Instantiate(Events[Random.Range(0, Events.Count - 1)]);
    }
}
