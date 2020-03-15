using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class StageControl : MonoBehaviour
{
    public enum GameState //to apply
    { Play, Pause, GameOver }
    [HideInInspector] static public GameState state;

    public GameObject[] scoreBrushes;

    public GameObject PauseCanvas;
    public GameObject GameOverCanvas;

    public Button nextButton;
    public Color disableColor;

    public bool isFreeMode = false;

    StrokesControl strokesControl;
    GameObject player;
    Goal goal;
    PostProcessVolume ppVolume;

    Canvas canvas;

    void Start()
    {
        SceneManager.UpdateScene();
        if (!SceneManager.IsStageExist(SceneManager.GetCurrScene() + 1)) //set next button
        {
            nextButton.GetComponent<Image>().color = disableColor;
            nextButton.interactable = false;
        }

        strokesControl = GameObject.Find("Strokes").GetComponent<StrokesControl>();
        player = GameObject.FindWithTag("Player");
        if (!isFreeMode) goal = GameObject.Find("Goal").GetComponent<Goal>();

        ppVolume = Camera.main.transform.Find("PostVolume").GetComponent<PostProcessVolume>();

        canvas = GetComponent<Canvas>();
        state = GameState.Play;//
        StopGame(false);
    }

    void Update()
    {
        if (state == GameState.GameOver) return;

        if (Input.GetKeyDown(KeyCode.Escape)) Pause();

        checkGameOver();
    }

    public void Pause()
    {
        if (state == GameState.Play) {
            state = GameState.Pause;
            StopGame(true);
        } else {
            state = GameState.Play;
            StopGame(false);
        }
    }

    void StopGame(bool pause)
    {
        Time.timeScale = pause? 0 : 1;
        Cursor.visible = pause;
        ppVolume.profile.GetSetting<DepthOfField>().enabled.value = pause;
        ppVolume.profile.GetSetting<Grain>().enabled.value = pause;
        canvas.enabled = pause;
    }

    public void Load(int i = 0) //-1: home, 0: restart, 1: next
    {
        //Debug.Log("StageControl.Load(int i = 0)");

        SceneManager.LoadScene(i, false);
    }

    void checkGameOver()
    {
        if (!isFreeMode && goal.triggered) GameOver();
        if (player.transform.position.y < -70) GameOver(false);
    }

    void GameOver(bool win = true)
    {
        state = GameState.GameOver;

        int score = win? strokesControl.GetPaintScore() : 0;
        Debug.Log("score: " + score);
        for (int i = 0; i < score; ++i) {
            scoreBrushes[i].GetComponent<Image>().enabled = false;
            scoreBrushes[i].transform.GetChild(0).GetComponent<Image>().enabled = true; //problem
            //Debug.Log(scoreBrushes[0].GetComponent<Image>().sprite.name + " " + scoreBrushes[0].GetComponentInChildren<Image>().sprite.name);
        }
        if (score == 0) GameOverCanvas.GetComponentInChildren<Text>().text = "IT CAN BE BETTER...";

        SceneManager.SaveScore(score);

        GameOverCanvas.SetActive(true);
        PauseCanvas.SetActive(false);

        StopGame(true);
    }
}
