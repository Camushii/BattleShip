using System;

class HumanPlayer
{
	public static object EnemyGrid { get; internal set; }
	public static object Hits { get; internal set; }
	public static object Missed { get; internal set; }
	public static object PlayerGrid { get; internal set; }
	public static bool ReadyToDeploy { get; internal set; }
	public static object Shots { get; internal set; }

	internal static void RandomizeDeployment ()
	{
		throw new NotImplementedException ();
	}
}