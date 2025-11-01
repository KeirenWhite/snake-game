using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Question")]
public class Question : ScriptableObject
{
    [SerializeField] public string questionText = "";
    [SerializeField] public int correctOption = 0;
    [SerializeField] public string optionText1 = "";
    [SerializeField] public string optionText2 = "";
    [SerializeField] public string optionText3 = "";
    [SerializeField] public string optionText4 = "";
}
