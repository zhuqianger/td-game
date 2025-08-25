using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    public class UIImage : Image
    {
        [SerializeField] private bool _visibleOnAwake = true;
        
        protected override void Awake()
        {
            base.Awake();
            SetVisible(_visibleOnAwake);
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

        public void SetSprite(Sprite sprite)
        {
            this.sprite = sprite;
        }

        public void SetColor(Color color)
        {
            this.color = color;
        }
    }
}