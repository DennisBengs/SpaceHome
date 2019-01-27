using UnityEngine;
using UnityEngine.UI;
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

    public GameObject EndTurnButton;
    public Text FoodText;
    public Text EnergyText;
    public Text TurnText;

    private Module blueprint;

    private int turnIndex;
    public int TurnIndex { 
        get { 
            return turnIndex; 
        } 
        set {
            turnIndex = value; 
            TurnText.text = turnIndex.ToString();
        } 
    }
    
    private int foodCount;
    public int FoodCount { 
        get { 
            return foodCount; 
        } 
        set {
            foodCount = value; 
            FoodText.text = foodCount.ToString();
        } 
    }

    private int energyCount;
    public int EnergyCount { 
        get { 
            return energyCount; 
        } 
        set { 
            energyCount = value; 
            EnergyText.text = energyCount.ToString();
        } 
    }
    public int EnergyUsage { 
        get {
            int sum = 0;
            foreach (Module module in modules) {
                sum += module.EnergyUsage[module.Type];
            }
            return sum;
        }
    }
    public int CrewCount { 
        get {
            int sum = 0;
            foreach (Module module in modules) {
                sum += module.CrewCount;
            }
            return sum;
        }
    }

    private List<Module> modules = new List<Module>();
    private List<GameEvent> gameEvents = new List<GameEvent>();

    private void Start() {
        if (Instance == null) {
            Instance = this;
        }
        
        StartGame();
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
    
    private IEnumerator StartGameRoutine() {
        EndTurnButton.SetActive(false);

        FoodCount = StartFoodCount;
        EnergyCount = 0;
        TurnIndex = 0;
        
        ModuleFactory.Instance.CreateElevatorModule(new Point(Random.Range(3, GridSizeX - 4), Random.Range(3, GridSizeY - 4)));

        yield return new WaitForSeconds(2);

        StartTurn();
        
        yield return null;
    }
    
    private IEnumerator EndGameRoutine() {
        yield return new WaitForSeconds(1);
        
        foreach (Module module in modules) {
            Destroy(module.gameObject);
        }
        modules.Clear();
        gameEvents.Clear();
        
        yield return new WaitForSeconds(1);
        
        StartGame();
        
        yield return null;
    }
   
    private IEnumerator StartTurnRoutine() {
        TurnIndex++;
        
        FindModuleByType(Module.ModuleType.Elevator).SpaceElevatorAtPlatform = true;

        yield return new WaitForSeconds(4);

        gameEvents.Add(EventFactory.Instance.CreateElevatorEvent());
        gameEvents.Add(EventFactory.Instance.CreateRandomEvent());

        foreach (GameEvent gameEvent in gameEvents) {
            gameEvent.StartTurn();
        }
        foreach (Module module in modules) {
            module.StartTurn();
        }

        blueprint = ModuleFactory.Instance.CreateRandomModule(new Point(GridSizeX / 2, GridSizeY / 2));

        EndTurnButton.SetActive(true);
        
        yield return null;
    }
    
    private IEnumerator EndTurnRoutine() {
        EndTurnButton.SetActive(false);
        
        if (!blueprint.Placed) {
            Destroy(blueprint.gameObject);
        }
        
        FindModuleByType(Module.ModuleType.Elevator).SpaceElevatorAtPlatform = false;

        yield return new WaitForSeconds(2);

        foreach (GameEvent gameEvent in gameEvents) {
            gameEvent.EndTurn();
        }
        foreach (Module module in modules) {
            module.EndTurn();
        }

        StartTurn();
        
        yield return null;
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
                bool[] connectorA = moduleA.Connectors[iA];
                bool[] connectorB = moduleB.Connectors[iB];
                for (int cA = 0; cA < 4; cA++) {
                    for (int cB = 0; cB < 4; cB++) {
                        if (connectorA[cA] && connectorB[cB] && 
                            tileA.x + moduleA.Offset.x + Module.SideOffsets[cA].x == tileB.x + moduleB.Offset.x && 
                            tileA.y + moduleA.Offset.y + Module.SideOffsets[cA].y == tileB.y + moduleB.Offset.y &&
                            tileA.x + moduleA.Offset.x == tileB.x + moduleB.Offset.x + Module.SideOffsets[cB].x && 
                            tileA.y + moduleA.Offset.y == tileB.y + moduleB.Offset.y + Module.SideOffsets[cB].y) {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    
    public bool TestModulePlacement(Module module) {
        if (IsModuleOutsideGrid(module)) {
            return false;
        }
        
        foreach (Module otherModule in modules) {
            if (module != otherModule && AreModulesOverlapping(module, otherModule)) {
                return false;
            }
        }
        
        foreach (Module otherModule in modules) {
            if (module != otherModule && AreModulesConnected(module, otherModule)) {
                return true;
            }
        }

        return false;
    }
    
    public void PlaceModule(Module module) {
        modules.Add(module);
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
