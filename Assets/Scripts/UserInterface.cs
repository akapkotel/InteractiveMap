using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class UserInterface : MonoBehaviour
{
    public static UserInterface Instance { get; private set; }
    
    public bool ActiveUi =>
        buttonsPanel.activeInHierarchy || infoPanel.gameObject.activeInHierarchy ||
        optionsPanel.activeInHierarchy;

    public GameObject[] minimapIndicators;
    public GameObject selectionMarker;
    public Material higlightLocationMaterial;
    
    [SerializeField] private GameObject buttonsPanel;
    [SerializeField] private GameObject searchingPanel;
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private GameObject lordInfoPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Image infoPanel;

    [SerializeField] private TextMeshProUGUI locationName, ownerName, lordName, lordTitle, lordSeignior, lordVassals, 
        lordPossesions, chiefLabel, chief, population, garrison, description;
    [SerializeField] private TMP_Dropdown locationsNamesList;
    [SerializeField] private TMP_InputField locationNameInput;
    
    [SerializeField] private Button descriptionButton;
    [SerializeField] private Button minimapButton;
    [SerializeField] private Button lordInfoButton;
    
    private bool _locationInfoDisplayed = false;

    private Location _locationUnderCursor;

    private char[] _charsToTrim = {',', ' '};

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    /* This method unpacks data obtained from currently mouse-pointed Village object
     * The unpacked data is assigned to the UI Text elements displayed to the user.*/
    public void SetLocationInfo(Location location)
    {
        if (_locationInfoDisplayed) return;
        
        locationName.text = location.locationName;
        ownerName.text = location.owner;
        chiefLabel.text = GetChiefType(location.locationType);
        chief.text = location.chief;
        description.text = location.description;

        if (location.GetComponent<PopulatedPlace>()) {
            population.text = location.GetComponent<PopulatedPlace>().population.ToString(); 
        }

        if (location.GetComponent<Fortress>()) {
            garrison.text = location.GetComponent<Fortress>().garrison.ToString();
        }

        lordInfoButton.interactable = ownerName.text.Length > 0 && LordsManager.Instance.Lords.ContainsKey(ownerName.text);

        descriptionButton.interactable = location.description.Length > 0;

        DisplayInfoPanel();
    }

    private static string GetChiefType(MapScript.LocationType locationLocationType)
    {
        switch (locationLocationType)
        {
            case MapScript.LocationType.Wioska:
                return "Sołtys:";
            case MapScript.LocationType.Forteca:
                return "Komendant:";
            case MapScript.LocationType.Miasteczko:
                return "Burmistrz:";
            case MapScript.LocationType.Miasto:
                return "Burmistrz:";
            case MapScript.LocationType.Opactwo:
                return "Opat:";
            case MapScript.LocationType.Gospoda:
                return "Gospodarz:";
            default:
                return "Zarządca:";
        }
    }

    private void LateUpdate()
    {
        if (Input.mousePosition.y > Screen.height - (Screen.height / 4)) {
            if (buttonsPanel.activeInHierarchy) return;
            buttonsPanel.SetActive(true);
        }
        else
        {
            if (!buttonsPanel.activeInHierarchy) return;
            buttonsPanel.SetActive(false);
            if (searchingPanel.activeInHierarchy)
            {
                searchingPanel.SetActive(false);
            }
        }
    }

    private void DisplayInfoPanel() {
        infoPanel.gameObject.SetActive(true);
    }

    public void HideInfoPanel() {
        infoPanel.gameObject.SetActive(false);
        population.text = "-";
        garrison.text = "-";
        chief.text = "-";
        description.text = String.Empty;
    }

    public void OpenDescriptionPanel()
    {
        descriptionPanel.SetActive(true);
        infoPanel.raycastTarget = false;
    }

    public void CloseDescriptionPanel()
    {
        descriptionPanel.SetActive(false);
        infoPanel.raycastTarget = true;
    }
    
    public void OpenLocationListMenu()
    {
        searchingPanel.SetActive(true);
        if (locationsNamesList.options.Count < MapScript.Instance.LocationsList.Length) {
            locationsNamesList.ClearOptions();
            locationsNamesList.AddOptions(MapScript.Instance.LocationNames);
        }
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }
    
    public void OpenLordPanel()
    {
        lordName.text = ownerName.text;
        lordInfoPanel.SetActive(true);

        if (LordsManager.Instance != null)
        {
            Dictionary<string, Lord> lords = LordsManager.Instance.Lords;
            Lord lord;
            lord = lords[lordName.text];

            lordTitle.text += lord.Title;
            
            lordSeignior.text += lords[lord.Seignior].Title + " " + lord.Seignior;
            
            foreach (var vassal in lord.Vassals) {
                lordVassals.text += vassal + ", ";
            }
            
            foreach (Location location in LordsManager.Instance.Lords[lordName.text].Possessions) {
                lordPossesions.text += location.locationName + ", ";
            }

            lordSeignior.text = lordSeignior.text.TrimEnd(_charsToTrim);
            lordPossesions.text = lordPossesions.text.TrimEnd(_charsToTrim);
        }
        
        infoPanel.raycastTarget = false;
    }

    public void CloseLordPanel()
    {
        lordInfoPanel.SetActive(false);
        lordTitle.text = "Tytuł: ";
        lordSeignior.text = "Jest wasalem: ";
        lordVassals.text = "Jego wasale: ";
        lordPossesions.text = "Posiadłości: ";
        
        infoPanel.raycastTarget = true;
    }

    public void TeleportOnMinimap()
    {
        Debug.Log("You should have been teleported to minimap-selected position.");
    }

    public void GetNameFromInput()
    {
        string nameFromInput = locationNameInput.text;
        SearchLocationByName(nameFromInput);
    }

    public void SelectNameFromList()
    {
        string selectedName = MapScript.Instance.LocationNames[locationsNamesList.value];
        SearchLocationByName(selectedName);
    }

    private void SearchLocationByName(string searchedName)
    {
        Debug.Log("Searching for location named: " + searchedName);

        if (MapScript.Instance.LocationNames.Contains(searchedName)) {
            Vector3 searchedLocation = MapScript.Instance.locationsDictionary[searchedName].transform.position;
            Vector3 desiredPosition = new Vector3(searchedLocation.x, searchedLocation.y+400, searchedLocation.z-200);
            
            CameraController.UserCamera.transform.parent.position = desiredPosition; 
        } else {
            Debug.Log("Sorry, such place does not exist on the map!");
        }  
    }
    
    public static UserInterface GetUserInterfaceInstance()
    {
        return Instance == null ? FindObjectOfType<UserInterface>() : Instance;
    }
}
