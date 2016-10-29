using UnityEngine;
using System.Collections;
using PathologicalGames;

public class PatternShooter : MonoBehaviour {
	public float ShotAngle;
	public float ShotAngleRange;
	public float ShotSpeed;
	public int Interval;
	private string[] Pattern;
	private int Width;
	private int Height;
	private int Timer = 0;
	private int ptcnt;
	public bool oneShoot = true;
	public bool canShoot = true;
	public int patternType = 1;
	public GameObject bullet;
	static SpawnPool spawnPool = null;
	Transform tmp, tr;
	Bullet tBullet;
	public int bulletMoney;
	public float bulletHp;
	void Awake()
	{
		tr = GetComponent<Transform>();
	}

	void Start () {
		if (spawnPool == null) {
			spawnPool = PoolManager.Pools ["Test"];
		}

		if (patternType == 1)
		{
			Width = 41;
			Height = 23;
		}
		ptcnt = 0;
		  
		Pattern = new string[23] 
		{	
			"#   # ##### ##### #  # ##### #    # #### ",
			"#   # #     #     # #  #     ##   # #   #",
			"# # # ####  ####  ##   ####  #  # # #   #",
			"# # # #     #     # #  #     #   ## #   #",
			" # #  ##### ##### #  # ##### #    # #### ",
			"                                         ",
			"                                         ",
			"                                         ",
			"                                         ",
			"          ####  ###   # #  #####         ",
			"         #     #   # # # # #             ",
			"         #  ## ##### # # # #####         ",
			"         #   # #   # #   # #             ",
			"          #### #   # #   # #####         ",
			"                                         ",
			"                                         ",
			"                                         ",
			"                                         ",
			"        ### #    # ####  ### #####       ",
			"         #  ##   # #   #  #  #           ",
			"         #  #  # # #   #  #  ####        ",
			"         #  #   ## #   #  #  #           ",
			"        ### #    # ####  ### #####       "
		};
	}

	void Update () {
		if (Timer % Interval == 0 && canShoot)
		{
			for (int i = Width - 1; i >= 0; i--)
			{
				if (Pattern [Height - 1 - ptcnt] [i] != ' ')
				{
					tmp = spawnPool.Spawn (bullet, Vector3.zero, Quaternion.identity);
					tBullet = tmp.GetComponent<Bullet> ();
					tmp.transform.position = tr.position;
					tBullet.speed = ShotSpeed;
					tBullet.angle = ShotAngle + ShotAngleRange * ((float)i / (Width - 1) - 0.5f);
					tBullet.speedRate = 0;
					tBullet.angleRate = 0;
					tBullet.basicHP = bulletHp;
					tBullet.hp = bulletHp;
					tBullet.money = bulletMoney;
				}
			}
			ptcnt++;
			if (ptcnt == Height) {
				canShoot = false;
				ptcnt = 0;
			}
		}
		if (canShoot)
			Timer = (Timer + 1) % (Interval * Height);
		if (!canShoot)
			Timer = 0;
	}
}
