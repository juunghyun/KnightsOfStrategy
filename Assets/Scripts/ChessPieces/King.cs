using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;

public class King : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        
        //우측
        if(currentX + 1 < tileCountX)
        {
            if(board[currentX+1, currentY]==null)
            {
                r.Add(new Vector2Int(currentX+1, currentY));
            }
            else if(board[currentX+1, currentY].team != team)
            {
                r.Add(new Vector2Int(currentX+1, currentY));
            }

            //상 - 우측
            if(currentY + 1 < tileCountY)
            {
                if(board[currentX+1, currentY+1]==null)
                {
                r.Add(new Vector2Int(currentX+1, currentY+1));
                }
                else if(board[currentX+1, currentY+1].team != team)
                {
                r.Add(new Vector2Int(currentX+1, currentY+1));
                }
            }

            //하 - 우측
            if(currentY - 1 >= 0)
            {
                if(board[currentX+1, currentY-1]==null)
                {
                    r.Add(new Vector2Int(currentX+1, currentY-1));
                }
                else if(board[currentX+1, currentY-1].team!=team)
                {
                    r.Add(new Vector2Int(currentX+1, currentY-1));
                }
            }
        }
        
        //좌측
        if(currentX - 1 >= 0)
        {
            if(board[currentX-1, currentY]==null)
            {
                r.Add(new Vector2Int(currentX-1, currentY));
            }
            else if(board[currentX-1, currentY].team != team)
            {
                r.Add(new Vector2Int(currentX-1, currentY));
            }
            //상 - 좌측
            if(currentY + 1 < tileCountY)
            {
                if(board[currentX-1, currentY+1]== null)
                {
                    r.Add(new Vector2Int(currentX-1, currentY+1));
                }
                else if(board[currentX-1, currentY+1].team!= team)
                {
                    r.Add(new Vector2Int(currentX-1, currentY+1));
                }
            }

            //하 - 좌측
            if(currentY - 1 >= 0)
            {
                if(board[currentX-1, currentY-1]== null)
                {
                    r.Add(new Vector2Int(currentX-1, currentY-1));
                }
                else if(board[currentX-1, currentY-1].team!= team)
                {
                    r.Add(new Vector2Int(currentX-1, currentY-1));
                }
            }

        }

        //위
        if(currentY + 1 < tileCountY)
        {
            if(board[currentX, currentY+1]==null)
            {
                r.Add(new Vector2Int(currentX, currentY+1));
            }
            else if(board[currentX, currentY+1].team != team)
            {
                r.Add(new Vector2Int(currentX, currentY+1));
            }
        }

        //아래
        if(currentY - 1 >= 0)
        {
            if(board[currentX, currentY-1]==null)
            {
                r.Add(new Vector2Int(currentX, currentY-1));
            }
            else if(board[currentX, currentY-1].team != team)
            {
                r.Add(new Vector2Int(currentX, currentY-1));
            }
        }



        return r;
    }

}
