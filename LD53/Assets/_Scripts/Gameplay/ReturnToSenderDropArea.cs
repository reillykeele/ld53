using UnityEngine;

namespace LD53.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class ReturnToSenderDropArea : MonoBehaviour, IReceivable
    {
        public bool Receive(Mail mail)
        {
            if (mail.HasPostage == false && mail.HasReceivedStamp && mail.HasReturnToSenderStamp)
            {
                GameManager.Instance.AddScore(1);
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