using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using PDollarGestureRecognizer;
using UnityEngine.Events;
using System.Collections;
using UnityEngine;
using System.IO;

public class XRTrickRecognizer : MonoBehaviour
{
    #region Members

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

    [SerializeField] private List<Gesture> trainingSet = new List<Gesture>();
    private List<Vector3> positionList = new List<Vector3>();

    #endregion

    #region Unity

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
                    else
                    {
                        Debug.LogError("Failed to load gesture: " + fileName + " | " + www.error);
                    }
                }
            }
        }

        // To load all the made gestures
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            string[] gestureFiles = Directory.GetFiles(Application.streamingAssetsPath, "*.xml");

            foreach (var item in gestureFiles)
            {
                trainingSet.Add(GestureIO.ReadGestureFromFile(item));
            }
        }

        // Extra code for Android/Quest
        else StartCoroutine(CO_LoadStreamingAssets());
    }

    private void FixedUpdate()
    {
        void UpdateMovement()
        {
            positionList.Add(_trackingPoint.position);
            if (debugCubePrefab)
            {
                Destroy(Instantiate(debugCubePrefab, _trackingPoint.position, Quaternion.identity), 1);
            }
        }

        if (this.gameObject.GetComponent<Shaker>() && _isShaking)
            UpdateMovement();
    }

    #endregion

    #region Helpers

    //these will be referenced on the unity event OnSelectEntered & OnSelectExited

    public void StartTrace()
    {
        if (_isShaking) return;

        _isShaking = true;
        positionList.Clear();
        positionList.Add(_trackingPoint.position);

        if (debugCubePrefab)
        {
            Destroy(Instantiate(debugCubePrefab,
                                _trackingPoint.position,
                                Quaternion.identity), 3);
        }
    }

    public void EndTrace()
    {
        if(!_isShaking) return;

        _isShaking = false;

        Point[] pointArray = new Point[positionList.Count];

        for(int i = 0; i<positionList.Count; i++)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(positionList[i]);

            pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
        }

        Gesture newGesture = new Gesture(pointArray);

        if(isCreationMode)
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

    #endregion
}
