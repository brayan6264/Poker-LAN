using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TableTestUI : MonoBehaviour
{
    public Button add10Button;
    public TextMeshProUGUI potText;

    private NetTable netTable;

    private IEnumerator Start()
    {
        // Espera a que el servidor spawnee NetTable y llegue al cliente
        while (netTable == null)
        {
            netTable = FindObjectOfType<NetTable>();
            yield return null; // siguiente frame
        }

        add10Button.onClick.AddListener(() =>
        {
            if (netTable != null)
            {
                netTable.CmdAddToPot(10);// Command al servidor
                Debug.Log("[CLIENT] Botón Add 10 presionado."+netTable.pot);
                Update();
            } 
        });
    }

    private void Update()
    {
        if (netTable != null && potText != null)
            potText.text = $"Pot: {netTable.pot}";
    }
}
