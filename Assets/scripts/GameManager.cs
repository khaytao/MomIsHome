using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public const int NumOfLevels = 5;
    private float timeStarted;
    public bool gameOver;
    
    private int taskCount;
    // how much time has elapsed since the start of game (in case there is an opening scene its not equal to Time.time)
    public float elapsedTime { get
        {
            if (timeStarted == -1)
                timeStarted = Time.time;
            return Time.time - timeStarted;
        }
    }
    private Dictionary<GameObject, Item> goToItem;
    private Dictionary<GameObject, Task> goToTask;
    private List<Bounds> wallBounds;
    private List<(GameObject, Bounds)> fireBounds;
    private List<(Task, Bounds)> furnitureTaskBounds;
    private List<Task> furnitureTasks;
    private PlayerScript playerScript;
    private TimeScript clockScript;
    private int curLevel = 1;

    public bool TrashCanOnFire = false;
    public int leakCount = 0;
    

    private const string NOT_INITIALIZED = "not_initialized";
    private Dictionary<string, string> alexaTexts = new Dictionary<string, string>()
    {
        {"alexaIn", NOT_INITIALIZED},
        {"alexaOut", NOT_INITIALIZED},
        {"alexaWin", "Don't worry, I won't tell... We should do this again"},
        {"alexaLose", "I told you. But you didn't listen"},
        {"alexaSnarky1", NOT_INITIALIZED},
        {"alexaSnarky2", NOT_INITIALIZED},
        {"alexaSnarky3", NOT_INITIALIZED},
        {"alexaSnarky4", NOT_INITIALIZED},
        {"alexaSnarky5", NOT_INITIALIZED},
        {"alexaSnarky6", NOT_INITIALIZED},
        {"alexaSnarky7", NOT_INITIALIZED},
        {"alexaSnarky8", NOT_INITIALIZED},
        {"alexaSnarky9", NOT_INITIALIZED},
        {"alexaSnarky10", NOT_INITIALIZED},
        {"timeLeft90", "mom will be home in 1 minute and 30 seconds. every thing is going to be alright I'm sure"},
        {"timeLeft60", "mom will be home in 1 minute, oh sure, plenty of time, why not crack a beer and relax"},
        {"timeLeft30", "Mom will be home in 30 seconds. She will be so happy to see... er... you"},
        {"timeLeft15", "Mom will be home in 15 seconds. It was nice knowing you"}
    };
    
    public GameManager()
    {
        ResetVals();
    }

    public void setClock(TimeScript clock)
    {
        clockScript = clock;
    }
    
    public PlayerScript getPlayerScript()
    {
        return playerScript;
    }

    public void setPlayerScript(PlayerScript player)
    {
        playerScript = player;
    }

    public void addWallBounds(Bounds wallBound)
    {
        wallBounds.Add(wallBound);
    }

    public void addFire(GameObject fireGO, Bounds fireBound)
    {
        fireBounds.Add((fireGO, fireBound));
        Task furniture = inFurniture(fireBound);
        if(furniture)
            furniture.burnFurniture();
    }

    public void removeFire(GameObject fireGO)
    {
        int i = 0;
        for (; i < fireBounds.Count; i++)
        {
            if (fireGO == fireBounds[i].Item1)
            {
                Destroy(fireGO);
                break;
            }
        }
        fireBounds.RemoveAt(i);
    }

    public bool isInWall(Bounds other)
    {
        foreach (Bounds bounds in wallBounds)
        {
            if (other.Intersects(bounds))
                return true;
        }

        return false;
    }

    public bool isInFire(Bounds other)
    {
        foreach (var (go, bounds) in fireBounds)
        {
            if (other.Intersects(bounds))
                return true;
        }

        return false;
    }

    public void addToTaskCount(int num)
    {
        taskCount += num;
        Debug.Log(taskCount);
    }

    public void addItem(Item item)
    {
        if (item.forTaskType == TaskType.Trash)
        {
            //Debug.Log(item.name);
            addToTaskCount(1);
        }
        goToItem.Add(item.gameObject, item);
    }

    // returns the furniture bounds is on
    public Task inFurniture(Bounds bounds)
    {
        foreach (var (furn, furnBounds) in furnitureTaskBounds)
        {
            if (furnBounds.Intersects(bounds))
                return furn;
        }

        return null;
    }

    public List<Task> getFurnitures()
    {
        return furnitureTasks;
    }

    public void addTask(Task task)
    {
        if (task.type != TaskType.Trash && task.type != TaskType.Furniture)
        {
            addToTaskCount(1);
        }
        if (task.type == TaskType.Furniture)
        {
            Bounds furnitureBounds = task.GetComponent<SpriteRenderer>().bounds;
            furnitureTaskBounds.Add((task, furnitureBounds));
            furnitureTasks.Add(task);
            if(isInFire(furnitureBounds))
                task.burnFurniture();
        }
        
        goToTask.Add(task.gameObject, task);
    }

    public void removeTask(Task task)
    {
        playerScript.removeTask(task);
        goToTask.Remove(task.gameObject);
    }

    public Item getItem(GameObject go)
    {
        if (!goToItem.ContainsKey(go))
            return null;
        return goToItem[go];
    }
    
    public void removeItem(Item item)
    {
        goToItem.Remove(item.gameObject);
    }

    public Task getTask(GameObject go)
    {
        if (!goToTask.ContainsKey(go))
            return null;
        return goToTask[go];
    }

    public void timeOver()
    {
        Debug.Log("Game over!  You lost" + Time.timeScale);
        endGame(false);
    }
    
    private void endGame(bool GameWon)
    {
        Time.timeScale = 0;
        gameOver = true;
        AudioManager.i.stopAll();
        if (GameWon)
        {
            //AudioManager.i.PlayBackGround(AudioFileGetter.i.BackGroundLevelWon);
            AudioManager.i.PlayBackGround(AudioFileGetter.i.winGame);
            AudioManager.i.PlaySound(AudioFileGetter.i.momWin);
            CanvasManager.instance.WonScreenFade();
        }
        else
        {
            //AudioManager.i.PlayBackGround(AudioFileGetter.i.BackGroundLevelWon);
            AudioManager.i.PlayBackGround(AudioFileGetter.i.loseGame);
            AudioManager.i.PlaySound(AudioFileGetter.i.momLose);
            CanvasManager.instance.LostScreen();
        }
        
    }

    public void reloadLevel()
    {
        ResetVals();
        LoadLevelPrefabs(curLevel);
        CanvasManager.instance.ResetClock();
        AudioManager.i.PlayBackGround(AudioFileGetter.i.BackGroundLevel, 0.5f);
    }

    public void areTasksOver()
    {
        Debug.Log(taskCount);
        
        if (taskCount <= 0)
        {
            Debug.Log("Game over! You Won");
            //taskCount--; //this removes the trash can autamatically
            endGame(true);
        }
    }

    public void FinishTask(Task curTask)
    {
        //progressBar.enabled = false;
        
        //Destroy(holdingItem.gameObject);
        //holdingItem.transform.parent = null;
        //holdingItem = null;
        addToTaskCount(-1);
        areTasksOver();
    }

    public void LoadLevelPrefabs(int levelNum)
    {
        Time.timeScale = 1;
        if (levelNum > NumOfLevels)
        {
            MainMenu();
        }
        
        foreach (var task in goToTask.Keys)
        {
            Destroy(task);
        }
        
        foreach (var item in goToItem.Keys)
        {
            Destroy(item);
        }


        string levelName = "levels/level " + 3;//levelNum;

        GameObject cur = GameObject.FindWithTag("level");
        Destroy(cur);
        GameObject house = GameObject.FindWithTag("House");
        Destroy(house);
        GameObject AI = GameObject.FindWithTag("AI");
        //Destroy(AI);
        foreach (GameObject unfinishedTask in GameObject.FindGameObjectsWithTag("Task"))
        {
            Destroy(unfinishedTask);
        }
        
        GameObject level = Instantiate(Resources.Load(levelName, typeof(GameObject)) as GameObject);
        //Instantiate(Resources.Load("House no walls", typeof(GameObject)) as GameObject);
        CanvasManager.instance.GameScreen();
    }

    IEnumerator LoadScene()
    {
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);
        Debug.Log("Started loading Scene");
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        //AsyncOperation unload = SceneManager.UnloadSceneAsync(currentScene.scene.buildIndex);

        LoadLevelPrefabs(1);
        AudioManager.i.PlayBackGround(AudioFileGetter.i.BackGroundLevel, 0.5f);
    }
    
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void NextLevel()
    {
        ResetVals();
        curLevel++;
        LoadLevelPrefabs(curLevel);
        AudioManager.i.PlayBackGround(AudioFileGetter.i.BackGroundLevel, 0.5f);
    }

    private void loadFisrt()
    {
        LoadLevelPrefabs(1);
    }

    private bool firstTime = true;
    public void FirstLevel()
    {
        
        ResetVals();
        curLevel = 1;
        
        if (firstTime)
        {
            StartCoroutine(LoadScene());
            firstTime = false;
        }
        else
        {
            
            StopCoroutine(LoadScene());
            StartCoroutine(LoadScene());
        }

        /*SceneManager.LoadScene(1);
        LoadLevelPrefabs(1);
        //Invoke(nameof(loadFisrt), 0.2f);
        AudioManager.i.PlayBackGround(AudioFileGetter.i.BackGroundLevel, 0.5f);*/
    }

    private void ResetVals()
    {
        furnitureTasks = new List<Task>();
        goToItem = new Dictionary<GameObject, Item>();
        goToTask = new Dictionary<GameObject, Task>();
        timeStarted = -1;
        gameOver = false;
        wallBounds = new List<Bounds>();
        fireBounds = new List<(GameObject, Bounds)>();
        furnitureTaskBounds = new List<(Task, Bounds)>();
        taskCount = 0;
        if(clockScript)
            clockScript.resetClock();
    }

    public bool isAlexaPlaying;

    public void ActivateAlexa(AudioClip comment, string message)
    {
        /*isAlexaPlaying = true;
        SoundManager.PlaySound(comment);
        CanvasManager.instance.alexa(message);
        Invoke(nameof(alexaDone), comment.length);*/
        StartCoroutine(_ActivateAlexa(comment, message));
    }

    private IEnumerator _ActivateAlexa(AudioClip comment, string message)
    {
        yield return new WaitUntil(() => isAlexaPlaying == false);
        isAlexaPlaying = true;
        AudioManager.i.PlaySound(comment);
        CanvasManager.instance.alexa(message);
        Invoke(nameof(alexaDone), comment.length);
    }

    public string getAlexaText(string clipName)
    {
        return alexaTexts.ContainsKey(clipName) ? alexaTexts[clipName] : NOT_INITIALIZED;
    }
    private void alexaDone()
    {
        isAlexaPlaying = false;
    }

    public bool IsThereAFire()
    {
        return (TrashCanOnFire || fireBounds.Count > 0);
    }

    public int FireCount()
    {
        int count = fireBounds.Count;
        if (TrashCanOnFire)
        {
            count++;
        }

        return count;
    }

    public void makeRandomAlexaComment()
    {
        
    }
}
