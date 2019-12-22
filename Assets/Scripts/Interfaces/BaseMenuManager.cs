using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BaseMenuManager : MonoBehaviour {
	
	public bool didInit = false;
	
	[Header("Base Settings")]
	public string gamePrefsName = "DefaultGame";
	
	[SerializeField]
	private float audioSFXSliderValue;
	[SerializeField]
	private Slider audioSFXSlider;
	
	private int detailLevels = 6;
	private bool needSaveOptions = false;
	
	[Header("Main window list")]
	[SerializeField]
	private AnimationOpenClose[] windowAnimations;
	
	[Header("DisActivate window")]
	[SerializeField]
	private AnimationOpenClose windowDisActivateAnimation;
	[SerializeField]
	private AnimationOpenClose consoleWindowDisActivateAnimation;
	
	[Header("Menu Data")]
	[SerializeField]
	protected int windowActive = -1;
	[SerializeField]
	protected int consoleWindowActive = -1;

	[Header("Console windows")]
	//YesNo
	[SerializeField]
	private Text consoleWInYesNoTextHead;
	private UnityEvent consoleWInYesNoActinYes = new UnityEvent();
	
	void Start()
	{
		RestoreOptionsPref ();
	}
	
	protected virtual void RestoreOptionsPref()
	{
		string stKey = "";
		
		stKey = string.Format("{0}_SFXVol", gamePrefsName);
		if (PlayerPrefs.HasKey (stKey)) {
			audioSFXSliderValue = PlayerPrefs.GetFloat (stKey);
		} else {
			audioSFXSliderValue = 1;
		}
		
		if (audioSFXSlider != null) {
			audioSFXSlider.value = audioSFXSliderValue;
		}

		didInit = true;
	}

	protected virtual void SaveOptionsPrefs()
	{
		string stKey = "";

		stKey = string.Format("{0}_SFXVol", gamePrefsName);
		PlayerPrefs.SetFloat(stKey, audioSFXSliderValue);
	}

	public void ChangeSFXVal(float val) {
		audioSFXSliderValue = val;

		if (didInit) {
			SaveOptionsPrefs ();
		}
	}

	protected virtual void ExitGame()
	{
		#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

	#region Animations
	private void PlayWindowAnim_Open(int number) {
		if (number < windowAnimations.Length) {
			AnimationOpenClose activeA = windowAnimations [number];

			if (activeA) {
				if (!activeA.IsOpen()) {
					activeA.Open ();
				}
			}
		}
	}

	private void PlayWindowAnim_Close(int number) {
		if (number < windowAnimations.Length) {
			AnimationOpenClose activeA = windowAnimations [number];

			if (activeA) {
				if (activeA.IsOpen()) {
					activeA.Close ();
				}
			}
		}
	}

	//window
	private void WindowDisActivate_Open() {
		if (windowDisActivateAnimation) {
			windowDisActivateAnimation.Open ();
		}
	}

	private void WindowDisActivate_Close() {
		if (windowDisActivateAnimation) {
			windowDisActivateAnimation.Close ();
		}
	}

	//consoleWindows
	private void ConsoleWindowDisActivate_Open() {
		if (consoleWindowDisActivateAnimation) {
			consoleWindowDisActivateAnimation.Open ();
		}
	}

	private void ConsoleWindowDisActivate_Close() {
		if (consoleWindowDisActivateAnimation) {
			consoleWindowDisActivateAnimation.Close ();
		}
	}
	#endregion

	#region Events
	protected virtual void ActivateWindowEvent() {
	
	}

	protected virtual void DisActivateWindowEvent() {
	
	}

	protected virtual void ChangeWindowEvent(int number) {
	
	}

	protected virtual void ActivateConsoleWEvent() {
	
	}

	protected virtual void DisActivateConsoleWEvent() {
	
	}

	protected virtual void ChangeConsoleWEvent(int number) {
	
	}
	#endregion
	
	//windows
	public int WindowActive {
		get { return windowActive; }
	}

	public void ActivateWindow(int number) {
		if (windowActive == number) {
			DisActivateWindow ();
		} else {
			if (windowActive > -1) {
				PlayWindowAnim_Close (windowActive);
			}

			PlayWindowAnim_Open (number);

			if (windowActive == -1) {
				WindowDisActivate_Open ();
				
				ActivateWindowEvent ();
			}

			windowActive = number;
			
			ChangeWindowEvent (number);
		}
	}

	public void DisActivateWindow() {
		if (windowActive > -1) {
			PlayWindowAnim_Close (windowActive);
			
			DisActivateWindowEvent ();
		}

		windowActive = -1;

		WindowDisActivate_Close ();
		
		if (needSaveOptions) {
			SaveOptionsPrefs ();

			needSaveOptions = !needSaveOptions;
		}
	}

	//consoleWindows
	public int ConsoleWindowActive {
		get { return consoleWindowActive; }
	}

	public void ActivateConsoleWindow(int number) {
		if (consoleWindowActive == number) {
			DisActivateConsoleWindow ();
		} else {
			if (consoleWindowActive > -1) {
				PlayWindowAnim_Close (consoleWindowActive);
			}

			PlayWindowAnim_Open (number);

			if (consoleWindowActive == -1) {
				ConsoleWindowDisActivate_Open ();
				
				ActivateConsoleWEvent ();
			}

			consoleWindowActive = number;
			
			ChangeConsoleWEvent (number);
		}
	}

	public void DisActivateConsoleWindow() {
		if (consoleWindowActive > -1) {
			PlayWindowAnim_Close (consoleWindowActive);
		}

		consoleWindowActive = -1;

		ConsoleWindowDisActivate_Close ();
		
		DisActivateConsoleWEvent ();
		
		if (needSaveOptions) {
			SaveOptionsPrefs ();

			needSaveOptions = !needSaveOptions;
		}
	}

	//console YesNo
	protected void ConsoleWinYesNo_SetTxt(string val) {
		consoleWInYesNoTextHead.text = val;
	}

	protected void ConsoleWinYesNo_SetYesAction(UnityAction val) {
		consoleWInYesNoActinYes.AddListener (val);
	}

	protected void ConsoleWinYesNo_ClearYesAction() {
		consoleWInYesNoActinYes.RemoveAllListeners ();
	}

	public virtual void ConsoleWinYesNo_ButtonYes() {
		consoleWInYesNoActinYes.Invoke ();
		
		DisActivateConsoleWindow ();
		
		ConsoleWinYesNo_ClearYesAction ();
	}

	public virtual void ConsoleWinYesNo_ButtonNo() {
		DisActivateConsoleWindow ();
		
		ConsoleWinYesNo_ClearYesAction ();
	}
}
