using System;
using UnityEngine;
using Util.Singleton;

namespace LD53.Gameplay
{
    public class GameManager : Singleton<GameManager>
    {

        public int GetHighestSortingOrder()
        {
            // TODO: for the love of god figure out a better way to do this
            var mails = FindObjectsOfType<Mail>();

            int maxSortOrder = 0;
            foreach (var mail in mails)
            {
                if (mail.SortOrder == Int16.MaxValue) 
                    continue;

                maxSortOrder = Mathf.Max(maxSortOrder, mail.SortOrder);
            }

            return maxSortOrder;
        }
    }
}