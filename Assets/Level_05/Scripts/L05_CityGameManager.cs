
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class L05_CityGameManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("UI")]
    public TMP_Text objectiveText;
    public TMP_Text feedbackText;
    public TMP_Text directionText;

    [Header("Interaction Settings")]
    public float interactRange = 1.5f;

    private List<L05_CityTarget> allTargets =
        new List<L05_CityTarget>();

    private List<L05_CityTarget> questions =
        new List<L05_CityTarget>();

    private int currentIndex = 0;
    private const int TotalQuestions = 4;
    private bool levelFinished = false;

    void Start()
    {
        L05_CityTarget[] found =
            FindObjectsByType<L05_CityTarget>(
                FindObjectsSortMode.None);

        HashSet<string> usedIds = new HashSet<string>();

        foreach (L05_CityTarget target in found)
        {
            if (target != null &&
                !string.IsNullOrWhiteSpace(target.targetId) &&
                usedIds.Add(target.targetId))
            {
                allTargets.Add(target);
            }
        }

        if (allTargets.Count < TotalQuestions)
        {
            if (objectiveText != null)
                objectiveText.text =
                    "Add at least 4 unique locations.";

            enabled = false;
            return;
        }

        // Shuffle locations randomly.
        for (int i = allTargets.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            L05_CityTarget temp = allTargets[i];
            allTargets[i] = allTargets[j];
            allTargets[j] = temp;
        }

        questions = allTargets.GetRange(0, TotalQuestions);

        if (directionText != null)
            directionText.text = "Dirección: —";

        ShowCurrentTarget();
    }

    void Update()
    {
        if (levelFinished)
            return;

        UpdateDirection();

        if (!Input.GetKeyDown(KeyCode.E))
            return;

        if (player == null)
        {
            if (feedbackText != null)
                feedbackText.text =
                    "No se ha asignado el jugador.";

            return;
        }

        L05_CityTarget nearestTarget = null;
        float nearestDistance = interactRange;

        foreach (L05_CityTarget target in allTargets)
        {
            if (target == null)
                continue;

            Collider2D col = target.GetComponent<Collider2D>();

            Vector2 closestPoint = col != null
                ? col.ClosestPoint(player.position)
                : (Vector2)target.transform.position;

            float distance = Vector2.Distance(
                player.position, closestPoint);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = target;
            }
        }

        if (nearestTarget == null)
        {
            if (feedbackText != null)
                feedbackText.text =
                    "Acércate a un lugar y pulsa E.";

            return;
        }

        L05_CityTarget correctTarget =
            questions[currentIndex];

        if (nearestTarget.targetId == correctTarget.targetId)
        {
            currentIndex++;

            if (currentIndex >= questions.Count)
            {
                levelFinished = true;
                StartCoroutine(FinishLevel());
                return;
            }

            if (feedbackText != null)
                feedbackText.text = "¡Correcto!";

            ShowCurrentTarget();
        }
        else
        {
            if (feedbackText != null)
                feedbackText.text =
                    "¡Lugar incorrecto! Busca: " +
                    correctTarget.spanishName;
        }
    }

    void UpdateDirection()
    {
        if (directionText == null)
            return;

        if (Input.GetKey(KeyCode.W))
            directionText.text = "Dirección: Arriba";
        else if (Input.GetKey(KeyCode.S))
            directionText.text = "Dirección: Abajo";
        else if (Input.GetKey(KeyCode.A))
            directionText.text = "Dirección: Izquierda";
        else if (Input.GetKey(KeyCode.D))
            directionText.text = "Dirección: Derecha";
    }

    void ShowCurrentTarget()
    {
        if (currentIndex >= questions.Count)
            return;

        if (objectiveText != null)
            objectiveText.text =
                "BUSCA: " +
                questions[currentIndex].spanishName;

        if (feedbackText != null)
            feedbackText.text =
                "Lugar " + (currentIndex + 1) + " de " +
                TotalQuestions;
    }

    IEnumerator FinishLevel()
    {
        if (objectiveText != null)
            objectiveText.text = "¡NIVEL COMPLETADO!";

        if (feedbackText != null)
            feedbackText.text =
                "¡Excelente! Has encontrado los 4 lugares.";

        if (directionText != null)
            directionText.text = "";

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("MainMenu");
    }
}
