
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Data;
using System.Diagnostics;
using SwinGameSDK;

/// <summary>
/// The battle phase is handled by the DiscoveryController.
/// </summary>
static class DiscoveryController
{

	/// <summary>
	/// Handles input during the discovery phase of the game.
	/// </summary>
	/// <remarks>
	/// Escape opens the game menu. Clicking the mouse will
	/// attack a location.
	/// </remarks>
	public static void HandleDiscoveryInput()
	{
		int pauseTime = 10000;

		if (SwinGame.KeyTyped (KeyCode.vk_p)) {
			System.Threading.Thread.Sleep (pauseTime);
		}

		if (SwinGame.KeyTyped(KeyCode.vk_ESCAPE)) {
			GameController.AddNewState(GameState.ViewingGameMenu);
		}

		if (SwinGame.KeyTyped (KeyCode.vk_SPACE)) {
			GameController.AddNewState (GameState.Quitting);
		}

		if (SwinGame.MouseClicked(MouseButton.LeftButton)) {
			DoAttack();
		}

		if (SwinGame.MouseClicked (MouseButton.RightButton)) {
			DoSpecial ();
		}
	}

	/// <summary>
	/// Attack the location that the mouse if over.
	/// </summary>
	private static void DoAttack()
	{
		Point2D mouse = default(Point2D);

		mouse = SwinGame.MousePosition();

		//Calculate the row/col clicked
		int row = 0;
		int col = 0;
		row = Convert.ToInt32(Math.Floor((mouse.Y - UtilityFunctions.FIELD_TOP) / (UtilityFunctions.CELL_HEIGHT + UtilityFunctions.CELL_GAP)));
		col = Convert.ToInt32(Math.Floor((mouse.X - UtilityFunctions.FIELD_LEFT) / (UtilityFunctions.CELL_WIDTH + UtilityFunctions.CELL_GAP)));

		if (row >= 0 & row < GameController.HumanPlayer.EnemyGrid.Height) {
			if (col >= 0 & col < GameController.HumanPlayer.EnemyGrid.Width) {
				GameController.Attack(row, col);
			}
		}
	}

	private static void DoSpecial ()
	{
		Point2D mouse = default (Point2D);

		mouse = SwinGame.MousePosition ();

		//Calculate the row/col clicked
		int row = 0;
		int col = 0;
		row = Convert.ToInt32 (Math.Floor ((mouse.Y - UtilityFunctions.FIELD_TOP) / (UtilityFunctions.CELL_HEIGHT + UtilityFunctions.CELL_GAP)));
		col = Convert.ToInt32 (Math.Floor ((mouse.X - UtilityFunctions.FIELD_LEFT) / (UtilityFunctions.CELL_WIDTH + UtilityFunctions.CELL_GAP)));

		if (row >= 0 & row < GameController.HumanPlayer.EnemyGrid.Height) {
			if (col >= 0 & col < GameController.HumanPlayer.EnemyGrid.Width) {
				GameController.SpecialAttack (row, col);
			}
		}
	}

	/// <summary>
	/// Draws the game during the attack phase.
	/// </summary>s
	public static void DrawDiscovery()
	{
		//GameController.Timer();
		//GameController.StopwatchUsingMethod ();

		const int SCORES_LEFT = 172;
		const int SHOTS_TOP = 155;
		const int HITS_TOP = 205;
		const int SPLASH_TOP = 255;

		if (SwinGame.KeyDown(KeyCode.vk_c)) {
			UtilityFunctions.DrawField(GameController.HumanPlayer.EnemyGrid, GameController.ComputerPlayer, true);
		} else {
			UtilityFunctions.DrawField(GameController.HumanPlayer.EnemyGrid, GameController.ComputerPlayer, false);
		}

		if (SwinGame.KeyTyped (KeyCode.vk_m)) 
		{
			GameController.MuteMusic ();
		}

		if (SwinGame.KeyTyped (KeyCode.vk_r)) {
			GameController.ResumeMusic ();
		}

		UtilityFunctions.DrawSmallField(GameController.HumanPlayer.PlayerGrid, GameController.HumanPlayer);
		UtilityFunctions.DrawMessage();

		SwinGame.DrawText(GameController.HumanPlayer.Shots.ToString(), SwinGame.RGBAColor (2, 167, 252, 255), GameResources.GameFont("Score"), SCORES_LEFT, SHOTS_TOP);
		SwinGame.DrawText(GameController.HumanPlayer.Hits.ToString(), SwinGame.RGBAColor (2, 167, 252, 255), GameResources.GameFont("Score"), SCORES_LEFT, HITS_TOP);
		SwinGame.DrawText(GameController.HumanPlayer.Missed.ToString(), SwinGame.RGBAColor (2, 167, 252, 255), GameResources.GameFont("Score"), SCORES_LEFT, SPLASH_TOP);
	}

}




//=======================================================
//Service provided by Telerik (www.telerik.com)
//Conversion powered by NRefactory.
//Twitter: @telerik
//Facebook: facebook.com/telerik
//=======================================================
