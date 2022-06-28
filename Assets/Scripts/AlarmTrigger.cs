using System.Collections;
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
    private int _alarmTriggerHash;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _alarmTriggerHash = Animator.StringToHash("IsInside");
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
            _animator.SetBool(_alarmTriggerHash, _isTriggered);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent<Robber>(out Robber robber))
        {
            _isTriggered = false;
            _isPlayingSound = false;
            _animator.SetBool(_alarmTriggerHash, _isTriggered);
        }
    }

    private void Update()
    {
        if (_isTriggered)
        {
            if (_isPlayingSound == false)
            {
                _isPlayingSound = true;
                StartCoroutine(WobbleSoundVolume());
                _audioSource.Play();
            }

        }
        else
        {
            _audioSource.Stop();
            StopCoroutine(WobbleSoundVolume());
        }

        _currentVolume = _audioSource.volume;
    }

    private IEnumerator WobbleSoundVolume()
    {
        bool isDoingAlways = true;
        float targetVolume;

        while (isDoingAlways)
        {
            if (_audioSource.volume > _maxVolume)
            {
                _isVolumeFadeOut = true;
            }
            else if (_audioSource.volume < _minVolume)
            {
                _isVolumeFadeOut = false;
            }

            if (_isVolumeFadeOut)
            {
                targetVolume = _minVolume;
            }
            else
            {
                targetVolume = _maxVolume;
            }

            _audioSource.volume = Mathf.MoveTowards(_audioSource.volume, targetVolume, _volumeFadeSpeed * Time.deltaTime);

            yield return null;
        }
    }
}
