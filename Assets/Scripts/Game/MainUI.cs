using UnityEngine;
using WXFramwork.UI;

namespace Game
{
    [UIBase(UIWindowId.Main)]
    public class MainUI : AUIBase
    {
        public override string AssetPath => "Assets/Bundles/UIMain.prefab";
        public override bool IsOtherCovered => true;

        public override void OnInit()
        {
            Debug.Log("初始化");
        }

        public override void OnShow()
        {
            Debug.Log("显示");
        }
    }
}