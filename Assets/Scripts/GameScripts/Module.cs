using UnityEngine;
using System.Collections.Generic;

public sealed class Module : MonoBehaviour {
    public enum ModuleType {
        Empty = 0,
        Hydroponics = 1,
        SolarCells = 2, 
        Elevator = 3,
        HPG = 4,
        PointDefense = 5,
        RobotControl = 6
    }
    
    private string elevatorShape =      
        "C##C|"+
        " ## |"+
        " CC ";
    
    private List<string> shapes = new List<string> {
        "   C   |"+
        "  #### |"+
        "C##  #C|"+
        "  #### |"+
        "   C   |"+
        "       |"+
        "       "
    };
    
    public bool Destroyed { get; private set; }

    private ModuleType type;
    public ModuleType Type { 
        get {
            return type;
        } 
        set {
            type = value;
            UpdateSprites();
        }
    }
    
    private bool damaged;
    public bool Damaged { 
        get {
            return damaged;
        }
        set {
            damaged = value;
            UpdateSprites();
        }
    }
    
    private bool powered;
    public bool Powered { 
        get {
            return powered;
        }
        set {
            powered = value;
            UpdateSprites();
        }
    }
    
    private bool underConstruction;
    public bool UnderConstruction { 
        get {
            return underConstruction;
        }
        set {
            underConstruction = value;
            UpdateSprites();
        }
    }
    
    private Vector2 placeOffset;
    private Vector2 dragFrom;
    private bool dragging = false;
    private bool placed = false;
    private bool validPlacement = false;
    
    public Point Offset { get; private set; }
    public List<Point> Tiles { get; private set; }
    public List<Point> Connectors { get; private set; }
    
    public int EnergyUsage { get; private set; }
    public int EnergyProduction { get; private set; }
    public int HumanCount { get; private set; }

    public SpriteRenderer SpriteTemplate;
    public CrewPosition CrewPositionTemplate;
    
    public Sprite ElevatorSprite;
    public Sprite TileExteriorSprite;
    public Sprite TileInteriorSprite;
    public Sprite TileInteriorJoinSprite;
    public Sprite TileConnectorSprite;
    public Sprite TileDamageSprite;
    public Color ValidPlaceColor = new Color(0.3f, 0.8f, 0.3f, 0.5f);
    public Color InvalidPlaceColor = new Color(0.8f, 0.3f, 0.3f, 0.5f);
    public Color UnpoweredColor = new Color(0.5f, 0.5f, 0.5f);
    public Color UnderConstructionColor = new Color(0.8f, 0.8f, 0.8f);
    public Color DamageColor = new Color(1.0f, 0.0f, 0.0f);
    public List<Color> TypeColors = new List<Color> {Color.green, Color.green, Color.yellow, Color.grey, Color.blue, Color.red, Color.cyan};
    public List<Sprite> TypeIconSprites;

    private SpriteRenderer icon;
    private List<SpriteRenderer> interiors = new List<SpriteRenderer>();
    private List<SpriteRenderer> exteriors = new List<SpriteRenderer>();
    private List<SpriteRenderer> damages = new List<SpriteRenderer>();
    private List<CrewPosition> crewPositions = new List<CrewPosition>();
    private bool dirtyPosition = true;
    
    private Point[] sideOffsets = {new Point(-1, 0), new Point(0, 1), new Point(1, 0), new Point(0, -1)};
    private float[] sideRotations = {0.0f, -90.0f, -180.0f, -270.0f};

