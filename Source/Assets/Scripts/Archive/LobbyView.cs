using UnityEngine;
using UnityEngine.UI;

public class LobbyView : MonoBehaviour {

    public static LobbyView Instance { get; private set; }

    [SerializeField] private Text playerInLobbyCount;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(this);
        }
    }

    public void ResetLobby() {
        if (playerInLobbyCount == null) return;
        playerInLobbyCount.text = "1";
    }

    public void UpdateLobbyCount(int count) {
        if (playerInLobbyCount == null) return;
        playerInLobbyCount.text = count.ToString();
    }
}
