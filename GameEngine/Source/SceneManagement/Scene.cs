using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine
{
	public abstract class Scene : GraphNode
	{
		public abstract void Update(float fDeltaTime);
	}
}
