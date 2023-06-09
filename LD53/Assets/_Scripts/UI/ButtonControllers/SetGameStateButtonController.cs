using Util.Enums;
using Util.Systems;
using Util.UI.Controllers.Selectables.Buttons;

namespace LD53.UI.ButtonControllers
{
    public class SetGameStateButtonController : AButtonController
    {
        public GameState TargetGameState;

        protected override void OnClick()
        {
            GameSystem.Instance.ChangeGameState(TargetGameState);
        }
    }
}