using UnityEngine;
using System.Collections.Generic;

public sealed class IconButton : MonoBehaviour {
    public SpriteRenderer Icon;
    public ModuleType Type;
    public bool Selected;
    private bool highlighted;
    private SpriteRenderer spriteRenderer;
    
    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Update() {
        spriteRenderer.color = Selected ? Color.clear : highlighted ? Color.green : Color.white;
    }
    
    private void OnMouseOver() {
        highlighted = true;
    }
    
    private void OnMouseExit() {
        highlighted = false;
    }
    
    private void OnMouseDown() {
        Selected = true;
    }
}
