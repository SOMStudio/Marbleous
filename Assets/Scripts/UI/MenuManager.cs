using UnityEngine;
using UnityEngine.SceneManagement;

namespace Marbleous.Menu
{
    public class MenuManager : BaseMenuManager
    {
        [Header("Level List Manager")]
        [SerializeField] private UiLevelList levelList;
        
        void LateUpdate() {
            if (didInit) {
                if (Input.GetKeyDown (KeyCode.Escape)) {
                    ClickEscapeEvent ();
                }
            }
        }

        protected override void RestoreOptionsPref()
        {
            base.RestoreOptionsPref();
            
            UserManager.Instance?.LoadPrivateDataPlayer();
            
            levelList.UpdateVisit(UserManager.Instance.GetLevel());

            var randomBall = FindObjectOfType<Mover>();
            randomBall.Move(MoveSplineDirection.increment);
        }

        protected override void SaveOptionsPrefs()
        {
            base.SaveOptionsPrefs();
            
            SoundManager.Instance?.UpdateVolume();
        }

        protected override void ChangeWindowEvent(int number)
        {
            base.ChangeWindowEvent(number);
            
            SoundManager.Instance?.PlaySoundByIndex(0, Vector3.zero);
        }

        protected override void ChangeConsoleWEvent(int number)
        {
            base.ChangeConsoleWEvent(number);
            
            SoundManager.Instance?.PlaySoundByIndex(0, Vector3.zero);
        }

        public void RunLevel(int value)
        {
            UserManager.Instance?.VisitLevel(value);
            
            SceneManager.LoadScene("Level" + value);
        }
        
        private void ClickEscapeEvent() {
            if (consoleWindowActive == -1) {
                if (windowActive == -1) {
                    ExitGameConsoleWindow ();
                }
            }
        }
        
        protected override void ExitGame ()
        {
            UserManager.Instance?.SavePrivateDataPlayer();
            
            SoundManager.Instance?.PlaySoundByIndex(1, Vector3.zero);
		    
            base.ExitGame ();
        }
        
        public void ExitGameConsoleWindow() {
            ConsoleWinYesNo_ActionCloseGame ();
            ActivateConsoleWindow (4);
        }
        
        private void ConsoleWinYesNo_ActionCloseGame() {
            ConsoleWinYesNo_SetTxt ("Do you want to Exit?");
            ConsoleWinYesNo_SetYesAction (ExitGame);
        }

        public override void ConsoleWinYesNo_ButtonNo()
        {
            base.ConsoleWinYesNo_ButtonNo();
            
            SoundManager.Instance?.PlaySoundByIndex(1, Vector3.zero);
        }

        public override void ConsoleWinYesNo_ButtonYes()
        {
            base.ConsoleWinYesNo_ButtonYes();
            
            SoundManager.Instance?.PlaySoundByIndex(1, Vector3.zero);
        }
    }
}