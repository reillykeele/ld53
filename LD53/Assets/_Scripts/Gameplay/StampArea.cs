using UnityEngine;

namespace LD53.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class ReceivedStampArea : AStamp
    {
        protected override bool ShouldStamp(Mail mail) => mail.HasReceivedStamp == false;

        protected override void StampMail(Mail mail)
        {
            mail.StampReceived();

            _audioSource.PlayOneShot(_stampAudioClip);
        }

        protected override void IgnoreMail(Mail mail) { }
    }
}