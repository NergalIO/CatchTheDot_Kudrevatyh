using UnityEngine;

/// <summary>
/// Точка-цель. Стоит на месте, пока её не поймают, затем по вызову
/// MoveToRandomPosition() переносится в случайное место экрана.
/// </summary>
public class Dot : MonoBehaviour
{
    // Границы области появления в мировых координатах (задаются в инспекторе).
    [SerializeField] private float minX = -4f;
    [SerializeField] private float maxX = 4f;
    [SerializeField] private float minY = -7f;
    [SerializeField] private float maxY = 7f;

    // Отступ от краёв экрана и высота полосы сверху, занятой счётом и таймером.
    [SerializeField] private float edgeMargin = 0.3f;
    [SerializeField] private float topUiHeight = 1.6f;

    // Корзина и минимальное расстояние до неё: новая точка не должна
    // появляться прямо под корзиной, иначе очко засчитается без тапа.
    [SerializeField] private Transform basket;
    [SerializeField] private float minDistanceFromBasket = 3f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // В начале игры точка сразу ставится в случайное место.
    private void Start()
    {
        MoveToRandomPosition();
    }

    public void MoveToRandomPosition()
    {
        // Видимая область ортографической камеры: на узких экранах она уже,
        // чем minX..maxX из инспектора, поэтому границы дополнительно обрезаются.
        Camera cam = Camera.main;
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        float radius = spriteRenderer.bounds.extents.x;
        Vector2 camPos = cam.transform.position;

        float xMin = Mathf.Max(minX, camPos.x - halfWidth + radius + edgeMargin);
        float xMax = Mathf.Min(maxX, camPos.x + halfWidth - radius - edgeMargin);
        float yMin = Mathf.Max(minY, camPos.y - halfHeight + radius + edgeMargin);
        float yMax = Mathf.Min(maxY, camPos.y + halfHeight - radius - topUiHeight);

        // Random.Range(float, float) включает оба конца диапазона.
        // Несколько попыток найти место подальше от корзины.
        Vector2 position = Vector2.zero;
        for (int attempt = 0; attempt < 10; attempt++)
        {
            position = new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax));
            if (basket == null || Vector2.Distance(position, basket.position) >= minDistanceFromBasket)
                break;
        }

        transform.position = position;
    }
}
