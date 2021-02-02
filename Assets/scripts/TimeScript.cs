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

    [Range(0, 1)]
    public float volumeScaleIdle;
    [Range(0, 1)]
    public float volumeScaleEnd;
    
    public float speed;
    // 360 / 60
    private float anglePerMinute = 6;
    // 360 / 60 / 12
    private float hourAnglePerMinute = 0.5f;
    private Vector3 goalHourAngle;

    private float shakeStarted;

    private float shakeForce;

    public float defaultShakeDuration;
    private float initialClockX;
    private float shakeDuration;

    private float x_original;
    private float y_original;
    
    // Start is called before the first frame update
    void Start()
    {
        initialClockX = transform.position.x;
        GameManager.Instance.setClock(this);
        goalHourAngle = -90 * Vector3.forward;

        resetClock();
        
        AudioManager.i.InitClockSound(AudioFileGetter.i.clock, volumeScaleIdle);

        var t = transform.position;
        x_original = t.x;
        y_original = t.y;
    }

    public void resetClock()
    {
        Vector3 hourAngle = goalHourAngle;
        hourAngle += gameDurationMinutes * hourAnglePerMinute * Vector3.forward;
        hourHand.transform.eulerAngles = hourAngle;

        Vector3 minuteAngle = Vector3.zero;
        minuteAngle += (gameDurationMinutes % 60) * anglePerMinute * Vector3.forward;
        minuteHand.transform.eulerAngles = minuteAngle;
        
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
        if (shakeStarted >= 0 && elapsedTime - shakeStarted < shakeDuration)
        {
            float shake_x = Mathf.Sin(speed * Time.time) * shakeForce;
            transform.position = new Vector3(initialClockX + shake_x,  transform.position.y, 0);
            return;
        }
        
        transform.position = new Vector3(initialClockX, transform.position.y, 0);
        string comment;
        float timeLeft = GetSecondsLeft();
        
        // 90 seconds left
        if (timeLeft <= 90 && timeLeft >= 90 - defaultShakeDuration)
        {
            comment = GameManager.Instance.getAlexaText("timeLeft90");
            startShake(1, AudioFileGetter.i.timeLeft90, defaultShakeDuration, comment);
        }
        
        // 60 seconds
        if (timeLeft <= 60 && timeLeft >= 60 - defaultShakeDuration)
        {
            comment = GameManager.Instance.getAlexaText("timeLeft60");
            startShake(3, AudioFileGetter.i.timeLeft60, defaultShakeDuration, comment);
        }
        

        // 30 seconds
        if (timeLeft <= 30 && timeLeft >= 30 - defaultShakeDuration)
        {
            comment = GameManager.Instance.getAlexaText("timeLeft30");
            startShake(5, AudioFileGetter.i.timeLeft30, defaultShakeDuration, comment);
        }
            

        // 15 seconds
        if (timeLeft <= 15 && timeLeft >= 0)
        {
            comment = GameManager.Instance.getAlexaText("timeLeft15");
            startShake(7, AudioFileGetter.i.timeLeft15, 15, comment);
            AudioManager.i.InitClockSound(AudioFileGetter.i.clock, volumeScaleEnd);
        }
        
        transform.position = new Vector3(x_original, y_original);
    }

    private void startShake(float shakeForce, AudioClip clip, float shakeDuration, string comment)
    {
        shakeStarted = GameManager.Instance.elapsedTime;
        this.shakeForce = shakeForce;
        this.shakeDuration = shakeDuration;
        
        GameManager.Instance.ActivateAlexa(clip, comment);
        //SoundManager.PlaySound(clip);
        // todo: should shakeLength change per frame?
    }
    
    private void startShake(float shakeForce, int timeLeft)
    {
        shakeStarted = GameManager.Instance.elapsedTime;
        this.shakeForce = shakeForce;

        AudioClip clip;
        string comment;
        if (timeLeft == 90)
        {
            clip = AudioFileGetter.i.timeLeft90;
            comment = GameManager.Instance.getAlexaText("timeLeft90");
        }
        else if (timeLeft == 60)
        {
            clip = AudioFileGetter.i.timeLeft60;
            comment = GameManager.Instance.getAlexaText("timeLeft60");
        }
        else if (timeLeft == 30)
        {
            clip = AudioFileGetter.i.timeLeft30;
            comment = GameManager.Instance.getAlexaText("timeLeft30");
        }
        else
        {
            clip = AudioFileGetter.i.timeLeft15;
            comment = GameManager.Instance.getAlexaText("timeLeft15");
        }
        
        GameManager.Instance.ActivateAlexa(clip, comment);
        //SoundManager.PlaySound(clip);
        // todo: should shakeLength change per frame?
    }
    
    
    private float GetSecondsLeft()
    {
        return gameDurationMinutes * (60 / gameTimeFactor) - GameManager.Instance.elapsedTime;
    }
}
