using TMPro;
using UnityEngine;

public class SimpleHP : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI HPTMP;
    [SerializeField] private SimplePlayer player;

    private void Update()
    {
        HPTMP.text = $"{player.HP} HP";
    }
}