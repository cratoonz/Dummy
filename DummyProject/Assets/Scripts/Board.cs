using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    // Start is called before the first frame update

    public int width;
    public int height;

    public GameObject tilePrefab;

    private Tile[,] m_allTiles;
    
    void Start()
    {
        m_allTiles = new Tile[width, height];
        SetupTiles();
    }

    void SetupTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject tile = Instantiate(tilePrefab,new Vector3(i,j,0),Quaternion.identity) as GameObject;

                tile.name = "Tile (" + i + "," + j + ")";

                m_allTiles[i, j] = tile.GetComponent<Tile>();

                tile.transform.parent = transform;
            }
        }
    }
}
