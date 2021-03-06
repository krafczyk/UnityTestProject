﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public enum BlockTypes
    {
        Red,
        Blue,
        Green
    }
    public const int Num_Colors = 3;

    public BlockTypes type;

    private Vector3 pos;
    private int row;
    private int col;
    public GameObject BlockObject {
        get { return gameObject; }
    }

    public Vector3 Pos
    {
        get { return gameObject.transform.position; }
        set { gameObject.transform.position = value; }
    }

    public int Row
    {
        get { return row; }
        set { row = value; }
    }
    public int Col
    {
        get { return col; }
        set { col = value; }
    }

    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }

	private void Awake()
    {
	}

    private void OnEnable()
	{
	    
	}

	private void Reset()
    {
        
    }	

	private void OnLevelWasLoaded(int level)
    {
        
    }

   	private void Start()
    {
        
	}

    private void OnApplicationPause(bool pause)
    {
        
	}

    private void FixedUpdate()
    {
        
	}

    private void Update ()
    {
    
	}

    private void LateUpdate()
    {
        	
    }
	
	private void OnDisable()
    {
        
	}

	private void OnDestroy()
    {
        
    }
}
