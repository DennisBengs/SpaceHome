using UnityEngine;

public sealed class ModuleFactory : MonoBehaviour {
    public static ModuleFactory Instance = null; 
    
    public Module BaseModule;
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }
    
    public Module CreateElevatorModule(Point offset) {
        Module module = Instantiate(BaseModule);
        module.Setup(ModuleType.Elevator, offset);
        return module;
    }

    public Module CreateRandomModule(Point offset) {
        Module module = Instantiate(BaseModule);
        module.Setup(ModuleType.Empty, offset);
        return module;
    }
}
