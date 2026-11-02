using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Корзина игрока. По тапу плавно едет к точке касания с постоянной скоростью
/// и ловит точку, когда расстояние между центрами меньше catchRadius.
/// </summary>
public class Basket : MonoBehaviour
{
    // Скорость движения корзины (мировых единиц в секунду).
    [SerializeField] private float moveSpeed = 12f;
    // Радиус поимки: расстояние между центрами корзины и точки.
    [SerializeField] private float catchRadius = 0.8f;
    [SerializeField] private Dot dot;

    private Vector2 targetPosition;
    // Защита от повторного засчитывания одной и той же поимки.
    private bool isCaught = false;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        targetPosition = transform.position;
    }

    private void Update()
    {
        // После Game Over корзина не реагирует на тапы и не двигается.
        if (!GameManager.Instance.IsRunning) return;

        // В проекте включён новый Input System, поэтому вместо Input.GetMouseButtonDown
        // используется Pointer.current: в редакторе это мышь, на телефоне — тачскрин.
        Pointer pointer = Pointer.current;
        if (pointer != null && pointer.press.wasPressedThisFrame)
        {
            // Экранные координаты (пиксели) переводятся в мировые координаты сцены.
            Vector3 worldPoint = cam.ScreenToWorldPoint(pointer.position.ReadValue());
            SetTarget(worldPoint);
        }

        // MoveTowards даёт постоянную скорость без «затухания», как у Lerp.
        transform.position = Vector2.MoveTowards(
            transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Попадание определяется без физики — сравнением расстояния.
        if (!isCaught &&
            Vector2.Distance(transform.position, dot.transform.position) < catchRadius)
        {
            isCaught = true;
            GameManager.Instance.AddScore(1);
            dot.MoveToRandomPosition();
            Invoke(nameof(ResetCatch), 0.2f);
        }
    }

    // Задаёт точку, к которой поедет корзина (в мировых координатах).
    public void SetTarget(Vector2 worldPosition)
    {
        targetPosition = worldPosition;
    }

    private void ResetCatch()
    {
        isCaught = false;
    }
}
