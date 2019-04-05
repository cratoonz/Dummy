using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Board : MonoBehaviour
{
    // Start is called before the first frame update

    public int width;
    public int height;

    public int borderSize;

    public GameObject tilePrefab;
    public GameObject[] gamePiecePrefabs;

    public float swapTime = 0.5f;

    private Tile[,] m_allTiles;
    private GamePiece[,] m_allGamePieces;

    private Tile m_clickedTile;
    private Tile m_targetTile;
    
    void Start()
    {
        m_allTiles = new Tile[width, height];
        m_allGamePieces = new GamePiece[width,height];
        SetupTiles();
        SetupCamera();
        FillRandom();
    }

    // ızgarayı oluşturur
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

                m_allTiles[i, j].Init(i, j, this);
            }
        }
    }

    void SetupCamera()
    {
        Camera.main.transform.position = new Vector3((float)(width-1)/2f,(float)(height-1)/2f,-10f);

        float aspectRatio = (float) Screen.width / (float) Screen.height;

        float verticalSize = (float) height / 2f + (float) borderSize;

        float horizontalSize = ((float) width / 2f + (float) borderSize) / aspectRatio;

        Camera.main.orthographicSize = (verticalSize > horizontalSize) ? verticalSize : horizontalSize;

    }

    GameObject GetRandomGamePiece()
    {
        int randomIdx = Random.Range(0, gamePiecePrefabs.Length);
        if (gamePiecePrefabs[randomIdx] == null)
        {
            Debug.LogWarning("Board:" + randomIdx + "does not contain a valid gamepiece prefab.");
        }

        return gamePiecePrefabs[randomIdx];
    }

    public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        if (gamePiece == null)
        {
            Debug.LogWarning("Board: invalid gamepiece");
            return;
        }
        
        gamePiece.transform.position = new Vector3(x,y,0);
        gamePiece.transform.rotation = Quaternion.identity;
