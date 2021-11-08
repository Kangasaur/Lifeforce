using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth, gridHeight;
    public float seaLevel;
    public float targetLandmass = 0.5f;
    public float targetError = 0.05f;
    public float noiseIntensity = 1;
    public float noiseSecondIntensity = 1;
    public float noiseSize = 5f;
    int landNum = 0;
    float randX, randY;
    public GameObject tile;
    GameObject[,] myGrid;
    Sprite[] landSprites;
    Sprite oceanSprite;

    void Start()
    {
        randX = Random.Range(-100000f, 100000f);
        randY = Random.Range(-100000f, 100000f);
        myGrid = new GameObject[gridWidth, gridHeight];
        landSprites = Resources.LoadAll<Sprite>("Sprites/maptiles");
        oceanSprite = Resources.Load<Sprite>("Sprites/ocean");
        GenerateGrid();
    }
    
    void GenerateTile(int xPos, int yPos)
    {
        Vector3 position = transform.position + new Vector3(xPos, yPos);
        GameObject newTile = Instantiate(tile, position, transform.rotation);
        if (GenerateHeight(xPos, yPos) > seaLevel)
        {
            newTile.GetComponent<TileData>().isLand = true;
            landNum++;
        }
        else newTile.GetComponent<TileData>().isLand = false;
        myGrid[xPos, yPos] = newTile;
    }

    void TextureTile(int xPos, int yPos)
    {
        GameObject checkTile = myGrid[xPos, yPos];
        if (!checkTile.GetComponent<TileData>().isLand) checkTile.GetComponent<SpriteRenderer>().sprite = oceanSprite;
        else
        {
            int tileIndex = 0;
            if (EvaluateTile(xPos - 1, yPos + 1)) tileIndex += 1;
            if (EvaluateTile(xPos,     yPos + 1)) tileIndex += 2;
            if (EvaluateTile(xPos + 1, yPos + 1)) tileIndex += 4;
            if (EvaluateTile(xPos + 1, yPos))     tileIndex += 8;
            if (EvaluateTile(xPos + 1, yPos - 1)) tileIndex += 16;
            if (EvaluateTile(xPos,     yPos - 1)) tileIndex += 32;
            if (EvaluateTile(xPos - 1, yPos - 1)) tileIndex += 64;
            if (EvaluateTile(xPos - 1, yPos))     tileIndex += 128;
            checkTile.GetComponent<SpriteRenderer>().sprite = landSprites[tileIndex];
        }
    }

    void GenerateGrid()
    {
        landNum = 0;
        int tries = 0;
        while ((GridRatio(landNum) < targetLandmass - targetError || GridRatio(landNum) > targetLandmass + targetError) && tries < 100)
        {
            landNum = 0;
            tries++;
            randX = Random.Range(-100000f, 100000f);
            randY = Random.Range(-100000f, 100000f);
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    GenerateTile(x, y);
                }
            }
            
        }
        Debug.Log(tries);
        Debug.Log(GridRatio(landNum));

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                TextureTile(x, y);
            }
        }
    }

    bool EvaluateTile(int xPos, int yPos)
    {
        if (xPos < 0 || xPos >= gridWidth || yPos < 0 || yPos >= gridHeight)
        {
            return false;
        }
        else return myGrid[xPos, yPos].GetComponent<TileData>().isLand;
    }

    float GenerateHeight(int xPos, int yPos)
    {
        Vector2 genPos = new Vector2(xPos / noiseSize + randX, yPos / noiseSize + randY);
        Vector2 genPosRotated = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (Vector2.Angle(Vector2.right, genPos) + 30)),
            Mathf.Sin(Mathf.Deg2Rad * (Vector2.Angle(Vector2.right, genPos) + 30)));
        
        float f = Mathf.PerlinNoise(genPos.x, genPos.y) * noiseIntensity;
        float f2 = Mathf.PerlinNoise(genPosRotated.x, genPosRotated.y) * noiseSecondIntensity;
        return (f + f2) / (2 * ((noiseIntensity + noiseSecondIntensity) / 2));
    }

    float GridRatio(float number)
    {
        return number / (gridWidth * gridHeight);
    }
}
