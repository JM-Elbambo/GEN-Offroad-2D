using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;
using System.Data;

public class SQLManager : MonoBehaviour
{
	const string databaseFileName = "database.db";
	string connectionString;

	private void Start()
    {
		connectionString = "URI=file:" + Application.persistentDataPath + "/" + databaseFileName;

		// Initialize Tables
		InitializeSessionTable();
	}

	void InitializeSessionTable()
    {
		using (IDbConnection dbConnection = new SqliteConnection(connectionString))
		{
			dbConnection.Open();

			// Crate table if it doesn't exist
			using (IDbCommand dbcmd = dbConnection.CreateCommand())
			{
				string query = "CREATE TABLE IF NOT EXISTS gameSession (" +
				"sessionNumber INTEGER PRIMARY KEY, " +
				"playerName varchar(20), " +
				"totalScore INTEGER, " +
				"bonusScore INTEGER, " +
				"distanceTravelled FLOAT, " +
				"gameDuration FLOAT" +
				");";
				dbcmd.CommandText = query;

				dbcmd.ExecuteReader();
			}
		}
	}

	public void AddToSession(SessionInfo sessionInfo)
	{
		using (IDbConnection dbConnection = new SqliteConnection(connectionString))
		{
			dbConnection.Open();

			// Create INSERT command
			using (IDbCommand cmnd = dbConnection.CreateCommand())
			{
				string query = "INSERT INTO gameSession " +
					"(playerName, totalScore, bonusScore, distanceTravelled, gameDuration) " +
					"VALUES (@playerName, @totalScore, @bonusScore, @distanceTravelled, @gameDuration);";
				cmnd.CommandText = query;

				cmnd.Parameters.Add(new SqliteParameter("@playerName", sessionInfo.playerName));
				cmnd.Parameters.Add(new SqliteParameter("@totalScore", sessionInfo.totalScore));
				cmnd.Parameters.Add(new SqliteParameter("@bonusScore", sessionInfo.bonusScore));
				cmnd.Parameters.Add(new SqliteParameter("@distanceTravelled", sessionInfo.distanceTravelled));
				cmnd.Parameters.Add(new SqliteParameter("@gameDuration", sessionInfo.gameDuration));

				cmnd.ExecuteNonQuery();
			}
		}
    }

	public List<string[]> GetLeaderboard(int count)
    {
		List<string[]> leaderboard = new List<string[]>();

		using (IDbConnection dbConnection = new SqliteConnection(connectionString))
		{
			dbConnection.Open();

			// Create SELECT command
			using (IDbCommand cmnd = dbConnection.CreateCommand())
			{
				IDataReader reader;
				string query = "SELECT sessionNumber, playerName, totalScore FROM gameSession " +
					"ORDER BY totalScore DESC limit @limit";
				cmnd.CommandText = query;
				cmnd.Parameters.Add(new SqliteParameter("@limit", count));
				reader = cmnd.ExecuteReader();

				// Fetch all results
				while (reader.Read())
				{
					leaderboard.Add((from i in Enumerable.Range(0, 3) select reader[i].ToString()).ToArray());
				}
			}
		}

		return leaderboard;
    }

	public string[] GetLeaderboardEntryInfo(int sessionNumber)
	{
		string[] leaderboardEntryInfo = new string[] { };

		using (IDbConnection dbConnection = new SqliteConnection(connectionString))
		{
			dbConnection.Open();

			// Create SELECT command
			using (IDbCommand cmnd = dbConnection.CreateCommand())
			{
				IDataReader reader;
				string query = "SELECT bonusScore, distanceTravelled, gameDuration FROM gameSession " +
					"WHERE sessionNumber = @sessionNumber";
				cmnd.CommandText = query;
				cmnd.Parameters.Add(new SqliteParameter("@sessionNumber", sessionNumber));
				reader = cmnd.ExecuteReader();

				// Fetch results
				if (reader.Read())
				{
					leaderboardEntryInfo = (from i in Enumerable.Range(0, 3) select reader[i].ToString()).ToArray();
				}
			}
		}

		return leaderboardEntryInfo;
	}
}
