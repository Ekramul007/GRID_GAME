using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileRaycast : MonoBehaviour
{
    public Text infoText;          
    public Material defaultMaterial; 
    public Material hoverMaterial;   

    private GameObject lastHoveredTile = null;  

    void Update()
    {
        RaycastHit hit;

        // Create a ray from the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            TileInfo tileInfo = hit.collider.GetComponent<TileInfo>();

            if (tileInfo != null)
            {
                
                infoText.text = tileInfo.GetTileInfo();

                // Highlight the tile being hovered
                if (hit.collider.gameObject != lastHoveredTile)
                {
                    if (lastHoveredTile != null)
                    {
                        lastHoveredTile.GetComponent<Renderer>().material = defaultMaterial;
                    }

                    hit.collider.GetComponent<Renderer>().material = hoverMaterial;
                    lastHoveredTile = hit.collider.gameObject;
                }
            }
        }
        else
        {
            
            if (lastHoveredTile != null)
            {
                lastHoveredTile.GetComponent<Renderer>().material = defaultMaterial;
                lastHoveredTile = null;
            }

            
            infoText.text = "";
        }
    }
}