using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController : MonoBehaviour {

    public enum BlockTypes
    {
        Empty,
        Red,
        Blue,
        Green
    }
    public const int Num_Colors = 3+1;

    private const int PLAY_AREA_WIDTH = 6;
    private const int PLAY_AREA_HEIGHT = 13;
    public int GameSpeed;
    public float Left = -3;
    public float Bottom = -5.5f;

    private bool GameLost = false;
    private int[,] occupancyList;

    System.Random rng;
 
    void Awake()
    {
        rng = new System.Random(0);
        occupancyList = new int[PLAY_AREA_HEIGHT,PLAY_AREA_WIDTH];
        occupancyList.Initialize();
    }

    void OnEnable()
    {

    }

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (GameLost)
        {
            StopCoroutine("StepGame");
        }
        else {
            AddRow();
            StartCoroutine("StepGame");
        }
	}

    void FixedUpdate()
    {

    }

    void OnDisable()
    {

    }

    public int GetBlock(int row, int col)
    {
        return occupancyList[row, col];
    }

    public void AddBlock(int row, int col, int type)
    {
        occupancyList[row, col] = type;
    }

    private bool PushUp()
    {
        // Check whether any blocks are in the top row.
        bool found_block = false;
        for (int idx = 0; idx < PLAY_AREA_WIDTH; ++idx)
        {
            if (GetBlock(0, idx) != 0)
            {
                found_block = true;
                break;
            }
        }
        if (found_block)
        {
            return false;
        }
        for (int row_idx = PLAY_AREA_HEIGHT; row_idx > 1; ++row_idx)
        {
            for(int col_idx = 0; col_idx < PLAY_AREA_WIDTH; ++col_idx)
            {
                occupancyList[row_idx, col_idx] = occupancyList[row_idx - 1, col_idx];
            }
        }
        for (int col_idx = 0; col_idx < PLAY_AREA_WIDTH; ++col_idx)
        {
            occupancyList[0, col_idx] = 0;
        }
        return true;
    }

    public void AddRow()
    {
        for (int i = 0; i < PLAY_AREA_WIDTH; ++i)
        {
            AddBlock(0, i, rng.Next(1, Num_Colors));
        }
    }

    public IEnumerator StepGame()
    {
        if (!PushUp())
        {
            GameLost = true;
        }
        AddRow();
        yield return new WaitForSeconds(GameSpeed);
    }

    public void DrawBlock(int row, int col, int type)
    {
        float x_pos = Left + col;
        float y_pos = Bottom + row;
        switch (type) {
            case 0:
                break;
            case 1:
                GameObject RedBlock = Instantiate(Resources.Load("RedBlock"), new Vector3(x_pos, y_pos, 0), Quaternion.identity) as GameObject;
                break;
            case 2:
                GameObject GreenBlock = Instantiate(Resources.Load("GreenBlock"), new Vector3(x_pos, y_pos, 0), Quaternion.identity) as GameObject;
                break;
            case 3:
                GameObject BlueBlock = Instantiate(Resources.Load("BlueBlock"), new Vector3(x_pos, y_pos, 0), Quaternion.identity) as GameObject;
                break;
        }
    }
}