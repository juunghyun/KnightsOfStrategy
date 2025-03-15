using System;
using System.IO.Compression;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Chessboard : MonoBehaviour
{
    // z
    [Header("Art stuff")]
    [SerializeField] private Material tileMaterial;  // 기본 타일 재질
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private Material hoverTileMaterial; // Hover 상태일 때 사용할 재질

    [Header("Prefabs && Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;

    
    //LOGIC
    private ChessPiece[,] chessPieces;
    private ChessPiece currentlyDragging;
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;

    private void Awake()
    {
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpwanAllPieces();
        PositionAllPieces();
    }

    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit info; // Ray가 충돌한 오브젝트의 정보를 담는 구조체
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition); // 마우스 클릭한 화면 좌표에서 카메라 기준으로 ray 생성(Camera -> Mouse)
        
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover"))) // ray가 Tile레이어에 속한 오브젝트가 있다면 info에 담고 true반환
        {
            // Get the indexes of the tile I've hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            // If we're hovering a tile after not hovering any tiles
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                // 타일에 HoverTiles 재질 적용
                tiles[hitPosition.x, hitPosition.y].GetComponent<MeshRenderer>().material = hoverTileMaterial;
            }

            // If we were already hovering a tile, change the previous one
            if (currentHover != hitPosition)
            {
                // 이전 타일 원래 색상으로 복원
                tiles[currentHover.x, currentHover.y].GetComponent<MeshRenderer>().material = tileMaterial;
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");

                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                // 새로 hover한 타일에 HoverTiles 재질 적용
                tiles[hitPosition.x, hitPosition.y].GetComponent<MeshRenderer>().material = hoverTileMaterial;
            }
        
            // 좌클릭
            if(Input.GetMouseButtonDown(0))
            {
                if(chessPieces[hitPosition.x, hitPosition.y]!= null)
                {
                    // 내 턴이야?(흑 턴인지 백 턴인지지)
                    if(true)
                    {
                        currentlyDragging = chessPieces[hitPosition.x, hitPosition.y];
                    }
                }
            }

            //좌클릭 해제
            if(currentlyDragging != null && Input.GetMouseButtonUp(0))
            {
                Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);

                bool validMove = MoveTo(currentlyDragging, hitPosition.x, hitPosition.y);

                //유효하지 않은 위치라면, 원래 있던 위치로 되돌리기
                if(!validMove)
                {
                    currentlyDragging.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y));
                    currentlyDragging = null;
                }
            }
        }
        else
        {
            // 마우스가 다른 곳으로 벗어났다면, hover 상태 해제
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].GetComponent<MeshRenderer>().material = tileMaterial;
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }
        }
    }


    // 보드 생성
    
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;

        // 체스판의 중심을 보드 중앙에 맞추는 방식으로 변경
        bounds = new Vector3(tileSize * (tileCountX / 2), 0, tileSize * (tileCountY / 2)) + boardCenter;

        tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
        {
            for (int y = 0; y < tileCountY; y++)
            {
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
            }
        }
    }

    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        // 이전 위치 계산 방식을 수정하여, bounds를 기준으로 위치를 맞추도록 변경
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds; // x, y를 tileSize와 결합하여 타일을 배치
        vertices[1] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) - bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) - bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }

    //기물 생성성
    private void SpwanAllPieces()
    {
        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

        int whiteTeam = 0;
        int blackTeam = 1;

        //백팀 생성
        chessPieces[0,0] = SpawnSinglePiece(chessPieceType.Rook, whiteTeam);
        chessPieces[1,0] = SpawnSinglePiece(chessPieceType.Knight, whiteTeam);
        chessPieces[2,0] = SpawnSinglePiece(chessPieceType.Bishop, whiteTeam);
        chessPieces[3,0] = SpawnSinglePiece(chessPieceType.Queen, whiteTeam);
        chessPieces[4,0] = SpawnSinglePiece(chessPieceType.King, whiteTeam);
        chessPieces[5,0] = SpawnSinglePiece(chessPieceType.Bishop, whiteTeam);
        chessPieces[6,0] = SpawnSinglePiece(chessPieceType.Knight, whiteTeam);
        chessPieces[7,0] = SpawnSinglePiece(chessPieceType.Rook, whiteTeam);
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            chessPieces[i,1] = SpawnSinglePiece(chessPieceType.Pawn, whiteTeam);
        }

        //흑팀 생성성
        chessPieces[0,7] = SpawnSinglePiece(chessPieceType.Rook, blackTeam);
        chessPieces[1,7] = SpawnSinglePiece(chessPieceType.Knight, blackTeam);
        chessPieces[2,7] = SpawnSinglePiece(chessPieceType.Bishop, blackTeam);
        chessPieces[3,7] = SpawnSinglePiece(chessPieceType.King, blackTeam);
        chessPieces[4,7] = SpawnSinglePiece(chessPieceType.Queen, blackTeam);
        chessPieces[5,7] = SpawnSinglePiece(chessPieceType.Bishop, blackTeam);
        chessPieces[6,7] = SpawnSinglePiece(chessPieceType.Knight, blackTeam);
        chessPieces[7,7] = SpawnSinglePiece(chessPieceType.Rook, blackTeam);
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            chessPieces[i,6] = SpawnSinglePiece(chessPieceType.Pawn, blackTeam);
        }
    }

    private ChessPiece SpawnSinglePiece(chessPieceType type, int team)
    {
        ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();

        cp.type = type;
        cp.team = team;
        cp.GetComponent<MeshRenderer>().material = teamMaterials[team];

        return cp;
    }

    //포지셔닝
    private void PositionAllPieces()
    {
        for(int x = 0; x<TILE_COUNT_X; x++)
            for(int y = 0; y< TILE_COUNT_Y; y++)
                if(chessPieces[x,y] != null)
                    PositionSinglePiece(x,y,true);
    }

    private void PositionSinglePiece(int x, int y, bool force = false)
    {
        chessPieces[x, y].currentX = x;
        chessPieces[x, y].currentY = y;
        chessPieces[x, y].SetPosition(GetTileCenter(x, y), force);
    }
    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize/2, 0, tileSize/2);
    }
    
    // Operations
    private bool MoveTo(ChessPiece cp, int x, int y)
    {
        Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);

        //목표 지점에 다른 기물이 있는 경우
        if(chessPieces[x,y]!= null)
        {
            ChessPiece ocp = chessPieces[x, y];
            if(cp.team == ocp.team) //같은팀 기물임?
            {
                return false;
            }
            else //다른팀 기물 -> 해당 기물 처치
            {
                //TODO
                return true;
            }
        }

        chessPieces[x,y] = cp;
        chessPieces[previousPosition.x, previousPosition.y] = null;

        PositionSinglePiece(x,y);

        return true;
    }
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (tiles[x, y] == hitInfo) return new Vector2Int(x, y);
            }
        }

        return -Vector2Int.one; // 못찾았다면 -1 -1 (INVALID)
    }


}
