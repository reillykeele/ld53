using UnityEngine;

namespace LD53.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class DropArea : MonoBehaviour
    {
        [SerializeField] private MailType _acceptedMailType;

        public void Receive(Mail mail)
        {
            // TODO: Process mail
            if (mail.MailType == _acceptedMailType)
            {
                Debug.Log("yum");
            }
            else
            {
                Debug.Log("bleh");
            }
        }

    }
}
