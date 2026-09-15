using UnityEngine;

public class FinishZone : MonoBehaviour
{
    [Header("FinishZone Settings")]
    [SerializeField] private Transform finishZone;

    [Tooltip("Distance at which the player triggers the game's winning state.")]
    [SerializeField] private float finishDistance = 2f;

    [SerializeField] private Transform player;

    [Header("FinishZone UI")]
    [SerializeField] private GameObject winScreen;

    private bool hasWon = false;

    void Awake()
    {
        if (finishZone == null)
            finishZone = transform;

        if (winScreen != null)
            winScreen.SetActive(false);
    }

    void Update()
    {
        if (hasWon || player == null || finishZone == null)
            return;

        Vector3 dir = finishZone.position - player.position;
        dir.y = 0f;

        if (dir.sqrMagnitude <= finishDistance * finishDistance)
            TriggerWin();
    }

    private void TriggerWin()
    {
        hasWon = true;

        if (winScreen != null)
            winScreen.SetActive(true);
    }
}