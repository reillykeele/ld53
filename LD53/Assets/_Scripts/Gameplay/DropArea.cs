using UnityEngine;
using Util.Attributes;
using Util.Systems;

namespace LD53.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class DropArea : ADropBin
    {
        [Header("Mail Bin")]
        [SerializeField] private MailType _acceptedMailType;

        protected override bool ShouldAccept(Mail mail) => 
            mail.HasPostage && mail.HasReceivedStamp && mail.HasReturnToSenderStamp == false && mail.MailType == _acceptedMailType;

        protected override void AcceptMail(Mail mail)
        {
            GameManager.Instance.AddScore(1);

            _audioSource?.PlayOneShot(_acceptClip);
        }

        protected override void RejectMail(Mail mail)
        {
            _audioSource?.PlayOneShot(_rejectClip);
        }
    }
}
