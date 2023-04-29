using UnityEngine;

namespace LD53.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class ReceivedStampArea : MonoBehaviour, IReceivable
    {
        public bool Receive(Mail mail)
        {
            mail.StampReceived();
            return true;
        }

    }
}