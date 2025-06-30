using UnityEngine;
using UnityEngine.UI;
using Battle.Models;
using Battle.Core;

namespace Battle.UI
{
    public class BattleUIManager : MonoBehaviour
    {
        // UI组件引用
        [Header("回合信息")]
        [SerializeField] private Text turnNumberText;
        [SerializeField] private Text currentPlayerText;
        
        [Header("单位信息")]
        [SerializeField] private GameObject unitInfoPanel;
        [SerializeField] private Text unitNameText;
        [SerializeField] private Text unitHpText;
        [SerializeField] private Text unitMpText;
        [SerializeField] private Image unitHpBar;
        [SerializeField] private Image unitMpBar;
        [SerializeField] private Button unitAttackButton;
        
        [Header("操作按钮")]
        [SerializeField] private Button endTurnButton;
        
        private void Start()
        {
            InitializeUI();
            RegisterEvents();
        }

        // 初始化UI
        private void InitializeUI()
        {
            // 初始化按钮事件
            endTurnButton.onClick.AddListener(OnEndTurnClicked);
            
            // 隐藏面板
            unitInfoPanel.SetActive(false);
        }

        // 注册事件
        private void RegisterEvents()
        {
            // 注册到BattleCoreManager
            BattleCoreManager.Instance.RegisterUIEvents(this);
        }

        // 更新回合信息
        public void UpdateTurnInfo(GameMatchData matchData)
        {
            turnNumberText.text = $"回合: {matchData.CurrentTurn}";
            currentPlayerText.text = $"当前玩家: {matchData.CurrentPlayerId}";
        }

        // 更新单位信息
        public void UpdateUnitInfo(BattleUnit unit)
        {
            if (unit == null)
            {
                unitInfoPanel.SetActive(false);
                return;
            }

            unitInfoPanel.SetActive(true);
            unitNameText.text = $"单位 {unit.UnitId}";
            unitHpText.text = $"HP: {unit.Hp}/{unit.MaxHp}";
            unitMpText.text = $"MP: {unit.Mp}/{unit.MaxMp}";
            
            unitHpBar.fillAmount = (float)unit.Hp / unit.MaxHp;
            unitMpBar.fillAmount = (float)unit.Mp / unit.MaxMp;
        }

        // 事件处理
        public void OnUnitSelected(BattleUnit unit)
        {
            UpdateUnitInfo(unit);
        }

        public void OnUnitDeselected()
        {
            unitInfoPanel.SetActive(false);
        }

        public void OnTurnStarted(GameMatchData matchData)
        {
            UpdateTurnInfo(matchData);
        }

        public void OnGameOver(long winnerId)
        {
            // TODO: 显示游戏结束UI
        }

        // 按钮事件处理
        private void OnEndTurnClicked()
        {
            BattleCoreManager.Instance.EndCurrentTurn();
        }

        private void OnDestroy()
        {
            BattleCoreManager.Instance.UnregisterUIEvents(this);
        }
    }
} 