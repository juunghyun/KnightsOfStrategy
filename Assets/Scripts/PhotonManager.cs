using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PhotonManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public TMP_InputField roomCodeInput;
    public TMP_InputField joinCodeInput;
    public Button createButton;
    public Button joinButton;
    public GameObject mainMenu;
    public GameObject createRoomScreen;
    public GameObject joinRoomScreen;
    public GameObject waitingScreen;

    [Header("Team Info UI")]
    public TMP_Text whiteTeamInfo;
    public TMP_Text blackTeamInfo;
    private DateTime dt1;

    private void Start()
    {
        PhotonNetwork.AuthValues = new AuthenticationValues
        {
            UserId = DateTime.Now.ToString("HHmmssff") // 디바이스 고유 ID 사용
        };
        Debug.Log(PhotonNetwork.AuthValues.UserId);
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.JoinLobby(); //로비 참가(현재 존재하는 방 보기 위해서)
        roomCodeInput.characterLimit = 4; // 4자리로 제한
        roomCodeInput.contentType = TMP_InputField.ContentType.IntegerNumber; //숫자만 입력
        joinCodeInput.characterLimit = 4; // ✅ 추가: 조인 입력 필드 설정
        joinCodeInput.contentType = TMP_InputField.ContentType.IntegerNumber;
    }

    //로비 참가 확인용
    public override void OnJoinedLobby()
    {
        Debug.Log("로비에 참가했습니다.");
        Hashtable props = new Hashtable();
        props["UserId"] = PhotonNetwork.AuthValues.UserId;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        
        UpdateTeamInfo();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log($"현재 생성된 방 수: {roomList.Count}");
        foreach (RoomInfo room in roomList)
        {
            Debug.Log($"방 이름: {room.Name}, 플레이어 수: {room.PlayerCount}/{room.MaxPlayers}");
        }
    }


    //방 만들기 버튼 클릭
    public void OnCreateRoomFromInput()
    {
        StartCoroutine(CreateRoomProcess());
    }

    //방 참가하기 버튼 클릭
    public void OnJoinRoomFromInput()
    {
        StartCoroutine(JoinRoomProcess());
    }

    //CreateRoomScreen에서 방 생성 클릭 시
    private IEnumerator CreateRoomProcess() // ✅ 코루틴
    {
        // UI 잠금
        roomCodeInput.interactable = false;
        createButton.interactable = false;

        string roomCode = roomCodeInput.text.PadLeft(4, '0'); // ✅ 4자리 맞춤

        if (int.TryParse(roomCode, out int roomNumber) && roomNumber >= 0 && roomNumber <= 9999)
        {
            CreateRoom(roomCode);
            yield return new WaitUntil(() => PhotonNetwork.InRoom); // ✅ 방 생성 완료 대기
            //TODO
            ShowWaitingScreen();
        }
        else
        {
            Debug.LogError("잘못된 방 번호");
            StartCoroutine(FlashInputField(roomCodeInput)); // ✅ 시각적 피드백
            roomCodeInput.text = "";
            yield return new WaitForSeconds(0.1f); // ✅ 짧은 지연 추가
            EnableUI(roomCodeInput);
        }
    }

    //JoinRoomScreen에서 방 참가 클릭 시
    private IEnumerator JoinRoomProcess()
    {
        joinCodeInput.interactable = false;
        joinButton.interactable = false;

        string roomCode = joinCodeInput.text.PadLeft(4, '0');

        if (int.TryParse(roomCode, out int roomNumber) && roomNumber >= 0 && roomNumber <= 9999)
        {
            PhotonNetwork.JoinRoom(roomCode); // ✅ 방 참가 시도

            float timeoutDuration = 20f;
            float elapsedTime = 0f;

            while (!PhotonNetwork.InRoom && PhotonNetwork.IsConnected && elapsedTime < timeoutDuration)
            {
                elapsedTime += Time.deltaTime;
                Debug.Log($"{elapsedTime}째 기다리는중...");
                yield return null;
            }

            if (PhotonNetwork.InRoom)
            {
                Debug.Log("방 입장 성공: " + roomCode);
                //TODO
                // 게임 씬으로 전환 (예시)
                // PhotonNetwork.LoadLevel("GameScene");
                ShowWaitingScreen();
            }
            else
            {
                Debug.LogError("방 입장 실패");
                joinCodeInput.text = "";
                StartCoroutine(FlashInputField(joinCodeInput)); // ✅ 수정: 조인 입력 필드에 효과
            }
        }
        else
        {
            Debug.LogError("잘못된 방 번호");
            StartCoroutine(FlashInputField(joinCodeInput)); // ✅ 수정
            joinCodeInput.text = "";
        }

        yield return new WaitForSeconds(0.1f);
        EnableUI(joinCodeInput); // ✅ 추가: 조인 UI 전용 활성화 함수
    }

    // 입력 필드 깜빡이는 효과
    private IEnumerator FlashInputField(TMP_InputField fieldName)
    {
        Image inputFieldImage = fieldName.GetComponent<Image>();
        Color originalColor = inputFieldImage.color;
        
        for(int i = 0; i < 2; i++)
        {
            inputFieldImage.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            inputFieldImage.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void CreateRoom(string roomCode)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.PublishUserId = true;
        roomOptions.MaxPlayers = 2;
        bool success = PhotonNetwork.CreateRoom(roomCode, roomOptions, TypedLobby.Default);
        if(success)
        {
            Debug.Log($"생성된 방 코드: {roomCode}");
        }
        else
        {
            Debug.LogError("방 생성 실패! 이미 존재하는 코드일 수 있음"); //photon에서는 이미 존재하는 코드의 방 중복 불가능
        }
        
    }

    private void EnableUI(TMP_InputField fieldName)
    {
        fieldName.interactable = true;
        if(fieldName == roomCodeInput)
        {
            createButton.interactable = true;
            EventSystem.current.SetSelectedGameObject(roomCodeInput.gameObject);
        }
        else
        {
            joinButton.interactable = true;
            EventSystem.current.SetSelectedGameObject(joinCodeInput.gameObject);
        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    
    //대기 방 들어가서의 로직

    private void UpdateTeamInfo()
    {
        whiteTeamInfo.text = "Waiting...";
        blackTeamInfo.text = "Waiting...";
        Debug.Log($"현재 방 상태: {PhotonNetwork.NetworkClientState}");
        Debug.Log($"플레이어 수: {PhotonNetwork.PlayerList.Length}");

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            //white team Info, Black team Info 수정해야함
            if (player != PhotonNetwork.LocalPlayer) //내가 아님?
            {
                if(player.IsMasterClient)
                {
                    whiteTeamInfo.text = player.UserId;
                }
                else
                {
                    blackTeamInfo.text = player.UserId;
                }
            }
            else //나임?
            {
                if(player.IsMasterClient)
                {
                    whiteTeamInfo.text = player.UserId;
                }
                else
                {
                    blackTeamInfo.text = player.UserId;
                }
            }

        }
    }
    
    //방 입, 퇴장에 관한 이벤트
    public override void OnJoinedRoom() => UpdateTeamInfo();
    public override void OnPlayerEnteredRoom(Player newPlayer) => UpdateTeamInfo();
    public override void OnPlayerLeftRoom(Player otherPlayer) => UpdateTeamInfo();
    public override void OnMasterClientSwitched(Player newMaster) => UpdateTeamInfo();
    
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("UserId"))
        {
            UpdateTeamInfo();
        }
    }
    
    // 나머지 메서드는 동일 유지
    public void OnCreateRoomButton() => ShowCreateRoomScreen();
    public void OnJoinRoomButton() => ShowJoinRoomScreen();
    public void ShowCreateRoomScreen() => SetScreen(createRoomScreen);
    public void ShowMainMenuScreen() => SetScreen(mainMenu);
    public void ShowWaitingScreen() => SetScreen(waitingScreen);

    public void ShowJoinRoomScreen() => SetScreen(joinRoomScreen);

    private void SetScreen(GameObject activeScreen)
    {
        mainMenu.SetActive(activeScreen == mainMenu);
        createRoomScreen.SetActive(activeScreen == createRoomScreen);
        joinRoomScreen.SetActive(activeScreen == joinRoomScreen);
        waitingScreen.SetActive(activeScreen == waitingScreen);
    }


    
    public override void OnConnectedToMaster() => Debug.Log("서버 연결 성공");
    public override void OnDisconnected(DisconnectCause cause) => Debug.Log($"연결 끊김: {cause}");
}