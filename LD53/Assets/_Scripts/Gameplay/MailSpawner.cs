using System;
using UnityEngine;
using Util.Attributes;
using Util.GameEvents;
using Util.Helpers;
using Util.Pooling;
using Util.Systems;
using Random = UnityEngine.Random;

namespace LD53.Gameplay
{
    public class MailSpawner : BasePool<Mail>
    {
        [SerializeField] private Transform _mailParent;
        [SerializeField, PrefabOnly] private Mail _mailPrefab;

        [SerializeField] private int _initialMailToSpawn = 1;

        [Header("Object Pool Settings")] 
        [SerializeField] private int _initialPoolSize = 250;
        [SerializeField] private int _maxPoolSize = 500;

        [Header("Spawn Settings")] 
        [SerializeField, Range(0f, 1f)] private float _postageRate = 1f;

        [Header("Spawn Area")]
        [SerializeField] private Vector2 _minSpawn = Vector2.zero;
        [SerializeField] private Vector2 _maxSpawn = Vector2.zero;
        [Space(5f)]
        [SerializeField] private Vector2 _minArea = Vector2.zero;
        [SerializeField] private Vector2 _maxArea = Vector2.zero;

        [SerializeField, Range(0f, 360f)] private float _randomRotation = 0f;

        [Header("Spawning")] 
        [SerializeField] private Vector2 _spawnIntervalRange = Vector2.up;
        [SerializeField] private AnimationCurve _spawnRateCurve;
        [SerializeField, ReadOnly] private float _spawnTimer = 0f;
        [SerializeField] private bool _useCurve = false;
        [SerializeField] public bool Active = true;

        [Header("Event Listeners")]
        [SerializeField] private VoidGameEventSO _mailStartGameEvent;
        [SerializeField] private VoidGameEventSO _timesUpGameEvent;

        private Array _mailTypes;

        void Start()
        {
            InitPool(_mailPrefab, _initialPoolSize, _maxPoolSize);

            _mailTypes = Enum.GetValues(typeof(MailType));
            
            for (var i = 0; i < _initialMailToSpawn; i++)
            {
                SpawnInitialMail();
            }
        }

        void OnEnable()
        {
            _mailStartGameEvent.OnEventRaised += ActivateSpawner;
            _timesUpGameEvent.OnEventRaised += DeactivateSpawner;
        }

        void OnDisable()
        {
            _mailStartGameEvent.OnEventRaised -= ActivateSpawner;
            _timesUpGameEvent.OnEventRaised -= DeactivateSpawner;
        }

        void Update()
        {
            if (Active == false || GameSystem.Instance.IsPlaying() == false) return;

            if (_spawnTimer <= 0f)
            {
                // spawn new
                SpawnMail();

                if (_useCurve)
                {
                    var t = GameManager.Instance.TimerStartDuration - GameManager.Instance.TimerRemaining;
                    _spawnTimer = _spawnRateCurve.Evaluate(t);
                }
                else
                {
                    _spawnTimer = GetRandomFromRange(_spawnIntervalRange);
                }
            }
            else
            {
                _spawnTimer -= Time.deltaTime;
            }
        }

        protected override Mail CreateSetup() => Instantiate(_mailPrefab, _mailParent);

        private void SpawnMail()
        {
            var z = GameManager.Instance.GetTopZ(); // TODO: FIND A BETTER WAY

            var spawnPos = GetRandomXYFromRange(_minSpawn, _maxSpawn);
            spawnPos.z = z;
            var spawnRot = Quaternion.Euler(0f, 0f, Random.Range(-_randomRotation, _randomRotation));

            var mail = Get();

            mail.Spawner = this;

            mail.transform.position = spawnPos;
            mail.transform.rotation = spawnRot;
            mail.transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);

            mail.HasPostage = Random.value <= _postageRate;
            mail.MailType = (MailType) _mailTypes.GetValue(Random.Range(0, _mailTypes.Length));

            // setup the spawn animation vals
            mail.IsSpawning = true;
            mail.GoalPosition =  GetRandomXYFromRange(_minArea, _maxArea);
            mail.GoalPosition.z = z;
            mail.GoalRotation = Quaternion.Euler(0f, 0f, Random.Range(-_randomRotation, _randomRotation));

            mail.Init();
        }

        private void SpawnInitialMail()
        {
            var z = GameManager.Instance.GetTopZ(); // TODO: FIND A BETTER WAY

            var spawnPos = GetRandomXYFromRange(_minArea, _maxArea);
            spawnPos.z = z;
            var spawnRot = Quaternion.Euler(0f, 0f, Random.Range(-_randomRotation, _randomRotation));

            var mail = Get();

            mail.Spawner = this;

            mail.transform.position = spawnPos;
            mail.transform.rotation = spawnRot;

            mail.HasPostage = Random.value <= _postageRate;
            mail.MailType = (MailType)_mailTypes.GetValue(Random.Range(0, _mailTypes.Length));
        }

        private void ActivateSpawner()
        {
            var t = GameManager.Instance.TimerStartDuration - GameManager.Instance.TimerRemaining;
            _spawnTimer = _spawnRateCurve.Evaluate(t);

            Active = true;
        }

        private void DeactivateSpawner()
        {
            Active = false;
        }

        void OnDrawGizmos()
        {
            // spawn
            var tl = new Vector3(_minSpawn.x, _minSpawn.y);
            var tr = new Vector3(_maxSpawn.x, _minSpawn.y);
            var bl = new Vector3(_minSpawn.x, _maxSpawn.y);
            var br = new Vector3(_maxSpawn.x, _maxSpawn.y);

            DebugDrawHelper.DrawSquare(tl, tr, bl, br, Color.green);

            // play area
            tl = new Vector3(_minArea.x, _minArea.y);
            tr = new Vector3(_maxArea.x, _minArea.y);
            bl = new Vector3(_minArea.x, _maxArea.y);
            br = new Vector3(_maxArea.x, _maxArea.y);

            DebugDrawHelper.DrawSquare(tl, tr, bl, br, Color.red);
        }

        private static Vector3 GetRandomXYFromRange(Vector2 min, Vector2 max)
        {
            return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), 0);
        }

        private float GetRandomFromRange(Vector2 range) => Random.Range(range.x, range.y);
    }
}
