using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using PDollarGestureRecognizer;

public class XRTrickRecognizer : MonoBehaviour
{
    #region Members

    [SerializeField] private bool _shakerCapped = false;
    [SerializeField] private bool _isShaking = false;
    [SerializeField] private Transform _trackingPoint;
    [SerializeField] private float _threshold;

    [System.Serializable]
    public class UnityStringEvent : UnityEvent<string> { }
    public UnityStringEvent OnRecognized;

    [Header("Debugging Mode")]
    [SerializeField] private GameObject debugCubePrefab;
    [SerializeField] private bool isCreationMode = true;
    [SerializeField] private string newGestureName;

    [SerializeField] private List<Gesture> trainingSet = new();
    private List<Vector3> _positionsList = new();

    #endregion

    #region Unity

    private void OnEnable()
    {
        Shaker.ShakerLocked += IsCapped;
        Shaker.ShakerUnlocked += ResetShaker;
    }

    private void OnDisable()
    {
        Shaker.ShakerLocked -= IsCapped;
        Shaker.ShakerUnlocked -= ResetShaker; 
    }

    private void Start()
    {
        IEnumerator CO_LoadStreamingAssets()
        {
            string[] trickGestures = { "Pass.xml", "Toss.xml", "Spin.xml" };

            foreach (var fileName in trickGestures)
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
                    else Debug.LogError("Failed to load gesture: " + fileName + " | " + www.error);
                }
            }
        }

        // To load all the made gestures
        if (Application.platform == RuntimePlatform.WindowsEditor || 
            Application.platform == RuntimePlatform.WindowsPlayer)
        {
            string gestureFolder = Path.Combine(Application.streamingAssetsPath, "TrickGestures");
            string[] gestureFiles = Directory.GetFiles(gestureFolder, "*.xml");

            foreach (var item in gestureFiles)
                trainingSet.Add(GestureIO.ReadGestureFromFile(item));
        }

        // Extra code for Android/Quest
        else StartCoroutine(CO_LoadStreamingAssets());
    }

    private void FixedUpdate()
    {
        if (gameObject.GetComponent<Shaker>() && _isShaking)
        {
            _positionsList.Add(_trackingPoint.position);
            
            if (debugCubePrefab)
                Destroy(Instantiate(debugCubePrefab, _trackingPoint.position, Quaternion.identity), 1f);
        }
    }

    #endregion

    #region Helpers

    public void StartTrace()
    {
        if (_isShaking && !_shakerCapped) return;

        _isShaking = true;
        _positionsList.Clear();
        _positionsList.Add(_trackingPoint.position);

        if (debugCubePrefab)
        {
            Destroy(Instantiate(debugCubePrefab,
                                _trackingPoint.position,
                                Quaternion.identity), 3);
        }
    }

    public void EndTrace()
    {
        if (!_isShaking) return;

        _isShaking = false;

        Point[] pointArray = new Point[_positionsList.Count];

        for (int i = 0; i < _positionsList.Count; i++)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(_positionsList[i]);

            pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
        }

        Gesture newGesture = new(pointArray);

        if (isCreationMode)
        {
            newGesture.Name = newGestureName;
            trainingSet.Add(newGesture);

            string fileName = Application.persistentDataPath + "/" + newGestureName + ".xml";
            GestureIO.WriteGesture(pointArray,newGestureName,fileName);
            Debug.Log($"Gesture saved at: {fileName}");
        }
        else
        {
            Result result = PointCloudRecognizer.Classify(newGesture,trainingSet.ToArray());
            Debug.Log("Gesture Result: " + result.GestureClass + result.Score);

            if (result.Score > _threshold)
            {
                OnRecognized.Invoke(result.GestureClass);
            }
        }
    }

    private void IsCapped() => _shakerCapped = true;
    private void ResetShaker() => _shakerCapped = false;

    #endregion
}
