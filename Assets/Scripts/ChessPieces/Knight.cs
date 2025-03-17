using UnityEngine;
using System.Collections.Generic;


public class Knight : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();


        //상 - 우측 이동
        int x = currentX + 1;
        int y = currentY + 2;
        if(x<tileCountX && y < tileCountY){
            if(board[x,y]== null || board[x,y].team != team)
            {
                r.Add(new Vector2Int(x,y));
            }
        }

        x = currentX + 2;
        y = currentY + 1;
        if(x<tileCountX && y < tileCountY){
            if(board[x,y]== null || board[x,y].team != team)
            {
                r.Add(new Vector2Int(x,y));
            }
        }

        //상 - 좌측 이동
        x = currentX - 1;
        y = currentY + 2;
        if(x>= 0 && y < tileCountY){
            if(board[x,y]== null || board[x,y].team != team)
            {
                r.Add(new Vector2Int(x,y));
            }
        }

        x = currentX - 2;
        y = currentY + 1;
        if(x >= 0 && y < tileCountY){
            if(board[x,y]== null || board[x,y].team != team)
            {
                r.Add(new Vector2Int(x,y));
            }
        }

        //하 - 우측 이동
        x = currentX + 1;
        y = currentY - 2;
        if(x < tileCountX && y >= 0){
            if(board[x,y]== null || board[x,y].team != team)
            {
                r.Add(new Vector2Int(x,y));
            }
        }

        x = currentX + 2;
        y = currentY - 1;
        if(x < tileCountX && y >= 0){
            if(board[x,y]== null || board[x,y].team != team)
            {
                r.Add(new Vector2Int(x,y));
            }
        }

        //하 - 좌측 이동
        x = currentX - 1;
        y = currentY - 2;
        if(x >= 0 && y >= 0){
            if(board[x,y]== null || board[x,y].team != team)
            {
                r.Add(new Vector2Int(x,y));
            }
        }

        x = currentX - 2;
        y = currentY - 1;
        if(x >= 0 && y >= 0){
            if(board[x,y]== null || board[x,y].team != team)
            {
                r.Add(new Vector2Int(x,y));
            }
        }
        

        //기물 처치 이동
        

        return r;
    }
}
