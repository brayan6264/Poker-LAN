using System.Collections.Generic;
using Mirror;
using Mirror.Discovery;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Refs UI")]
    public TMP_InputField inputNickname;
    public TMP_InputField inputIP;
    public TMP_InputField inputMaxPing;
    public Button btnHost;
    public Button btnSearch;
    public Button btnStop;
    public Button btnJoinIP;
    public TextMeshProUGUI statusText;

    [Header("Lista LAN")]
    public Transform contentRoot;      // RoomScroll/Viewport/Content
    public GameObject roomItemPrefab;  // Prefab con RoomItemUI
    public GameObject emptyLabel;      // "No hay salas..." (opcional)

    [Header("Net")]
    public NetworkDiscovery networkDiscovery;

    readonly Dictionary<long, RoomItemUI> items = new();
    NetworkManager manager;

    void Awake()
    {
        manager = NetworkManager.singleton ?? FindObjectOfType<NetworkManager>();

        btnHost.onClick.AddListener(OnHost);
        btnSearch.onClick.AddListener(OnSearch);
        btnStop.onClick.AddListener(OnStop);
        btnJoinIP.onClick.AddListener(OnJoinIp);

        if (PlayerPrefs.HasKey("nickname"))
            inputNickname.text = PlayerPrefs.GetString("nickname");
        if (inputMaxPing && string.IsNullOrWhiteSpace(inputMaxPing.text))
            inputMaxPing.text = "200";
    }

    void OnEnable()
    {
        if (networkDiscovery != null)
            networkDiscovery.OnServerFound.AddListener(OnServerFound);
    }

    void OnDisable()
    {
        if (networkDiscovery != null)
            networkDiscovery.OnServerFound.RemoveListener(OnServerFound);
    }

    // --- Botones ---
    void OnHost()
    {
        if (!inputNickname) { Debug.LogError("inputNickname no asignado"); return; }
        if (!manager) { Debug.LogError("NetworkManager no encontrado"); return; }
        string nick = SanitizeNickname(inputNickname.text);
        if (string.IsNullOrEmpty(nick)) { SetStatus("Escribe tu nombre."); return; }
        PlayerPrefs.SetString("nickname", nick);

        btnHost.interactable = false;
        btnSearch.interactable = false;
        btnJoinIP.interactable = false;
        SetStatus("Creando mesa…");

        manager.StartHost();                    // Mueve a Table (Online Scene)
        if (networkDiscovery != null) networkDiscovery.AdvertiseServer();
    }

    void OnSearch()
    {
        ClearList();
        btnSearch.interactable = false;
        btnStop.interactable = true;
        SetStatus("Buscando…");
        if (emptyLabel) emptyLabel.SetActive(false);
        if (networkDiscovery != null) networkDiscovery.StartDiscovery();
        else SetStatus("Discovery no asignado.");
    }

    void OnStop()
    {
        btnSearch.interactable = true;
        btnStop.interactable = false;
        SetStatus("Idle");
        if (networkDiscovery != null) networkDiscovery.StopDiscovery();
    }

    void OnJoinIp()
    {
        if (!inputIP) { Debug.LogError("inputIP no asignado"); return; }
        if (!manager) { Debug.LogError("NetworkManager no encontrado"); return; }
        string ip = inputIP.text.Trim();
        if (string.IsNullOrEmpty(ip)) { SetStatus("Ingresa una IP (ej: 192.168.1.34)."); return; }
        GuardarNick();
        manager.networkAddress = ip;
        SetStatus($"Conectando a {ip}…");
        manager.StartClient();                  // Al conectar, carga Table
    }

    // --- Discovery callback ---
    void OnServerFound(ServerResponse info)
    {
        if (items.ContainsKey(info.serverId)) return;

        var go = Instantiate(roomItemPrefab, contentRoot);
        var ui = go.GetComponent<RoomItemUI>();
        string nombre = $"Sala \"{info.EndPoint.Address}\""; // título simple
        string ip = info.EndPoint.Address.ToString();
        int ping = 0; // puedes calcularlo después

        ui.Setup(nombre, ip, ping, OnJoinDiscovered);
        items.Add(info.serverId, ui);
    }

    void OnJoinDiscovered(string ip)
    {
        GuardarNick();
        manager.networkAddress = ip;
        SetStatus($"Conectando a {ip}…");
        manager.StartClient();
    }

    // --- Util ---
    void ClearList()
    {
        foreach (Transform t in contentRoot) Destroy(t.gameObject);
        items.Clear();
        if (emptyLabel) emptyLabel.SetActive(true);
    }

    void SetStatus(string s) { if (statusText) statusText.text = $"Estado: {s}"; }

    void GuardarNick()
    {
        string nick = SanitizeNickname(inputNickname.text);
        if (!string.IsNullOrEmpty(nick)) PlayerPrefs.SetString("nickname", nick);
    }

    string SanitizeNickname(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return string.Empty;
        s = s.Trim();
        if (s.Length > 15) s = s.Substring(0, 15);
        return s;
    }
}
