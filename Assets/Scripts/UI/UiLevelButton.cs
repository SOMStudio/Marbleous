using UnityEngine;
using UnityEngine.UI;

namespace Marbleous.Menu
{
    public class UiLevelButton : MonoBehaviour
    {
        [SerializeField] private Button buttonLevel;
        [SerializeField] private Toggle toggleVisitLevel;

        public bool Interactable
        {
            get { return buttonLevel.interactable; }
            set { buttonLevel.interactable = value; }
        }

        public bool Visit
        {
            get { return toggleVisitLevel.isOn; }
            set { toggleVisitLevel.isOn = value; }
        }
    }
}