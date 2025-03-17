using UnityEngine;
using System.Collections.Generic; //System.Collections.Generic 네임스페이스는 다양한 컬렉션 클래스를 담고 있다. 그 중 List<T>, Queue<T>, Stack<T>, Dictionary<TKey, TValue> 가 특히 많이 사용되는듯듯

public class Pawn : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (team == 0) ? 1 : -1; //백팀이면 위로, 흑팀이면 아래로

        //1칸 전진
        if(board[currentX,currentY + direction] == null) r.Add(new Vector2Int(currentX, currentY + direction));
        
        //2칸 전진 
        if(board[currentX,currentY + direction] == null)
        {
            if(team == 0 && currentY == 1 && board[currentX, currentY + direction*2] == null)
            r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            if(team == 1 && currentY == 6 && board[currentX, currentY + direction*2] == null)
            r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
        }

        //기물 처치 이동
        if(currentX != tileCountX - 1) //타일 밖 검사 안함
            if(board[currentX + 1, currentY + direction] != null && board[currentX + 1, currentY + direction].team != team)
                r.Add(new Vector2Int(currentX + 1, currentY + direction));
        if(currentX != 0) //타일 밖 검사 안함
            if(board[currentX - 1, currentY + direction] != null && board[currentX - 1, currentY + direction].team != team)
                r.Add(new Vector2Int(currentX - 1, currentY + direction));

        return r;
    }
}
