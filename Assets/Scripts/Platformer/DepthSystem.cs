using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class DepthSystem : MonoBehaviour
{

    // Grundlegende Tiefeneinstellungen
    public int currentDepth = 1;
    public int maxDepth = 4;

    // Referenzen auf wichtige Komponenten
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    // Einstellungen für visuelle Effekte
    [Header("Visual Settings")]
    public float maxAlphaReduction = 0.5f;
    public float depthScaleFactor = 0.9f;

    // Einstellungen für Tiefenwechsel
    [Header("Depth Change Settings")]
    public float depthChangeSpeed = 0.5f;
    public AnimationCurve depthChangeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // Referenz auf die Tilemaps für jede Tiefenebene
    public Tilemap[] depthTilemaps;

    // Start wird aufgerufen, bevor das erste Frame aktualisiert wird
    void Start()
    {
        // Holen Sie sich die erforderlichen Komponenten
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // Initialisieren Sie die visuellen Aspekte basierend auf der Starttiefe
        UpdateVisuals();

        // Stellen Sie sicher, dass die Tilemaps korrekt eingerichtet sind
        SetupTilemaps();
    }

    // Diese Methode richtet die Tilemaps für jede Tiefenebene ein
    void SetupTilemaps()
    {
        if (depthTilemaps == null || depthTilemaps.Length != maxDepth)
        {
            Debug.LogWarning("Tilemaps sind nicht korrekt eingerichtet. Bitte weisen Sie jeder Tiefenebene eine Tilemap zu.");
            return;
        }

        for (int i = 0; i < depthTilemaps.Length; i++)
        {
            if (depthTilemaps[i] != null)
            {
                // Setzen Sie die Sortierreihenfolge basierend auf der Tiefe
                depthTilemaps[i].GetComponent<TilemapRenderer>().sortingOrder = i;

                // Passen Sie die Transparenz an
                Color tileColor = depthTilemaps[i].color;
                tileColor.a = 1f - (float)i / maxDepth * maxAlphaReduction;
                depthTilemaps[i].color = tileColor;
            }
        }
    }

    // Diese Methode wird aufgerufen, um die Tiefe zu ändern
    public void ChangeDepth(int newDepth)
    {
        // Überprüfen Sie, ob die neue Tiefe gültig ist
        if (newDepth < 1 || newDepth > maxDepth)
        {
            Debug.LogWarning("Versuchte Tiefenänderung außerhalb des gültigen Bereichs.");
            return;
        }

        // Starten Sie die Tiefenänderung
        StartCoroutine(SmoothDepthChange(newDepth));
    }

    // Diese Coroutine führt einen sanften Übergang zwischen Tiefen durch
    private IEnumerator SmoothDepthChange(int targetDepth)
    {
        float startTime = Time.time;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition;
        targetPosition.z = targetDepth * -0.1f; // Leichte Z-Verschiebung für visuelle Trennung

        while (Time.time - startTime < depthChangeSpeed)
        {
            float t = (Time.time - startTime) / depthChangeSpeed;
            float curveT = depthChangeCurve.Evaluate(t);

            // Interpolieren Sie die Position
            transform.position = Vector3.Lerp(startPosition, targetPosition, curveT);

            // Interpolieren Sie visuelle Aspekte
            UpdateVisualsLerp(currentDepth, targetDepth, curveT);

            yield return null;
        }

        // Setzen Sie die endgültige Tiefe und aktualisieren Sie die Visuals
        currentDepth = targetDepth;
        transform.position = targetPosition;
        UpdateVisuals();

        // Aktivieren Sie Kollisionen für die aktuelle Tiefe und deaktivieren Sie sie für andere
        UpdateCollisions();
    }

    // Diese Methode aktualisiert die Kollisionen basierend auf der aktuellen Tiefe
    void UpdateCollisions()
    {
        for (int i = 0; i < depthTilemaps.Length; i++)
        {
            if (depthTilemaps[i] != null)
            {
                depthTilemaps[i].GetComponent<TilemapCollider2D>().enabled = (i == currentDepth - 1);
            }
        }
    }

    // Diese Methode aktualisiert die visuellen Aspekte des Spielers basierend auf der aktuellen Tiefe
    void UpdateVisuals()
    {
        if (spriteRenderer != null)
        {
            // Aktualisieren Sie die Sortierreihenfolge
            spriteRenderer.sortingOrder = maxDepth - currentDepth + 1;

            // Aktualisieren Sie die Transparenz
            float alpha = 1f - (float)(currentDepth - 1) / maxDepth * maxAlphaReduction;
            spriteRenderer.color = new Color(1f, 1f, 1f, alpha);

            // Aktualisieren Sie die Skalierung
            float scaleFactor = Mathf.Pow(depthScaleFactor, currentDepth - 1);
            transform.localScale = Vector3.one * scaleFactor;
        }
    }

    // Diese Methode interpoliert die visuellen Aspekte während eines Tiefenwechsels
    void UpdateVisualsLerp(int fromDepth, int toDepth, float t)
    {
        if (spriteRenderer != null)
        {
            // Interpolieren Sie die Sortierreihenfolge
            int fromOrder = maxDepth - fromDepth + 1;
            int toOrder = maxDepth - toDepth + 1;
            spriteRenderer.sortingOrder = Mathf.RoundToInt(Mathf.Lerp(fromOrder, toOrder, t));

            // Interpolieren Sie die Transparenz
            float fromAlpha = 1f - (float)(fromDepth - 1) / maxDepth * maxAlphaReduction;
            float toAlpha = 1f - (float)(toDepth - 1) / maxDepth * maxAlphaReduction;
            float alpha = Mathf.Lerp(fromAlpha, toAlpha, t);
            spriteRenderer.color = new Color(1f, 1f, 1f, alpha);

            // Interpolieren Sie die Skalierung
            float fromScale = Mathf.Pow(depthScaleFactor, fromDepth - 1);
            float toScale = Mathf.Pow(depthScaleFactor, toDepth - 1);
            float scale = Mathf.Lerp(fromScale, toScale, t);
            transform.localScale = Vector3.one * scale;
        }
    }

    // Diese Methode prüft, ob ein Tiefenwechsel in eine bestimmte Richtung möglich ist
    public bool CanChangeDepthInDirection(Vector2 direction)
    {
        // Überprüfen Sie, ob es eine Tilemap für die nächste Tiefe gibt
        int nextDepth = direction.y > 0 ? currentDepth + 1 : currentDepth - 1;
        if (nextDepth < 1 || nextDepth > maxDepth || depthTilemaps[nextDepth - 1] == null)
        {
            return false;
        }

        // Überprüfen Sie, ob es an der Zielposition einen Boden gibt
        Vector3Int targetCell = depthTilemaps[nextDepth - 1].WorldToCell(transform.position + (Vector3)direction);
        return depthTilemaps[nextDepth - 1].HasTile(targetCell);
    }

    // Diese Methode führt einen Tiefenwechsel in eine bestimmte Richtung durch, wenn möglich
    public bool TryChangeDepthInDirection(Vector2 direction)
    {
        if (CanChangeDepthInDirection(direction))
        {
            ChangeDepth(direction.y > 0 ? currentDepth + 1 : currentDepth - 1);
            return true;
        }
        return false;
    }
}

