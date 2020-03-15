using UnityEngine;
using UnityEngine.UI;

namespace FancyScrollView.Example02
{
    public class Cell : FancyScrollViewCell<ItemData, Context>
    {
        [SerializeField] Color32 color1 = new Color32(0xFF, 0x61, 0x61, 0xFF); //FF6161
        [SerializeField] Color32 color2 = new Color32(0xAA, 0xFF, 0xC9, 0xFF); //AAFFC9

        [SerializeField] Animator animator = default;
        [SerializeField] Text message = default;
        [SerializeField] Image image = default;
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

            var selected = Context.SelectedIndex == Index;
            image.color = selected
                ? color1 //new Color32(0, 255, 255, 100)
                : color2; //new Color32(255, 255, 255, 77);
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
