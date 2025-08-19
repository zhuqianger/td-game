using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TDGame.Role
{
    /// <summary>
    /// 干员UI类
    /// </summary>
    public class OperatorUI : MonoBehaviour
    {
        [Header("基础信息UI")]
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI professionText;
        public Image rarityImage;
        public Image portraitImage;
        
        [Header("属性UI")]
        public Slider healthSlider;
        public TextMeshProUGUI healthText;
        public TextMeshProUGUI attackText;
        public TextMeshProUGUI defenseText;
        public TextMeshProUGUI magicResistanceText;
        public TextMeshProUGUI deployCostText;
        public TextMeshProUGUI blockCountText;
        public TextMeshProUGUI attackIntervalText;
        
        [Header("状态UI")]
        public GameObject deployedIndicator;
        public GameObject aliveIndicator;
        public GameObject deadIndicator;
        public Image cooldownOverlay;
        
        [Header("技能UI")]
        public Transform skillContainer;
        public GameObject skillSlotPrefab;
        
        [Header("天赋UI")]
        public Transform talentContainer;
        public GameObject talentSlotPrefab;
        
        [Header("颜色设置")]
        public Color[] rarityColors = new Color[6];
        public Color healthColor = Color.green;
        public Color lowHealthColor = Color.red;
        
        private Operator targetOperator;
        private OperatorSkillSlot[] skillSlots;
        private OperatorTalentSlot[] talentSlots;
        
        /// <summary>
        /// 设置目标干员
        /// </summary>
        public void SetTargetOperator(Operator op)
        {
            targetOperator = op;
            UpdateUI();
        }
        
        /// <summary>
        /// 更新UI显示
        /// </summary>
        public void UpdateUI()
        {
            if (targetOperator == null) return;
            
            UpdateBasicInfo();
            UpdateStats();
            UpdateStatus();
            UpdateSkills();
            UpdateTalents();
        }
        
        /// <summary>
        /// 更新基础信息
        /// </summary>
        private void UpdateBasicInfo()
        {
            if (nameText != null)
                nameText.text = targetOperator.operatorName;
                
            if (levelText != null)
                levelText.text = $"Lv.{targetOperator.level}";
                
            if (professionText != null)
                professionText.text = GetProfessionName(targetOperator.profession);
                
            if (rarityImage != null && targetOperator.rarity > 0 && targetOperator.rarity <= rarityColors.Length)
                rarityImage.color = rarityColors[targetOperator.rarity - 1];
        }
        
        /// <summary>
        /// 更新属性显示
        /// </summary>
        private void UpdateStats()
        {
            // 血量
            if (healthSlider != null)
            {
                healthSlider.value = targetOperator.GetHealthPercentage();
                healthSlider.fillRect.GetComponent<Image>().color = 
                    targetOperator.GetHealthPercentage() > 0.3f ? healthColor : lowHealthColor;
            }
            
            if (healthText != null)
                healthText.text = $"{targetOperator.currentStats.currentHP}/{targetOperator.currentStats.maxHP}";
                
            // 攻击力
            if (attackText != null)
                attackText.text = $"攻击: {targetOperator.currentStats.attack}";
                
            // 防御力
            if (defenseText != null)
                defenseText.text = $"防御: {targetOperator.currentStats.defense}";
                
            // 法术抗性
            if (magicResistanceText != null)
                magicResistanceText.text = $"法抗: {targetOperator.currentStats.magicResistance}%";
                
            // 部署费用
            if (deployCostText != null)
                deployCostText.text = $"费用: {targetOperator.currentStats.deployCost}";
                
            // 阻挡数
            if (blockCountText != null)
                blockCountText.text = $"阻挡: {targetOperator.currentStats.blockCount}";
                
            // 攻击间隔
            if (attackIntervalText != null)
                attackIntervalText.text = $"攻速: {targetOperator.currentStats.attackInterval:F1}s";
        }
        
        /// <summary>
        /// 更新状态显示
        /// </summary>
        private void UpdateStatus()
        {
            if (deployedIndicator != null)
                deployedIndicator.SetActive(targetOperator.isDeployed);
                
            if (aliveIndicator != null)
                aliveIndicator.SetActive(targetOperator.isAlive);
                
            if (deadIndicator != null)
                deadIndicator.SetActive(!targetOperator.isAlive);
        }
        
        /// <summary>
        /// 更新技能显示
        /// </summary>
        private void UpdateSkills()
        {
            if (skillContainer == null) return;
            
            // 清理现有技能槽
            foreach (Transform child in skillContainer)
            {
                Destroy(child.gameObject);
            }
            
            // 创建技能槽
            skillSlots = new OperatorSkillSlot[targetOperator.skills.Count];
            
            for (int i = 0; i < targetOperator.skills.Count; i++)
            {
                GameObject slotObj = Instantiate(skillSlotPrefab, skillContainer);
                OperatorSkillSlot slot = slotObj.GetComponent<OperatorSkillSlot>();
                
                if (slot != null)
                {
                    slot.SetSkill(targetOperator.skills[i]);
                    skillSlots[i] = slot;
                }
            }
        }
        
        /// <summary>
        /// 更新天赋显示
        /// </summary>
        private void UpdateTalents()
        {
            if (talentContainer == null) return;
            
            // 清理现有天赋槽
            foreach (Transform child in talentContainer)
            {
                Destroy(child.gameObject);
            }
            
            // 创建天赋槽
            talentSlots = new OperatorTalentSlot[targetOperator.talents.Count];
            
            for (int i = 0; i < targetOperator.talents.Count; i++)
            {
                GameObject slotObj = Instantiate(talentSlotPrefab, talentContainer);
                OperatorTalentSlot slot = slotObj.GetComponent<OperatorTalentSlot>();
                
                if (slot != null)
                {
                    slot.SetTalent(targetOperator.talents[i]);
                    talentSlots[i] = slot;
                }
            }
        }
        
        /// <summary>
        /// 获取职业名称
        /// </summary>
        private string GetProfessionName(OperatorProfession profession)
        {
            switch (profession)
            {
                case OperatorProfession.Vanguard: return "先锋";
                case OperatorProfession.Guard: return "近卫";
                case OperatorProfession.Defender: return "重装";
                case OperatorProfession.Sniper: return "狙击";
                case OperatorProfession.Caster: return "术师";
                case OperatorProfession.Medic: return "医疗";
                case OperatorProfession.Supporter: return "辅助";
                case OperatorProfession.Specialist: return "特种";
                default: return "未知";
            }
        }
        
        private void Update()
        {
            // 实时更新UI
            if (targetOperator != null)
            {
                UpdateStats();
                
                // 更新技能冷却
                if (skillSlots != null)
                {
                    for (int i = 0; i < skillSlots.Length; i++)
                    {
                        if (skillSlots[i] != null)
                        {
                            skillSlots[i].UpdateCooldown();
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 干员技能槽
    /// </summary>
    public class OperatorSkillSlot : MonoBehaviour
    {
        [Header("技能UI")]
        public Image skillIcon;
        public TextMeshProUGUI skillNameText;
        public TextMeshProUGUI cooldownText;
        public Image cooldownOverlay;
        public Button skillButton;
        
        private Skill skill;
        
        /// <summary>
        /// 设置技能
        /// </summary>
        public void SetSkill(Skill targetSkill)
        {
            skill = targetSkill;
            
            if (skillNameText != null)
                skillNameText.text = skill.skillName;
                
            if (skillButton != null)
                skillButton.onClick.AddListener(OnSkillButtonClick);
        }
        
        /// <summary>
        /// 更新冷却显示
        /// </summary>
        public void UpdateCooldown()
        {
            if (skill == null) return;
            
            float cooldownProgress = skill.GetCooldownProgress();
            float spProgress = skill.GetSpProgress();
            
            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = 1f - cooldownProgress;
                
            if (cooldownText != null)
            {
                if (skill.skillType == SkillType.Passive)
                {
                    cooldownText.text = "被动";
                }
                else if (skill.CanUse())
                {
                    cooldownText.text = "可用";
                }
                else
                {
                    cooldownText.text = $"技力: {skill.GetSpProgress():P0}";
                }
            }
        }
        
        /// <summary>
        /// 技能按钮点击事件
        /// </summary>
        private void OnSkillButtonClick()
        {
            if (skill != null && skill.skillType == SkillType.Active)
            {
                // 这里应该触发技能使用
                Debug.Log($"尝试使用技能: {skill.skillName}");
            }
        }
    }
    
    /// <summary>
    /// 干员天赋槽
    /// </summary>
    public class OperatorTalentSlot : MonoBehaviour
    {
        [Header("天赋UI")]
        public Image talentIcon;
        public TextMeshProUGUI talentNameText;
        public TextMeshProUGUI statusText;
        public Button infoButton;
        
        private Talent talent;
        
        /// <summary>
        /// 设置天赋
        /// </summary>
        public void SetTalent(Talent targetTalent)
        {
            talent = targetTalent;
            
            if (talentNameText != null)
                talentNameText.text = talent.talentName;
                
            if (infoButton != null)
                infoButton.onClick.AddListener(OnInfoButtonClick);
        }
        
        /// <summary>
        /// 信息按钮点击事件
        /// </summary>
        private void OnInfoButtonClick()
        {
            if (talent != null)
            {
                // 显示天赋详细信息
                Debug.Log($"天赋信息: {talent.GetFullDescription()}");
            }
        }
    }
} 