using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject outerHighLight;
    [SerializeField] private SpriteRenderer innerHighLight;

    public GridTile Tiles {get; set;}
    public bool isWalkable = true;

    public void Init(bool isOffset) {
        spriteRenderer.color = isOffset ? offsetColor : baseColor;
    }

    private void OnMouseEnter() {
        outerHighLight.SetActive(true);
    }

    private void OnMouseExit() {
        outerHighLight.SetActive(false);
    }

    // public void SetOuterColor(Color col) {
    //     outerHighLight.color = col;
    // }

    public void SetInnerColor(Color col) {
        innerHighLight.color = col;
    }
}
