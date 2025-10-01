using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PDollarGestureRecognizer;
using UnityEngine.Events;


public class TraceRecognizer : MonoBehaviour
{
    #region Members
    [SerializeField] private bool isDrawing = false;
    [SerializeField] private Transform movementSource;
    [SerializeField] private float recognitionThreshold = 0.9f;

    [System.Serializable]
    public class UnityStringEvent : UnityEvent<string> { }
    public UnityStringEvent OnRecognized;

    [Header("Debugging Mode")]
    [SerializeField] private GameObject debugCubePrefab;
    [SerializeField] private bool isCreationMode = true;
    [SerializeField] private string newGestureName;

    [SerializeField] private List<Gesture> trainingSet = new List<Gesture>();
    private List<Vector3> positionList = new List<Vector3>();
    #endregion

    #region Unity
    // Start is called before the first frame update
    private void Start()
    {
        //To load all the made gestures
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            string[] gestureFiles = Directory.GetFiles(Application.streamingAssetsPath, "*.xml");

            foreach (var item in gestureFiles)
            {
                trainingSet.Add(GestureIO.ReadGestureFromFile(item));
            }
        }
        else
        {
            // Extra code for Android/Quest
            StartCoroutine(CO_LoadGesturesFromStreamingAssets());
        }
    }
    #endregion

    #region All TriggerFunctions
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Marker>())
        {
            StartTrace();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.GetComponent<Marker>())
        {
            UpdateMovement();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Marker>())
        {
            EndTrace();
        }
    }
    #endregion

    private IEnumerator CO_LoadGesturesFromStreamingAssets()
    {
        // If you know the filenames, list them manually:
        string[] gestureFileNames = { "O.xml" }; 

        foreach (var fileName in gestureFileNames)
        {
            string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);

            using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    string xmlContent = www.downloadHandler.text;
                    trainingSet.Add(GestureIO.ReadGestureFromXML(xmlContent));
                }
                else
                {
                    Debug.LogError("Failed to load gesture: " + fileName + " | " + www.error);
                }
            }
        }
    }

    #region TracingRelatedFunctions
    private void StartTrace()
    {
        isDrawing = true;
        positionList.Clear();
        positionList.Add(movementSource.position);

        if(debugCubePrefab) 
        {
            Destroy(Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity),3);
        }
        
    }

    private void EndTrace()
    {
        isDrawing = false;

        //Create Gesture from position list  
        Point[] pointArray = new Point[positionList.Count];

        for(int i = 0; i < positionList.Count; i++) 
        {
           Vector2 screenPoint = Camera.main.WorldToScreenPoint(positionList[i]);

           pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
        }

        Gesture newGesture = new Gesture(pointArray);

        //Adding our own gesture into the list
        if (isCreationMode)
        {
            newGesture.Name = newGestureName;
            trainingSet.Add(newGesture);

            //to Store gesture into the PC
            //We will later transfer this (name).xml file into the StreamingAssets folder where Start() will run and load all .xml files
            string fileName = Application.persistentDataPath + "/" + newGestureName + ".xml";
            GestureIO.WriteGesture(pointArray, newGestureName, fileName);
        }
        //recognize
        else
        {
            Result result = PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());
            Debug.Log("Gesture Result: " + result.GestureClass + result.Score);

            if(result.Score > recognitionThreshold)
            {
                OnRecognized.Invoke(result.GestureClass);
            }
        }
    }

    private void UpdateMovement()
    {
        //Vector3 lastPosition = positionList[positionList.Count - 1];
        positionList.Add(movementSource.position);

        if (debugCubePrefab)
        {
            Destroy(Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity), 1);
        }
    }
    #endregion


}
