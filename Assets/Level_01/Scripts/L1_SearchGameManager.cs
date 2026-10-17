
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class L1_SearchGameManager : MonoBehaviour
{
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

        // Keep one entry per unique target name.
        HashSet<string> usedNames = new HashSet<string>();

        foreach (L1_SearchTarget target in allTargets)
        {
            if (usedNames.Add(target.targetId))
                targetList.Add(target);
        }

        if (targetList.Count < TotalQuestions)
        {
            Debug.LogError(
                "Create at least 5 different searchable target types.");
            gameOver = true;
            objectiveText.text = "Not enough searchable objects!";
            return;
        }

        // Randomize the target order.
        for (int i = targetList.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            L1_SearchTarget temp = targetList[i];
            targetList[i] = targetList[j];
            targetList[j] = temp;
        }

        questions = targetList.GetRange(0, TotalQuestions);

        feedbackText.text = "";
        ShowCurrentTarget();
    }

    void ShowCurrentTarget()
    {
        if (currentIndex >= TotalQuestions)
        {
            gameOver = true;
            objectiveText.text = "GAME OVER!";
            feedbackText.text = "¡GANASTE! ¡Has encontrado los 5 objetos!";
            return;
        }

        objectiveText.text =
    "BUSCA: " + questions[currentIndex].displayName;

        feedbackText.text =
            "Object " + (currentIndex + 1) + " of " + TotalQuestions;
    }

    public void CheckTarget(L1_SearchTarget clickedTarget)
    {
        if (gameOver)
            return;

        L1_SearchTarget correctTarget = questions[currentIndex];

        if (clickedTarget.targetId == correctTarget.targetId)
        {
            feedbackText.text = "Correct! Well done!";
            currentIndex++;
            ShowCurrentTarget();
        }
        else
        {
            feedbackText.text =
    "¡Objeto incorrecto! Busca: " + correctTarget.displayName;
        }
    }
}
