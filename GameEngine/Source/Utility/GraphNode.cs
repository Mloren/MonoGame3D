using System;
using System.Collections.Generic;
using System.Text;

//----------------------------------------------------------------------------------
//----------------------------------------------------------------------------------
namespace GameEngine
{
	//----------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------
	public class GraphNode
	{
		private GraphNode m_ParentNode = null;
		private List<GraphNode> m_ChildList = new List<GraphNode>();

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetParent(GraphNode parentNode)
		{
			if(m_ParentNode == parentNode)
				return;

			if(m_ParentNode != null)
				m_ParentNode.RemoveChild(this);

			m_ParentNode = parentNode;

			if(m_ParentNode != null)
				m_ParentNode.AddChild(this);
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public GraphNode GetParent()
		{
			return m_ParentNode;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		private void AddChild(GraphNode childNode)
		{
			if(Assert.True(!m_ChildList.Contains(childNode), "Child already added to node:", childNode))
			{
				m_ChildList.Add(childNode);
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		private void RemoveChild(GraphNode childNode)
		{
			m_ChildList.Remove(childNode);
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public int GetChildCount()
		{
			return m_ChildList.Count;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public GraphNode GetChild(int nChildIndex)
		{
			if(Assert.True(nChildIndex < m_ChildList.Count, "GetChild: index out of range."))
			{
				return m_ChildList[nChildIndex];
			}
			return null;
		}
	}
}
