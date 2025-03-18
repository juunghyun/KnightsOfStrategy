using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

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

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        roomCodeInput.characterLimit = 4; // 4자리로 제한
        roomCodeInput.contentType = TMP_InputField.ContentType.IntegerNumber; //숫자만 입력
        joinCodeInput.characterLimit = 4; // ✅ 추가: 조인 입력 필드 설정
        joinCodeInput.contentType = TMP_InputField.ContentType.IntegerNumber;
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
            ShowMainMenuScreen();
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
            yield return new WaitUntil(() => PhotonNetwork.InRoom || !PhotonNetwork.IsConnected);

            if (PhotonNetwork.InRoom)
            {
                Debug.Log("방 입장 성공: " + roomCode);
                // 게임 씬으로 전환 (예시)
                // PhotonNetwork.LoadLevel("GameScene");
                ShowMainMenuScreen();
            }
            else
            {
                Debug.LogError("방 입장 실패");
                joinCodeInput.text = "";
                StartCoroutine(FlashInputField(joinCodeInput)); // ✅ 수정: 조인 입력 필드에 효과
                yield return new WaitForSeconds(0.1f);
                EnableUI(joinCodeInput);
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

    // ✅ 입력 필드 깜빡이는 효과
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
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(roomCode, roomOptions, TypedLobby.Default);
        Debug.Log($"create room num : {roomCode}");
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

    // 나머지 메서드는 동일 유지
    public void OnCreateRoomButton() => ShowCreateRoomScreen();
    public void OnJoinRoomButton() => ShowJoinRoomScreen();
    public void ShowCreateRoomScreen() => SetScreen(createRoomScreen);
    public void ShowMainMenuScreen() => SetScreen(mainMenu);

    public void ShowJoinRoomScreen() => SetScreen(joinRoomScreen);

    private void SetScreen(GameObject activeScreen)
    {
        mainMenu.SetActive(activeScreen == mainMenu);
        createRoomScreen.SetActive(activeScreen == createRoomScreen);
        joinRoomScreen.SetActive(activeScreen == joinRoomScreen);
    }

    public override void OnConnectedToMaster() => Debug.Log("서버 연결 성공");
    public override void OnDisconnected(DisconnectCause cause) => Debug.Log($"연결 끊김: {cause}");
}
