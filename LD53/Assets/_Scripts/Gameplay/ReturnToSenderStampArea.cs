using UnityEngine;

namespace LD53.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class ReturnToSenderStampArea : AStamp
    {
        protected override bool ShouldStamp(Mail mail) => mail.HasReturnToSenderStamp == false;

        protected override void StampMail(Mail mail)
        {
            mail.StampReturnToSender();

            _audioSource.PlayOneShot(_stampAudioClip);
        }

        protected override void IgnoreMail(Mail mail) { }
    }
}