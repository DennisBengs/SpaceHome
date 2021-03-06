using UnityEngine;
using System.Collections.Generic;

public sealed class Module : MonoBehaviour {
    public Dictionary<ModuleType, int> CrewCapacity = new Dictionary<ModuleType, int>() {
        { ModuleType.Empty, 0 },
        { ModuleType.Hydroponics, 2 },
        { ModuleType.SolarCells, 0 },
        { ModuleType.Elevator, 1 },
        { ModuleType.PointDefense, 1 }
    };

    public Dictionary<ModuleType, int> EnergyUsage = new Dictionary<ModuleType, int>() {
        { ModuleType.Empty, 0 },
        { ModuleType.Hydroponics, 1 },
        { ModuleType.SolarCells, 0 },
        { ModuleType.Elevator, 0 },
        { ModuleType.PointDefense, 1 }
    };

    public bool IsDestroyed { get; private set; }
    
    public int MinTileCount = 2;
    public int MaxTileCount = 8;
    public int MinConnectorCount = 2;
    public int MaxConnectorCount = 5;

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
    
    public bool Disconnected { get; private set; }
    
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

    public bool Placed { get; private set; }
    
    private Vector2 placeOffset;
    private Vector2 dragFrom;
    private Vector2 center;
    private bool highlighted = false;
    private bool dragging = false;
    private bool validPlacement = false;
    
    public Point Offset { get; private set; }
    public List<Point> Tiles { get; private set; }
    public List<bool[]> Connectors { get; private set; }

    public bool SpaceElevatorAtPlatform { get; set; }
    
    public int CrewCount { get; private set; }

    public GameObject SpaceElevatorContainer;
    public Transform SpaceElevator;
    public SpriteRenderer Turret;
    public SpriteRenderer SpriteTemplate;
    public CrewPosition CrewPositionTemplate;
    
    public Sprite TileExteriorSprite;
    public Sprite TileInteriorSprite;
    public Sprite TileInteriorJoinSprite;
    public Sprite TileConnectorSprite;
    public Sprite TileDamageSprite;
    public Color ValidPlaceColor;
    public Color InvalidPlaceColor;
    public Color ValidPlaceHullColor;
    public Color InvalidPlaceHullColor;
    public Color UnpoweredColor;
    public Color DamageColor;
    public Color HullColor;
    public Color HighlightHullColor;
    public List<Color> TypeColors;
    public List<IconButton> IconButtons;
    
    private List<SpriteRenderer> interiors = new List<SpriteRenderer>();
    private List<SpriteRenderer> exteriors = new List<SpriteRenderer>();
    private List<SpriteRenderer> damages = new List<SpriteRenderer>();
    private List<CrewPosition> crewPositions = new List<CrewPosition>();
    private bool dirtyPosition = true;
    
    public static readonly Point[] SideOffsets = {new Point(-1, 0), new Point(0, 1), new Point(1, 0), new Point(0, -1)};
    public static readonly float[] SideRotations = {0.0f, -90.0f, -180.0f, -270.0f};

