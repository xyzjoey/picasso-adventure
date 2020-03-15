using System.Linq;
using UnityEngine;
using UnityEngine.UI;
//using System.Collections;//
using System.Collections.Generic;//

namespace FancyScrollView.Example03
{
    public class Example03 : MonoBehaviour
    {
        [SerializeField] GameObject prevCanvas;
        [SerializeField] ScrollView scrollView = default;

        void Awake()
        {
            var items = Enumerable.Range(0, SceneManager.GetStageNum() + 1)
                .Select( i => new ItemData(i, SceneManager.GetScore(i), SceneManager.GetStageState(i)) )//$"Cell {i}"})
               .ToArray();
            //List<ItemData> items = new List<ItemData>();
            //for (int i = 0; i < Save.GetStageNum(); ++i)
            //{
            //    items.Add(new ItemData());
            //}

            scrollView.UpdateData(items);
            scrollView.SelectCell(0);
        }

        public void Disable() //
        {
            prevCanvas.SetActive(true);
            gameObject.SetActive(false);
        }

        //public void Test()
        //{ Debug.Log("Test()"); }
    }
}
