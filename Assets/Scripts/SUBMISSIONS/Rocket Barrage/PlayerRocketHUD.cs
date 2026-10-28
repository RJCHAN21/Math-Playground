using TMPro;
using UnityEngine;

public class PlayerRocketHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text rocketCounterTMP;

    private void Start()
    {
        PlayerRocketController.Instance.RocketCountChanged += UpdateRocketCounter;

        UpdateRocketCounter(
            PlayerRocketController.Instance.CurrentRocketCount
        );
    }

    private void OnDestroy()
    {
        if (PlayerRocketController.Instance != null)
        {
            PlayerRocketController.Instance.RocketCountChanged -= UpdateRocketCounter;
        }
    }

    private void UpdateRocketCounter(int rocketCount)
    {
        rocketCounterTMP.text = $"Rockets: {rocketCount.ToString()}/{PlayerRocketController.Instance.MaxRocketCount}";
    }
}