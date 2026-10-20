
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class L02_SearchGameManager : MonoBehaviour
{
    [Header("Player")]
    public Transform mosquito;

    [Header("UI")]
    public TMP_Text objectiveText;
    public TMP_Text feedbackText;

    [Header("Interaction Settings")]
    public float biteRange = 10f;

    [Tooltip("If multiple targets overlap, prefer the target with the closest collider point.")]
    public bool prioritizeClosestCollider = true;

    private List<L02_BodyPartTarget> allTargets =
        new List<L02_BodyPartTarget>();

    private List<L02_BodyPartTarget> questions =
        new List<L02_BodyPartTarget>();

    private int currentIndex = 0;
    private bool gameOver = false;

    void Start()
    {
        L02_BodyPartTarget[] targets =
            FindObjectsByType<L02_BodyPartTarget>(
                FindObjectsSortMode.None);

        Dictionary<string, L02_BodyPartTarget> uniqueParts =
            new Dictionary<string, L02_BodyPartTarget>();

        foreach (L02_BodyPartTarget target in targets)
        {
            if (target == null ||
                string.IsNullOrWhiteSpace(target.targetId))
                continue;

            allTargets.Add(target);

            if (!uniqueParts.ContainsKey(target.targetId))
                uniqueParts.Add(target.targetId, target);
        }

        if (uniqueParts.Count < 5)
        {
            Debug.LogError(
                "Level 2 needs at least 5 unique body-part types.");

            if (objectiveText != null)
                objectiveText.text = "Faltan partes del cuerpo.";

            gameOver = true;
            return;
        }

        questions.AddRange(uniqueParts.Values);

        // Shuffle the unique body-part questions.
        for (int i = questions.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            L02_BodyPartTarget temp = questions[i];
            questions[i] = questions[j];
            questions[j] = temp;
        }

        if (questions.Count > 5)
            questions.RemoveRange(5, questions.Count - 5);

        ShowCurrentTarget();
    }

    void Update()
    {
        if (gameOver || !Input.GetKeyDown(KeyCode.E))
            return;

        if (mosquito == null)
        {
            Debug.LogError(
                "Assign the mosquito in the GameManager Inspector.");
            return;
        }

        L02_BodyPartTarget nearestTarget = FindBestTarget();

        if (nearestTarget == null)
        {
            if (feedbackText != null)
                feedbackText.text =
                    "Acércate a la parte correcta y pulsa E.";

            return;
        }

        CheckTarget(nearestTarget);
    }

    L02_BodyPartTarget FindBestTarget()
    {
        L02_BodyPartTarget bestTarget = null;
        float bestDistance = float.MaxValue;

        foreach (L02_BodyPartTarget target in allTargets)
        {
            if (target == null)
                continue;

            Collider2D[] colliders =
                target.GetComponentsInChildren<Collider2D>();

            float targetDistance = float.MaxValue;

            if (colliders.Length == 0)
            {
                targetDistance = Vector2.Distance(
                    mosquito.position, target.transform.position);
            }
            else
            {
                foreach (Collider2D col in colliders)
                {
                    if (col == null || !col.enabled)
                        continue;

                    Vector2 closestPoint =
                        col.ClosestPoint(mosquito.position);

                    float distance = Vector2.Distance(
                        mosquito.position, closestPoint);

                    if (distance < targetDistance)
                        targetDistance = distance;
                }
            }

            if (targetDistance <= biteRange &&
                targetDistance < bestDistance)
            {
                bestDistance = targetDistance;
                bestTarget = target;
            }
        }

        return bestTarget;
    }

    void CheckTarget(L02_BodyPartTarget target)
    {
        if (currentIndex >= questions.Count)
            return;

        L02_BodyPartTarget correctTarget =
            questions[currentIndex];

        // Matching IDs allow either eye, ear, or leg to count.
        if (target.targetId == correctTarget.targetId)
        {
            currentIndex++;

            if (currentIndex >= questions.Count)
            {
                gameOver = true;
                StartCoroutine(FinishLevel());
                return;
            }

            ShowCurrentTarget();

            if (feedbackText != null)
                feedbackText.text = "¡Correcto! ¡Buen trabajo!";
        }
        else
        {
            if (feedbackText != null)
                feedbackText.text =
                    "¡Incorrecto! Busca: " +
                    correctTarget.displayName;
        }
    }

    void ShowCurrentTarget()
    {
        if (currentIndex >= questions.Count)
            return;

        if (objectiveText != null)
            objectiveText.text =
                "BUSCA: " + questions[currentIndex].displayName;

        if (feedbackText != null)
            feedbackText.text =
                "Parte " + (currentIndex + 1) + " de " +
                questions.Count +
                "\nAcércate y pulsa E para picar.";
    }

    IEnumerator FinishLevel()
    {
        if (objectiveText != null)
            objectiveText.text = "¡NIVEL COMPLETADO!";

        if (feedbackText != null)
            feedbackText.text =
                "¡Excelente! Has encontrado las 5 partes del cuerpo.";

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("MainMenu");
    }
}
