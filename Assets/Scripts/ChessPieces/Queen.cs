using UnityEngine;
using System.Collections.Generic;

public class Queen : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        //아래 이동
        for (int i = currentY - 1; i >= 0; i--)
        {
            if(board[currentX, i] == null)
            {
                r.Add(new Vector2Int(currentX, i));
            }

            if(board[currentX, i] != null) //무언가 있다면
            {
                if(board[currentX, i].team != team) //다른 팀이면?
                {
                    r.Add(new Vector2Int(currentX, i)); // 잡을 수 있는 위치니 추가
                }
                break;
            }
        }

        //위 이동
        for (int i = currentY + 1; i < tileCountY; i++)
        {
            if(board[currentX, i] == null)
            {
                r.Add(new Vector2Int(currentX, i));
            }

            if(board[currentX, i] != null) //무언가 있다면
            {
                if(board[currentX, i].team != team) //다른 팀이면?
                {
                    r.Add(new Vector2Int(currentX, i)); // 잡을 수 있는 위치니 추가
                }
                break;
            }
        }

        //좌측 이동
        for (int i = currentX - 1; i >= 0; i--)
        {
            if(board[i, currentY] == null)
            {
                r.Add(new Vector2Int(i, currentY));
            }

            if(board[i, currentY] != null) //무언가 있다면
            {
                if(board[i, currentY].team != team) //다른 팀이면?
                {
                    r.Add(new Vector2Int(i, currentY)); // 잡을 수 있는 위치니 추가
                }
                break;
            }
        }

        //우측 이동
        for (int i = currentX + 1; i < tileCountX; i++)
        {
            if(board[i, currentY] == null)
            {
                r.Add(new Vector2Int(i, currentY));
            }

            if(board[i, currentY] != null) //무언가 있다면
            {
                if(board[i, currentY].team != team) //다른 팀이면?
                {
                    r.Add(new Vector2Int(i, currentY)); // 잡을 수 있는 위치니 추가
                }
                break;
            }
        }

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
