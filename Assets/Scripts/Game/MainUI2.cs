using WXFramwork.UI;

namespace Game
{
    [UIBase(UIWindowId.Main2)]
    public class MainUI2 : AUIBase
    {
        public override string AssetPath => "Assets/Bundles/UIMain2.prefab";
        public override bool IsOtherCovered => true;
    }
}