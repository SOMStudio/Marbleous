using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("Base/GameController")]

public class BaseGameController : MonoBehaviour
{
	bool paused;

	[SerializeField]
	protected GameObject explosionPrefab;

	public void Explode ( Vector3 aPosition )
	{
		Instantiate( explosionPrefab, aPosition, Quaternion.identity );
	}

	public virtual void PlayerDestroyed()
	{
		
	}
	
	public virtual void EnemyDestroyed()
	{
		
	}
	
	public virtual void BossDestroyed()
	{
		
	}
	
	public virtual void RestartGameButtonPressed()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene().name);
	}

    public bool Paused
    {
        get 
        {
	        return paused; 
        }
        set
        {
	        paused = value;

			if (paused)
			{
				Time.timeScale = 0f;
			} else {
				Time.timeScale = 1f;
            }
        }
    }
	
}
