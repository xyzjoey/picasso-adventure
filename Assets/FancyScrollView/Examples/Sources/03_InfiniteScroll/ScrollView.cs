using System;
using System.Collections.Generic;
using UnityEngine;
using EasingCore;

namespace FancyScrollView.Example03
{
    public class ScrollView : FancyScrollView<ItemData, Context>
    {
        [SerializeField] Scroller scroller = default;
        [SerializeField] GameObject cellPrefab = default;

        protected override GameObject CellPrefab => cellPrefab;

        bool eventEnabled = false;

        void Awake()
        {
            Context.OnCellClicked = SelectCell;
        }

        void Start()
        {
            scroller.OnValueChanged(UpdatePosition);
            scroller.OnSelectionChanged(UpdateSelection);
        }

        void Update()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                SelectCell((Context.SelectedIndex == 0) ? ItemsSource.Count - 1 : (Context.SelectedIndex - 1) % ItemsSource.Count);
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                SelectCell((Context.SelectedIndex + 1) % ItemsSource.Count);
        }

        void UpdateSelection(int index)
        {
            if (Context.SelectedIndex == index)
            {
                return;
            }

            Context.SelectedIndex = index;
            Refresh();
        }

        public void UpdateData(IList<ItemData> items)
        {
            UpdateContents(items);
            scroller.SetTotalCount(items.Count);
        }

        public void SelectCell(int index = -1)
        {
            if (Context.SelectedIndex == -1 && index == -1) index = 0;
            else if (index == -1) index = Context.SelectedIndex;

            if (index < 0 || index >= ItemsSource.Count) { Debug.Log("Error: ex2.ScrollView.SelectCell " + index + " < || >= ItemsSource.Count " + ItemsSource.Count); return; }

            if (eventEnabled && index == Context.SelectedIndex)
            {
                CallCellEvent(index); //
                return;
            }
            SetEventEnable(true);

            UpdateSelection(index);
            scroller.ScrollTo(index, 0.35f, Ease.OutCubic);
        }

        public void SetEventEnable(bool enable) { eventEnabled = enable; }

        public void CallCellEvent(int index) //todo
        {
            Debug.Log("CallCellEvent");
            SceneManager.LoadScene(SceneManager.Scene.STAGE, index);
        }
    }
}
