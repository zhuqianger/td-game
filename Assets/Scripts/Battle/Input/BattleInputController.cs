using UnityEngine;
using Battle.Core;
using UnityEngine.Tilemaps;

namespace Battle.Input
{
    public class BattleInputController : MonoBehaviour
    {
        // 组件引用
        [SerializeField] private Tilemap groundTilemap;
        [SerializeField] private Tilemap unitTilemap;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask unitLayer;

        // 输入状态
        private BattleUnit selectedUnit;
        private Vector2Int? targetPosition;
        private bool isDragging = false;
        private Vector3 dragStartPosition;

        private void Update()
        {
            HandleMouseInput();
        }

        // 处理鼠标输入
        private void HandleMouseInput()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                // HandleMouseDown();
            }
            else if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                HandleMouseUp();
            }
            else if (UnityEngine.Input.GetMouseButton(0))
            {
                HandleMouseDrag();
            }

            // 处理右键点击
            if (UnityEngine.Input.GetMouseButtonDown(1))
            {
                HandleRightClick();
            }
        }

        // 处理鼠标按下
        // private void HandleMouseDown()
        // {
        //     // 获取鼠标点击的世界坐标
        //     Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        //     mouseWorldPos.z = 0;
        //
        //     // 将世界坐标转换为Tilemap的单元格坐标
        //     Vector3Int cellPosition = groundTilemap.WorldToCell(mouseWorldPos);
        //     Vector2Int gridPosition = new Vector2Int(cellPosition.x, cellPosition.y);
        //
        //     // 检查是否点击到单位
        //     Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, unitLayer);
        //     
        //     if (hit.collider != null)
        //     {
        //         var unit = hit.collider.GetComponent<BattleUnit>();
        //         if (unit != null)
        //         {
        //             SelectUnit(unit);
        //             return;
        //         }
        //     }
        //
        //     // 检查是否点击到地面
        //     if (groundTilemap.HasTile(cellPosition))
        //     {
        //         if (selectedUnit != null)
        //         {
        //             targetPosition = gridPosition;
        //             BattleCoreManager.Instance.ExecuteUnitMove(selectedUnit.UnitId, gridPosition);
        //         }
        //     }
        //
        //     // 开始拖动
        //     isDragging = true;
        //     dragStartPosition = UnityEngine.Input.mousePosition;
        // }

        // 处理鼠标抬起
        private void HandleMouseUp()
        {
            isDragging = false;
            targetPosition = null;
        }

        // 处理鼠标拖动
        private void HandleMouseDrag()
        {
            if (!isDragging) return;

            // 计算拖动距离
            Vector3 dragDelta = UnityEngine.Input.mousePosition - dragStartPosition;
            if (dragDelta.magnitude > 10f) // 拖动阈值
            {
                // 旋转相机
                float rotationSpeed = 0.5f;
                mainCamera.transform.RotateAround(Vector3.zero, Vector3.up, dragDelta.x * rotationSpeed);
            }
        }

        // 处理右键点击
        private void HandleRightClick()
        {
            Ray ray = mainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            RaycastHit hit;

            // 检查是否点击到单位
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, unitLayer))
            {
                var targetUnit = hit.collider.GetComponent<BattleUnit>();
                if (targetUnit != null && selectedUnit != null)
                {
                    // 执行攻击
                    BattleCoreManager.Instance.ExecuteUnitAttack(selectedUnit.UnitId, targetUnit.UnitId);
                }
            }
        }

        // 选择单位
        private void SelectUnit(BattleUnit unit)
        {
            // 取消之前的选择
            if (selectedUnit != null)
            {
                // TODO: 取消选中效果
            }

            selectedUnit = unit;

            // 显示选中效果
            // TODO: 实现选中效果
        }

        // 取消选择
        private void DeselectUnit()
        {
            if (selectedUnit != null)
            {
                // TODO: 取消选中效果
                selectedUnit = null;
            }
        }

        // 检查位置是否可移动
        private bool IsPositionWalkable(Vector2Int position)
        {
            // TODO: 实现位置检查逻辑
            return true;
        }

        // 检查单位是否可攻击
        private bool IsUnitAttackable(BattleUnit attacker, BattleUnit target)
        {
            // TODO: 实现攻击检查逻辑
            return true;
        }
    }
} 