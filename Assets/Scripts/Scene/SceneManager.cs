using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SceneManager : MonoBehaviour
{
    [SerializeField] static SceneManager S;

    //public enum Mode { STAGE, FREE, CONFIG };

    public enum StageState { UNLOCKED, LOCKED, NOTEXIST };
    public int[] scores;
    public int maxScore = 3;
    [SerializeField] int stageNum = 10;

    public enum Scene { START, STAGE, FREE };
    public int startMenuScene = 0;
    public int stageBeginScene = 1;
    public int freeModeScene = 2;
    public int currScene = 0;

    static string destination;

    void Awake()
    {
        if (S != null) GameObject.Destroy(gameObject);
        else S = this;
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        freeModeScene = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings - 1;
        currScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

        scores = new int[S.stageNum];
        for (int i = 0; i < S.stageNum; ++i) scores[i] = 0;

        destination = Application.persistentDataPath + "/save.dat";
        CreateFile();
        LoadAll();
    }

    static public bool IsSceneType(int sceneIndex, Scene sceneType)
    {
        switch (sceneType)
        {
            case Scene.START:
                return sceneIndex == SceneManager.GetScene(sceneType);
            case Scene.STAGE:
                return sceneIndex >= S.stageBeginScene && sceneIndex < S.freeModeScene;
            case Scene.FREE:
                return sceneIndex == SceneManager.GetScene(sceneType);
            default:
                Debug.Log("Error: SceneManager.IsCurrSceneType() sceneType \"" + sceneType + "\" does not exist");
                return false;
        }
    }
    static public bool IsCurrSceneType(Scene sceneType)
    { return IsSceneType(S.currScene, sceneType); }

    static public int GetCurrScene() { return S.currScene; }
    static public int GetStageScene(int stageIndex) { return GetScene(Scene.STAGE) + stageIndex; }
    static public int GetScene(Scene sceneType)
    {
        switch (sceneType)
        {
            case Scene.START:
                return S.startMenuScene;
            case Scene.STAGE:
                return S.stageBeginScene;
            case Scene.FREE:
                return S.freeModeScene;
            default:
                return 0;
        }
    }

    static public void UpdateScene() { S.currScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex; }

    static public void SetScene(int i) //exact scene index
    {
        S.currScene = i;
    }
    static public void SetScene(Scene sceneType, int stageIndex = -1)
    {
        switch (sceneType)
        {
            case Scene.START:
                S.currScene =  S.startMenuScene;
                break;
            case Scene.STAGE: //to check
                S.currScene = GetStageScene(stageIndex);
                break;
            case Scene.FREE:
                S.currScene = S.freeModeScene;
                break;
            default:
                break;
        }
    }

    static public void LoadScene(Scene sceneType, int stageIndex = -1)
    {
        //Debug.Log("SceneManager.LoadScene() sceneType: " + sceneType + " stageIndex: " + stageIndex);

        switch (sceneType)
        {
            case Scene.START:
                LoadScene(S.startMenuScene, true);
                break;
            case Scene.STAGE: //to check
                if (!IsStageExist(stageIndex)) { Debug.Log("Error: SceneManager.LoadScene() !StageExist " + stageIndex); return; }
                LoadScene(GetStageScene(stageIndex), true);
                break;
            case Scene.FREE:
                LoadScene(S.freeModeScene, true);
                break;
            default:
                break;
        }
    }

    static public void LoadScene(int i, bool exactIndex = false) //-1: home, 0: restart, 1: next
    {
        //Debug.Log("SceneManager.LoadScene() i: " + i + " exactIndex: " + exactIndex);

        if (exactIndex)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(i);
            return;
        }

        if (i == -1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(S.startMenuScene);
        }
        else if (i == 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(S.currScene);
        }
        else if (i == 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(S.currScene + 1);
        }
        else { }
    }

    static public int GetStageNum() { return S.stageNum; }
    static public int GetScore(int i) { return IsStageExist(i)? S.scores[i] : 0; }
    static public bool IsStageExist(int i)
    { return (i >= 0) && (i < S.stageNum) && (GetScene(Scene.STAGE) + i) < S.freeModeScene; }

    static public StageState GetStageState(int i)
    {
        if (!IsStageExist(i)) return StageState.NOTEXIST;
        else if (IsStageLocked(i)) return StageState.LOCKED;
        else return StageState.UNLOCKED;
    }

    static public bool IsStageLocked(int i)
    {
        if (!IsStageExist(i)) return true;

        int range = 5;

        if (i < range) return false;

        int end = i - i % range - 1;
        int start = end - range + 1;

        int prevRangeScore = GetScore(start, end);
        int frontScore = GetScore(0, start - 1) / (S.maxScore * range);

        return (prevRangeScore + frontScore) < (2 * S.maxScore / 3) * range;
    }

    static int GetScore(int start, int end)
    {
        int score = 0;
        for (int i = start; i <= end; ++i) score += S.scores[i];
        return score;
    }

    //static public void SaveColor() //to check
    //{
    //    GameData data = LoadGameData();
    //    data.SetColors(BrushType.GetColors());
    //    SaveGameData(data);
    //}

    //static public void LoadColor()
    //{
    //    GameData data = LoadGameData();
    //    BrushType.SetColors(data.GetColors());
    //}

    static public void SaveScore(int score)
    {
        UpdateScene();
        SaveScore(score, S.currScene - S.stageBeginScene);
    }

    static public void SaveScore(int score, int index) //to check
    {
        if (!IsStageExist(index) || score < 0 || score > S.maxScore)
        {
            Debug.Log("Error: Save.SaveScore() --> index: " + index + " score: " + score);
            return;
        }

        if (S.scores[index] >= score) return;
        
        S.scores[index] = score;
        SaveStageData();
    }

    static public void SaveStageData() //to check
    {
        GameData data = LoadGameData();
        data.SetScores(S.scores);
        SaveGameData(data);
    }

    static public void LoadStageData()
    {
        GameData data = LoadGameData();
        int minLength = Mathf.Min(data.scores.Length, S.scores.Length);
        for (int i = 0; i < minLength; ++i) S.scores[i] = data.scores[i];
    }

    static void LoadAll()
    {
        //LoadColor();
        LoadStageData();
    }

    static void CreateFile()
    {
        if (File.Exists(destination)) return;

        FileStream file;
        file = File.Create(destination);

        GameData data = new GameData(BrushType.GetColors(), S.scores);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, (GameData)data);
        file.Close();
    }

    static void SaveGameData(GameData data) //to check
    {
        FileStream file;

        if (!File.Exists(destination)) CreateFile();
        file = File.OpenWrite(destination);
        file.Position = 0; //problem?

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, (GameData)data);
        file.Close();
    }

    static GameData LoadGameData()
    {
        FileStream file;

        if (!File.Exists(destination)) CreateFile();
        file = File.OpenRead(destination);
        file.Position = 0; //problem?

        BinaryFormatter bf = new BinaryFormatter();
        GameData data = (GameData)bf.Deserialize(file);

        file.Close();

        return data;
    }
}
