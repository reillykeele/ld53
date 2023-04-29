using UnityEngine;

namespace LD53.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class ReceivedStampArea : MonoBehaviour, IReceivable
    {
        public bool Receive(Mail mail)
        {
            if (mail.HasReceivedStamp == false)
            {
                mail.StampReceived();
                return true;
            }
            
            return false;
        }

    }
}