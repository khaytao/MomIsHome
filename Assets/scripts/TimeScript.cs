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


    public float speed;
    public float amount;
    private Vector3 A0;
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
        
        A0 = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 hourAngle = Time.deltaTime * (gameTimeFactor / 60) * hourAnglePerMinute * Vector3.forward;
        hourHand.transform.eulerAngles -= hourAngle;

        if (GetSecondsLeft() < 30)
        {
            //speed = 700;
            //amount = 1.1f;
            float shake_x = Mathf.Sin(speed * Time.time) * amount;
            
            transform.position = A0 + new Vector3(shake_x,  0, 0);
        }
        if(GetSecondsLeft() <= 0)
        {
            GameManager.Instance.timeOver();
        }

        Vector3 minuteAngle = Time.deltaTime * (gameTimeFactor / 60) * anglePerMinute * Vector3.forward;
        minuteHand.transform.eulerAngles -= minuteAngle;
    }

    private float GetSecondsLeft()
    {
        return gameDurationMinutes * (60 / gameTimeFactor) - GameManager.Instance.elapsedTime;
    }
}
