using UnityEngine;
using Util.Systems;
using Util.UI.Controllers.Selectables.Buttons;

namespace LD53.UI.ButtonControllers
{
    public class PauseGameButtonController : AButtonController
    {
        protected override void OnClick() => GameSystem.Instance.PauseGame();
    }
}
