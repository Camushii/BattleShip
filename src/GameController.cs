
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Data;
using System.Diagnostics;
using SwinGameSDK;
using System.Timers;
/// <summary>
/// The GameController is responsible for controlling the game,
/// managing user input, and displaying the current state of the
/// game.
/// </summary>
public static class GameController
{

	private static BattleShipsGame _theGame;
	private static Player _human;

	private static AIPlayer _ai;
	//private static DateTime _myDateTime;
	//private static System.Timers.Timer _timer;
	private static Stack<GameState> _state = new Stack<GameState>();

	private static AIOption _aiSetting;
	/// <summary>
	/// Returns the current state of the game, indicating which screen is
	/// currently being used
	/// </summary>
	/// <value>The current state</value>
	/// <returns>The current state</returns>
	public static GameState CurrentState {
		get { return _state.Peek(); }
	}

	/// <summary>
	/// Returns the human player.
	/// </summary>
	/// <value>the human player</value>
	/// <returns>the human player</returns>
	public static Player HumanPlayer {
		get { return _human; }
	}

	/// <summary>
	/// Returns the computer player.
	/// </summary>
	/// <value>the computer player</value>
	/// <returns>the conputer player</returns>
	public static Player ComputerPlayer {
		get { return _ai; }
	}

	static GameController()
	{
		//bottom state will be quitting. If player exits main menu then the game is over
		_state.Push(GameState.Quitting);

		//at the start the player is viewing the main menu
		_state.Push(GameState.ViewingMainMenu);
	}

	/// <summary>
	/// Starts a new game.
	/// </summary>
	/// <remarks>
	/// Creates an AI player based upon the _aiSetting.
	/// </remarks>
	public static void StartGame()
	{
		if (_theGame != null)
			EndGame();

		//Create the game
		_theGame = new BattleShipsGame();

		//create the players
		switch (_aiSetting) {
			case AIOption.Easy:
				_ai = new AIEasyPlayer (_theGame);
				break;
			case AIOption.Medium:
				_ai = new AIMediumPlayer(_theGame);
				break;
			case AIOption.Hard:
				_ai = new AIHardPlayer(_theGame);
				break;
			default:
				_ai = new AIMediumPlayer(_theGame);
				break;
		}

		_human = new Player(_theGame);

		//AddHandler _human.PlayerGrid.Changed, AddressOf GridChanged
		_ai.PlayerGrid.Changed += GridChanged;
		_theGame.AttackCompleted += AttackCompleted;

		AddNewState(GameState.Deploying);
	}

	/// <summary>
	/// Stops listening to the old game once a new game is started
	/// </summary>

	private static void EndGame()
	{
		//RemoveHandler _human.PlayerGrid.Changed, AddressOf GridChanged
		_ai.PlayerGrid.Changed -= GridChanged;
		_theGame.AttackCompleted -= AttackCompleted;
	}

	/// <summary>
	/// Listens to the game grids for any changes and redraws the screen
	/// when the grids change
	/// </summary>
	/// <param name="sender">the grid that changed</param>
	/// <param name="args">not used</param>
	private static void GridChanged(object sender, EventArgs args)
	{
		DrawScreen();
		SwinGame.RefreshScreen();
	}

	private static void PlayHitSequence(int row, int column, bool showAnimation)
	{
		if (showAnimation) {
			UtilityFunctions.AddExplosion(row, column);
		}

		Audio.PlaySoundEffect(GameResources.GameSound("Hit"));

		UtilityFunctions.DrawAnimationSequence();
	}

	private static void PlayMissSequence(int row, int column, bool showAnimation)
	{
		if (showAnimation) {
			UtilityFunctions.AddSplash(row, column);
		}

		Audio.PlaySoundEffect(GameResources.GameSound("Miss"));

		UtilityFunctions.DrawAnimationSequence();
	}

