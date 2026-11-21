using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class SpriteMover : MonoBehaviour
{
    // Атрибут [SerializeField] позволяет менять скорость прямо в редакторе Unity,
    // оставляя саму переменную защищённой от других скриптов (инкапсуляция C#).
    [SerializeField] private float moveSpeed = 5f;

    // Вектор для хранения текущего направления движения (X и Y)
    private Vector2 currentDirection = Vector2.zero;
    private Vector2 nextDirection = Vector2.zero;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Этот метод вызывается автоматически компонентом Player Input при нажатии WASD или стрелок,
    // так как в настройках действий есть событие с именем "Move".
    private void OnMove(InputValue value)
    {
        // Считываем направление нажатия в виде двухмерного вектора
        Vector2 input = value.Get<Vector2>();

        if (input != Vector2.zero)
        {
            nextDirection = input;
        }

        if (input.x < 0) //влево
        {
            spriteRenderer.flipX = true;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (input.x > 0) //вправо
        {
            spriteRenderer.flipX = false;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (input.y < 0) //вниз
        {
            spriteRenderer.flipX = false;
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (input.y > 0) //вверх
        {
            spriteRenderer.flipX = false;
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }

    }

    // Метод Update выполняется каждый кадр
    private void Update()
    {
        if (nextDirection != Vector2.zero)
        {
            currentDirection = nextDirection;
        }

        // Двигаем персонажа через Rigidbody2D
        Vector2 movement = currentDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }
}