    public void Setup(ModuleType moduleType, Point offset) {
        Tiles = new List<Point>();
        Connectors = new List<bool[]>();

        Offset = offset;
        placeOffset = new Vector2(offset.x, offset.y);
        
        if (moduleType == ModuleType.Elevator) {
            SpaceElevatorContainer.SetActive(true);

            GameController.Instance.PlaceModule(this);
            Placed = true;

            Tiles.Add(new Point(0, 0));
            Connectors.Add(new bool[] { true, true, true, true });
        } else {
            int tileCount = Random.Range(MinTileCount, MaxTileCount + 1);
            int connectorCount = Random.Range(MinConnectorCount, MaxConnectorCount + 1);

            Tiles.Add(new Point(0, 0));
            Connectors.Add(new bool[] { false, false, false, false });

            for (int i = 0; i < tileCount; i++) {
                while (true) {
                    Point fromTile = Tiles[Random.Range(0, Tiles.Count)];
                    Point sideOffset = SideOffsets[Random.Range(0, 4)];
                    Point newTile = new Point(fromTile.x + sideOffset.x, fromTile.y + sideOffset.y);

                    bool found = false;
                    foreach (Point tile in Tiles) {
                        if (tile.x == newTile.x && tile.y == newTile.y) {
                            found = true;
                            break;
                        }
                    }
                    if (!found) {
                        Tiles.Add(newTile);
                        Connectors.Add(new bool[] { false, false, false, false });
                        break;
                    }
                }
            }
            
            for (int i = 0; i < connectorCount; i++) {
                for (int tries = 0; tries < 50; tries++) {
                    int tileIndex = Random.Range(0, Tiles.Count);
                    int sideIndex = Random.Range(0, 4);
                    
                    Point fromTile = Tiles[tileIndex];
                    Point sideOffset = SideOffsets[sideIndex];
                    Point targetTile = new Point(fromTile.x + sideOffset.x, fromTile.y + sideOffset.y);
                    
                    bool found = false;
                    foreach (Point tile in Tiles) {
                        if (tile.x == targetTile.x && tile.y == targetTile.y) {
                            found = true;
                            break;
                        }
                    }
                    if (!found) {
                        Connectors[tileIndex][sideIndex] = true;
                        break;
                    }
                }       
            }
        }
        
        for (int n = 0; n < Tiles.Count; n++) {
            Point tile = Tiles[n];
            bool[] connector = Connectors[n];
            
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
            exteriors.Add(sprite);

            sprite = Instantiate(SpriteTemplate);
            sprite.sprite = TileInteriorSprite;
            sprite.transform.SetParent(transform);
            sprite.transform.localPosition = new Vector3(
                (tile.x + 0.5f) * GameController.Instance.TileSize,
                (tile.y + 0.5f) * GameController.Instance.TileSize,
                2.0f);
            interiors.Add(sprite);
                
            sprite = Instantiate(SpriteTemplate);
            sprite.sprite = TileDamageSprite;
            sprite.transform.SetParent(transform);
            sprite.transform.localPosition = new Vector3(
                (tile.x + 0.5f) * GameController.Instance.TileSize,
                (tile.y + 0.5f) * GameController.Instance.TileSize,
                1.1f);
            damages.Add(sprite);
        
            for (int i = 0; i < 4; i++) {
                Point o = SideOffsets[i];
                float r = SideRotations[i];
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
                if (connector[i]) {
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
            if (!Placed) {
                renderer.color = validPlacement ? ValidPlaceColor : InvalidPlaceColor;
            } else if (!powered) {
                renderer.color = UnpoweredColor;
            } else {
                renderer.color = TypeColors[(int)Type];
            }
        }
        foreach (SpriteRenderer renderer in exteriors) {
            renderer.color = highlighted ? HighlightHullColor : !Placed ? validPlacement ? ValidPlaceHullColor : InvalidPlaceHullColor : HullColor;
        }
        foreach (SpriteRenderer renderer in damages) {
            renderer.color = damaged ? DamageColor : new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
        Turret.gameObject.SetActive(Type == ModuleType.PointDefense);
    }
    
    public void Disconnect() {
        Disconnected = true;
        IsDestroyed = true;
    }
    
    public void Damage() {
        if (damaged) {
            Disconnect();
        } else {
            damaged = true;
            UpdateSprites();
        }
    }

    public void StartTurn() {
        GameController.Instance.EnergyCount = Mathf.Max(GameController.Instance.EnergyCount - EnergyUsage[Type], 0);
    }

    public void EndTurn() {
        switch (Type) {
            case ModuleType.Empty:
                break;
            case ModuleType.Hydroponics:
                GameController.Instance.FoodCount += 2;
                break;
            case ModuleType.SolarCells:
                GameController.Instance.EnergyCount += 2;
                break;
            case ModuleType.Elevator:
                break;
            case ModuleType.PointDefense:
                break;
        }
    }
    
    public Vector3 GetCenter() {
        Vector3 avg = new Vector3();
        foreach (Point tile in Tiles) {
            avg.x += transform.position.x + (tile.x + 0.5f) * GameController.Instance.TileSize;
            avg.y += transform.position.y + (tile.y + 0.5f) * GameController.Instance.TileSize;
        }
        return avg / Tiles.Count;
    }
    
    private void OnMouseOver() {
        if (!Input.GetMouseButtonDown(0)) {
            highlighted = true;
            UpdateSprites();
        }
    }

    private void OnMouseExit() {
        highlighted = false;
        UpdateSprites();
    }
    
    private void OnMouseDown() {
        dragFrom = Module.GetMousePosition();
        dragging = true;
    }

    private void OnMouseUp() {
        dragging = false;
    }

    private void Update() {
        if (Disconnected) {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.01f);
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y - 15.0f, transform.position.z), 0.01f);
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(45.0f, 30.0f, 45.0f), 0.03f);
        }
        
        if (Type == ModuleType.Empty && Placed) {
            for (int i = 0; i < IconButtons.Count; i++) {
                IconButton button = IconButtons[i];
                button.gameObject.SetActive(true);
                float angle = (((float)i / IconButtons.Count) + Time.realtimeSinceStartup * 0.05f) * Mathf.PI * 2.0f;
                button.transform.position = new Vector3(center.x + Mathf.Cos(angle) * 1.5f, center.y + Mathf.Sin(angle) * 1.5f, button.transform.position.z);
            }
            
            foreach (IconButton button in IconButtons) {
                if (button.Selected) {
                    Type = button.Type;
                    foreach (IconButton otherButton in IconButtons) {
                        if (otherButton != button) {
                            otherButton.gameObject.SetActive(false);
                        }
                    }
                    button.transform.position = new Vector3(center.x, center.y, button.transform.position.z);
                    Powered = true;
                }
            }
        
        }
        
        if (Type == ModuleType.Elevator) {
            SpaceElevator.transform.localPosition = Vector3.Lerp(SpaceElevator.transform.localPosition, 
                new Vector3(1.0f, SpaceElevatorAtPlatform ? -1.0f : -25.0f, 1.0f), 0.02f);
        }
        
        if (!Placed) {
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
                Placed = true;
                center = GetCenter();
                UpdateSprites();
            }
        }
        
        if (dirtyPosition) {
            transform.position = new Vector3(
                GameController.Instance.GridOffsetX + Offset.x * GameController.Instance.TileSize,
                GameController.Instance.GridOffsetY + Offset.y * GameController.Instance.TileSize,
                Placed ? 0.0f : -0.1f);
            dirtyPosition = false;
        }
    }

    private static Vector3 GetMousePosition() {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
    }
}
