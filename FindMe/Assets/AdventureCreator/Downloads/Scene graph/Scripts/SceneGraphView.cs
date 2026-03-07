#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;

namespace AC.Downloads.SceneGraphTool
{

	public class SceneGraphView : GraphView
	{

		#region Variables

		private readonly SceneGraph sceneGraph;
		private readonly Vector2 defaultNodeSize = new Vector2 (100, 150);
		private readonly new List<SceneGraphNode> nodes = new List<SceneGraphNode> ();

		#endregion


		#region Constructors

		public SceneGraphView (SceneGraph _sceneGraph)
		{
			SetupZoom (ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

			this.AddManipulator (new ContentDragger ());
			this.AddManipulator (new SelectionDragger ());
			this.AddManipulator (new RectangleSelector ());

			sceneGraph = _sceneGraph;

			Refresh ();
		}

		#endregion


		#region PublicFunctions

		public void Refresh ()
		{
			foreach (SceneGraphNode node in nodes)
			{
				foreach (Edge edge in edges.ToList ())
				{
					RemoveElement (edge);
				}
				
				RemoveElement (node);
			}

			GenerateNodes ();
			GenerateNodeLinks ();
			
			UnityEditor.EditorApplication.delayCall += () =>
			{
				FrameAll ();
			}; 
		}


		public void Save ()
		{
			List<Edge> Edges = edges.ToList ();

			foreach (SceneGraphNode node in nodes)
			{
				bool doContinue = false;

				foreach (SceneData sceneData in sceneGraph.GraphData.SceneDatas)
				{
					if (sceneData.SceneName == node.SceneName)
					{
						sceneData.UpdatePosition (node.GetPosition ().position);

						// save connections
						for (int p = 0; p < sceneData.ExitNodes.Length; p++)
						{
							SceneExitNode currentExitNode = sceneData.ExitNodes[p];

							bool haveData = false;
							foreach (Edge edge in Edges)
							{
								if (edge.output.node != node) continue;
								if (edge.output.portName != currentExitNode.Name) continue;

								string inputName = edge.input.portName;

								SceneGraphNode connectedNode = (SceneGraphNode) edge.input.node;
								string connectedSceneName = connectedNode.SceneName;
								SceneData connectedSceneData = GetDataForScene (connectedSceneName);
								if (connectedSceneData == null) continue;

								for (int e = 0; e < connectedSceneData.ExitNodes.Length; e++)
								{
									if (connectedSceneData.ExitNodes[e].Name == inputName)
									{
										currentExitNode.UpdateConnection (connectedSceneName, connectedSceneData.ExitNodes[e].ConstantID);
										haveData = true;
									}
								}
							}

							if (!haveData)
							{
								currentExitNode.ClearConnection ();
							}
						}

						doContinue = true;
					}
				}

				if (doContinue)
				{
					continue;
				}
			}
		}


		public override List<Port> GetCompatiblePorts (Port startPort, NodeAdapter nodeAdapter)
		{
			List<Port> compatiblePorts = new List<Port> ();

			ports.ForEach (port =>
			{
				if (startPort != port && startPort.node != port.node)
				{
					compatiblePorts.Add (port);
				}
			});

			return compatiblePorts;
		}

		#endregion


		#region PrivateFunctions

		private void GenerateNodes ()
		{
			nodes.Clear ();

			foreach (SceneData sceneData in sceneGraph.GraphData.SceneDatas)
			{
				SceneGraphNode node = new SceneGraphNode (sceneData.SceneName);
				node.SetPosition (new Rect (sceneData.NodePosition, defaultNodeSize));

				foreach (SceneExitNode exitNode in sceneData.ExitNodes)
				{
					Port outputPort = node.InstantiatePort (Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null);
					outputPort.portName = exitNode.Name;
					node.outputContainer.Add (outputPort);

					Port inputPort = node.InstantiatePort (Orientation.Horizontal, Direction.Input, Port.Capacity.Single, null);
					inputPort.portName = exitNode.Name;
					node.inputContainer.Add (inputPort);
				}

				node.RefreshExpandedState ();
				node.RefreshPorts ();

				nodes.Add (node);
				AddElement (node);
			}
		}


		private void GenerateNodeLinks ()
		{
			foreach (SceneData sceneData in sceneGraph.GraphData.SceneDatas)
			{
				SceneGraphNode node = GetNodeForScene (sceneData.SceneName);
				if (node == null) continue;

				for (int i = 0; i < sceneData.ExitNodes.Length; i++)
				{
					SceneExitNode exitNode = sceneData.ExitNodes[i];

					if (!string.IsNullOrEmpty (exitNode.ConnectedSceneName) && exitNode.ConnectedConstantID != 0)
					{
						SceneGraphNode connectedNode = GetNodeForScene (exitNode.ConnectedSceneName);
						if (connectedNode == null) continue;

						SceneData connectedSceneData = GetDataForScene (exitNode.ConnectedSceneName);
						if (connectedSceneData == null) continue;

						int connectedPortIndex = GetPortIndex (connectedSceneData, exitNode.ConnectedConstantID);
						if (connectedPortIndex < 0) continue;

						LinkNodes ((Port) node.outputContainer[i], (Port) connectedNode.inputContainer[connectedPortIndex]);
					}
				}
			}
		}


		private int GetPortIndex (SceneData sceneData, int constantID)
		{
			if (constantID != 0)
			{
				for (int i = 0; i < sceneData.ExitNodes.Length; i++)
				{
					if (sceneData.ExitNodes[i].ConstantID == constantID)
					{
						return i;
					}
				}
			}
			return -1;
		}


		private SceneGraphNode GetNodeForScene (string sceneName)
		{
			foreach (SceneGraphNode node in nodes)
			{
				if (node.SceneName == sceneName)
				{
					return node;
				}
			}
			return null;
		}


		private SceneData GetDataForScene (string sceneName)
		{
			foreach (SceneData sceneData in sceneGraph.GraphData.SceneDatas)
			{
				if (sceneData.SceneName == sceneName)
				{
					return sceneData;
				}
			}
			return null;
		}


		private void LinkNodes (Port output, Port input)
		{
			Edge edge = new Edge ();
			edge.input = input;
			edge.output = output;

			edge?.input.Connect (edge);
			edge?.output.Connect (edge);

			Color color = new Color (0f, 0.5f, 1f, 1f);
			input.portColor = color;
			output.portColor = color;

			Add (edge);
		}

		#endregion

	}

}

#endif