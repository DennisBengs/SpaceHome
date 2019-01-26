using UnityEngine;
using System.Collections.Generic;

public sealed class ModuleGrid : MonoBehaviour {
    public Transform GridTile;
    private List<Transform> grid;
    
    private void Start() {
        for (int x = 0; x < GameController.Instance.GridSizeX; x++) {
            for (int y = 0; y < GameController.Instance.GridSizeY; y++) {
                Transform gridTile = Instantiate(GridTile);
                gridTile.transform.position = new Vector3(
                    (x + 0.5f) * GameController.Instance.TileSize + GameController.Instance.GridOffsetX,
                    (y + 0.5f) * GameController.Instance.TileSize + GameController.Instance.GridOffsetY,
                    0.0f);
            }
        }
    }
}
