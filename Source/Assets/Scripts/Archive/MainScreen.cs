using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainScreen : MonoBehaviour {

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;

    [SerializeField] GameObject mainScreenView;
    [SerializeField] GameObject lobbyScreenView;

    private void Start() {
        RegisterEvents();
        InitSlideVolumeValue();
    }

    private void InitSlideVolumeValue() {
        if (audioMixer == null) return;
        if (volumeSlider == null) return;
        float volume = 0;
        audioMixer.GetFloat("MasterVolume", out volume);
        volumeSlider.value = volume;
    }

    public void ManageVolume(float volume) {
        if (audioMixer == null) return;
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void JoinLobby() {
        LobbyManager lobby = NetworkManager.singleton as LobbyManager;
        if (lobby == null) return;
        lobby.GetFirstMatchListed();
    }

    public void LeaveLobby() {
        LobbyManager lobby = NetworkManager.singleton as LobbyManager;
        if (lobby != null) lobby.ResetSelf();
        if (LobbyView.Instance != null) LobbyView.Instance.ResetLobby();
        InitSlideVolumeValue();
    }

    public void ReadyInLobby() {
        LobbyManager lobby = NetworkManager.singleton as LobbyManager;
        if (lobby != null) lobby.GetLocalNetworkLobbyPlayer().SendReadyToBeginMessage();

        UpdatePlayerView();
    }

    private void RegisterEvents() {
        LobbyManager lobby = NetworkManager.singleton as LobbyManager;
        if (lobby == null) return;
        lobby.OnPlayerJoined = UpdatePlayerView;
    }

    private void UpdatePlayerView() {
        LobbyManager lobby = NetworkManager.singleton as LobbyManager;
        if (lobby == null) return;
        if (LobbyView.Instance == null) return;

        int i = 0;
        foreach (var item in lobby.lobbySlots) {
            if (item == null) continue;
            i++;
        }

        LobbyView.Instance.UpdateLobbyCount(i);

        if (lobbyScreenView == null) return;
        if (mainScreenView == null) return;
        /*if (i <= 0) {
            LeaveLobby();
            lobbyScreenView.SetActive(false);
            mainScreenView.SetActive(true);
        }*/
    }
}
