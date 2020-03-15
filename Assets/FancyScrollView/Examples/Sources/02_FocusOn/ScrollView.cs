using System;
using System.Collections.Generic;
using UnityEngine;
using EasingCore;
using UnityEngine.Events;

namespace FancyScrollView.Example02
{
    public class ScrollView : FancyScrollView<ItemData, Context>
    {
        [SerializeField] Scroller scroller = default;
        [SerializeField] GameObject cellPrefab = default;

        [SerializeField] UnityEvent[] cellEvents;//

        Action<int> onSelectionChanged;

        bool eventEnabled = false;

        protected override GameObject CellPrefab => cellPrefab;

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
            if (Input.GetAxis("Mouse ScrollWheel") > 0f && Context.SelectedIndex > 0)
                SelectCell(Context.SelectedIndex - 1);
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f && Context.SelectedIndex < ItemsSource.Count - 1)
                SelectCell(Context.SelectedIndex + 1);
        }

        void UpdateSelection(int index)
        {
            if (Context.SelectedIndex == index)
            {
                return;
            }

            Context.SelectedIndex = index;
            Refresh();

            onSelectionChanged?.Invoke(index);
        }

        public void UpdateData(IList<ItemData> items)
        {
            UpdateContents(items);
            scroller.SetTotalCount(items.Count);
        }

        public void OnSelectionChanged(Action<int> callback)
        {
            onSelectionChanged = callback;
        }

        public void SelectNextCell()
        {
            SelectCell(Context.SelectedIndex + 1);
        }

        public void SelectPrevCell()
        {
            SelectCell(Context.SelectedIndex - 1);
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

        //
        public void SetCellEvents(UnityEvent[] events)
        { cellEvents = events; }

        public void CallCellEvent(int index)
        {
            if (index < cellEvents.Length && cellEvents[index] != null)
                cellEvents[index].Invoke();
        }
    }
}
