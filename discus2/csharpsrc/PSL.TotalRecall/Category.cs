using System;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Represents a category in the database
	/// </summary>
	public class Category
	{

		private string name;
		private string policyId;

		public Category()
		{
			
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

		public string PolicyID 
		{
			get 
			{
				return policyId;
			}

			set 
			{
				policyId = value;
			}

		}

		public override string ToString() 
		{
			return name;
		}


	}
}
