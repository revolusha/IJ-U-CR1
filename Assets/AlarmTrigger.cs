using UnityEngine;

public class AlarmTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _maxVolume = 0.4f;
    [SerializeField] private float _minVolume = 0.1f;
    [SerializeField] private float _volumeFadeSpeed = 3;
    [SerializeField] private float _secondsToChangeVolumeFadeDirection = 3;
    [SerializeField] private float _currentVolume;

    private Animator _animator;
    private bool _isTriggered;
    private bool _isPlayingSound;
    private bool _isVolumeFadeOut;
    private float _passedTime;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _isTriggered = false;
        _isPlayingSound = false;
        _isVolumeFadeOut = false;
        _passedTime = 0;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<Robber>(out Robber robber))
        {
            _isTriggered = true;
            _animator.SetBool("IsInside", _isTriggered);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent<Robber>(out Robber robber))
        {
            _isTriggered = false;
            _isPlayingSound = false;
            _animator.SetBool("IsInside", _isTriggered);
        }
    }

    private void Update()
    {
        if (_isTriggered)
        {
            if (_isPlayingSound == false)
            {
                _isPlayingSound = true;
                _audioSource.Play();
            }

            WobbleSoundVolume();
        }
        else
        {
            _audioSource.Stop();
        }

        _currentVolume = _audioSource.volume;
    }

    private void WobbleSoundVolume()
    {
        float targetVolume;

        if (_isVolumeFadeOut)
        {
            targetVolume = _minVolume;
        }
        else
        {
            targetVolume = _maxVolume;
        }

        _passedTime += Time.deltaTime;

        if (_passedTime > _secondsToChangeVolumeFadeDirection)
        {
            _isVolumeFadeOut = !_isVolumeFadeOut;
            _passedTime = 0;
        }

        _audioSource.volume = Mathf.MoveTowards(_audioSource.volume, targetVolume, _volumeFadeSpeed * Time.deltaTime);
    }
}
