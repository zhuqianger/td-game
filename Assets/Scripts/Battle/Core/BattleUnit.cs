using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    // 单位基础属性
    public long UnitId { get; private set; }
    public int TeamId { get; private set; }
    public Vector2Int Position { get; set; }
    
    // 单位状态
    public int Hp { get; private set; }
    public int MaxHp { get; private set; }
    public int Mp { get; private set; }
    public int MaxMp { get; private set; }
    
    // 移动相关
    public int MoveRange { get; private set; }
    public int AttackRange { get; private set; }
    
    // 状态标志
    public bool IsAlive { get; private set; }
    public bool CanMove { get; private set; }
    public bool CanAttack { get; private set; }

    // 初始化单位
    public void Initialize(long unitId, int teamId, Vector2Int position)
    {
        UnitId = unitId;
        TeamId = teamId;
        Position = position;
        
        // 设置默认属性
        MaxHp = 100;
        Hp = MaxHp;
        MaxMp = 50;
        Mp = MaxMp;
        MoveRange = 3;
        AttackRange = 1;
        IsAlive = true;
        CanMove = true;
        CanAttack = true;
    }

    // 设置单位属性
    public void SetStats(int maxHp, int maxMp, int moveRange, int attackRange)
    {
        MaxHp = maxHp;
        Hp = maxHp;
        MaxMp = maxMp;
        Mp = maxMp;
        MoveRange = moveRange;
        AttackRange = attackRange;
    }

    // 移动单位
    public void MoveTo(Vector2Int newPosition)
    {
        if (!CanMove) return;
        
        Position = newPosition;
        transform.position = new Vector3(newPosition.x, 0, newPosition.y);
        CanMove = false;
    }

    // 执行攻击
    public void Attack(BattleUnit target)
    {
        if (!CanAttack) return;
        
        // 计算伤害
        int damage = CalculateDamage(target);
        target.TakeDamage(damage);
        
        CanAttack = false;
    }

    // 受到伤害
    public void TakeDamage(int damage)
    {
        Hp = Mathf.Max(0, Hp - damage);
        if (Hp <= 0)
        {
            Die();
        }
    }

    // 单位死亡
    private void Die()
    {
        IsAlive = false;
        gameObject.SetActive(false);
    }

    // 计算伤害
    private int CalculateDamage(BattleUnit target)
    {
        // TODO: 实现伤害计算逻辑
        return 10;
    }

    // 重置回合状态
    public void ResetTurnState()
    {
        CanMove = true;
        CanAttack = true;
    }
}