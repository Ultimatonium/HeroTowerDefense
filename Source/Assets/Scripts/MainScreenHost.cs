using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainScreenHost : MonoBehaviour
{

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] GameObject mainScreenView;
    [SerializeField] GameObject lobbyScreenView;
    [SerializeField] InputField inputFieldIPAddress;
    [SerializeField] Text ipAddress;
    [SerializeField] Text joinButtonText;

    private void Start()
    {
        GetAndShowIPAddress();
    }

    private void GetAndShowIPAddress()
    {
        if (ipAddress == null) return;
        ipAddress.text = GetLocalIpAddress();
    }

    /// <summary>
    /// From: https://answers.unity.com/questions/1544275/how-to-get-local-ip-in-unity-201824.html
    /// </summary>
    private string GetLocalIpAddress()
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "0.0.0.0";
    }

    public void InitSlideVolumeValue()
    {
        if (audioMixer == null) return;
        if (volumeSlider == null) return;
        float volume = 0;
        audioMixer.GetFloat("MasterVolume", out volume);
        volumeSlider.value = volume;
    }

    public void ManageVolume(float volume)
    {
        if (audioMixer == null) return;
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void CreateHost()
    {
        AutoNetworkManager lobby = NetworkManager.singleton as AutoNetworkManager;
        if (lobby == null) return;
        lobby.CreateHost();
    }

    public void JoinHost()
    {
        if (inputFieldIPAddress == null) return;
        if (string.IsNullOrEmpty(inputFieldIPAddress.text)) return;

        AutoNetworkManager lobby = NetworkManager.singleton as AutoNetworkManager;
        if (lobby == null) return;

        string ipAddress = inputFieldIPAddress.text;

        lobby.JoinHost(ipAddress);
        if (joinButtonText == null) return;
        joinButtonText.text = "Connecting...";
    }
}
