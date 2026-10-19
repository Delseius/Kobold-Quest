
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class AssemblyStationUI : MonoBehaviour
{
    [Header("Assembly Station UI")]
    [SerializeField] private GameObject koboldAssemblystations;

    private bool nearAssemblyStation = false;
    private bool uiIsOpen = false;

    private void Start()
    {
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
    }

    private void CloseUI()
    {
        uiIsOpen = false;

        if (koboldAssemblystations != null)
        {
            koboldAssemblystations.SetActive(false);
        }
    }
}

