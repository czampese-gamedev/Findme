using System;
using UnityEngine;

namespace AC.Downloads.SceneGraphTool
{

	[Serializable]
	public class SceneExitNode
	{

		#region Variables

		[SerializeField] private string name;
		[SerializeField] private int constantID;
		[SerializeField] private string connectedSceneName;
		[SerializeField] private int connectedConstantID;

		#endregion


		#region Constructors

		public SceneExitNode (string _name, int _constantID)
		{
			name = _name;
			constantID = _constantID;
		}

		#endregion


		#region PublicFunctions

		public void RestoreBackup (SceneExitNode backupExitNode)
		{
			connectedSceneName = backupExitNode.ConnectedSceneName;
			connectedConstantID = backupExitNode.ConnectedConstantID;
		}


		public void UpdateConnection (string _connectedSceneName, int _connectedConstantID)
		{
			connectedSceneName = _connectedSceneName;
			connectedConstantID = _connectedConstantID;
		}


		public void ClearConnection ()
		{
			UpdateConnection (string.Empty, 0);
		}

		#endregion


		#region GetSet

		public int ConstantID { get { return constantID; } }
		public string ConnectedSceneName { get { return connectedSceneName; } }
		public int ConnectedConstantID { get { return connectedConstantID; } }
		public string Name { get { return name; } }

		#endregion

	}

}