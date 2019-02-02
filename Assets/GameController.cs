﻿using System.Collections;
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
        Block theBlock = newBlock.GetComponent<Block>();
        theBlock.Row = row;
        theBlock.Col = col;
        occupancyList[row, col] = theBlock;
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
                    occupancyList[row_idx, col_idx].Row = row_idx;
                    occupancyList[row_idx, col_idx].Col = col_idx;
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
            CheckMatch();
            yield return new WaitForSeconds(GameSpeed);
        }
        GameLost = true;
    }

    private void DestroyBlock(int row, int col)
    {
        occupancyList[row, col].Destroy();
        occupancyList[row, col] = null;
    }

    private void CheckMatch()
    {
        // Start Checking for combos, we start on upper left block.
        List<Block> BlocksToDestroy = new List<Block>();
        List<Block> Blocks = new List<Block>();
        for (int row_idx = PLAY_AREA_HEIGHT - 1; row_idx > 0; --row_idx)
        {
            for (int col_idx = 0; col_idx < PLAY_AREA_WIDTH; ++col_idx)
            {
                if (occupancyList[row_idx,col_idx] != null)
                {
                    Blocks.Add(occupancyList[row_idx, col_idx]);
                }
            }
        }
        foreach(Block theBlock in Blocks)
        {
            int init_row = theBlock.Row;
            int init_col = theBlock.Col;
            Block.BlockTypes the_type = theBlock.type;
            // Up/Down search
            int ud_combo_length = 1;
            int up_extent = init_row;
            int down_extent = init_row;
            // Up
            for (int row_idx = init_row + 1; row_idx < PLAY_AREA_HEIGHT; ++row_idx)
            {
                Block tempBlock = occupancyList[row_idx, init_col];
                if (tempBlock)
                {
                    if (tempBlock.type != the_type)
                    {
                        break;
                    }
                    else
                    {
                        ud_combo_length += 1;
                        up_extent = row_idx;
                    }
                }
                else
                {
                    break;
                }
            }
            // Right
            for (int row_idx = init_row - 1; row_idx > 0; --row_idx)
            {
                Block tempBlock = occupancyList[row_idx, init_col];
                if (tempBlock)
                {
                    if (tempBlock.type != the_type)
                    {
                        break;
                    }
                    else
                    {
                        ud_combo_length += 1;
                        down_extent = row_idx;
                    }
                }
                else
                {
                    break;
                }
            }
            if (ud_combo_length >= 3)
            {
                for (int row_idx = down_extent; row_idx <= up_extent; ++row_idx)
                {
                    Block tempBlock = occupancyList[row_idx, init_col];
                    if(!BlocksToDestroy.Contains(tempBlock)) {
                        BlocksToDestroy.Add(tempBlock);
                    }
                }
            }
            // Left/Right search
            int lr_combo_length = 1;
            int left_extent = init_col;
            int right_extent = init_col;
            // Left
            for (int col_idx = init_col - 1; col_idx >= 0; --col_idx)
            {
                Block tempBlock = occupancyList[init_row, col_idx];
                if (tempBlock)
                {
                    if (tempBlock.type != the_type)
                    {
                        break;
                    }
                    else
                    {
                        lr_combo_length += 1;
                        left_extent = col_idx;
                    }
                }
                else
                {
                    break;
                }
            }
            // Right
            for (int col_idx = init_row + 1; col_idx < PLAY_AREA_WIDTH; ++col_idx)
            {
                Block tempBlock = occupancyList[init_row, col_idx];
                if (tempBlock)
                {
                    if (tempBlock.type != the_type)
                    {
                        break;
                    }
                    else
                    {
                        lr_combo_length += 1;
                        right_extent = col_idx;
                    }
                }
                else
                {
                    break;
                }
            }
            if (lr_combo_length >= 3)
            {
                for (int col_idx = left_extent; col_idx <= right_extent; ++col_idx)
                {
                    Block tempBlock = occupancyList[init_row, col_idx];
                    if(!BlocksToDestroy.Contains(tempBlock)) {
                        BlocksToDestroy.Add(tempBlock);
                    }
                }
            }
        }
        foreach (Block theBlock in BlocksToDestroy)
        {
            theBlock.Destroy();
        }
    }
}