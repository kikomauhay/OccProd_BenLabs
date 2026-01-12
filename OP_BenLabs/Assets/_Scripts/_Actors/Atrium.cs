using System.Collections;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SoundEmitter), typeof(MeshCollider))]
public class Atrium : Actor, IInteractable
{
    #region Members

    [SerializeField] private float _rotSpeed;

    [Header("SFX")]
    [SerializeField] private Sound[] _meowSFXs;
    [SerializeField] private Sound _purrSFX;
    
    private GameManager _gameMgr;
    private AudioManager _sndMgr;

    private SoundEmitter _soundEmitter;
    private MeshCollider _col;
    private WaitForSeconds _spinLength;

    #endregion

    protected override void Start()
    {
        base.Start();
        _soundEmitter.PlaySound(_meowSFXs[Random.Range(0, _meowSFXs.Length)]);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Left Hand Physics") ||
            other.gameObject.layer == LayerMask.NameToLayer("Right Hand Physics"))
        {
            INT_Interact();
            a_logger.Log("MEOW", a_isDevMode);
        }
    }

    public void INT_Interact()
    {      
        IEnumerator CO_FunnyRotation()
        {            
            float duration = 2f;
            Vector3 rotPos = new(0f, 360f * _rotSpeed, 0f);
            Vector3 movePos = new (transform.position.x,
                                   transform.position.y + 0.02f,
                                   transform.position.z);

            _col.enabled = false; // prevents multiple triggers
            _soundEmitter.PlaySound(_purrSFX);

            transform.DORotate(-rotPos, duration, RotateMode.FastBeyond360)
                     .SetLoops(-1, LoopType.Restart)
                     .SetEase(Ease.Linear);

            transform.DOMove(movePos, 0.2f)
                     .SetLoops(-1, LoopType.Yoyo)
                     .SetEase(Ease.Linear);

            yield return _spinLength;

            _sndMgr.PlaySound("SND_Poof");
            _gameMgr.Poof(transform);
            
            _gameMgr.AtriumActive = false;
            _gameMgr.Atrium = null;
            
            transform.DOKill(); // disables DOTween
            Destroy(gameObject);
        }

        StartCoroutine(CO_FunnyRotation());
    }

    #region Helpers

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            INT_Interact();
        
    }

    protected override void AssertComponents()
    {
        Debug.Assert(_meowSFXs.Length == 2, "Missing _meowSFX elements!", this);
        Debug.Assert(_purrSFX, "Missing _purrSFX reference!", this);
    }
    protected override void InitComponents()
    {    
        _soundEmitter = GetComponent<SoundEmitter>();
        _col = GetComponent<MeshCollider>();
    }
    protected override void InitVariables()
    {
        _gameMgr = GameManager.Instance;
        _sndMgr = AudioManager.Instance;

        _spinLength = new WaitForSeconds(2f);

        _col.enabled = true;
        _col.isTrigger = true;
    }

    #endregion
}
