//using UnityEngine;
//using System.Collections;
//
//public class PatternShooter : MonoBehaviour {
//	public float ShotAngle;
//	public float ShotAngleRange;
//	public float ShotSpeed;
//	public int Interval;
//	public string Pattern;
//	public int Width;
//	public int Height;
//	public int Timer;
//	public float radius;
//	public int ptcnt;
//	public bool oneShoot;
//	public bool loop;
//	public bool canShoot = true;
//	public int patternType;
//	public GameObject bullet;
//
//	void Start () {
//		if (patternType == 1)
//		{
//			Width = 37;
//			Height = 24;
//		}
//		else if (patternType == 2)
//		{
//			Width = 30;
//			Height = 21;
//		}
//		else if (patternType == 3)
//		{
//			Width = 32;
//			Height = 18;
//		}
//		oneShoot = true;
//		ptcnt = 0;
//		canShoot = false;
//			if (patternType == 1)
//			{
//			 char pattern[890] =
//					"                                     "
//					"                                     "
//					"                                     "
//					"####  ##### ####  #   # #     #  ####"
//					"#   # #     #   # #   # #     # #    "
//					"####  ##### ####  #   # #     # #    "
//					"#   # #     #     #   # #     # #    "
//					"#   # ##### #      ###  ##### #  ####"
//					"                                     "
//					"                                     "
//					"                                     "
//					"             ###  #####              "
//					"            #   # #                  "
//					"            #   # #####              "
//					"            #   # #                  "
//					"             ###  #                  "
//					"                                     "
//					"                                     "
//					"                                     "
//					"        ###   ###   # #  #####       "
//					"       #     #   # # # # #           "
//					"       #  ## ##### # # # #####       "
//					"       #   # #   # #   # #           "
//					"        ###  #   # #   # #####       ";
//				Pattern = pattern;
//			}
//			else if (patternType == 2)
//			{
//				a = sizeof("");
//
//				static char pattern[635] =
//					"########    ##################"
//					"########    ##################"
//					"#####       ##             ###"
//					"#####       ##             ###"
//					"#####     ####             ###"
//					"#####              ####    ###"
//					"#####              ####    ###"
//					"#####              ####    ###"
//					"#####              ####    ###"
//					"#####              ##      ###"
//					"#####################      ###"
//					"#####################      ###"
//					"#####################    #####"
//					"#####################    #####"
//					"#####################    #####"
//					"##############           #####"
//					"##############           #####"
//					"##############           #####"
//					"##############   #############"
//					"##############   #############"
//					"##############   #############";
//				Pattern = pattern;
//			}
//			else if (patternType == 3)
//			{
//				static char pattern[600] =
//					"                                "
//					"                                "
//					"                                "
//					"                                "
//					" #   # ##### #      #      ###  "
//					" #   # #     #      #     #   # "
//					" ##### ##### #      #     #   # "
//					" #   # #     #      #     #   # "
//					" #   # ##### #####  #####  ###  "
//					"                                "
//					"                                "
//					"                                "
//					"                                "
//					" #   #   ###  ####  #     ####  "
//					" #   #  #   # #   # #     #   # "
//					" # # #  #   # ####  #     #   # "
//					"  # #   #   # #   # #     #   # "
//					"  # #    ###  #   # ##### ####  ";
//				Pattern = pattern;
//			}
//		
//	}
//
//	void Update () {
//		if (oneShoot)
//		{
//			if (Timer%Interval == 0 && canShoot)
//			{
//				string p = Pattern + (Height - 1 - Timer / Interval)*Width;
//
//				for (int i = Width - 1; i >= 0; i--, p++)
//				{
//					if (p[i] != ' ')
//					{
//						GameObject tmp = Instantiate (bullet) as GameObject;
//						Bullet tBullet = tmp.GetComponent<Bullet> ();
//						tmp.transform.position = transform.position;
//						tBullet.speed = ShotSpeed;
//						tBullet.angle = ShotAngle + ShotAngleRange * ((float)i / (Width - 1) - 0.5f);
//					}
//				}
//				ptcnt++;
//				if (ptcnt == Height)
//					oneShoot = false;
//			}
//		}
//		else
//			ptcnt = 0;
//		if (!oneShoot && loop)
//		{
//			if (Timer%Interval == 0 && canShoot)
//			{
//				string p = Pattern + (Height - 1 - Timer / Interval)*Width;
//
//				for (int i = Width - 1; i >= 0; i--)
//				{
//					if (p[ptcnt][i] != ' ')
//					{
//						GameObject tmp = Instantiate (bullet) as GameObject;
//						Bullet tBullet = tmp.GetComponent<Bullet> ();
//						tmp.transform.position = transform.position;
//						tBullet.speed = ShotSpeed;
//						tBullet.angle = ShotAngle + ShotAngleRange * ((float)i / (Width - 1) - 0.5f);
//					}
//				}
//				ptcnt++;
//			}
//		}
//		if (canShoot)
//			Timer = (Timer + 1) % (Interval*Height);
//		if (!canShoot)
//			Timer = 0;
//	}
//}
