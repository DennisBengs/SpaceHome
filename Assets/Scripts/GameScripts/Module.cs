using UnityEngine;
using System;
using System.Collections.Generic;

public sealed class Module : MonoBehaviour {
    public Point Offset { get; private set; }
    public List<Point> Tiles { get; private set; }
    public List<Point> Connectors { get; private set; }
}
