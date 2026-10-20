using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Runtime presentation layer for Aventura en España.
/// Adds consistent typography, HUD panels, button feedback and audio
/// without requiring changes to each scene's existing gameplay scripts.
/// </summary>
public class GamePolishBootstrap : MonoBehaviour
{
    private static GamePolishBootstrap instance;
    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioClip clickClip, correctClip, wrongClip, victoryClip, musicClip, interactClip, outOfRangeClip;
    private readonly HashSet<int> styledButtons = new HashSet<int>();
    private readonly HashSet<int> styledTexts = new HashSet<int>();
    private readonly Dictionary<int, string> lastFeedback = new Dictionary<int, string>();
    private readonly HashSet<int> victoryPlayed = new HashSet<int>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Boot()
    {
        if (instance != null) return;
        GameObject root = new GameObject("AventuraGamePolish");
        instance = root.AddComponent<GamePolishBootstrap>();
        DontDestroyOnLoad(root);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = 0.18f;
        musicSource.spatialBlend = 0f;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.volume = 0.65f;
        sfxSource.spatialBlend = 0f;

        clickClip = Resources.Load<AudioClip>("Audio/UI_Click");
        correctClip = Resources.Load<AudioClip>("Audio/Correct");
        wrongClip = Resources.Load<AudioClip>("Audio/Wrong");
        victoryClip = Resources.Load<AudioClip>("Audio/Victory");
        musicClip = Resources.Load<AudioClip>("Audio/Spanish_Menu_Loop");
        interactClip = Resources.Load<AudioClip>("Audio/Interact");
        outOfRangeClip = Resources.Load<AudioClip>("Audio/OutOfRange");

        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(ApplyAfterSceneReady());
    }

    private void OnDestroy()
    {
        if (instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ApplyAfterSceneReady());
    }

    private IEnumerator ApplyAfterSceneReady()
    {
        // Let scene objects and their Start methods initialize first.
        yield return null;
        ApplyPresentation();
        UpdateMusicForScene();
    }

    private void Update()
    {
        // Give every E interaction a short tactile sound in gameplay scenes.
        if (Input.GetKeyDown(KeyCode.E) && SceneManager.GetActiveScene().name != "MainMenu")
            PlayInteract();

        TMP_Text[] texts = FindObjectsByType<TMP_Text>(FindObjectsSortMode.None);
        foreach (TMP_Text text in texts)
        {
            if (text == null) continue;
            string objectName = text.gameObject.name.ToLowerInvariant();
            string value = text.text ?? string.Empty;
            int id = text.GetInstanceID();
            if (value.Contains("NIVEL COMPLETADO") || value.Contains("LEVEL COMPLETED"))
            {
                if (victoryPlayed.Add(id)) PlayVictory();
            }
            if (objectName.Contains("feedback"))
            {
                if (lastFeedback.TryGetValue(id, out string previous) && previous != value)
                {
                    string lower = value.ToLowerInvariant();
                    if (lower.Contains("acércate") || lower.Contains("acercate") || lower.Contains("fuera de rango")) PlayOutOfRange();
                    else if (lower.Contains("incorrect") || lower.Contains("incorrecto") || lower.Contains("wrong")) PlayWrong();
                    else if (lower.Contains("correct") || lower.Contains("bien hecho") || lower.Contains("excelente")) PlayCorrect();
                }
                lastFeedback[id] = value;
            }
        }
    }

    private void ApplyPresentation()
    {
        TMP_Text[] texts = FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (TMP_Text text in texts)
        {
            if (text == null || !styledTexts.Add(text.GetInstanceID())) continue;

            string objectName = text.gameObject.name.ToLowerInvariant();
            bool isObjective = objectName.Contains("objective");
            bool isFeedback = objectName.Contains("feedback");
            bool isTitle = objectName.Contains("title") || objectName.Contains("heading");

            text.enableAutoSizing = true;
            text.fontSizeMin = isTitle ? 24 : 14;
            text.fontSizeMax = isTitle ? 64 : 32;
            text.enableWordWrapping = true;
            text.margin = new Vector4(8f, 5f, 8f, 5f);

            if (isObjective || isTitle)
            {
                text.fontStyle |= FontStyles.Bold;
                text.color = new Color(1f, 0.91f, 0.69f, 1f);
            }
            else if (isFeedback)
            {
                text.fontStyle |= FontStyles.Bold;
                text.color = new Color(0.92f, 0.96f, 1f, 1f);
            }
            else if (text.color.a > 0.01f)
            {
                // Keep authored colors when they are intentionally vivid.
                text.color = Color.Lerp(text.color, new Color(0.97f, 0.96f, 0.93f, 1f), 0.18f);
            }

            if (text.GetComponent<Shadow>() == null)
            {
                Shadow shadow = text.gameObject.AddComponent<Shadow>();
                shadow.effectColor = new Color(0f, 0f, 0f, 0.65f);
                shadow.effectDistance = new Vector2(1.5f, -1.5f);
            }

            if (isObjective || isFeedback)
                EnsureHudPanel(text, isObjective);
        }

        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            if (button == null || !styledButtons.Add(button.GetInstanceID())) continue;

            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.18f, 0.24f, 0.31f, 0.96f);
            colors.highlightedColor = new Color(0.31f, 0.43f, 0.52f, 1f);
            colors.pressedColor = new Color(0.82f, 0.48f, 0.20f, 1f);
            colors.selectedColor = new Color(0.31f, 0.43f, 0.52f, 1f);
            colors.disabledColor = new Color(0.25f, 0.25f, 0.25f, 0.55f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.12f;
            button.colors = colors;
            button.transition = Selectable.Transition.ColorTint;

            Image image = button.GetComponent<Image>();
            if (image != null)
                image.raycastTarget = true;

            PolishedButtonFeedback feedback = button.GetComponent<PolishedButtonFeedback>();
            if (feedback == null) feedback = button.gameObject.AddComponent<PolishedButtonFeedback>();
            feedback.Configure(this);

            TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
            {
                label.fontStyle |= FontStyles.Bold;
                label.color = Color.white;
                label.alignment = TextAlignmentOptions.Center;
            }
        }