    public void Setup(ModuleType moduleType, Point offset) {
        Tiles = new List<Point>();
        Connectors = new List<Point>();

        Offset = offset;
        if (moduleType == ModuleType.Elevator) {
            GameController.Instance.PlaceModule(this);
            placed = true;
        }

        string shape = moduleType == ModuleType.Elevator ? elevatorShape : shapes[Random.Range(0, shapes.Count -1)];
        string[] rows = shape.Split('|');
        for (int y = 0; y < rows.Length; y++) {
            for (int x = 0; x < rows[y].Length; x++) {
                char c = rows[y][x];
                if (c != '#') {
                    continue;
                }
                Tiles.Add(new Point(x, y));
                Point connector = new Point(0, 0);
                for (int i = 0; i < 4; i++) {
                    Point o = sideOffsets[i];
                    int x2 = x + o.x;
                    int y2 = y + o.y;
                    if (x2 >= 0 && y2 >= 0 && x2 < rows[y].Length && 
                        y2 < rows.Length && rows[y2][x2] == 'C') {
                        connector = o;
                    }
                }
                Connectors.Add(connector);
            }
        }
        
        icon = Instantiate(SpriteTemplate);
        icon.transform.SetParent(transform);
        icon.transform.position = GetCenter();
        
        for (int n = 0; n < Tiles.Count; n++) {
            Point tile = Tiles[n];
            Point conn = Connectors[n];
            
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(GameController.Instance.TileSize, GameController.Instance.TileSize);
            boxCollider.center = new Vector3((tile.x + 0.5f) * GameController.Instance.TileSize, (tile.y + 0.5f) * GameController.Instance.TileSize);
            
            SpriteRenderer sprite = Instantiate(SpriteTemplate);
            sprite.sprite = TileExteriorSprite;
            sprite.transform.SetParent(transform);
            sprite.transform.localPosition = new Vector3(
                (tile.x + 0.5f) * GameController.Instance.TileSize,
                (tile.y + 0.5f) * GameController.Instance.TileSize,
                3.0f);
            interiors.Add(sprite);

            sprite = Instantiate(SpriteTemplate);
            sprite.sprite = TileInteriorSprite;
            sprite.transform.SetParent(transform);
            sprite.transform.localPosition = new Vector3(
                (tile.x + 0.5f) * GameController.Instance.TileSize,
                (tile.y + 0.5f) * GameController.Instance.TileSize,
                2.0f);
            exteriors.Add(sprite);
                
            sprite = Instantiate(SpriteTemplate);
            sprite.sprite = TileDamageSprite;
            sprite.transform.SetParent(transform);
            sprite.transform.localPosition = new Vector3(
                (tile.x + 0.5f) * GameController.Instance.TileSize,
                (tile.y + 0.5f) * GameController.Instance.TileSize,
                2.0f);
            damages.Add(sprite);
        
            for (int i = 0; i < 4; i++) {
                Point o = sideOffsets[i];
                float r = sideRotations[i];
                int x2 = tile.x + o.x;
                int y2 = tile.y + o.y;
                foreach (Point otherTile in Tiles) {
                    if (otherTile.x == x2 && otherTile.y == y2) {
                        sprite = Instantiate(SpriteTemplate);
                        sprite.sprite = TileInteriorJoinSprite;
                        sprite.transform.SetParent(transform);
                        sprite.transform.eulerAngles = new Vector3(0.0f, 0.0f, r);
                        sprite.transform.localPosition = new Vector3(
                            (tile.x + 0.5f) * GameController.Instance.TileSize,
                            (tile.y + 0.5f) * GameController.Instance.TileSize,
                            2.0f);

                        interiors.Add(sprite);
                    }
                }
                if (conn.x == o.x && conn.y == o.y) {
                    sprite = Instantiate(SpriteTemplate);
                    sprite.sprite = TileConnectorSprite;
                    sprite.transform.SetParent(transform);
                    sprite.transform.eulerAngles = new Vector3(0.0f, 0.0f, r);
                    sprite.transform.localPosition = new Vector3(
                        (tile.x + 0.5f) * GameController.Instance.TileSize,
                        (tile.y + 0.5f) * GameController.Instance.TileSize,
                        1.0f);

                    exteriors.Add(sprite);
                }
            }
        }
        
        Type = moduleType;
    }
    
    private void UpdateSprites() {
        foreach (SpriteRenderer renderer in interiors) {
            if (!placed) {
                renderer.color = validPlacement ? ValidPlaceColor : InvalidPlaceColor;
            } else if (!powered) {
                renderer.color = UnpoweredColor;
            } else if (underConstruction) {
                renderer.color = UnderConstructionColor;
            } else {
                renderer.color = TypeColors[(int)Type];
            }
        }
        foreach (SpriteRenderer renderer in exteriors) {
            renderer.color = placed ? new Color(0.0f, 0.0f, 0.0f, 1.0f) : validPlacement ? ValidPlaceColor : InvalidPlaceColor;
        }
        foreach (SpriteRenderer renderer in damages) {
            renderer.color = damaged ? new Color(1.0f, 0.0f, 0.0f, 1.0f) : new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
        icon.sprite = TypeIconSprites[(int)Type];
        icon.color = placed ? new Color(1.0f, 1.0f, 1.0f, 1.0f) : new Color(0.0f, 0.0f, 0.0f, 0.0f);
    }

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
    
    private void OnMouseDown() {
        dragFrom = Module.GetMousePosition();
        dragging = true;
    }

    private void OnMouseUp() {
        dragging = false;
    }

    private void Update() {
        if (!placed) {
            if (dragging) {
                Vector2 dragAt = Module.GetMousePosition();
                placeOffset += (dragAt - dragFrom) / GameController.Instance.TileSize;
                dragFrom = dragAt;

                int newPlaceX = (int)Mathf.Round(placeOffset.x);
                int newPlaceY = (int)Mathf.Round(placeOffset.y);
                if (Offset.x != newPlaceX || Offset.y != newPlaceY) {
                    Offset = new Point(newPlaceX, newPlaceY);
                    dirtyPosition = true;
                    validPlacement = GameController.Instance.TestModulePlacement(this);
                    UpdateSprites();
                }
            }
                
            if (!dragging && validPlacement) {
                GameController.Instance.PlaceModule(this);
                placed = true;
                UpdateSprites();
            }
        }
        
        if (dirtyPosition) {
            transform.position = new Vector3(
                GameController.Instance.GridOffsetX + Offset.x * GameController.Instance.TileSize,
                GameController.Instance.GridOffsetY + Offset.y * GameController.Instance.TileSize,
                0.0f);
            dirtyPosition = false;
        }
    }

    private static Vector3 GetMousePosition() {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
    }
}
