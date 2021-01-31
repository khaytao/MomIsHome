using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScriptSeconds : MonoBehaviour
{
    public float GameSeconds;
    // how long the game will take (until it hits 3 o'clock)
    private float gameDurationMinutes;
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

    private float shakeStarted;

    private float shakeForce;

    public float shakeLength;
    // Start is called before the first frame update
    void Start()
    {
        gameDurationMinutes = GameSeconds * gameTimeFactor / 60;
        goalHourAngle = -90 * Vector3.forward;

        Vector3 hourAngle = goalHourAngle;
        hourAngle += gameDurationMinutes * hourAnglePerMinute * Vector3.forward;
        hourHand.transform.eulerAngles = hourAngle;

        Vector3 minuteAngle = Vector3.zero;
        minuteAngle += (gameDurationMinutes % 60) * anglePerMinute * Vector3.forward;
        minuteHand.transform.eulerAngles = minuteAngle;
        
        A0 = transform.position;
        shakeStarted = -1;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 hourAngle = Time.deltaTime * (gameTimeFactor / 60) * hourAnglePerMinute * Vector3.forward;
        hourHand.transform.eulerAngles -= hourAngle;
        
        shakeClock();
        
        if(GetSecondsLeft() <= 0)
            GameManager.Instance.timeOver(); 

        Vector3 minuteAngle = Time.deltaTime * (gameTimeFactor / 60) * anglePerMinute * Vector3.forward;
        minuteHand.transform.eulerAngles -= minuteAngle;
    }

    private void shakeClock()
    {
        float elapsedTime = GameManager.Instance.elapsedTime;
        if (shakeStarted >= 0 && elapsedTime - shakeStarted < shakeLength)
        {
            float shake_x = Mathf.Sin(speed * Time.time) * shakeForce;
            transform.position = A0 + new Vector3(shake_x,  0, 0);
            return;
        }

        float timeLeft = GetSecondsLeft();
        transform.position = A0;
        // 90 seconds left
        if (timeLeft <= 90 && timeLeft >= 90 - shakeLength)
            startShake(1, AudioFileGetter.i.timeLeft90);

        // 60 seconds
        if (timeLeft <= 60 && timeLeft >= 60 - shakeLength)
            startShake(3, AudioFileGetter.i.timeLeft60);

        // 30 seconds
        if (timeLeft <= 30 && timeLeft >= 30 - shakeLength)
            startShake(5, AudioFileGetter.i.timeLeft30);

        // 15 seconds
        if (timeLeft <= 15 && timeLeft >= 15 - shakeLength)
            startShake(7, AudioFileGetter.i.timeLeft15);
    }

    private void startShake(float shakeForce, AudioClip clip)
    {
        shakeStarted = GameManager.Instance.elapsedTime;
        this.shakeForce = shakeForce;
        SoundManager.PlaySound(clip);
        // todo: should shakeLength change per frame?
    }

    private float GetSecondsLeft()
    {
        return gameDurationMinutes * (60 / gameTimeFactor) - GameManager.Instance.elapsedTime;
    }
}