//        if(IsWithinBounds(x,y)){
//            m_allGamePieces[x, y] = gamePiece;
//        }
        m_allGamePieces[x, y] = gamePiece;
        gamePiece.SetCoord(x,y);
    }

    bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    void FillRandom()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject randomPiece = Instantiate(GetRandomGamePiece(),Vector3.zero,Quaternion.identity) as GameObject;
                if (randomPiece != null)
                {
                    randomPiece.GetComponent<GamePiece>().Init(this);
                    PlaceGamePiece(randomPiece.GetComponent<GamePiece>(),i,j);
                    randomPiece.transform.parent = transform;
                }
            }
        }
    }

    public void ClickTile(Tile tile)
    {
        if (m_clickedTile == null)
        {
            m_clickedTile = tile;
            Debug.Log("clicked tile : "+ tile.name);
        }
    }
    
    public void DragToTile(Tile tile)
    {
        if (m_clickedTile != null /*&& IsNextTo(tile,m_clickedTile)*/)
        {
            m_targetTile = tile;
            Debug.Log("dragged tile : "+ tile.name);
        }
    }

    public void ReleaseTile()
    {
        if (m_clickedTile != null && m_targetTile != null)
        {
            SwitchTiles(m_clickedTile, m_targetTile);
        }
        
        m_clickedTile = null;
        m_targetTile = null;
    }

    void SwitchTiles(Tile clickedTile, Tile targetTile)
    {
        GamePiece clickedPiece = m_allGamePieces[clickedTile.xIndex, clickedTile.yIndex];
        GamePiece targetPiece = m_allGamePieces[targetTile.xIndex, targetTile.yIndex];
        clickedPiece.Move(targetTile.xIndex,targetTile.yIndex,swapTime);
        targetPiece.Move(clickedTile.xIndex,clickedTile.yIndex,swapTime);

    }

    bool IsNextTo(Tile start, Tile end)
    {
        if (Mathf.Abs(start.xIndex - end.xIndex) == 1 && start.yIndex == end.yIndex)
        {
            return true;
        }

        if (Mathf.Abs(start.yIndex - end.yIndex) == 1 && start.xIndex == end.xIndex)
        {
            return true;
        }

        return false;
    }
    
    List<GamePiece> FindMatches(int startX, int startY, Vector2 searchDirection, int minLength = 3)
    {
        // running list of matching pieces
        List<GamePiece> matches = new List<GamePiece>();

        // start searching from this GamePiece
        GamePiece startPiece = null;

        // if our Tile coordinate is within the Board, get the corresponding GamePiece
        if (IsWithinBounds(startX, startY))
        {
            startPiece = m_allGamePieces[startX, startY];
        }

        // our starting piece is the first element of our matches list
        if (startPiece !=null)
        {
            matches.Add(startPiece);
        }
        // if the Tile is empty, return null
        else
        {
            return null;
        }

        // coordinates for next tile to search
        int nextX;
        int nextY;

        // we can set our maximum search to the width or height of the Board, whichever is greater
        int maxValue = (width > height) ? width: height;

        // start searching Tile at (startX, startY); increment depending on how we set our searchDirection
        for (int i = 1; i < maxValue - 1; i++)
        {
            nextX = startX + (int) Mathf.Clamp(searchDirection.x,-1,1) * i;
            nextY = startY + (int) Mathf.Clamp(searchDirection.y,-1,1) * i;

            // if we hit the edge of the board, stop searching
            if (!IsWithinBounds(nextX, nextY))
            {
                break;
            }

            // get the correspond GamePiece to the (nextX, nextY) coordinate
            GamePiece nextPiece = m_allGamePieces[nextX, nextY];

            // if the next GamePiece has a matching value and is not already in our list
            if (nextPiece.matchValue == startPiece.matchValue && !matches.Contains(nextPiece))
            {
                matches.Add(nextPiece);
            }
            // we encounter a GamePiece that does not have a matching value or is already in our list, stop searching
            else
            {
                break;
            }
        }

        // if our list of matching pieces is greater than our minimum to be considered a match, return it
        if (matches.Count >= minLength)
        {
            return matches;
        }

        // we don't have the minimum number of matches, return null
        return null;

    }
    
    List<GamePiece> FindVerticalMatches(int startX, int startY, int minLength = 3)
	{
		List<GamePiece> upwardMatches = FindMatches(startX, startY, new Vector2(0,1), 2);
		List<GamePiece> downwardMatches = FindMatches(startX, startY, new Vector2(0,-1), 2);

		if (upwardMatches == null)
		{
			upwardMatches = new List<GamePiece>();
		}

		if (downwardMatches == null)
		{
			downwardMatches = new List<GamePiece>();
		}

		var combinedMatches = upwardMatches.Union(downwardMatches).ToList();

		return (combinedMatches.Count >= minLength) ? combinedMatches : null;

	}

	List<GamePiece> FindHorizontalMatches(int startX, int startY, int minLength = 3)
	{
		List<GamePiece> rightMatches = FindMatches(startX, startY, new Vector2(1,0), 2);
		List<GamePiece> leftMatches = FindMatches(startX, startY, new Vector2(-1,0), 2);

		if (rightMatches == null)
		{
			rightMatches = new List<GamePiece>();
		}

		if (leftMatches == null)
		{
			leftMatches = new List<GamePiece>();
		}

		var combinedMatches = rightMatches.Union(leftMatches).ToList();

		return (combinedMatches.Count >= minLength) ? combinedMatches : null;

	}
		
	public void HighlightMatches()
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				SpriteRenderer spriteRenderer = m_allTiles[i,j].GetComponent<SpriteRenderer>();
				spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);

				var combinedMatches = FindAllMatches(i, j);

                if (combinedMatches.Count > 0)
				{
					foreach (GamePiece piece in combinedMatches)
					{
						spriteRenderer = m_allTiles[piece.xIndex, piece.yIndex].GetComponent<SpriteRenderer>();
						spriteRenderer.color = piece.GetComponent<SpriteRenderer>().color;
					}
				}
			}
		}
	}

    private List<GamePiece> FindAllMatches(int x, int y)
    {
        List<GamePiece> horizMatches = FindHorizontalMatches(x, y, 3);
        List<GamePiece> vertMatches = FindVerticalMatches(x, y, 3);

        if (horizMatches == null)
        {
            horizMatches = new List<GamePiece>();
        }

        if (vertMatches == null)
        {
            vertMatches = new List<GamePiece>();
        }

        var combinedMatches = horizMatches.Union(vertMatches).ToList();
        return combinedMatches;
    }
}
