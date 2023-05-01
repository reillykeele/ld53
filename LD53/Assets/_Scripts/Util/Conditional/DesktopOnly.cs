using UnityEngine;
using Util.Helpers;

namespace Util.Conditional
{
    public class DesktopOnly : MonoBehaviour
    {
        void Awake()
        {
            #if UNITY_WEBGL
            gameObject.Disable();
            #endif
        }
    }
}