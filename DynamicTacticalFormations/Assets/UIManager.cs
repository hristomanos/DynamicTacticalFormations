using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] TextMeshProUGUI m_NumberOfUnitsText;
    [SerializeField] TextMeshProUGUI m_CurrentFormationText;




    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUnitsInFormation(int value)
    {
        m_NumberOfUnitsText.text = "Units in formation: " + value;
    }

    public void UpdateCurrentFormationUI(FormationType type)
    {
        m_CurrentFormationText.text = "Current formation: " + type;
    }

}
