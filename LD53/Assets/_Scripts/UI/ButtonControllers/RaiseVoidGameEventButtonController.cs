using UnityEngine;
using Util.GameEvents;
using Util.UI.Controllers.Selectables.Buttons;

namespace LD53.UI.ButtonControllers
{
    public class RaiseVoidGameEventButtonController : AButtonController
    {
        [SerializeField] private VoidGameEventSO _event;

        protected override void OnClick() => _event.RaiseEvent();
    }
}