using System;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Encapsulates an access policy
	/// </summary>
	public class AccessPolicy
	{
		private string id;
		private string name;
		private string doc;
		
		public AccessPolicy()
		{
		}

		public string Id 
		{
			get 
			{
				return id;
			}

			set 
			{
				id = value;
			}
		}

		public string Name 
		{
			get 
			{
				return name;
			}

			set 
			{
				name = value;
			}
		}

		public string Document
		{
			get 
			{
				return doc;
			}

			set 
			{
				doc = value;
			}
		}

		public override string ToString() 
		{
			return name + " [id=" + id + "]";
		}
		

	}
}
