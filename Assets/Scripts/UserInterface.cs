using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(MapScript))]
public class UserInterface : MonoBehaviour
{
    public static UserInterface Instance;

    public Material higlightLocationMaterial;
    public GameObject buttonsPanel;
    public GameObject searchingPanel;
    public Image infoPanel;
    public TextMeshProUGUI locationName, house, owner, chief, population, garrison;
    public TMP_Dropdown locationsNamesList;
    public TMP_InputField locationNameInput;

    /*This Dictionary is used to find Vector3 location of searched place and transform it to the
     new position of the Camera. Locations are stored as lists of floats since we already implemented
     simmilar storing system in SaveScript.*/
    private Dictionary<string, List<float>> locationsPositions;

    private bool locationInfoDisplayed;

    private Location locationUnderCursor;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private Dictionary<string, List<float>> CreateLocationsTable()
    {
        Dictionary<string, List<float>> dict = new Dictionary<string, List<float>>();

        for (int i = 0; i < MapScript.Instance.locations.Count; i++)
        {
            Location location = MapScript.Instance.locations[i];
            List<float> pseudoTransform = new List<float>(3);

            pseudoTransform.Add(location.transform.position.x);
            pseudoTransform.Add(location.transform.position.y + 400f); // to place Camera above the found location
            pseudoTransform.Add(location.transform.position.z - 200f); // and to the south

            dict.Add(location.locationName, pseudoTransform);
        }
        return dict;
    }

    /* This method unpacks data obtained from currently mouse-pointed Village object
     * The unpacked data is assigned to the UI Text elements displayed to the user.*/
    public void SetLocationInfo(Location location) {

        locationName.text = location.locationName;
        house.text = location.house;
        owner.text = location.owner;

        if (location.GetComponent<PopulatedPlace>()) {
            population.text = location.GetComponent<PopulatedPlace>().population.ToString();
            chief.text = location.GetComponent<PopulatedPlace>().chief;
        }

        if (location.GetComponent<Fortress>())
        {
            garrison.text = location.GetComponent<Fortress>().garrison.ToString();
        }
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Location hitLocation = hit.collider.GetComponent<Location>();

            if (CameraController.Instance.locationsVisible && hitLocation) {
                locationUnderCursor = hitLocation;

                if (!locationUnderCursor.locationHiglighted) {
                    locationUnderCursor.HiglightLocationObjects(true);
                }
            } else if (locationUnderCursor != null) {
                locationUnderCursor.HiglightLocationObjects(false);
                locationUnderCursor = null;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                Location location = hit.collider.GetComponent<Location>();

                if (location && CameraController.Instance.locationsVisible)
                {
                    if (!locationInfoDisplayed)
                    {
                        locationInfoDisplayed = true;
                        SetLocationInfo(location);
                    }
                }
                else if (infoPanel)
                {
                    locationInfoDisplayed = false;
                }
            }
            UpdateInfoPanel(locationInfoDisplayed);
        }
    }

    private void LateUpdate()
    {
        if (Input.mousePosition.y > Screen.height - (Screen.height / 4)) {
            if (!buttonsPanel.activeInHierarchy) {
                buttonsPanel.SetActive(true);
                if (locationsPositions == null) {
                    locationsPositions = CreateLocationsTable();
                    //Debug.Log(locationsPositions.Count);
                }
            }
        } else {
            if (buttonsPanel.activeInHierarchy) {
                buttonsPanel.SetActive(false);
                if (searchingPanel.activeInHierarchy) {
                    searchingPanel.SetActive(false);
                }
            }
        }
    }

    public void UpdateInfoPanel(bool shouldBeActive) {
        if (shouldBeActive) {
            if (!infoPanel.IsActive()) {
                DisplayInfoPanel();
            }
        } else {
            if (infoPanel.IsActive()) {
                HideInfoPanel();
            }
        }
    }

    void DisplayInfoPanel() {
        infoPanel.gameObject.SetActive(true);
    }

    void HideInfoPanel() {
        infoPanel.gameObject.SetActive(false);
        population.text = "-";
        garrison.text = "-";
        chief.text = "-";
    }

    public void OpenLocationListMenu()
    {
        //Debug.Log("Now, list of locations and input field should be displayed.");
        searchingPanel.SetActive(true);

        if (locationsNamesList.options.Count == 0) {
            locationsNamesList.AddOptions(MapScript.Instance.locationNames);
        }
    }

    public void GetNameFromInput()
    {
        string nameFromInput = locationNameInput.text.ToString();
        SearchLocationByName(nameFromInput);
    }

    public void SelectNameFromList()
    {
        string selectedName = MapScript.Instance.locationNames[locationsNamesList.value].ToString();
        SearchLocationByName(selectedName);
    }

    public void SearchLocationByName(string searchedName)
    {
        Debug.Log("Searching for location named: " + searchedName);

        // retrieve desired Camera position from our Dicitionary using name as a key:
        if (locationsPositions.ContainsKey(searchedName)) {

            Vector3 desiredCameraPosition = new Vector3();

            desiredCameraPosition.x = locationsPositions[searchedName][0];
            desiredCameraPosition.y = locationsPositions[searchedName][1];
            desiredCameraPosition.z = locationsPositions[searchedName][2];

            //Debug.Log(desiredCameraPosition);

            Camera.main.transform.parent.position = desiredCameraPosition; 
        } else {
            Debug.Log("Sorry, such place does not exist on the map!");
        }  
    }
}
