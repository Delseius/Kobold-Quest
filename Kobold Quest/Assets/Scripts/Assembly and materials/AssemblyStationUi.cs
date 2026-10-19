
//using System.Diagnostics;
//using System.Diagnostics;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;




public class AssemblyStationUI : MonoBehaviour
{
    [Header("Assembly Station UI")]
    [SerializeField] private GameObject koboldAssemblystations;

    [Header("UI Panels")]
    [SerializeField] private GameObject assemblyUIPanel;
    [SerializeField] private GameObject recipeTabPanel;

    [Header("Buttons")]
    [SerializeField] private Button recipeButton;

    private bool nearAssemblyStation = false;
    private bool uiIsOpen = false;

    private void Start()
    {
        // make sure KoboldAssemblystation is closed
        if (koboldAssemblystations != null)
        {
            koboldAssemblystations.SetActive(false);
        }
        else
        {
            Debug.LogWarning(
                "KoboldAssemblystations UI has not been assigned."
            );
        }
        // make sure Recipe Tab as closed
        if (recipeTabPanel != null)
        {
            recipeTabPanel.SetActive(false);
        }

        // Register button click event listener
        if (recipeButton != null)
        {
            recipeButton.onClick.AddListener(OnRecipeButtonClicked);
        }
    }

    private void Update()
    {
        if (nearAssemblyStation &&
            Keyboard.current != null &&
            Keyboard.current.jKey.wasPressedThisFrame)
        {
            ToggleUI();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("workstations"))
        {
            nearAssemblyStation = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("workstations"))
        {
            nearAssemblyStation = false;
            CloseUI();
        }
    }

    private void ToggleUI()
    {
        if (koboldAssemblystations == null)
            return;

        uiIsOpen = !uiIsOpen;
        koboldAssemblystations.SetActive(uiIsOpen);
        //  If closing the main UI, hide the recipe tab too
        if (!uiIsOpen && recipeTabPanel != null)
        {
            recipeTabPanel.SetActive(false);
        }
    }

    private void CloseUI()
    {
        uiIsOpen = false;

        if (koboldAssemblystations != null)
        {
            koboldAssemblystations.SetActive(false);
        }
        // Hide recipe tab when walking away from station
        if (recipeTabPanel != null)
        {
            recipeTabPanel.SetActive(false);
        }
    }

    public void OnRecipeButtonClicked()
    {
        // Core Logic: Only open recipe tab if Assembly UI is actively open
        if (uiIsOpen && recipeTabPanel != null)
        {
            recipeTabPanel.SetActive(true);
            Debug.Log("Successfully opened the Recipe Tab!");
        }
    }
}

