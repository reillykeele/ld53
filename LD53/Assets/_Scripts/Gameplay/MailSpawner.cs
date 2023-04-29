using System;
using UnityEngine;
using Util.Attributes;
using Random = UnityEngine.Random;

namespace LD53.Gameplay
{
    public class MailSpawner : MonoBehaviour
    {
        [SerializeField, PrefabOnly] private Mail _mailPrefab;

        [SerializeField] private int _mailToSpawn = 1;

        [Header("Spawn Area")]
        [SerializeField] private Vector2 _minSpawn = Vector2.zero;
        [SerializeField] private Vector2 _maxSpawn = Vector2.zero;

        [SerializeField, Range(0f, 360f)] private float _randomRotation = 0f;

        void Start()
        {
            var mailTypes = Enum.GetValues(typeof(MailType));
            for (var i = 0; i < _mailToSpawn; i++)
            {
                var x = Random.Range(_minSpawn.x, _maxSpawn.x);
                var y = Random.Range(_minSpawn.y, _maxSpawn.y);
                var pos = new Vector3(x, y, 0);

                var rot = Quaternion.Euler(0f, 0f, Random.Range(-_randomRotation, _randomRotation));

                var mail = Instantiate(_mailPrefab, pos, rot);

                mail.MailType = (MailType) mailTypes.GetValue(Random.Range(0, mailTypes.Length));
                mail.SortOrder = i;
            }
        }

        void OnDrawGizmos()
        {
            var tl = new Vector3(_minSpawn.x, _minSpawn.y);
            var tr = new Vector3(_maxSpawn.x, _minSpawn.y);
            var bl = new Vector3(_minSpawn.x, _maxSpawn.y);
            var br = new Vector3(_maxSpawn.x, _maxSpawn.y);

            Debug.DrawLine(tl, tr, Color.red);
            Debug.DrawLine(tl, bl, Color.red);
            Debug.DrawLine(tr, br, Color.red);
            Debug.DrawLine(bl, br, Color.red);
        }
    }
}
