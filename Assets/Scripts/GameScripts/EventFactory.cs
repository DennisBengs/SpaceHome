using UnityEngine;
using System.Collections.Generic;

public sealed class EventFactory : MonoBehaviour {
    public List<GameEvent> Events;
    public GameEvent ElevatorEvent;
    
    public GameEvent CreateEvent() {
        return Object.Instantiate(ElevatorEvent);
    }
    
    public GameEvent CreateRandomEvent() {
        throw new UnityException();
    }
}
