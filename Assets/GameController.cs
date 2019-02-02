using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController : MonoBehaviour {
    private const int PLAY_AREA_WIDTH = 6;
    private const int PLAY_AREA_HEIGHT = 13;
    public int GameSpeed;
    public float Left = -3;
    public float Bottom = -5.5f;

    private bool GameLost = false;
    private Block[,] occupancyList;

    System.Random rng;

    private Int32 GetUnixTimestamp()
    {
        return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970,1,1)).TotalSeconds);
    }

    void Awake()
    {
        rng = new System.Random(GetUnixTimestamp());
        occupancyList = new Block[PLAY_AREA_HEIGHT,PLAY_AREA_WIDTH];
        occupancyList.Initialize();
    }

    void OnEnable()
    {

    }

	// Use this for initialization
	void Start () {
        StartCoroutine("StepGame");
	}

    // Update is called once per frame
    void Update()
    {
	}

    void FixedUpdate()
    {

    }

    void OnDisable()
    {

    }

    public Block GetBlock(int row, int col)
    {
        return occupancyList[row, col];
    }

    private Vector3 getWorldPositionFromArray(int row, int col)
    {
        return new Vector3(Left + col, Bottom + row, 0);
    }
        
    public void AddBlock(int row, int col, Block.BlockTypes type)
    {
        Vector3 position = getWorldPositionFromArray(row, col);
        GameObject newBlock = null;
        switch (type) {
            case Block.BlockTypes.Red:
                newBlock = Instantiate(Resources.Load("RedBlock"), position, Quaternion.identity) as GameObject;
                break;
            case Block.BlockTypes.Green:
                newBlock = Instantiate(Resources.Load("GreenBlock"), position, Quaternion.identity) as GameObject;
                break;
            case Block.BlockTypes.Blue:
                newBlock = Instantiate(Resources.Load("BlueBlock"), position, Quaternion.identity) as GameObject;
                break;
        }
        occupancyList[row, col] = newBlock.GetComponent<Block>();
    }

    private bool PushUp()
    {
        // Check whether any blocks are in the top row.
        bool found_block = false;
        for (int idx = 0; idx < PLAY_AREA_WIDTH; ++idx)
        {
            if (GetBlock(PLAY_AREA_HEIGHT-1, idx) != null)
            {
                found_block = true;
                break;
            }
        }
        if (found_block)
        {
            return false;
        }
        for (int row_idx = PLAY_AREA_HEIGHT-1; row_idx >= 1; --row_idx)
        {
            for(int col_idx = 0; col_idx < PLAY_AREA_WIDTH; ++col_idx)
            {
                occupancyList[row_idx, col_idx] = occupancyList[row_idx - 1, col_idx];
                if (occupancyList[row_idx, col_idx] != null)
                {
                    occupancyList[row_idx, col_idx].Pos = getWorldPositionFromArray(row_idx, col_idx);
                }
            }
        }
        for (int col_idx = 0; col_idx < PLAY_AREA_WIDTH; ++col_idx)
        {
            occupancyList[0, col_idx] = null;
        }
        return true;
    }

    public void AddRow()
    {
        for (int i = 0; i < PLAY_AREA_WIDTH; ++i)
        {
            Block.BlockTypes type = (Block.BlockTypes) rng.Next(0, Block.Num_Colors);
            AddBlock(0, i, type);
        }
    }

    public IEnumerator StepGame()
    {
        while (PushUp())
        {
            AddRow();
            yield return new WaitForSeconds(GameSpeed);
        }
        GameLost = true;
    }

    public GameObject DrawBlock(int row, int col, int type)
    {
        float x_pos = Left + col;
        float y_pos = Bottom + row;
        switch (type) {
            case 0:
                return null;
                break;
            case 1:
                return Instantiate(Resources.Load("RedBlock"), new Vector3(x_pos, y_pos, 0), Quaternion.identity) as GameObject;
                break;
            case 2:
                return Instantiate(Resources.Load("GreenBlock"), new Vector3(x_pos, y_pos, 0), Quaternion.identity) as GameObject;
                break;
            case 3:
                return Instantiate(Resources.Load("BlueBlock"), new Vector3(x_pos, y_pos, 0), Quaternion.identity) as GameObject;
                break;
            default:
                return null;
                break;
        }
    }
}