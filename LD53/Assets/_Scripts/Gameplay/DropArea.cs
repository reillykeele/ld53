using UnityEngine;

namespace LD53.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class DropArea : MonoBehaviour, IReceivable
    {
        [SerializeField] private MailType _acceptedMailType;

        public bool Receive(Mail mail)
        {
            // TODO: Process mail
            if (mail.HasPostage && mail.HasReceivedStamp && mail.HasReturnToSenderStamp == false && mail.MailType == _acceptedMailType)
            {
                Debug.Log("yum");
            }
            else
            {
                Debug.Log("bleh");
            }

            Destroy(mail.gameObject);
            return true;
        }
    }
}
