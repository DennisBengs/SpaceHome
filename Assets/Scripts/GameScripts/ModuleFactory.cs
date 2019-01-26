using UnityEngine;

public sealed class ModuleFactory : MonoBehaviour {
    public Module ElevatorModule;
    public Module BaseModule;
    
    public Module CreateElevatorModule() {
        return Instantiate(ElevatorModule);
    }

    public Module CreateRandomModule() {
        return Instantiate(BaseModule);
    }
}
