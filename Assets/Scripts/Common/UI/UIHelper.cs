using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Common.UI
{
    /// <summary>
    /// UI工具类
    /// </summary>
    public static class UIHelper
    {
        /// <summary>
        /// 设置Canvas适配
        /// </summary>
        /// <param name="canvas">Canvas组件</param>
        /// <param name="referenceWidth">参考宽度</param>
        /// <param name="referenceHeight">参考高度</param>
        /// <param name="matchMode">适配模式</param>
        public static void SetupCanvasScaler(Canvas canvas, float referenceWidth = 1920f, float referenceHeight = 1080f, CanvasScaler.ScreenMatchMode matchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight)
        {
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(referenceWidth, referenceHeight);
            scaler.screenMatchMode = matchMode;
            scaler.matchWidthOrHeight = 0.5f;
        }

        /// <summary>
        /// 设置安全区域适配
        /// </summary>
        /// <param name="rectTransform">RectTransform组件</param>
        public static void SetupSafeArea(RectTransform rectTransform)
        {
            Rect safeArea = Screen.safeArea;
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
            
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }

        /// <summary>
        /// 设置按钮点击效果
        /// </summary>
        /// <param name="button">按钮组件</param>
        /// <param name="scaleOnClick">点击时缩放</param>
        /// <param name="scaleValue">缩放值</param>
        public static void SetupButtonEffect(Button button, bool scaleOnClick = true, float scaleValue = 0.95f)
        {
            if (button == null) return;

            // 添加点击音效
            button.onClick.AddListener(() => {
                // 这里可以播放音效
                // AudioManager.Instance.PlaySound("button_click");
            });

            if (scaleOnClick)
            {
                // 添加缩放效果
                button.onClick.AddListener(() => {
                    LeanTween.scale(button.gameObject, Vector3.one * scaleValue, 0.1f)
                        .setEaseOutQuad()
                        .setOnComplete(() => {
                            LeanTween.scale(button.gameObject, Vector3.one, 0.1f)
                                .setEaseOutQuad();
                        });
                });
            }
        }

        /// <summary>
        /// 设置图片自适应
        /// </summary>
        /// <param name="image">图片组件</param>
        /// <param name="sprite">精灵</param>
        /// <param name="preserveAspect">保持宽高比</param>
        public static void SetupImage(Image image, Sprite sprite, bool preserveAspect = true)
        {
            if (image == null) return;

            image.sprite = sprite;
            image.preserveAspect = preserveAspect;
            
            if (preserveAspect)
            {
                image.type = Image.Type.Simple;
            }
        }

        /// <summary>
        /// 设置文本自适应
        /// </summary>
        /// <param name="text">文本组件</param>
        /// <param name="content">内容</param>
        /// <param name="bestFit">最佳适配</param>
        /// <param name="minSize">最小字体大小</param>
        /// <param name="maxSize">最大字体大小</param>
        public static void SetupText(Text text, string content, bool bestFit = true, int minSize = 12, int maxSize = 48)
        {
            if (text == null) return;

            text.text = content;
            text.bestFit = bestFit;
            text.minSize = minSize;
            text.maxSize = maxSize;
        }

        /// <summary>
        /// 设置滚动视图
        /// </summary>
        /// <param name="scrollRect">滚动视图组件</param>
        /// <param name="horizontal">是否支持水平滚动</param>
        /// <param name="vertical">是否支持垂直滚动</param>
        /// <param name="movementType">移动类型</param>
        public static void SetupScrollRect(ScrollRect scrollRect, bool horizontal = false, bool vertical = true, ScrollRect.MovementType movementType = ScrollRect.MovementType.Elastic)
        {
            if (scrollRect == null) return;

            scrollRect.horizontal = horizontal;
            scrollRect.vertical = vertical;
            scrollRect.movementType = movementType;
            scrollRect.inertia = true;
            scrollRect.decelerationRate = 0.135f;
        }

        /// <summary>
        /// 设置输入框
        /// </summary>
        /// <param name="inputField">输入框组件</param>
        /// <param name="placeholder">占位符文本</param>
        /// <param name="characterLimit">字符限制</param>
        /// <param name="contentType">内容类型</param>
        public static void SetupInputField(InputField inputField, string placeholder = "", int characterLimit = 0, InputField.ContentType contentType = InputField.ContentType.Standard)
        {
            if (inputField == null) return;

            if (!string.IsNullOrEmpty(placeholder) && inputField.placeholder != null)
            {
                Text placeholderText = inputField.placeholder as Text;
                if (placeholderText != null)
                {
                    placeholderText.text = placeholder;
                }
            }

            inputField.characterLimit = characterLimit;
            inputField.contentType = contentType;
        }

        /// <summary>
        /// 设置Toggle组件
        /// </summary>
        /// <param name="toggle">Toggle组件</param>
        /// <param name="isOn">是否选中</param>
        /// <param name="onValueChanged">值改变回调</param>
        public static void SetupToggle(Toggle toggle, bool isOn = false, System.Action<bool> onValueChanged = null)
        {
            if (toggle == null) return;

            toggle.isOn = isOn;
            if (onValueChanged != null)
            {
                toggle.onValueChanged.AddListener(onValueChanged);
            }
        }

        /// <summary>
        /// 设置Slider组件
        /// </summary>
        /// <param name="slider">Slider组件</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="value">当前值</param>
        /// <param name="onValueChanged">值改变回调</param>
        public static void SetupSlider(Slider slider, float minValue = 0f, float maxValue = 1f, float value = 0f, System.Action<float> onValueChanged = null)
        {
            if (slider == null) return;

            slider.minValue = minValue;
            slider.maxValue = maxValue;
            slider.value = value;
            
            if (onValueChanged != null)
            {
                slider.onValueChanged.AddListener(onValueChanged);
            }
        }

        /// <summary>
        /// 设置Dropdown组件
        /// </summary>
        /// <param name="dropdown">Dropdown组件</param>
        /// <param name="options">选项列表</param>
        /// <param name="value">当前选中值</param>
        /// <param name="onValueChanged">值改变回调</param>
        public static void SetupDropdown(Dropdown dropdown, List<string> options, int value = 0, System.Action<int> onValueChanged = null)
        {
            if (dropdown == null) return;

            dropdown.ClearOptions();
            dropdown.AddOptions(options);
            dropdown.value = value;
            
            if (onValueChanged != null)
            {
                dropdown.onValueChanged.AddListener(onValueChanged);
            }
        }

        /// <summary>
        /// 设置对象层级
        /// </summary>
        /// <param name="gameObject">游戏对象</param>
        /// <param name="layer">层级</param>
        public static void SetLayer(GameObject gameObject, UILayer layer)
        {
            if (gameObject == null) return;

            Canvas canvas = gameObject.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = (int)layer * 100;
            }
        }

        /// <summary>
        /// 设置对象透明度
        /// </summary>
        /// <param name="gameObject">游戏对象</param>
        /// <param name="alpha">透明度值</param>
        public static void SetAlpha(GameObject gameObject, float alpha)
        {
            if (gameObject == null) return;

            CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = Mathf.Clamp01(alpha);
        }

        /// <summary>
        /// 设置对象交互性
        /// </summary>
        /// <param name="gameObject">游戏对象</param>
        /// <param name="interactable">是否可交互</param>
        public static void SetInteractable(GameObject gameObject, bool interactable)
        {
            if (gameObject == null) return;

            CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            canvasGroup.interactable = interactable;
            canvasGroup.blocksRaycasts = interactable;
        }

        /// <summary>
        /// 获取屏幕坐标转UI坐标
        /// </summary>
        /// <param name="screenPosition">屏幕坐标</param>
        /// <param name="canvas">Canvas组件</param>
        /// <param name="camera">摄像机</param>
        /// <returns>UI坐标</returns>
        public static Vector2 ScreenToUIPosition(Vector3 screenPosition, Canvas canvas, Camera camera = null)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return screenPosition;
            }

            if (camera == null)
            {
                camera = Camera.main;
            }

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPosition,
                canvas.worldCamera,
                out localPoint);

            return localPoint;
        }

        /// <summary>
        /// 获取UI坐标转屏幕坐标
        /// </summary>
        /// <param name="uiPosition">UI坐标</param>
        /// <param name="canvas">Canvas组件</param>
        /// <param name="camera">摄像机</param>
        /// <returns>屏幕坐标</returns>
        public static Vector2 UIToScreenPosition(Vector2 uiPosition, Canvas canvas, Camera camera = null)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return uiPosition;
            }

            if (camera == null)
            {
                camera = Camera.main;
            }

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(
                camera,
                canvas.transform.TransformPoint(uiPosition));

            return screenPoint;
        }
    }
} 