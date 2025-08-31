using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItemUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI pingText;
    public Button joinButton;

    string ip;
    System.Action<string> onJoin;

    public void Setup(string roomName, string ipAddress, int pingMs, System.Action<string> onJoinCallback)
    {
        titleText.text = roomName;
        pingText.text = pingMs > 0 ? $"{pingMs} ms" : "—";
        ip = ipAddress;
        onJoin = onJoinCallback;

        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(() => onJoin?.Invoke(ip));
    }
}
