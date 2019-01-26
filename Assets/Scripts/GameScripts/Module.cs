using UnityEngine;
using System.Collections.Generic;

public sealed class Module : MonoBehaviour {
    public enum ModuleType {
        Empty,
        Hydroponics,
        SolarCells, 
        Elevator,
        HPG,
        PointDefense,
        RobotControl
    }
    
    public bool IsDestroyed { get; private set; }
    public ModuleType Type { get; private set; }
    
    /// Offset of the module in the tile grid
    public Point Offset { get; private set; }

    /// Position of the module tiles relative to the offset
    public List<Point> Tiles { get; private set; }
    
    /// Direction of the connectors for each tile (0,0) = no connector
    public List<Point> Connectors { get; private set; }
    
    public int EnergyUsage { get; private set; }
    public int EnergyProduction { get; private set; }
    public int HumanCount { get; private set; }

    public void StartTurn() {
    }

    public void EndTurn() {
    }
    
    public Vector3 GetCenter() {
        Vector3 avg = new Vector3();
        foreach (Point tile in Tiles) {
            avg.x += transform.position.x + tile.x * GameController.Instance.TileSize;
            avg.y += transform.position.y + tile.y * GameController.Instance.TileSize;
        }
        return avg / Tiles.Count;
    }
}
