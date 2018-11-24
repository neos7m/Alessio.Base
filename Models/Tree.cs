using System;
using System.Collections.Generic;
using System.Text;

namespace Alessio.Base.Models
{
	public class TreeNode<T>
	{
		public T Value { get; set; }

		public TreeNode<T> Parent { get; set; }
		public List<TreeNode<T>> Children { get; set; }

		public bool IsLeaf => Children.Count == 0;

		public TreeNode(T value = default(T), TreeNode<T> parent = null)
		{
			Value = value;
			Parent = parent;
			Children = new List<TreeNode<T>>();
		}

		public void AddSubtree(TreeNode<T> node)
		{
			node.Parent = this;
			Children.Add(node);
		}
	}
}
