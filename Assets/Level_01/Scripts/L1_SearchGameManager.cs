
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class L1_SearchGameManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text objectiveText;
    public TMP_Text feedbackText;

    private List<L1_SearchTarget> targetList =
        new List<L1_SearchTarget>();

    private List<L1_SearchTarget> questions =
        new List<L1_SearchTarget>();

    private int currentIndex = 0;
    private const int TotalQuestions = 5;
    private bool gameOver = false;

    void Start()
    {
        L1_SearchTarget[] allTargets =
            FindObjectsByType<L1_SearchTarget>(
                FindObjectsSortMode.None);

        HashSet<string> usedIds = new HashSet<string>();

        foreach (L1_SearchTarget target in allTargets)
        {
            if (target != null &&
                !string.IsNullOrWhiteSpace(target.targetId) &&
                usedIds.Add(target.targetId))
            {
                targetList.Add(target);
            }
        }

        if (targetList.Count < TotalQuestions)
        {
            Debug.LogError(
                "Create at least 5 different searchable target types.");

            gameOver = true;

            if (objectiveText != null)
                objectiveText.text = "Not enough searchable objects!";

            return;
        }

        // Assign this manager to every searchable object.
        foreach (L1_SearchTarget target in allTargets)
        {
            if (target != null)
                target.gameManager = this;
        }

        // Shuffle targets randomly.
        for (int i = targetList.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            L1_SearchTarget temp = targetList[i];
            targetList[i] = targetList[j];
            targetList[j] = temp;
        }

        questions = targetList.GetRange(0, TotalQuestions);

        if (feedbackText != null)
            feedbackText.text = "";

        ShowCurrentTarget();
    }

    void Update()
    {
        if (gameOver || !Input.GetMouseButtonDown(0))
            return;

        Camera cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("No camera tagged MainCamera.");
            return;
        }

        Vector2 mousePosition = cam.ScreenToWorldPoint(
            Input.mousePosition);

        Collider2D[] hits =
            Physics2D.OverlapPointAll(mousePosition);

        L1_SearchTarget bestTarget = null;
        int bestSortingLayer = int.MinValue;
        int bestOrder = int.MinValue;

        foreach (Collider2D hit in hits)
        {
            if (hit == null || !hit.enabled)
                continue;

            L1_SearchTarget target =
                hit.GetComponent<L1_SearchTarget>();

            if (target == null)
                target = hit.GetComponentInParent<L1_SearchTarget>();

            if (target == null)
                continue;

            SpriteRenderer sr =
                target.GetComponent<SpriteRenderer>();

            int sortingLayer = sr != null
    ? SortingLayer.GetLayerValueFromID(sr.sortingLayerID)
    : 0;

            int order = sr != null
                ? sr.sortingOrder
                : 0;

            if (bestTarget == null ||
                sortingLayer > bestSortingLayer ||
                (sortingLayer == bestSortingLayer &&
                 order > bestOrder))
            {
                bestTarget = target;
                bestSortingLayer = sortingLayer;
                bestOrder = order;
            }
        }

        if (bestTarget != null)
        {
            CheckTarget(bestTarget);
        }
    }

    void ShowCurrentTarget()
    {
        if (currentIndex >= TotalQuestions)
        {
            gameOver = true;
            StartCoroutine(FinishLevel());
            return;
        }

        if (objectiveText != null)
        {
            objectiveText.text =
                "BUSCA: " + questions[currentIndex].displayName;
        }

        if (feedbackText != null)
        {
            feedbackText.text =
                "Objeto " + (currentIndex + 1) +
                " de " + TotalQuestions;
        }
    }

    public void CheckTarget(L1_SearchTarget clickedTarget)
    {
        if (gameOver || clickedTarget == null ||
            currentIndex >= questions.Count)
            return;

        L1_SearchTarget correctTarget =
            questions[currentIndex];

        if (clickedTarget.targetId == correctTarget.targetId)
        {
            currentIndex++;

            if (currentIndex >= TotalQuestions)
            {
                gameOver = true;
                StartCoroutine(FinishLevel());
                return;
            }

            if (feedbackText != null)
                feedbackText.text = "¡Correcto! ¡Bien hecho!";

            ShowCurrentTarget();
        }
        else
        {
            if (feedbackText != null)
            {
                feedbackText.text =
                    "¡Objeto incorrecto! Busca: " +
                    correctTarget.displayName;
            }
        }
    }

    IEnumerator FinishLevel()
    {
        if (objectiveText != null)
            objectiveText.text = "¡NIVEL COMPLETADO!";

        if (feedbackText != null)
        {
            feedbackText.text =
                "¡Felicidades! Has encontrado los 5 objetos.";
        }

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("MainMenu");
    }
}
