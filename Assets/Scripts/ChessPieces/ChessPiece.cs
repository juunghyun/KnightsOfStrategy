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
    private Vector3 desiredScale = Vector3.one; //기물이 누군가를 잡아먹으면 해당 기물 크기 변경 후 사이드 배치
    
    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime*10);
    }

    public virtual void SetPosition(Vector3 position, bool force = false)
    {
        desiredPosition = position;
        if(force)
        {
            transform.position = desiredPosition;
        }
    }

    public virtual void SetScale(Vector3 scale, bool force = false)
    {
        desiredScale = scale;
        if(force)
        {
            transform.localScale = desiredScale;
        }
    }
}
