using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

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
    //[SerializeField] private TextMeshProUGUI powerupName;
    //[SerializeField] private Image powerupIcon;
    [SerializeField] private GameObject incorrectResponse;
    //[SerializeField] private List<Sprite> powerupIcons;

    [Header("Questions")]
    [SerializeField] private List<Question> questions;
    private Question currentQuestion;
    private int questionIndex = 0;

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
            correctResponse.SetActive(true);
            
        }
        else
        {
            /*managerAudioSource.clip = incorrectSound;
            managerAudioSource.Play();*/
            incorrectResponse.SetActive(true);
        }
    }

    private void ContinueAfterQuestion()
    {
        /*managerAudioSource.clip = nextButtonPress;
        managerAudioSource.Play();*/
        PauseAll(false);
        questionPanel.SetActive(false);
        correctResponse.SetActive(false);
        incorrectResponse.SetActive(false);
        questionUI.SetActive(false);
    }

    public void PauseAll(bool pause)
    {
        snake.Pause(pause);
        
        isPaused = pause;
    }
}
