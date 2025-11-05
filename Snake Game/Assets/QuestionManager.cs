using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class QuestionManager : MonoBehaviour
{
    private bool isPaused = false;
    public Snake snake;

    [Header("Question UI Assign")]
    [SerializeField] public GameObject questionPanel;
    [SerializeField] private GameObject questionUI;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button option1;
    private TextMeshProUGUI optionText1;
    [SerializeField] private Button option2;
    private TextMeshProUGUI optionText2;
    [SerializeField] private Button option3;
    private TextMeshProUGUI optionText3;
    [SerializeField] private Button option4;
    private TextMeshProUGUI optionText4;
    [SerializeField] private GameObject correctResponse;
    [SerializeField] private GameObject incorrectResponse;
    public TMP_Text incorrectText;
    public TMP_Text correctText;
    public TMP_Text quizProgressText;
    

    [Header("Questions")]
    [SerializeField] private List<Question> questions;
    private Question currentQuestion;
    private int questionIndex = 0;

    [HideInInspector] public bool cactusQuestion = false;
    [HideInInspector] public int allCorrect = 0;
    [HideInInspector] public int allAnswered = 0;
    [HideInInspector] public int quizProgress = 0;

    private void Awake()
    {
        questionUI.SetActive(false);

        optionText1 = option1.GetComponentInChildren<TextMeshProUGUI>();
        optionText2 = option2.GetComponentInChildren<TextMeshProUGUI>();
        optionText3 = option3.GetComponentInChildren<TextMeshProUGUI>();
        optionText4 = option4.GetComponentInChildren<TextMeshProUGUI>();

        Button correctCont = correctResponse.GetComponentInChildren<Button>();
        correctCont.onClick.AddListener(ContinueAfterQuestion);
        Button incorrectCont = incorrectResponse.GetComponentInChildren<Button>();
        incorrectCont.onClick.AddListener(ContinueAfterQuestion);

        option1.onClick.AddListener(delegate { OptionClick(0); });
        option2.onClick.AddListener(delegate { OptionClick(1); });
        option3.onClick.AddListener(delegate { OptionClick(2); });
        option4.onClick.AddListener(delegate { OptionClick(3); });
    }
    private void Start()
    {
        UpdateProgressText();
    }

    public void GetQuestion()
    {
        questionUI.SetActive(true);

        currentQuestion = questions[questionIndex];
        questionText.text = currentQuestion.questionText;
        optionText1.text = currentQuestion.optionText1;
        optionText2.text = currentQuestion.optionText2;
        optionText3.text = currentQuestion.optionText3;
        optionText4.text = currentQuestion.optionText4;

        questionIndex++;
        if (questionIndex >= questions.Count) { questionIndex = 0; }
    }

    private void OptionClick(int optionNum)
    {
        questionUI.SetActive(false);

        if (optionNum == currentQuestion.correctOption)
        {
            /*managerAudioSource.clip = correctSound;
            managerAudioSource.Play();*/

            if (!cactusQuestion)
            {
                correctResponse.SetActive(true);
                snake.wrongStreak = 0;
                allAnswered++;
                allCorrect++;
                quizProgress++;
                UpdateProgressText();
                correctText.text = "Correct! You lost 1 segment!";
                snake.RemoveSegment();
            }
            else
            {
                cactusQuestion = false;
                correctResponse.SetActive(true);
                correctText.text = "Correct! You somehow swallowed the cactus and survived!";
                snake.wrongStreak = 0;
                allAnswered++;
                allCorrect++;
            }
            
            
        }
        else
        {
            /*managerAudioSource.clip = incorrectSound;
            managerAudioSource.Play();*/

            if (!cactusQuestion)
            {
                incorrectResponse.SetActive(true);
                snake.wrongStreak++;
                allAnswered++;
                incorrectText.text = $"Incorrect, you choked on the food... You answered {snake.wrongStreak} questions wrong in a row and you gain {snake.wrongStreak} segments.";
                snake.AddSegment();
            }
            else
            {
                incorrectResponse.SetActive(true);
                allAnswered++;
                incorrectText.text = $"Incorrect, you choked on the cactus you lose!";
                snake.GameOver();              
            }
        }
    }
    
    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(.1f);
        PauseAll(false);
    }

    private void ContinueAfterQuestion()
    {
        /*managerAudioSource.clip = nextButtonPress;
        managerAudioSource.Play();*/
        
        questionPanel.SetActive(false);
        correctResponse.SetActive(false);
        incorrectResponse.SetActive(false);
        questionUI.SetActive(false);
        StartCoroutine(WaitTime());
        
    }

    public void UpdateProgressText()
    {
        quizProgressText.text = $"{quizProgress}/20";
    }

    public void PauseAll(bool pause)
    {
        snake.Pause(pause);
        
        isPaused = pause;
    }
}