	/// <summary>
	/// Listens for attacks to be completed.
	/// </summary>
	/// <param name="sender">the game</param>
	/// <param name="result">the result of the attack</param>
	/// <remarks>
	/// Displays a message, plays sound and redraws the screen
	/// </remarks>
	private static void AttackCompleted(object sender, AttackResult result)
	{
		bool isHuman = false;
		isHuman = object.ReferenceEquals(_theGame.Player, HumanPlayer);

		if (isHuman) {
			UtilityFunctions.Message = "You " + result.ToString();
		} else {
			UtilityFunctions.Message = "The AI " + result.ToString();
		}

		switch (result.Value) {
			case ResultOfAttack.Destroyed:
				PlayHitSequence(result.Row, result.Column, isHuman);
			Audio.PlaySoundEffect(GameResources.GameSound("Sink"));

				break;
			case ResultOfAttack.GameOver:
				PlayHitSequence(result.Row, result.Column, isHuman);
			Audio.PlaySoundEffect(GameResources.GameSound("Sink"));

				while (Audio.SoundEffectPlaying(GameResources.GameSound("Sink"))) {
					SwinGame.Delay(10);
					SwinGame.RefreshScreen();
				}

				if (HumanPlayer.IsDestroyed) {
				Audio.PlaySoundEffect(GameResources.GameSound("Lose"/*\nThe time taken: "+ _timer*/));
				} else {
					Audio.PlaySoundEffect(GameResources.GameSound("Winner"/*\\nThe time taken: " + _timer*/));
				}

				break;
			case ResultOfAttack.Hit:
				PlayHitSequence(result.Row, result.Column, isHuman);
				break;
			case ResultOfAttack.Miss:
				PlayMissSequence(result.Row, result.Column, isHuman);
				break;
			case ResultOfAttack.ShotAlready:
				Audio.PlaySoundEffect(GameResources.GameSound("Error"));
				break;
		}
	}

	/// <summary>
	/// Completes the deployment phase of the game and
	/// switches to the battle mode (Discovering state)
	/// </summary>
	/// <remarks>
	/// This adds the players to the game before switching
	/// state.
	/// </remarks>
	public static void EndDeployment()
	{
		//deploy the players
		_theGame.AddDeployedPlayer(_human);
		_theGame.AddDeployedPlayer(_ai);

		SwitchState(GameState.Discovering);
	}

	/// <summary>
	/// Mutes the music.
	/// </summary>
	public static void MuteMusic ()
	{
		SwinGame.StopMusic ();
	}

	/// <summary>
	/// Resumes the music.
	/// </summary>
	public static void ResumeMusic ()
	{
		SwinGame.PlayMusic (GameResources.GameMusic ("Background"));
	}



	/*public static void StopwatchUsingMethod ()
	{
		var timer = new Stopwatch ();
		timer.Start ();

		TimeSpan timetaken = timer.Elapsed;
		string time = "Time: " + timetaken.ToString (@"m\:ss\.fff");
		SwinGame.DrawText (time, SwinGame.RGBAColor (2, 167, 252, 255), GameResources.GameFont ("Menu"), 172, 87);
	}
	/// <summary>
	/// Start the timer
	/// </summary>
	public static void Timer ()
	{
		_timer = new System.Timers.Timer ();
		_timer.Interval = 1000;
		_timer.Elapsed += Timer_Tick;

		_myDateTime = DateTime.Now;
		_timer.Start ();
		SwinGame.DrawText ("Time:"+_timer, SwinGame.RGBAColor (2, 167, 252, 255), GameResources.GameFont ("Menu"), 172, 87);
	}

	public static void Timer_Tick (object sender, EventArgs e)
	{
		var diff = DateTime.Now.Subtract (_myDateTime);
		//this.textBox1.Text = diff.ToString ();
	}*/

	/// <summary>
	/// Gets the player to attack the indicated row and column.
	/// </summary>
	/// <param name="row">the row to attack</param>
	/// <param name="col">the column to attack</param>
	/// <remarks>
	/// Checks the attack result once the attack is complete
	/// </remarks>
	public static void Attack(int row, int col)
	{
		AttackResult result = default(AttackResult);
		result = _theGame.Shoot(row, col);
		CheckAttackResult(result);
	}

	public static void SpecialAttack (int row, int col)
	{
		AttackResult result = default (AttackResult);
		result = _theGame.Special (row, col);
		CheckSpecialAttackResult (result, row, col);
	}

	/// <summary>
	/// Gets the AI to attack.
	/// </summary>
	/// <remarks>
	/// Checks the attack result once the attack is complete.
	/// </remarks>
	private static void AIAttack()
	{
		AttackResult result = default(AttackResult);
		result = _theGame.Player.Attack();
		CheckAttackResult(result);
	}

