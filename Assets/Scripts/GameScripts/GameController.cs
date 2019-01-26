using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class GameController : MonoBehaviour {
    public static GameController Instance = null; 

    public int StartFoodCount = 10;
    public float GridOffsetX = 1.0f;
    public float GridOffsetY = 1.0f;
    public int GridSizeX = 14;
    public int GridSizeY = 9;
    public float TileSize = 2.0f;

    private int foodCount;
    public int FoodCount { 
        get { 
            return foodCount; 
        } 
        set { 
            foodCount = value; 
            UpdateGUI(); 
        } 
    }
    public int EnergyUsage { 
        get {
            int sum = 0;
            foreach (Module module in modules) {
                sum += module.EnergyUsage;
            }
            return sum;
        }
    }
    public int EnergyProduction { 
        get {
            int sum = 0;
            foreach (Module module in modules) {
                sum += module.EnergyProduction;
            }
            return sum;
        }
    }
    public int HumanCount { 
        get {
            int sum = 0;
            foreach (Module module in modules) {
                sum += module.HumanCount;
            }
            return sum;
        }
    }

    private List<Module> modules = new List<Module>();
    private List<GameEvent> gameEvents = new List<GameEvent>();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        
        PlayIntro();
    }

    public void PlayIntro() {
        StartCoroutine(PlayIntroRoutine());
    }

    public void StartGame() {
        StartCoroutine(StartGameRoutine());
    }

    public void EndGame() {
        StartCoroutine(EndGameRoutine());
    }
    
    public void StartTurn() {
        StartCoroutine(StartTurnRoutine());
    }

    public void EndTurn() {
        StartCoroutine(EndTurnRoutine());
    }
    
    private IEnumerator PlayIntroRoutine() {
        yield return new WaitForSeconds(1);
    }
    
    private IEnumerator StartGameRoutine() {
        yield return new WaitForSeconds(1);
        
        FoodCount = StartFoodCount;
    }
    
    private IEnumerator EndGameRoutine() {
        yield return new WaitForSeconds(1);
    }
   
    private IEnumerator StartTurnRoutine() {
        yield return new WaitForSeconds(1);

        EnableInput();

        foreach (GameEvent gameEvent in gameEvents) {
            gameEvent.StartTurn();
        }
        foreach (Module module in modules) {
            module.StartTurn();
        }
    }
    
    private IEnumerator EndTurnRoutine() {
        DisableInput();
        
        yield return new WaitForSeconds(1);

        foreach (GameEvent gameEvent in gameEvents) {
            gameEvent.EndTurn();
        }
        foreach (Module module in modules) {
            module.EndTurn();
        }
    }

    public void DisableInput() {
    }
    
    public void EnableInput() {
    }
    
    public void UpdateGUI() {
    }
    
    public bool IsModuleOutsideGrid(Module module) {
        foreach (Point tile in module.Tiles) {
            if (tile.x + module.Offset.x < 0 ||
                tile.y + module.Offset.y < 0 ||
                tile.x + module.Offset.x >= GridSizeX ||
                tile.y + module.Offset.y >= GridSizeY) {
                return true;
            }
        }
        return false;
    }
    
    public bool AreModulesOverlapping(Module moduleA, Module moduleB) {
        foreach (Point tileA in moduleA.Tiles) {
            foreach (Point tileB in moduleB.Tiles) {
                if (tileA.x + moduleA.Offset.x == tileB.x + moduleB.Offset.x && 
                    tileA.y + moduleA.Offset.y == tileB.y + moduleB.Offset.y) {
                    return true;
                }
            }
        }
        return false;
    }

    public bool AreModulesConnected(Module moduleA, Module moduleB) {
        for (int iA = 0; iA < moduleA.Tiles.Count; iA++) {
            for (int iB = 0; iB < moduleB.Tiles.Count; iB++) {
                Point tileA = moduleA.Tiles[iA];
                Point tileB = moduleB.Tiles[iB];
                Point connA = moduleA.Connectors[iA];
                Point connB = moduleB.Connectors[iB];
                if (tileA.x + moduleA.Offset.x + connA.x == tileB.x + moduleB.Offset.x + connB.x && 
                    tileA.y + moduleA.Offset.y + connA.y == tileB.y + moduleB.Offset.y + connB.y) {
                    return true;
                }
            }
        }
        return false;
    }
    
    public bool TryPlaceModule(Module module, bool test) {
        if (IsModuleOutsideGrid(module)) {
            return false;
        }
        
        foreach (Module otherModule in modules) {
            if (AreModulesOverlapping(module, otherModule)) {
                return false;
            }
        }
        
        foreach (Module otherModule in modules) {
            if (AreModulesConnected(module, otherModule)) {
                if (!test) {
                    modules.Add(module);
                }
                return true;
            }
        }

        return false;
    }

    public Module FindModuleByTile(Point tile) {
        foreach (Module module in modules) {
            foreach (Point otherTile in module.Tiles) {
                if (tile.x + module.Offset.x == otherTile.x && 
                    tile.y + module.Offset.y == otherTile.y) {
                    return module;
                }
            }
        }
        return null;
    }

    public Module FindModuleByType(Module.ModuleType moduleType) {
        foreach (Module module in modules) {
            if (module.Type == moduleType) {
                return module;
            }
        }
        return null;
    }

    public Module GetRandomModule(List<Module> ignore = null) {
        List<Module> candidates = new List<Module>();
        foreach (Module module in modules) {
            if (ignore == null || !ignore.Contains(module)) {
                candidates.Add(module);
            }
        }
        if (candidates.Count == 0) {
            return null;
        }
        return candidates[Random.Range(0, candidates.Count - 1)];
    }
}
