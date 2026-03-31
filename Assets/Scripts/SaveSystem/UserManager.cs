using UnityEngine;

namespace Marbleous.SaveSystem
{
	public class UserManager : BaseUserManager {
	
		public static UserManager Instance { get; private set; }

		private ISaveSystem fileSaveSystem; 
	
		private bool dataWasRead;
		private bool dataNeedWrite;

		private void Awake()
		{
			if (!Instance)
			{
				Instance = this;
			}
			else
			{
				Destroy(this.gameObject);
			}
		
			fileSaveSystem = new FileSaveSystem(Application.persistentDataPath + "/playerinfo.dat");
		}

		private void Start()
		{
			DontDestroyOnLoad(this.gameObject);
		}

		public void VisitLevel(int value)
		{
			if (dataWasRead)
			{
				if (GetLevel() < value)
				{
					SetLevel(value);

					dataNeedWrite = true;
				}
			}
			else
			{
				LoadPrivateDataPlayer();
			}
		}
		
		public void SavePrivateDataPlayer()
		{
			if (dataWasRead)
			{
				if (dataNeedWrite)
				{
					PlayerData data = new PlayerData();
					data.playerName = playerName;
					data.level = GetLevel();

					fileSaveSystem.Save(data);
				
					dataNeedWrite = false;
				}
			}
			else
			{
				LoadPrivateDataPlayer();
			}
		}
		
		public void LoadPrivateDataPlayer()
		{
			if (!dataWasRead)
			{
				PlayerData data = new PlayerData();
			
				if (fileSaveSystem.Load(out data))
				{
					playerName = data.playerName;
					SetLevel(data.level);
				}
				else
				{
					GetDefaultData();
				}
			
				dataWasRead = true;
			}
		}
	}

	[System.Serializable]
	public class PlayerData
	{
		public string playerName;
	
		public int level;
	}
}