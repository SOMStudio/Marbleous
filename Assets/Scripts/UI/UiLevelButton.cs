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
            get => buttonLevel.interactable;
            set => buttonLevel.interactable = value;
        }

        public bool Visit
        {
            get => toggleVisitLevel.isOn;
            set => toggleVisitLevel.isOn = value;
        }
    }
}