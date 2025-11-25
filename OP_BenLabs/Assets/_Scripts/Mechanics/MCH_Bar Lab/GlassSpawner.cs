using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GlassSpawner : XRBaseInteractable
{
    #region Serialize Field

    [SerializeField] private GameObject _glassPrefab;
    [SerializeField] private Transform _spawnPoint;

    #endregion

    #region Private

    private bool _isSpawning;

    #endregion
    protected override void OnEnable()
    {
        base.OnEnable();
        selectEntered.AddListener(GlassSpawn);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        selectEntered.RemoveListener(GlassSpawn);
    }

    private void Start()
    {
        if (interactionManager == null)
            interactionManager = FindObjectOfType<XRInteractionManager>();

        _isSpawning = false;
    }

    private void GlassSpawn(SelectEnterEventArgs args)
    {
        if (_isSpawning) return;

        _isSpawning = true;

        GameObject glass = Instantiate(_glassPrefab);

        XRGrabInteractable grabInteractable = glass.GetComponent<XRGrabInteractable>();
        interactionManager.SelectEnter(args.interactorObject, grabInteractable);

        base.OnSelectEntered(args);
        StartCoroutine(CO_FinishSpawning());
    }

    private IEnumerator CO_FinishSpawning()
    {
        yield return new WaitForSeconds(2F);
        _isSpawning=false;
    }
}
