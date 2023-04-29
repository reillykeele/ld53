using UnityEngine;

namespace LD53.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class ReturnToSenderStampArea : MonoBehaviour, IReceivable
    {
        public bool Receive(Mail mail)
        {
            if (mail.HasReturnToSenderStamp == false)
            {
                mail.StampReturnToSender();
                return true;
            }
            
            return false;
        }

    }
}