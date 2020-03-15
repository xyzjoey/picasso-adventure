using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace FancyScrollView.Example02
{
    public class Example02 : MonoBehaviour
    {
        [SerializeField] string[] cellNames;//
        [SerializeField] UnityEvent[] cellEvents;//
        [SerializeField] ScrollView scrollView = default;
        [SerializeField] Button prevCellButton = default;
        [SerializeField] Button nextCellButton = default;
        [SerializeField] Text selectedItemInfo = default;

        void Awake()
        {
            //prevCellButton.onClick.AddListener(scrollView.SelectPrevCell);
            //nextCellButton.onClick.AddListener(scrollView.SelectNextCell);
            scrollView.OnSelectionChanged(OnSelectionChanged);
            scrollView.SetCellEvents(cellEvents);

            var items = Enumerable.Range(0, cellNames.Length)
                .Select(i => new ItemData { Message = cellNames[i].Replace("\\n", "\n") })//$"Cell {i}"})
                .ToArray();

            scrollView.UpdateData(items);
            scrollView.SelectCell(0);
        }

        void OnSelectionChanged(int index)
        {
            selectedItemInfo.text = $"Selected item info: index {index}";
        }

        public void Disable() //
        { gameObject.SetActive(false); }

        public void EnableCanvas(GameObject canvas)
        {
            if (canvas == null) return;
            canvas.SetActive(true);
            gameObject.SetActive(false);
        }

        public void LoadFreeMode()
        { SceneManager.LoadScene(SceneManager.Scene.FREE); }
    }
}