	/// <summary>
	/// Checks the results of the attack and switches to
	/// Ending the Game if the result was game over.
	/// </summary>
	/// <param name="result">the result of the last
	/// attack</param>
	/// <remarks>Gets the AI to attack if the result switched
	/// to the AI player.</remarks>
	private static void CheckAttackResult(AttackResult result)
	{
		switch (result.Value) {
			case ResultOfAttack.Miss:
				if (object.ReferenceEquals(_theGame.Player, ComputerPlayer))
					AIAttack();
				break;
					
			case ResultOfAttack.GameOver:
				SwitchState(GameState.EndingGame);
				break;
		}
	}

	private static void CheckSpecialAttackResult(AttackResult result, int row, int col)
	{
		switch (result.Value) {
		case ResultOfAttack.Miss:
			if (object.ReferenceEquals (_theGame.Player, ComputerPlayer))
				SpecialAttack (row, col);
			//AIAttack ();
			break;

		case ResultOfAttack.GameOver:
			SwitchState (GameState.EndingGame);
			break;
		}
	}

	/// <summary>
	/// Handles the user SwinGame.
	/// </summary>
	/// <remarks>
	/// Reads key and mouse input and converts these into
	/// actions for the game to perform. The actions
	/// performed depend upon the state of the game.
	/// </remarks>
	public static void HandleUserInput()
	{
		//Read incoming input events
		SwinGame.ProcessEvents();

		switch (CurrentState) {
			case GameState.ViewingMainMenu:
				MenuController.HandleMainMenuInput();
				break;
			case GameState.ViewingGameMenu:
				MenuController.HandleGameMenuInput();
				break;
			case GameState.AlteringSettings:
				MenuController.HandleSetupMenuInput();
				break;
			case GameState.Deploying:
				DeploymentController.HandleDeploymentInput();
				break;
			case GameState.Discovering:
				DiscoveryController.HandleDiscoveryInput ();
				break;
			case GameState.EndingGame:
				EndingGameController.HandleEndOfGameInput();
				break;
			case GameState.ViewingHighScores:
				HighScoreController.HandleHighScoreInput();
				break;
		}

		UtilityFunctions.UpdateAnimations();
	}

	/// <summary>
	/// Draws the current state of the game to the screen.
	/// </summary>
	/// <remarks>
	/// What is drawn depends upon the state of the game.
	/// </remarks>
	public static void DrawScreen()
	{
		UtilityFunctions.DrawBackground();

		switch (CurrentState) {
			case GameState.ViewingMainMenu:
				MenuController.DrawMainMenu();
				break;
			case GameState.ViewingGameMenu:
				MenuController.DrawGameMenu();
				break;
			case GameState.AlteringSettings:
				MenuController.DrawSettings();
				break;
			case GameState.Deploying:
				DeploymentController.DrawDeployment();
				break;
			case GameState.Discovering:
				DiscoveryController.DrawDiscovery();
				break;
			case GameState.EndingGame:
				EndingGameController.DrawEndOfGame();
				break;
			case GameState.ViewingHighScores:
				HighScoreController.DrawHighScores();
				break;
		}

		UtilityFunctions.DrawAnimations();

		SwinGame.RefreshScreen();
	}

	/// <summary>
	/// Move the game to a new state. The current state is maintained
	/// so that it can be returned to.
	/// </summary>
	/// <param name="state">the new game state</param>
	public static void AddNewState(GameState state)
	{
		_state.Push(state);
		UtilityFunctions.Message = "";
	}

	/// <summary>
	/// End the current state and add in the new state.
	/// </summary>
	/// <param name="newState">the new state of the game</param>
	public static void SwitchState(GameState newState)
	{
		EndCurrentState();
		AddNewState(newState);
	}

	/// <summary>
	/// Ends the current state, returning to the prior state
	/// </summary>
	public static void EndCurrentState()
	{
		_state.Pop();
	}

	/// <summary>
	/// Sets the difficulty for the next level of the game.
	/// </summary>
	/// <param name="setting">the new difficulty level</param>
	public static void SetDifficulty(AIOption setting)
	{
		_aiSetting = setting;
	}

}

//=======================================================
//Service provided by Telerik (www.telerik.com)
//Conversion powered by NRefactory.
//Twitter: @telerik
//Facebook: facebook.com/telerik
//=======================================================
