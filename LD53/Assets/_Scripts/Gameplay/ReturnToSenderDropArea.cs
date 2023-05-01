using UnityEngine;
using Util.Attributes;
using Util.Systems;

namespace LD53.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class ReturnToSenderDropArea : ADropBin
    {
        protected override bool ShouldAccept(Mail mail) => mail.HasPostage == false && mail.HasReceivedStamp && mail.HasReturnToSenderStamp;

        protected override void AcceptMail(Mail mail)
        {
            GameManager.Instance.AddScore(2);

            _audioSource?.PlayOneShot(_acceptClip);
        }

        protected override void RejectMail(Mail mail)
        {
            _audioSource?.PlayOneShot(_rejectClip);
        }
    }
}