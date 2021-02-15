using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class MyGameManager : Singleton<MyGameManager>
{
    public const int NumOfLevels = 9;
    public Vector3 oblivion;
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
    public List<Transform> pointsAI;
    public int curAIPointCount;
    private List<Bounds> wallBounds;
    private List<(GameObject, BoxCollider2D)> fireCollider;
    private List<(Task, Bounds)> furnitureTaskBounds;
    private List<Task> furnitureTasks;
    private PlayerScript playerScript;
    private TimeScript clockScript;
    private Task trashBin;
    private int curLevel = 1;

    public bool TrashCanOnFire = false;
    public int leakCount = 0;

    private string dataCollector;
    private bool dumped;

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
    
    public MyGameManager()
    {
        oblivion = new Vector3(-1000, -1000, 0);
        dataCollector = "";
        curAIPointCount = 0;
        pointsAI = new List<Transform>();
        furnitureTasks = new List<Task>();
        goToItem = new Dictionary<GameObject, Item>();
        goToTask = new Dictionary<GameObject, Task>();
        wallBounds = new List<Bounds>();
        fireCollider = new List<(GameObject, BoxCollider2D)>();
        furnitureTaskBounds = new List<(Task, Bounds)>();
        // ResetVals();
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

    public void addAIPoint(GameObject go)
    {
        pointsAI.Add(go.transform);
    }

    public void addFire(GameObject fireGO, BoxCollider2D collider)
    {
        fireCollider.Add((fireGO, collider));
    }

    // todo: remove fire
    public void removeFireBounds(GameObject fireGO)
    {
        // int i = 0;
        // for (; i < fireCollider.Count; i++)
        // {
        //     if (fireGO == fireCollider[i].Item1)
        //     {
        //         // Destroy(fireGO);
        //         // fireBounds.RemoveAt(i);
        //         fireBounds[i] = (fireGO, new Bounds(oblivion, fireBounds[i].Item2.size));
        //         return;
        //     }
        // }
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
        foreach (var (go, collider) in fireCollider)
        {
            if (other.Intersects(collider.bounds))
                return true;
        }

        return false;
    }

    public void addToTaskCount(int num)
    {
        taskCount += num;
        CanvasManager.instance.updateCurTasks(taskCount);
    }

    public void addItem(Item item)
    {
        dataCollector += "Item, " + item.name + ", " + item.forTaskType + ", " + item.gameObject.transform.position.x + "," + item.gameObject.transform.position.y + "," + item.gameObject.transform.position.z + "\\n";
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

    public void dumpLevelInfo()
    {
        if (dumped)
            return;

        dumped = true;
        GameObject[] passingPoints = GameObject.FindGameObjectsWithTag("AI_Point");
        foreach (var point in passingPoints)
        {
            dataCollector += "AI_Point, " + point.gameObject.transform.position.x + "," + point.gameObject.transform.position.y + "," + point.gameObject.transform.position.z + "\\n";
        }
        string fileName = "level_data_1.txt";
        if (File.Exists(fileName))
        {
            Debug.Log(fileName+" already exists.");
            return;
        }
        var sr = File.CreateText(fileName);
        sr.WriteLine(dataCollector);
        sr.Close();
    }

    public void addTask(Task task)
    {
        // collect data for level prefab export
        // if (task.type != TaskType.Furniture)
        // {
        //     // if (task.type == TaskType.Fire) // for level 7
        //     // {
        //     //     task.gameObject.transform.position = new Vector3((task.gameObject.transform.position.x - 5.14243f), (task.gameObject.transform.position.y - 0.15336153f - 0.169186f), 0);
        //     // }
        //     dataCollector += "Task, " + task.name + ", " + task.type + ", " + task.gameObject.transform.position.x +
        //                      "," + task.gameObject.transform.position.y + "," + task.gameObject.transform.position.z +
        //                      "\\n";
        //     
        // }

        if (task.type == TaskType.Trash)
            dataCollector += "burnChance, " + task.burnChance + "\\n";
        
        if (task.type == TaskType.Furniture)
        {
            Bounds furnitureBounds = task.GetComponent<SpriteRenderer>().bounds;
            furnitureTaskBounds.Add((task, furnitureBounds));
            furnitureTasks.Add(task);
        }

        if (task.type == TaskType.Trash)
            trashBin = task;
        
        goToTask.Add(task.gameObject, task);
    }

    // collect break data for level prefab export
    public void addLevelToBreakData(List<Task> toBreak)
    {
        foreach (var task in toBreak)
        {
            dataCollector += "toBreak, " + task.furnitureType + ", " + task.name + "\\n";
        }
    }

    // todo: no longer use
    public void removeTask(Task task)
    {
        playerScript.removeTask(task);
        task.resetForLevel();
    }

    public Item getItem(GameObject go)
    {
        if (!goToItem.ContainsKey(go))
            return null;
        return goToItem[go];
    }
    
    // todo: no longer use
    public void removeItem(Item item)
    {
        item.resetForLevel();
    }

    public Task getTask(GameObject go)
    {
        if (!goToTask.ContainsKey(go))
            return null;
        return goToTask[go];
    }

    public void timeOver()
    {
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
        // ResetVals();
        // LoadLevelPrefabs(curLevel);
        loadLevel(curLevel);
        CanvasManager.instance.ResetClock();
        AudioManager.i.PlayBackGround(AudioFileGetter.i.BackGroundLevel);
        CanvasManager.instance.GameScreen();
    }

    public void areTasksOver()
    {
        CanvasManager.instance.updateCurTasks(taskCount);
        if (taskCount <= 0)
        {
            endGame(true);
        }
    }

    public void FinishTask(Task curTask)
    {

        addToTaskCount(-1);
        areTasksOver();
    }

    private void resetGameForLevel()
    {
        foreach (var goTask in goToTask)
            goTask.Value.resetForLevel();

        foreach (var goItem in goToItem)
            goItem.Value.resetForLevel();
        
        playerScript.resetForLevel();

        TrashCanOnFire = false;
        curAIPointCount = 0;
        timeStarted = -1;
        gameOver = false;
        taskCount = 0;
        if(clockScript)
            clockScript.resetClock();
    }

    private void loadLevel(int levelNum)
    {
        resetGameForLevel();
        
        string levelConfig = LevelsConfig.getLevelConfig(levelNum);
        string[] instructions = levelConfig.Split('\n');
        List<Task> npcs = new List<Task>();
        Task curTask;
        Item item;
        foreach (string instruction in instructions)
        {
            string[] details = instruction.Split(new string[] {", "}, StringSplitOptions.None);
            switch (details[0])
            {
                case "burnChance":
                    trashBin.burnChance = Int32.Parse(details[1]);
                    break;
                case "toBreak":
                    breakFurniture(details[2]);
                    break;
                case "Item":
                    item = placeItemAt(details[1], details[2], details[3]);
                    if (item.forTaskType == TaskType.Trash)
                        taskCount++;
                    break;
                case "Task":
                    curTask = placeTaskAt(details[1], details[2], details[3]);
                    if(curTask.type == TaskType.NPC)
                        npcs.Add(curTask);
                    // trash can not counted, fire and puddle are counted at their create function
                    if(curTask.type != TaskType.Trash && curTask.type != TaskType.Fire && curTask.type != TaskType.Sweep)
                        taskCount++;
                    break;
                case "AI_Point":
                    placeAIPointAt(details[1], curAIPointCount++);
                    break;
                default:
                    throw new Exception("Invalid level setup instruction that starts with " + details[0]);
            }
        }

        // set npc points
        foreach (Task npc in npcs)
        {
            if(npc.enemyAI)
                npc.enemyAI.initForLevel();
        }
        
        CanvasManager.instance.updateCurTasks(taskCount);
    }

    private Item placeItemAt(string goName, string stType, string stPosition)
    {
        TaskType type = (TaskType) Enum.Parse(typeof(TaskType), stType);
        Vector3 position = stringToVector3(stPosition);
        Item item;
        string prefabName = goName.Split(' ')[0];
        if (type != TaskType.Trash)
        {
            foreach (var goItem in goToItem)
            {
                item = goItem.Value;
                if (!item.isActive && item.forTaskType == type)
                {
                    item.gameObject.transform.position = position;
                    item.initForLevel();
                    return item;
                }
            }
            // throw new Exception("Couldn't find Item object of type " + stType);
        }
        // trash - find by name
        else
        {
            foreach (var goItem in goToItem)
            {
                item = goItem.Value;
                string curName = item.gameObject.name.Split(' ')[0];
                if (!item.isActive && curName.Equals(prefabName))
                {
                    item.gameObject.transform.position = position;
                    item.initForLevel();
                    return item;
                }
            }
        }
        // couldn't find an available object
        GameObject newItemGO = Instantiate(Resources.Load(prefabName), position, Quaternion.identity) as GameObject;
        Item newItem = newItemGO.GetComponent<Item>();
        newItem.forTaskType = type;
        newItem.isActive = true;
        // newItem.initForLevel();
        return newItem;
        // todo: add it properly?
    }
    
    private Task placeTaskAt(string goName, string stType, string stPosition)
    {
        TaskType type = (TaskType) Enum.Parse(typeof(TaskType), stType);
        Vector3 position = stringToVector3(stPosition);
        Task task;
        // fire
        if (type == TaskType.Fire)
            return createFireAt(position, goName);
        
        // puddle
        if (type == TaskType.Sweep)
            return createPuddleAt(position, false);
        
        foreach (var goTask in goToTask)
        {
            task = goTask.Value;
            if (!task.isActive && task.type == type)
            {
                task.gameObject.transform.position = position;
                task.initForLevel();
                if (type == TaskType.NPC)
                {
                    task.enemyAI.pointsList.Clear();
                    task.enemyAI.pointsList = new List<Transform>();
                }
                return task;
            }
        }
        // couldn't find an available object
        string prefabName = goName.Split(' ')[0];
        GameObject newTaskGO = Instantiate(Resources.Load(prefabName), position, Quaternion.identity) as GameObject;
        Task newTask = newTaskGO.GetComponent<Task>();
        newTask.type = type;
        newTask.isActive = true;
        // newTask.initForLevel();

        return newTask;
    }
    
    public Task createFireAt(Vector3 position, string goName = "Fire")
    {
        Task task, furniture;
        string prefabName = goName.Split(' ')[0];
        Bounds fireBoundsTemp;
        // need to activate gameobject to get correct bounds of collider
        if (!fireCollider[0].Item1.activeSelf)
        {
            fireCollider[0].Item1.SetActive(true);
            fireBoundsTemp = fireCollider[0].Item2.bounds;
            fireCollider[0].Item1.SetActive(false);
        }
        else
            fireBoundsTemp = fireCollider[0].Item2.bounds;

        Vector3 centerDiff = fireBoundsTemp.center - fireCollider[0].Item1.gameObject.transform.position;
        foreach (var goTask in goToTask)
        {
            task = goTask.Value;
            string curName = task.gameObject.name.Split(' ')[0];
            if (!task.isActive && curName.Equals(prefabName))
            {
                task.gameObject.transform.position = position;
                // update fire bounds position
                // for (int i=0;i<fireCollider.Count; i++)
                // {
                //     if (fireCollider[i].Item1 == task.gameObject)
                //     {
                //         fireCollider[i] = (fireBounds[i].Item1, new Bounds(position + centerDiff, fireBounds[i].Item2.size));
                //         break;
                //     }
                // }
                task.initForLevel();
                fireBoundsTemp = new Bounds(position + centerDiff, fireBoundsTemp.size);
                furniture = inFurniture(fireBoundsTemp);
                if(furniture)
                    furniture.burnFurniture();
                addToTaskCount(1);
                
                return task;
            }
        }
        // couldn't find an available object
        GameObject newTaskGO = Instantiate(Resources.Load(prefabName), position, Quaternion.identity) as GameObject;
        Task newTask = newTaskGO.GetComponent<Task>();
        // newTask.initForLevel();
        fireBoundsTemp = new Bounds(position + centerDiff, fireBoundsTemp.size);
        furniture = inFurniture(fireBoundsTemp);
        if(furniture)
            furniture.burnFurniture();
        addToTaskCount(1);
        newTask.isActive = true;
        newTask.type = TaskType.Fire;
        // todo: add it properly?
        return newTask;
    }
    
    public Task createPuddleAt(Vector3 position = new Vector3(), bool isPuke = true)
    {
        Task task;
        foreach (var goTask in goToTask)
        {
            task = goTask.Value;
            if (!task.isActive && task.type == TaskType.Sweep)
            {
                task.gameObject.transform.position = position;
                task.initForLevel();
                if (!isPuke)
                {
                    task.gameObject.transform.localScale = new Vector3(0.15f, 0.15f, 0);
                    task.duration *= 2;
                }
                else
                    task.gameObject.transform.localScale = new Vector3(0.045f, 0.045f, 0);
                addToTaskCount(1);
                return task;
            }
        }
        // couldn't find an available object
        GameObject newTaskGO = Instantiate(Resources.Load("Puddle"), position, Quaternion.identity) as GameObject;
        Task newTask = newTaskGO.GetComponent<Task>();
        // newTask.initForLevel();
        if (!isPuke)
        {
            newTaskGO.gameObject.transform.localScale = new Vector3(0.15f, 0.15f, 0);
            newTask.duration *= 2;
        }
        else
            newTask.gameObject.transform.localScale = new Vector3(0.045f, 0.045f, 0);
        addToTaskCount(1);
        newTask.isActive = true;
        newTask.type = TaskType.Sweep;
        // todo: add it properly?
        return newTask;
    }
    
    private void placeAIPointAt(string stPosition, int pointIdx)
    {
        if (pointIdx >= pointsAI.Count)
            return;

        pointsAI[pointIdx].transform.position = stringToVector3(stPosition);
    }

    private void breakFurniture(string goName)
    {
        foreach (Task furniture in furnitureTasks)
            if(furniture.gameObject.name.Equals(goName))
                furniture.breakFurniture();
    }

    private Vector3 stringToVector3(string stPosition)
    {
        string[] stAxis = stPosition.Split(',');
        float x = float.Parse(stAxis[0]);
        float y = float.Parse(stAxis[1]);
        float z = float.Parse(stAxis[2]);
        
        return new Vector3(x, y, z);
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


        string levelName = "levels/level " + 1;

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

        try
        {
            GameObject level = Instantiate(Resources.Load(levelName, typeof(GameObject)) as GameObject);
        }
        catch (ArgumentException e)
        {
            SceneManager.LoadScene(0);
        }
        

        //Instantiate(Resources.Load("House no walls", typeof(GameObject)) as GameObject);
        CanvasManager.instance.GameScreen();
    }

    IEnumerator LoadScene()
    {
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Final");
        Debug.Log("Started loading Scene");
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        //AsyncOperation unload = SceneManager.UnloadSceneAsync(currentScene.scene.buildIndex);

        // LoadLevelPrefabs(1);
        loadLevel(curLevel);
        CanvasManager.instance.Resume();
        AudioManager.i.PlayBackGround(AudioFileGetter.i.BackGroundLevel);
    }
    
    public void MainMenu()
    {
        clearObjects();
        SceneManager.LoadScene(0);
    }

    public void NextLevel()
    {
        // ResetVals();
        // curLevel++;
        // LoadLevelPrefabs(curLevel);
        curLevel++;
        if (curLevel > LevelsConfig.getLevelsCount())
        {
            MainMenu();
            return;
        }
        loadLevel(curLevel);
        AudioManager.i.PlayBackGround(AudioFileGetter.i.BackGroundLevel);
        CanvasManager.instance.Resume();
    }

    private void loadFisrt()
    {
        LoadLevelPrefabs(1);
    }

    private bool firstTime = true;
    public void FirstLevel()
    {
        
        // ResetVals();
        curLevel = 1;
        
        // if (firstTime)
        // {
            StartCoroutine(LoadScene());
            firstTime = false;
        // }
        // else
        // {
        //     
        //     StopCoroutine(LoadScene());
        //     StartCoroutine(LoadScene());
        // }

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
        fireCollider = new List<(GameObject, BoxCollider2D)>();
        furnitureTaskBounds = new List<(Task, Bounds)>();
        taskCount = 0;
        if(clockScript)
            clockScript.resetClock();
    }

    private void clearObjects()
    {
        furnitureTasks.Clear();
        goToItem.Clear();
        goToTask.Clear();
        wallBounds.Clear();
        fireCollider.Clear();
        furnitureTaskBounds.Clear();
        pointsAI.Clear();
        curAIPointCount = 0;
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
        AudioManager.i.PlaySound(comment, AudioManager.i.AS_Alexa);
        CanvasManager.instance.alexa(message);
        Invoke(nameof(alexaDone), comment.length);
    }

    public string getAlexaText(string clipName)
    {
        return alexaTexts.ContainsKey(clipName) ? alexaTexts[clipName] : NOT_INITIALIZED;
    }

    public string getRandomAlexaComment()
    {
        
        List<string> stringNames = alexaTexts.Keys.ToList();
        int random = Random.Range(0, stringNames.Count);
        return stringNames.ElementAt(random);
    }
    private void alexaDone()
    {
        isAlexaPlaying = false;
    }

    public bool IsThereAFire()
    {
        return (TrashCanOnFire || fireCollider.Count > 0);
    }

    public int FireCount()
    {
        int count = 0;
        foreach (var goTask in goToTask)
            if (goTask.Value.isActive && goTask.Value.type == TaskType.Fire)
                count++;
        
        if (TrashCanOnFire)
            count++;

        return count;
    }

    public void makeRandomAlexaComment()
    {
        
    }
}
