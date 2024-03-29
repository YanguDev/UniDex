using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniDex.UI
{
    public class ScrollViewCulling : MonoBehaviour
    {
        [SerializeField]
        private UIDocument uiDocument;
        [SerializeField]
        private int cullingPadding = 400;

        private int firstVisibleIndex = 0;
        private int lastVisibleIndex = int.MaxValue;
        private ScrollView ScrollView => uiDocument.rootVisualElement.Q<ScrollView>();

        private void OnEnable()
        {
            ScrollView.verticalScroller.valueChanged += OnScrollValueChanged;
            ScrollView.horizontalScroller.valueChanged += OnScrollValueChanged;

            CoroutinesUtility.DelayByFrame(this, InitializeVisibility);
        }
        
        private void OnScrollValueChanged(float scrollChanged)
        {
            UpdateVisibility();
        }

        public void InitializeVisibility()
        {
            // Force rebuild ScrollView size
            ScrollView.schedule.Execute(() => {
                var fakeOldRect = Rect.zero;
                var fakeNewRect = ScrollView.layout;

                using (var ev = GeometryChangedEvent.GetPooled(fakeOldRect, fakeNewRect))
                {
                    ev.target = ScrollView.contentContainer;
                    ScrollView.contentContainer.SendEvent(ev);
                }
            });

            int? firstVisibleIndex = null;
            int? lastVisibleIndex = null;
            for (int i = 0; i < ScrollView.childCount; i++)
            {
                VisualElement element = ScrollView.contentContainer[i];
                if (IsElementVisible(element))
                {
                    firstVisibleIndex ??= i;
                    element.visible = true;
                }
                else
                {
                    element.visible = false;
                    if (firstVisibleIndex != null)
                    {
                        lastVisibleIndex ??= i - 1;
                    }
                }
            }

            if (firstVisibleIndex != null)
            {
                this.firstVisibleIndex = firstVisibleIndex.Value;
            }
            else
            {
                this.firstVisibleIndex = 0;
            }

            if (lastVisibleIndex != null)
            {
                this.lastVisibleIndex = lastVisibleIndex.Value;
            }
            else
            {
                this.lastVisibleIndex = ScrollView.childCount - 1;
            }
        }

        private void UpdateVisibility()
        {
            if (ScrollView.childCount == 0) return;

            // Check if elements before first index are now visible
            bool firstIndexChanged = false;
            for (int i = Mathf.Max(0, firstVisibleIndex - 1); i >= 0; i--)
            {
                VisualElement element = ScrollView.contentContainer[i];
                if (!IsElementVisible(element)) break;

                element.visible = true;
                firstVisibleIndex = i;
                firstIndexChanged = true;
            }

            // Check if elements after last index are now visible
            bool lastIndexChanged = false;
            for (int i = Mathf.Min(lastVisibleIndex + 1, ScrollView.childCount); i < ScrollView.childCount; i++)
            {
                VisualElement element = ScrollView.contentContainer[i];
                if (!IsElementVisible(element)) break;

                element.visible = true;
                lastVisibleIndex = i;
                lastIndexChanged = true;
            }

            // Update elements visibility for elements that were already visible before scrolling
            if (firstIndexChanged)
            {
                for (int i = lastVisibleIndex; i >= firstVisibleIndex; i--)
                {
                    VisualElement element = ScrollView.contentContainer[i];
                    if (IsElementVisible(element))
                    {
                        lastVisibleIndex = i;
                        break;
                    }

                    element.visible = false;
                }
            }

            if (lastIndexChanged)
            {
                for (int i = firstVisibleIndex; i <= lastVisibleIndex; i++)
                {
                    VisualElement element = ScrollView.contentContainer[i];
                    if (IsElementVisible(element))
                    {
                        firstVisibleIndex = i;
                        break;
                    }

                    element.visible = false;
                }
            }
        }

        private bool IsElementVisible(VisualElement element)
        {
            Rect position = element.worldBound;
            return position.yMax >= 0 - cullingPadding && position.yMin < Screen.height + cullingPadding;
        }
    }
}
