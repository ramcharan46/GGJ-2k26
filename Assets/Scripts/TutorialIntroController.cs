using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialIntroDialogue : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI loreText;
    public TextMeshProUGUI continueHintText;

    [Header("Dialogue")]
    [TextArea(2,5)]
    public string[] dialogueLines;
    public float typingSpeed = 0.04f;
    public float timeBetweenLines = 1f;

    [Header("Hint Animation")]
    public float hintFadeSpeed = 3f;
    public float hintBlinkSpeed = 2f;

    [Header("Input")]
    public float inputCooldown = 0.15f; // prevents spam

    private int currentLine = 0;
    private bool isTyping = false;
    private bool lineFinished = false;
    private bool introEnding = false;
    private bool inputLocked = false;

    private string[] currentSegments;
    private int currentSegmentIndex = 0;

    private Coroutine typingCoroutine;

    private float hintAlpha = 0f;
    private bool hintVisible = false;

    void Start()
    {
        McMovement.canMove = false;
        loreText.text = "";
        SetHintAlpha(0);
        StartLine();
    }

    void Update()
    {
        if (introEnding || inputLocked) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(InputCooldown());

            if (isTyping)
            {
                CompleteSegmentInstantly();
            }
            else if (lineFinished)
            {
                ProceedDialogue();
            }
        }

        AnimateHint();
    }

    IEnumerator InputCooldown()
    {
        inputLocked = true;
        yield return new WaitForSeconds(inputCooldown);
        inputLocked = false;
    }

    void StartLine()
    {
        loreText.text = "";
        currentSegments = dialogueLines[currentLine].Split('|');
        currentSegmentIndex = 0;
        StartTypingSegment(currentSegments[currentSegmentIndex]);
    }

    void StartTypingSegment(string segment)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSegment(segment));
    }

    IEnumerator TypeSegment(string segment)
    {
        isTyping = true;

        foreach (char c in segment)
        {
            loreText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        lineFinished = true;
        ShowHint(true);
    }

    void CompleteSegmentInstantly()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        string fullSegment = currentSegments[currentSegmentIndex];
        if (!loreText.text.EndsWith(fullSegment))
            loreText.text += fullSegment.Substring(Mathf.Min(loreText.text.Length, fullSegment.Length));

        isTyping = false;
        lineFinished = true;
        ShowHint(true);
    }

    void ProceedDialogue()
    {
        ShowHint(false);
        lineFinished = false;

        currentSegmentIndex++;

        if (currentSegmentIndex < currentSegments.Length)
        {
            loreText.text += " ";
            StartTypingSegment(currentSegments[currentSegmentIndex]);
        }
        else
        {
            NextLine();
        }
    }

    void NextLine()
    {
        currentLine++;

        if (currentLine < dialogueLines.Length)
        {
            StartCoroutine(NextLineRoutine());
        }
        else
        {
            StartCoroutine(EndIntro());
        }
    }

    IEnumerator NextLineRoutine()
    {
        yield return new WaitForSeconds(timeBetweenLines);
        StartLine();
    }

    IEnumerator EndIntro()
    {
        introEnding = true;
        ShowHint(false);

        yield return new WaitForSeconds(1.5f);

        float fade = 1;
        while (fade > 0)
        {
            fade -= Time.deltaTime;
            loreText.alpha = fade;
            yield return null;
        }

        McMovement.canMove = true;
        gameObject.SetActive(false);
    }

    void ShowHint(bool show)
    {
        hintVisible = show;
    }

    void AnimateHint()
    {
        float target = hintVisible ? 1f : 0f;
        hintAlpha = Mathf.MoveTowards(hintAlpha, target, hintFadeSpeed * Time.deltaTime);

        float blink = 1f;
        if (hintAlpha > 0.95f)
            blink = Mathf.Lerp(0.5f, 1f, Mathf.PingPong(Time.time * hintBlinkSpeed, 1));

        SetHintAlpha(hintAlpha * blink);
    }

    void SetHintAlpha(float a)
    {
        Color c = continueHintText.color;
        c.a = a;
        continueHintText.color = c;
    }
}
