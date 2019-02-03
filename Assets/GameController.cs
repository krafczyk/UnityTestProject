using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController : MonoBehaviour {
    private const int PLAY_AREA_WIDTH = 6;
    private const int PLAY_AREA_HEIGHT = 13;
    public float GameSpeed;
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
        if(newBlock != null) {
            Block theBlock = newBlock.GetComponent<Block>();
            theBlock.Row = row;
            theBlock.Col = col;
            occupancyList[row, col] = theBlock;
        }
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
                Block theBlock = occupancyList[row_idx, col_idx] = occupancyList[row_idx - 1, col_idx];
                if (theBlock != null)
                {
                    theBlock.Pos = getWorldPositionFromArray(row_idx, col_idx);
                    theBlock.Row = row_idx;
                    theBlock.Col = col_idx;
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
            yield return new WaitForSeconds(GameSpeed/2.0f);
            CheckMatch();
            yield return new WaitForSeconds(GameSpeed/2.0f);
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

        // Row checking
        for (int row_idx = 1; row_idx < PLAY_AREA_HEIGHT; ++row_idx)
        {
            int count = 0;
            for (int col_idx = 0; col_idx < PLAY_AREA_WIDTH; ++col_idx)
            {
                if (col_idx - 1 >= 0)
                {
                    Block prevBlock = occupancyList[row_idx, col_idx - 1];
                    Block thisBlock = occupancyList[row_idx, col_idx];
                    bool chain_broken = false;
                    if(prevBlock != null) {
                        if (thisBlock == null)
                        {
                            chain_broken = true;
                        }
                        else
                        {
                            if (prevBlock.type == thisBlock.type)
                            {
                                if (count == 0)
                                {
                                    count = 2;
                                }
                                else
                                {
                                    count += 1;
                                }
                            }
                            else
                            {
                                chain_broken = true;
                            }
                        }
                    }
                    if(chain_broken) {
                        if (count >= 3)
                        {
                            for (int c_idx = 0; c_idx < count; ++c_idx)
                            {
                                Block blockToAdd = occupancyList[row_idx, col_idx - 1 - c_idx];
                                if (!BlocksToDestroy.Contains(blockToAdd))
                                {
                                    BlocksToDestroy.Add(blockToAdd);
                                }
                            }
                        }
                        count = 0;
                    }
                }
            }
            // Remove combo at edge of playable area.
            if(count >= 3)
            {
                for(int c_idx = 0; c_idx < count; ++c_idx)
                {
                    Block blockToAdd = occupancyList[row_idx, PLAY_AREA_WIDTH - 1 - c_idx];
                    if (!BlocksToDestroy.Contains(blockToAdd))
                    {
                        BlocksToDestroy.Add(blockToAdd);
                    }
                }
            }
        }

        // Col checking
        for (int col_idx = 0; col_idx < PLAY_AREA_WIDTH; ++col_idx)
        {
            int count = 0;
            for (int row_idx = 1; row_idx < PLAY_AREA_HEIGHT; ++row_idx)
            {
                if (row_idx - 1 >= 1)
                {
                    Block prevBlock = occupancyList[row_idx - 1, col_idx];
                    Block thisBlock = occupancyList[row_idx, col_idx];
                    bool chain_broken = false;
                    if(prevBlock != null) {
                        if (thisBlock == null)
                        {
                            chain_broken = true;
                        }
                        else
                        {
                            if (prevBlock.type == thisBlock.type)
                            {
                                if (count == 0)
                                {
                                    count = 2;
                                }
                                else
                                {
                                    count += 1;
                                }
                            }
                            else
                            {
                                chain_broken = true;
                            }
                        }
                    }
                    if(chain_broken) {
                        if (count >= 3)
                        {
                            for (int c_idx = 0; c_idx < count; ++c_idx)
                            {
                                Block blockToAdd = occupancyList[row_idx - 1 - c_idx, col_idx];
                                if (!BlocksToDestroy.Contains(blockToAdd))
                                {
                                    BlocksToDestroy.Add(blockToAdd);
                                }
                            }
                        }
                        count = 0;
                    }
                }
            }
            // Remove combo at edge of playable area.
            if(count >= 3)
            {
                for(int c_idx = 0; c_idx < count; ++c_idx)
                {
                    Block blockToAdd = occupancyList[PLAY_AREA_HEIGHT - 1 - c_idx, col_idx];
                    if (!BlocksToDestroy.Contains(blockToAdd))
                    {
                        BlocksToDestroy.Add(blockToAdd);
                    }
                }
            }
        }

        // Destroy detected matched blocks.
        foreach (Block theBlock in BlocksToDestroy)
        {
            occupancyList[theBlock.Row, theBlock.Col] = null;
            theBlock.Destroy();
        }
    }
}
