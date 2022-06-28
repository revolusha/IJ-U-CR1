using System.Collections;
using UnityEngine;

public class AlarmTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _maxVolume = 0.4f;
    [SerializeField] private float _minVolume = 0.1f;
    [SerializeField] private float _volumeFadeSpeed = 2;
    [SerializeField] private float _currentVolume;

    private Coroutine _soundWobbler;
    private Animator _animator;
    private bool _isTriggered;
    private int _alarmTriggerHash;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _alarmTriggerHash = Animator.StringToHash("IsInside");
        _audioSource.volume = _minVolume;
        _isTriggered = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<Robber>(out Robber robber))
        {
            _isTriggered = true;
            _animator.SetBool(_alarmTriggerHash, _isTriggered);
            RestartCoroutine();
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent<Robber>(out Robber robber))
        {
            _isTriggered = false;
        }
    }

    private void Update()
    {
        _currentVolume = _audioSource.volume;
    }

    private void RestartCoroutine()
    {
        if (_soundWobbler != null)
            StopCoroutine(_soundWobbler);

        _soundWobbler = StartCoroutine(WobbleSoundVolumeController());
        _audioSource.Play();
    }

    private IEnumerator WobbleSoundVolumeController()
    {
        while (_isTriggered)
        {
            yield return WobbleVolumeCycle();
        }
    }

    private IEnumerator WobbleVolumeCycle()
    {
        float targetVolume = _maxVolume;

        yield return SoundVolumeReacher(targetVolume);
        targetVolume = _minVolume;
        yield return SoundVolumeReacher(targetVolume);

        if (_isTriggered == false)
        {
            _audioSource.Stop();
            _animator.SetBool(_alarmTriggerHash, _isTriggered);
        }
    }

    private IEnumerator SoundVolumeReacher(float targetValue)
    {
        while (_audioSource.volume != targetValue)
        {
            _audioSource.volume = Mathf.MoveTowards(_audioSource.volume, targetValue, _volumeFadeSpeed * Time.deltaTime);

            yield return null;
        }
    }
}