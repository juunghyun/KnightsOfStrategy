using UnityEngine;
using System.Collections.Generic;
using System.Numerics;

public class Bishop : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        //상 - 우측
        for(int x = currentX + 1, y = currentY + 1; x < tileCountX && y < tileCountY; x++, y++)
        {
            if(board[x,y]==null)
            {
                r.Add(new Vector2Int(x,y));
            }
            else
            {
                if(board[x,y].team != team)
                {
                    r.Add(new Vector2Int(x,y));
                }
                break;
            }
        }
        //상 - 좌측
        for(int x = currentX - 1, y = currentY + 1; x >= 0 && y < tileCountY; x--, y++)
        {
            if(board[x,y]==null)
            {
                r.Add(new Vector2Int(x,y));
            }
            else
            {
                if(board[x,y].team != team)
                {
                    r.Add(new Vector2Int(x,y));
                }
                break;
            }
        }
        //하 - 우측
        for(int x = currentX + 1, y = currentY - 1; x < tileCountX && y >= 0; x++, y--)
        {
            if(board[x,y]==null)
            {
                r.Add(new Vector2Int(x,y));
            }
            else
            {
                if(board[x,y].team != team)
                {
                    r.Add(new Vector2Int(x,y));
                }
                break;
            }
        }
        //하 - 좌측
        for(int x = currentX - 1, y = currentY - 1; x >= 0 && y >= 0; x--, y--)
        {
            if(board[x,y]==null)
            {
                r.Add(new Vector2Int(x,y));
            }
            else
            {
                if(board[x,y].team != team)
                {
                    r.Add(new Vector2Int(x,y));
                }
                break;
            }
        }
        
        return r;
    }
}
