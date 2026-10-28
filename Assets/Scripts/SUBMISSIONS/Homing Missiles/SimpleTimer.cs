using TMPro;
using UnityEngine;

public class SimpleTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerTMP;

    private float elapsed;

    void Update()
    {
        elapsed += Time.deltaTime;

        int totalMilliseconds = Mathf.FloorToInt(elapsed * 1000f);
        int minutes = totalMilliseconds / 60000 % 60;
        int seconds = totalMilliseconds / 1000 % 60;
        int milliseconds = totalMilliseconds % 1000;

        timerTMP.text = $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }
}