        EnsureBackButton();
        EnsureCharacterIdleAnimations();
    }

    private void EnsureHudPanel(TMP_Text text, bool objective)
    {
        RectTransform rect = text.rectTransform;
        Transform parent = rect.parent;
        if (parent == null || parent.Find(text.gameObject.name + "_PolishPanel") != null) return;

        GameObject panelObject = new GameObject(text.gameObject.name + "_PolishPanel", typeof(RectTransform), typeof(Image), typeof(Outline));
        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.SetParent(parent, false);
        panelRect.anchorMin = rect.anchorMin;
        panelRect.anchorMax = rect.anchorMax;
        panelRect.pivot = rect.pivot;
        panelRect.anchoredPosition = rect.anchoredPosition;
        panelRect.sizeDelta = rect.sizeDelta + new Vector2(12f, 8f);
        panelRect.localRotation = rect.localRotation;
        panelRect.localScale = rect.localScale;

        Image image = panelObject.GetComponent<Image>();
        image.color = objective
            ? new Color(0.035f, 0.075f, 0.12f, 0.72f)
            : new Color(0.055f, 0.085f, 0.12f, 0.60f);
        image.raycastTarget = false;

        Outline outline = panelObject.GetComponent<Outline>();
        outline.effectColor = objective
            ? new Color(0.91f, 0.58f, 0.25f, 0.85f)
            : new Color(0.35f, 0.58f, 0.68f, 0.65f);
        outline.effectDistance = new Vector2(1f, -1f);
        outline.useGraphicAlpha = true;

        panelObject.transform.SetSiblingIndex(rect.GetSiblingIndex());
    }

    private void EnsureCharacterIdleAnimations()
    {
        SpriteRenderer[] renderers = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (SpriteRenderer sr in renderers)
        {
            if (sr == null || sr.GetComponent<Animator>() != null || sr.GetComponentInParent<Animator>() != null)
                continue;

            string n = sr.gameObject.name.ToLowerInvariant();
            if (n.Contains("player") || n.Contains("mosquito") || n.Contains("target") ||
                n.Contains("map") || n.Contains("background") || n.Contains("wall"))
                continue;

            bool characterLike = n.Contains("npc") || n.Contains("student") ||
                n.Contains("character") || n.Contains("villager") ||
                n.Contains("pedestrian") || n.Contains("boy") ||
                n.Contains("girl") || n.Contains("person");

            if (characterLike && sr.GetComponent<GentleCharacterIdle>() == null)
                sr.gameObject.AddComponent<GentleCharacterIdle>();
        }
    }

    private void EnsureBackButton()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.name == "MainMenu") return;

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        Transform existing = canvas.transform.Find("RuntimeBackToMenuButton");
        if (existing != null) return;
        // The project already contains authored back buttons in its levels.
        // Do not create a duplicate if a scene already has a UI button.
        if (canvas.GetComponentsInChildren<Button>(true).Length > 0) return;

        GameObject buttonObject = new GameObject("RuntimeBackToMenuButton", typeof(RectTransform), typeof(Image), typeof(Button));
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.SetParent(canvas.transform, false);
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(22f, -22f);
        rect.sizeDelta = new Vector2(176f, 52f);

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.12f, 0.20f, 0.27f, 0.96f);

        Button button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.18f, 0.24f, 0.31f, 0.96f);
        colors.highlightedColor = new Color(0.31f, 0.43f, 0.52f, 1f);
        colors.pressedColor = new Color(0.82f, 0.48f, 0.20f, 1f);
        button.colors = colors;
        PolishedButtonFeedback feedback = buttonObject.AddComponent<PolishedButtonFeedback>();
        feedback.Configure(this);

        GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.SetParent(buttonObject.transform, false);
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(6f, 3f);
        labelRect.offsetMax = new Vector2(-6f, -3f);

        TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
        label.text = "←  MAIN MENU";
        label.fontSize = 19f;
        label.fontStyle = FontStyles.Bold;
        label.color = Color.white;
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;
    }

    private void UpdateMusicForScene()
    {
        if (musicClip == null || musicSource == null) return;
        if (!musicSource.isPlaying)
        {
            musicSource.clip = musicClip;
            musicSource.Play();
        }
    }

    public void PlayClick() { PlaySfx(clickClip, 0.55f); }
    public void PlayInteract() { PlaySfx(interactClip != null ? interactClip : clickClip, 0.48f); }
    public void PlayOutOfRange() { PlaySfx(outOfRangeClip != null ? outOfRangeClip : wrongClip, 0.58f); }
    public void PlayCorrect() { PlaySfx(correctClip, 0.7f); }
    public void PlayWrong() { PlaySfx(wrongClip, 0.6f); }
    public void PlayVictory() { PlaySfx(victoryClip, 0.8f); }

    private void PlaySfx(AudioClip clip, float volume)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip, volume);
    }

    public static GamePolishBootstrap Instance => instance;
}

