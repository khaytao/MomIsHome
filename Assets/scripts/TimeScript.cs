using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScript : MonoBehaviour
{
    // how long the game will take (until it hits 3 o'clock)
    public float gameDurationMinutes;
    // how much each second is worth (60 means 1 second is 1 minute in game time)
    public float gameTimeFactor = 60; 
    public Image minuteHand;
    public Image hourHand;

    // 360 / 60
    private float anglePerMinute = 6;
    // 360 / 60 / 12
    private float hourAnglePerMinute = 0.5f;
    private Vector3 goalHourAngle;
    // Start is called before the first frame update
    void Start()
    {
        goalHourAngle = -90 * Vector3.forward;

        Vector3 hourAngle = goalHourAngle;
        hourAngle += gameDurationMinutes * hourAnglePerMinute * Vector3.forward;
        hourHand.transform.eulerAngles = hourAngle;

        Vector3 minuteAngle = Vector3.zero;
        minuteAngle += (gameDurationMinutes % 60) * anglePerMinute * Vector3.forward;
        minuteHand.transform.eulerAngles = minuteAngle;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isGameOver())
        {
            return;
        }

        Vector3 hourAngle = Time.deltaTime * (gameTimeFactor / 60) * hourAnglePerMinute * Vector3.forward;
        hourHand.transform.eulerAngles -= hourAngle;

        if(GameManager.Instance.elapsedTime >= gameDurationMinutes * (60 / gameTimeFactor))
        {
            GameManager.Instance.timeOver();
        }

        Vector3 minuteAngle = Time.deltaTime * (gameTimeFactor / 60) * anglePerMinute * Vector3.forward;
        minuteHand.transform.eulerAngles -= minuteAngle;
    }
}
