
//using System.Diagnostics;
//using System.Diagnostics;
//using System.Diagnostics;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;




public class AssemblyStationUI : MonoBehaviour
{

    [SerializeField] private Rigidbody2D koboldRigidbody;
  
    [Header("Assembly Station UI")]
    [SerializeField] private GameObject koboldAssemblystations;

    [Header("UI Panels")]
    [SerializeField] private GameObject assemblyUIPanel;
    [SerializeField] private GameObject recipeTabPanel;

    [Header("Buttons")]
    [SerializeField] private Button recipeButton;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 1f;

    //private bool nearAssemblyStation = false;
    //private bool uiIsOpen = false;

    private Camera mainCamera;
    private Transform targetStation;
    private bool movingToStation;
    private bool nearAssemblyStation;
    private Transform nearbyStation;
    private bool uiIsOpen;

    private void Start()
    {
        
        mainCamera = Camera.main;

        if (koboldRigidbody == null)
            koboldRigidbody = GetComponent<Rigidbody2D>();

        if (koboldAssemblystations != null)
            koboldAssemblystations.SetActive(false);

        uiIsOpen = false;

        if (koboldRigidbody == null)
            Debug.LogError("Assign the kobold Rigidbody2D.");

        if (koboldAssemblystations == null)
            Debug.LogError("Assign KoboldAssemblystations in the Inspector.");
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
            movingToStation = false;
            ToggleUI();
        }
        // Click a workstation.
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            TrySelectWorkstation();
        }
    }
    private void FixedUpdate()
    {
        if (!movingToStation || targetStation == null ||
            koboldRigidbody == null)
            return;

        Vector2 currentPosition = koboldRigidbody.position;
        Vector2 targetPosition = targetStation.position;
        float distance = Vector2.Distance(currentPosition, targetPosition);

        if (distance <= stoppingDistance)
        {
            movingToStation = false;
            OpenUI();
            return;
        }

        Vector2 nextPosition = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        koboldRigidbody.MovePosition(nextPosition);
    }
    private void TrySelectWorkstation()
    {
        if (mainCamera == null)
            return;

        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition =
            mainCamera.ScreenToWorldPoint(mousePosition);

        Collider2D hit = Physics2D.OverlapPoint(
            new Vector2(worldPosition.x, worldPosition.y)
        );

        if (hit == null)
            return;

        // Support a collider on a child of the workstation.
        Transform station = hit.transform;

        while (station != null &&
               !station.CompareTag("workstations"))
        {
            station = station.parent;
        }

        if (station == null)
            return;

        targetStation = station;
        nearbyStation = station;

        if (koboldRigidbody != null &&
            Vector2.Distance(koboldRigidbody.position,
                targetStation.position) > stoppingDistance)
        {
            movingToStation = true;
            CloseUI();
        }
        else
        {
            movingToStation = false;
            OpenUI();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Transform station = FindWorkstationParent(other.transform);

        if (station != null)
        {
            nearbyStation = station;
            nearAssemblyStation = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        Transform station = FindWorkstationParent(other.transform);

        if (station != null && station == nearbyStation)
        {
            nearAssemblyStation = false;
            nearbyStation = null;
            movingToStation = false;
            CloseUI();
        }
    }
    private Transform FindWorkstationParent(Transform item)
    {
        while (item != null)
        {
            if (item.CompareTag("workstations"))
                return item;

            item = item.parent;
        }

        return null;
    }


    /*private void CloseUI()
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
    }*/

    public void OnRecipeButtonClicked()
    {
        // Core Logic: Only open recipe tab if Assembly UI is actively open
        if (uiIsOpen && recipeTabPanel != null)
        {

            recipeTabPanel.SetActive(true);
            Debug.Log("Successfully opened the Recipe Tab!");
            
        }
    }
    private void ToggleUI()
    {
        if (uiIsOpen)
            CloseUI();
        else
            OpenUI();
    }

    private void OpenUI()
    {
        if (koboldAssemblystations == null)
            return;

        uiIsOpen = true;
        koboldAssemblystations.SetActive(true);
    }

    private void CloseUI()
    {
        uiIsOpen = false;

        if (koboldAssemblystations != null)
            koboldAssemblystations.SetActive(false);
    }
}

