﻿using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class GamePiece : MonoBehaviour
{

    public int xIndex;

    public int yIndex;
    
    private Board m_board;

    private bool m_isMoving = false;
    
    public InterpType interpolation = InterpType.SmootherStep;

    public enum InterpType
    {
        Linear,
        EaseOut,
        EaseIn,
        SmoothStep,
        SmootherStep
    };

    public MatchValue matchValue;
    public enum MatchValue
    {
        Yellow,
        Blue,
        Magenta,
        Indigo,
        Green,
        Teal,
        Red,
        Cyan,
        Wild
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private Animator anim;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move((int)transform.position.x+1,(int)transform.position.y,0.5f);
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move((int)transform.position.x-1,(int)transform.position.y,0.5f);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Move((int)transform.position.x,(int)transform.position.y+1,0.5f);
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move((int)transform.position.x,(int)transform.position.y-1,0.5f);
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            m_board.HighlightMatches();
        }
    }
    
    public void Init( Board board)
         {
        m_board = board;
        }

    public void SetCoord(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    public void Move(int destX, int destY,float timeToMove)
    {
        if(!m_isMoving)
            StartCoroutine(MoveRoutine(new Vector3(destX, destY, 0), timeToMove));
    }

    IEnumerator MoveRoutine(Vector3 destination, float timeToMove)
    {
        Vector3 startPosition = transform.position;

        anim = GetComponent<Animator>();
        
        anim.SetTrigger("Bounce2");

        bool reachedDestination = false;

        float elapsedTime = 0f;

        m_isMoving = true;

        while (!reachedDestination)
        {
            //eger destinationa çok yakınsak
            if (Vector3.Distance(transform.position, destination) < 0.01f)
            {
                reachedDestination = true;

                if (m_board != null)
                {
                    m_board.PlaceGamePiece(this,(int)destination.x,(int)destination.y);
                }
                
                break;
            }

            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp(elapsedTime / timeToMove,0f,1f);
            
            switch (interpolation)
            {
                case InterpType.Linear:
                    break;
                case InterpType.EaseOut:
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);
                    break;
                case InterpType.EaseIn:
                    t = 1 - Mathf.Cos(t * Mathf.PI * 0.5f);
                    break;
                case InterpType.SmoothStep:
                    t = t*t*(3 - 2*t);
                    break;
                case InterpType.SmootherStep:
                    t =  t*t*t*(t*(t*6 - 15) + 10);
                    break;
            }
            
            transform.position = Vector3.Lerp(startPosition,destination,t);
            
            yield return null;
        }

        m_isMoving = false;
    }
}
