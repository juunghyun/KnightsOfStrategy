using UnityEngine;

public enum chessPieceType
{
    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6
}

public class ChessPiece : MonoBehaviour
{
    public int team; //흑, 백팀 구별용
    public chessPieceType type; // 위 enum타입으로 기물 구별
    public int currentX;
    public int currentY;


    private Vector3 desiredPosition;
    private Vector3 desiredScale; //기물이 누군가를 잡아먹으면 해당 기물 크기 변경 후 사이드 배치
    

}
