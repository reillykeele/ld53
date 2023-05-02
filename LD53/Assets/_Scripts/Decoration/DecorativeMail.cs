using System.Collections.Generic;
using System.Linq;
using LD53.Gameplay;
using UnityEngine;
using Util.Attributes;
using Util.Helpers;
using Random = UnityEngine.Random;

namespace LD53.Decoration
{
    public class DecorativeMail : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer _address;
        [SerializeField] private SpriteRenderer _postage;
        [Space]
        [SerializeField] private SpriteRenderer _receivedStamp;
        [SerializeField] private SpriteRenderer _returnToSenderStamp;
        [Space]
        [SerializeField] private SpriteRenderer _outline;

        [Header("Properties")] 
        [SerializeField, ReadOnly] public DecorativeMailSpawner Spawner;
        [SerializeField, ReadOnly] public MailType MailType;
        [SerializeField, ReadOnly] public bool HasPostage;

        [Header("Spawn Animation")]
        [SerializeField] private float _spawnMoveDuration = 1f;
        [SerializeField] private float _spawnRotationDuration = 1f;
        [Space]
        [SerializeField] private LeanTweenType _spawnMoveEase = LeanTweenType.notUsed;
        [SerializeField] private LeanTweenType _spawnRotationEase = LeanTweenType.notUsed;
        [Space]
        [SerializeField, ReadOnly] public Vector3 GoalPosition;
        [SerializeField, ReadOnly] public Quaternion GoalRotation;
        [Space]
        [SerializeField, ReadOnly] public LTDescr _spawnMoveTween;
        [SerializeField, ReadOnly] public LTDescr _spawnRotateTween;
        [Space]
        [SerializeField, ReadOnly] public bool _spawnMoveFinished = false;
        [SerializeField, ReadOnly] public bool _spawnRotateFinished = false;

        [Header("Other")] 
        [SerializeField] private List<MailColorMap> _colorMap;
        [SerializeField] private Sprite[] _postageStamps;

        public void Init()
        {
            // setup mail
            _address.color = _colorMap.FirstOrDefault(x => MailType == x.MailType).color;

            if (HasPostage)
            {
                _postage.gameObject.Enable();

                _postage.sprite = _postageStamps[Random.Range(0, _postageStamps.Length)];
            }
            else
            {
                _postage.gameObject.Disable();
            }
            
            _receivedStamp.gameObject.Disable();
            _returnToSenderStamp.gameObject.Disable();

            // spawn animation
            _spawnMoveTween = LeanTween.move(gameObject, GoalPosition, _spawnMoveDuration)
                    .setEase(_spawnMoveEase)
                    .setIgnoreTimeScale(false);
                _spawnMoveTween.setOnComplete(ReleaseMe);

            _spawnRotateTween = LeanTween.rotate(gameObject, GoalRotation.eulerAngles, _spawnRotationDuration)
                .setEase(_spawnRotationEase)
                .setIgnoreTimeScale(false);
        }

        private void ReleaseMe() => Spawner.Release(this);
    
    }
}

