using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using SwinGameSDK;

/// <summary>
/// The AIEasyPlayer is a type of AIPlayer where it will only search for new target even when a ship is found.
/// </summary>
public class AIEasyPlayer : AIPlayer
{
	public AIEasyPlayer (BattleShipsGame controller) : base (controller)
	{
	}

	/// <summary>
	/// Generates random shooting coordinates.
	/// </summary>
	/// <param name="row">Row.</param>
	/// <param name="column">Column.</param>
	protected override void GenerateCoords (ref int row, ref int column)
	{
		do {
			SearchCoords (ref row, ref column);
		} while ((row < 0 || column < 0 || row >= EnemyGrid.Height || column >= EnemyGrid.Width || EnemyGrid [row, column] != TileView.Sea));

	}

	/// <summary>
	/// Randomly generate shots.
	/// </summary>
	/// <param name="row">Row.</param>
	/// <param name="column">Column.</param>
	private void SearchCoords (ref int row, ref int column)
	{
		row = _Random.Next (0, EnemyGrid.Height);
		column = _Random.Next (0, EnemyGrid.Width);
	}

	protected override void ProcessShot (int row, int col, AttackResult result)
	{
	}

}
