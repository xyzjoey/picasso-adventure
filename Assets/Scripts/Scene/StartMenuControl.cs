using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StartMenuControl : MonoBehaviour
{
    //GameObject startCanvas;
    [SerializeField] GameObject modeCanvas;
    [SerializeField] GameObject stageSelectCanvas;
    [SerializeField] FancyScrollView.Example02.ScrollView modeScroll;
    [SerializeField] FancyScrollView.Example03.ScrollView stageScroll;

    void Start()
    {
        if (!SceneManager.IsCurrSceneType(SceneManager.Scene.START))//(SceneManager.GetCurrScene() != SceneManager.GetScene(SceneManager.Scene.START))
        {
            if (SceneManager.IsCurrSceneType(SceneManager.Scene.FREE))//SceneManager.GetCurrScene() == SceneManager.GetScene(SceneManager.Scene.FREE))
            {
                modeCanvas.SetActive(true);
                modeScroll.SetEventEnable(false);
                modeScroll.SelectCell(1); //tochange //to check
            }
            else
            {
                stageSelectCanvas.SetActive(true);
                stageScroll.SetEventEnable(false);
                stageScroll.SelectCell(SceneManager.GetCurrScene() - SceneManager.GetScene(SceneManager.Scene.STAGE));
            }
            SceneManager.UpdateScene();
        }
    }

    public void Enter()
    {
        modeCanvas.SetActive(true);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
