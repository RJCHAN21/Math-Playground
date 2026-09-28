using UnityEngine;

public class GoalPoint : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float minimumDistanceRequired = 0.8f;
    [SerializeField] private GameObject winPanel;

    private bool accomplished = false;

    private void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false);  
    }

    private void Update()
    {
        if (player == null || winPanel == null || accomplished)
            return;

        float dist = (player.position - transform.position).magnitude;

        if (dist <= minimumDistanceRequired)
        {
            TriggerWin();
        }
    }

    private void TriggerWin()
    {
        accomplished = true;

        Player2DController playerController = player.GetComponent<Player2DController>();

        if (playerController != null)
            playerController.IsInvincible = true;
            
        foreach (Turret2D turret in FindObjectsByType<Turret2D>())
            turret.DeactivateTurret();

        if (winPanel != null)
            winPanel.SetActive(true);
    }
}
