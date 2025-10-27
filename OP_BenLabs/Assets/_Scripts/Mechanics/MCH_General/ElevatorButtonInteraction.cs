using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorButtonInteraction : MonoBehaviour
{
    [SerializeField] private Button _button;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<VR_Hands>() != null)
        {
            _button.onClick.Invoke();
            StartCoroutine(DisableButton());
        }
        else
        {
            Debug.LogWarning("NO INTERACTION");
        }
    }

    private IEnumerator DisableButton()
    {
        _button.interactable = false;
        yield return new WaitForSeconds(3.0f);
        _button.interactable = true;
    }
    
}
