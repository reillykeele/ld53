using System;
using UnityEngine;
using Util.Attributes;
using Util.Helpers;
using Util.Systems;
using Random = UnityEngine.Random;

namespace LD53.Gameplay
{
    public class MailSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _mailParent;
        [SerializeField, PrefabOnly] private Mail _mailPrefab;

        [SerializeField] private int _mailToSpawn = 1;

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

        private Array _mailTypes;

        void Start()
        {
            _mailTypes = Enum.GetValues(typeof(MailType));
            
            // for (var i = 0; i < _mailToSpawn; i++)
            // {
            //     SpawnMail();
            // }
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

        private void SpawnMail()
        {
            var spawnPos = GetRandomXYFromRange(_minSpawn, _maxSpawn);
            var spawnRot = Quaternion.Euler(0f, 0f, Random.Range(-_randomRotation, _randomRotation));

            var mail = Instantiate(_mailPrefab, spawnPos, spawnRot, _mailParent);

            mail.HasPostage = Random.value <= _postageRate;
            mail.MailType = (MailType) _mailTypes.GetValue(Random.Range(0, _mailTypes.Length));
            mail.SortOrder = GameManager.Instance.GetHighestSortingOrder() + 1; // TODO: FIND A BETTER WAY

            // setup the animation vals
            mail.GoalPosition =  GetRandomXYFromRange(_minArea, _maxArea);
            mail.GoalRotation = Quaternion.Euler(0f, 0f, Random.Range(-_randomRotation, _randomRotation));
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
