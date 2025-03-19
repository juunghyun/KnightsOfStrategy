using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CameraController : MonoBehaviourPunCallbacks
{
    public Transform[] teamCameraPositions;
    [SerializeField] Camera mainCamera;
    [SerializeField] float smoothTime = 0.5f; // 이동 부드러움 정도
    [SerializeField] float transitionDuration = 1.5f; // 이동 시간
    
    private bool isTeamAssigned = false;
    private bool isTransitioning = false;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        if (!mainCamera) mainCamera = Camera.main;
    }

    private void Start()
    {
        StartCoroutine(WaitForTeamAssignment());
    }

    private IEnumerator WaitForTeamAssignment()
    {
        while (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
        {
            yield return null;
        }
        CheckTeamAssignment();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer.IsLocal && changedProps.ContainsKey("Team") && !isTeamAssigned)
        {
            CheckTeamAssignment();
        }
    }

    private void CheckTeamAssignment()
    {
        if (isTeamAssigned || isTransitioning) return;
        
        int myTeam = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
        StartCoroutine(SmoothTransition(myTeam));
        isTeamAssigned = true;
    }

    private IEnumerator SmoothTransition(int team)
    {
        isTransitioning = true;
        float elapsedTime = 0f;
        
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;
        
        Vector3 targetPos = teamCameraPositions[team].position;
        Quaternion targetRot = teamCameraPositions[team].rotation;

        while (elapsedTime < transitionDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(
                startPos, 
                targetPos, 
                elapsedTime / transitionDuration
            );
            
            mainCamera.transform.rotation = Quaternion.Lerp(
                startRot, 
                targetRot, 
                elapsedTime / transitionDuration
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최종 위치 보정
        mainCamera.transform.position = targetPos;
        mainCamera.transform.rotation = targetRot;
        
        isTransitioning = false;
        Debug.Log($"{team}팀 카메라 이동 완료");
    }
}