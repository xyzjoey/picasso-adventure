using UnityEngine;
using UnityEngine.UI;

namespace FancyScrollView.Example03
{
    public class Cell : FancyScrollViewCell<ItemData, Context>
    {
        [SerializeField] Color32 color1 = new Color32(0xFF, 0x61, 0x61, 0xFF); //FF6161
        [SerializeField] Color32 color2 = new Color32(0xAA, 0xFF, 0xC9, 0xFF); //AAFFC9
        [SerializeField] Color32 color3 = new Color32(0xFF, 0xFF, 0xBB, 0xFF); //FFFFB8
        [SerializeField] GameObject scoreDisplay;
        [SerializeField] GameObject[] scoreBrushes;
        [SerializeField] Animator animator = default;
        [SerializeField] Text message = default;
        [SerializeField] Text messageLarge = default;
        [SerializeField] Image image = default;
        [SerializeField] Image imageLarge = default;
        [SerializeField] Button button = default;

        static class AnimatorHash
        {
            public static readonly int Scroll = Animator.StringToHash("scroll");
        }

        void Start()
        {
            button.onClick.AddListener(() => Context.OnCellClicked?.Invoke(Index));
        }

        public override void UpdateContent(ItemData itemData)
        {
            message.text = itemData.Message;
            messageLarge.text = itemData.MessageLarge; //(Index + 1).ToString();
            messageLarge.fontSize = itemData.MessageLargeFontSize;
            
            var selected = Context.SelectedIndex == Index;

            //
            switch (itemData.State)
            {
            case SceneManager.StageState.UNLOCKED:

                scoreDisplay.SetActive(true);

                for (int i = 0; i < itemData.Score && i < scoreBrushes.Length; ++i)
                {
                    scoreBrushes[i].GetComponent<Image>().enabled = false;
                    scoreBrushes[i].transform.GetChild(0).gameObject.SetActive(true);//problem?
                }
                for (int i = itemData.Score; i < scoreBrushes.Length; ++i)
                {
                    scoreBrushes[i].GetComponent<Image>().enabled = true;
                    scoreBrushes[i].transform.GetChild(0).gameObject.SetActive(false);//problem?
                }

                imageLarge.color = image.color = selected
                    ? color1//
                    : color2;//

                break;
            case SceneManager.StageState.LOCKED:
                scoreDisplay.SetActive(false);
                image.color = selected? color1 : color3;
                imageLarge.color = color3;
                break;
            case SceneManager.StageState.NOTEXIST:
                scoreDisplay.SetActive(false);
                image.color = selected ? color1 : color3;
                imageLarge.color = color3;
                break;
            }
        }

        public override void UpdatePosition(float position)
        {
            currentPosition = position;
            animator.Play(AnimatorHash.Scroll, -1, position);
            animator.speed = 0;
        }

        // GameObject が非アクティブになると Animator がリセットされてしまうため
        // 現在位置を保持しておいて OnEnable のタイミングで現在位置を再設定します
        float currentPosition = 0;

        void OnEnable() => UpdatePosition(currentPosition);
    }
}
