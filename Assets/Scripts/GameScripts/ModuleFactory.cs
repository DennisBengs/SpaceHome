using UnityEngine;

public sealed class ModuleFactory : MonoBehaviour {
    public Module Prefab;
    
    public Module CreateRandomModule() {
        throw new UnityException();
    }
}
