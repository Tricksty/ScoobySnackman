using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.Windows;

public class StrictGridMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private Tilemap wallTilemap; // Перетащите сюда Tilemap со стенами

    private Vector2 currentDir = Vector2.zero;
    private Vector2 desiredDir = Vector2.zero;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        // Оставляем строго одно направление (без диагоналей)
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            if (Mathf.Abs(input.x) > 0.1f)
                desiredDir = new Vector2(Mathf.Sign(input.x), 0f);
        }
        else if (Mathf.Abs(input.y) > 0.1f)
        {
            desiredDir = new Vector2(0f, Mathf.Sign(input.y));
        }
        Debug.Log($"Input received: {input}");
    }

    private void Update()
    {
        if (currentDir.x < 0)
            spriteRenderer.flipX = true;
        else if (currentDir.x > 0)
            spriteRenderer.flipX = false;
    }

    private void FixedUpdate()
    {
        if (wallTilemap == null) return;

        Vector3Int currentCell = wallTilemap.WorldToCell(rb.position);
        Vector2 cellCenter = wallTilemap.GetCellCenterWorld(currentCell);

        // Порог прохождения центра клетки за один физический шаг
        float step = moveSpeed * Time.fixedDeltaTime;
        float snapThreshold = Mathf.Max(step * 0.75f, 0.01f);

        bool nearCenter = Vector2.Distance(rb.position, cellCenter) <= snapThreshold;

        // В строгом режиме поворот и смена направления возможны только в центре клетки
        if (nearCenter || currentDir == Vector2.zero)
        {
            // Сначала пробуем повернуть в желаемое направление, если там нет стены
            if (desiredDir != Vector2.zero && CanMoveInDirection(currentCell, desiredDir))
            {
                rb.position = cellCenter;
                currentDir = desiredDir;
            }
            // Иначе, если текущее направление заблокировано стеной — встаём в центр и останавливаемся
            else if (currentDir != Vector2.zero && !CanMoveInDirection(currentCell, currentDir))
            {
                rb.position = cellCenter;
                currentDir = Vector2.zero;
                return;
            }
            else if (nearCenter && currentDir != Vector2.zero)
            {
                // Мягко держим ось коридора в центре клетки
                rb.position = cellCenter;
            }
        }

        if (currentDir != Vector2.zero)
        {
            rb.MovePosition(rb.position + currentDir * step);
        }
    }

    private bool CanMoveInDirection(Vector3Int fromCell, Vector2 dir)
    {
        Vector3Int offset = new Vector3Int(
            Mathf.RoundToInt(dir.x),
            Mathf.RoundToInt(dir.y),
            0
        );
        Debug.Log($"Wall in path: {wallTilemap.HasTile(fromCell + offset)}");
        return !wallTilemap.HasTile(fromCell + offset);
    }
}