using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;	

using PSL.TotalRecall;

namespace PSL.TotalRecall.PolicyManager
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class PolicyEditor : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TabPage resourcesTab;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TabPage categoriesTab;
		private System.Windows.Forms.TabPage policiesTab;
		private System.Windows.Forms.Button evaluatePolicyButton;
		private System.Windows.Forms.TextBox policyText;
		private System.Windows.Forms.Button addCategoryToResourceButton;
		private System.Windows.Forms.ListBox resourceCategoriesList;
		private System.Windows.Forms.ListBox resourcesList;
		private System.Windows.Forms.ListBox availableCategoriesList;
		private System.Windows.Forms.ListBox categoriesList;
		private System.Windows.Forms.Button removeCategoryFromResourceButton;
		private System.Windows.Forms.Button setPolicyButton;
		private System.Windows.Forms.ListBox availablePoliciesList;
		private System.Windows.Forms.Button newPolicyButton;
		private System.Windows.Forms.TextBox currentPolicyText;
		private System.Windows.Forms.Button addResourceButton;
		private System.Windows.Forms.ListBox policiesList;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox policyIDText;
		private System.Windows.Forms.Button modifyPolicyButton;
		private System.Windows.Forms.Button newCategoryButton;

		private PolicyDAO policyDAO;
		private System.Windows.Forms.TabControl tabs;
		private ResourceDAO resourceDAO;
		private CategoryDAO categoryDAO;

		public PolicyEditor()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			policyDAO = new PolicyDAO(PolicyManager.DatabaseConnectionString);
			resourceDAO = new ResourceDAO(PolicyManager.DatabaseConnectionString);
			categoryDAO = new CategoryDAO(PolicyManager.DatabaseConnectionString);

			RefreshResourceList();
			RefreshCategoryLists();

		}
		
		/// <summary>
		/// Loads all resource names into the resources list
		/// </summary>
		private void RefreshResourceList()
		{
			ArrayList resourceList = resourceDAO.GetAllResources();
			Resource[] resources = (Resource[]) resourceList.ToArray(typeof(Resource));
			
			resourcesList.Items.Clear();
			resourcesList.Items.AddRange(resources);

		}

		/// <summary>
		/// Loads all category names into the categories lists
		/// </summary>
		private void RefreshCategoryLists()
		{

			ArrayList categoryList = categoryDAO.GetAllCategories();
			Category[] categories = (Category[]) categoryList.ToArray(typeof(Category));

			categoriesList.Items.Clear();
			categoriesList.Items.AddRange(categories);
			
			availableCategoriesList.Items.Clear();
			availableCategoriesList.Items.AddRange(categories);

		}

		/// <summary>
		/// Loads the categories for this resource
		/// </summary>
		private void RefreshResourceCategoryList() 
		{
			Resource res = resourcesList.SelectedItem;
			if (res == null) 
			{
				return;
			}
			
			ArrayList categoryList = resourceDAO.GetResourceCategories(res.ID);
			Category[] categories = (Category[]) categoryList.ToArray(typeof(Category));

			resourceCategoriesList.Items.Clear();
			resourceCategoriesList.Items.AddRange(categories);
			
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tabs = new System.Windows.Forms.TabControl();
			this.resourcesTab = new System.Windows.Forms.TabPage();
			this.addResourceButton = new System.Windows.Forms.Button();
			this.addCategoryToResourceButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.resourceCategoriesList = new System.Windows.Forms.ListBox();
			this.resourcesList = new System.Windows.Forms.ListBox();
			this.availableCategoriesList = new System.Windows.Forms.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.removeCategoryFromResourceButton = new System.Windows.Forms.Button();
			this.categoriesTab = new System.Windows.Forms.TabPage();
			this.currentPolicyText = new System.Windows.Forms.TextBox();
			this.setPolicyButton = new System.Windows.Forms.Button();
			this.availablePoliciesList = new System.Windows.Forms.ListBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.newCategoryButton = new System.Windows.Forms.Button();
			this.categoriesList = new System.Windows.Forms.ListBox();
			this.label4 = new System.Windows.Forms.Label();
			this.policiesTab = new System.Windows.Forms.TabPage();
			this.policyIDText = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.policiesList = new System.Windows.Forms.ListBox();
			this.evaluatePolicyButton = new System.Windows.Forms.Button();
			this.newPolicyButton = new System.Windows.Forms.Button();
			this.policyText = new System.Windows.Forms.TextBox();
			this.modifyPolicyButton = new System.Windows.Forms.Button();
			this.tabs.SuspendLayout();
			this.resourcesTab.SuspendLayout();
			this.categoriesTab.SuspendLayout();
			this.policiesTab.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabs
			// 
			this.tabs.Controls.AddRange(new System.Windows.Forms.Control[] {
																			   this.resourcesTab,
																			   this.categoriesTab,
																			   this.policiesTab});
			this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(392, 397);
			this.tabs.TabIndex = 7;
			this.tabs.SelectedIndexChanged += new System.EventHandler(this.tabs_SelectedIndexChanged);
			// 
			// resourcesTab
			// 
			this.resourcesTab.Controls.AddRange(new System.Windows.Forms.Control[] {
																					   this.addResourceButton,
																					   this.addCategoryToResourceButton,
																					   this.label1,
																					   this.resourceCategoriesList,
																					   this.resourcesList,
																					   this.availableCategoriesList,
																					   this.label2,
																					   this.label3,
																					   this.removeCategoryFromResourceButton});
			this.resourcesTab.Location = new System.Drawing.Point(4, 22);
			this.resourcesTab.Name = "resourcesTab";
			this.resourcesTab.Size = new System.Drawing.Size(384, 371);
			this.resourcesTab.TabIndex = 0;
			this.resourcesTab.Text = "Resources";
			// 
			// addResourceButton
			// 
			this.addResourceButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.addResourceButton.Location = new System.Drawing.Point(8, 320);
			this.addResourceButton.Name = "addResourceButton";
			this.addResourceButton.Size = new System.Drawing.Size(96, 24);
			this.addResourceButton.TabIndex = 14;
			this.addResourceButton.Text = "Add resource...";
			// 
			// addCategoryToResourceButton
			// 
			this.addCategoryToResourceButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.addCategoryToResourceButton.Location = new System.Drawing.Point(176, 248);
			this.addCategoryToResourceButton.Name = "addCategoryToResourceButton";
			this.addCategoryToResourceButton.Size = new System.Drawing.Size(32, 24);
			this.addCategoryToResourceButton.TabIndex = 13;
			this.addCategoryToResourceButton.Text = "<";
			this.addCategoryToResourceButton.Click += new System.EventHandler(this.addCategoryToResourceButton_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(152, 16);
			this.label1.TabIndex = 11;
			this.label1.Text = "Resources";
			// 
			// resourceCategoriesList
			// 
			this.resourceCategoriesList.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.resourceCategoriesList.Location = new System.Drawing.Point(8, 232);
			this.resourceCategoriesList.Name = "resourceCategoriesList";
			this.resourceCategoriesList.Size = new System.Drawing.Size(160, 82);
			this.resourceCategoriesList.TabIndex = 9;
			// 
			// resourcesList
			// 
			this.resourcesList.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.resourcesList.Location = new System.Drawing.Point(8, 24);
			this.resourcesList.Name = "resourcesList";
			this.resourcesList.Size = new System.Drawing.Size(368, 186);
			this.resourcesList.TabIndex = 7;
			this.resourcesList.SelectedIndexChanged += new System.EventHandler(this.resourcesList_SelectedIndexChanged);
			// 
			// availableCategoriesList
			// 
			this.availableCategoriesList.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.availableCategoriesList.Location = new System.Drawing.Point(216, 232);
			this.availableCategoriesList.Name = "availableCategoriesList";
			this.availableCategoriesList.Size = new System.Drawing.Size(160, 82);
			this.availableCategoriesList.TabIndex = 8;
			// 
			// label2
			// 
			this.label2.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.label2.Location = new System.Drawing.Point(8, 216);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(152, 16);
			this.label2.TabIndex = 10;
			this.label2.Text = "In categories:";
			// 
			// label3
			// 
			this.label3.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.label3.Location = new System.Drawing.Point(216, 216);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(152, 16);
			this.label3.TabIndex = 12;
			this.label3.Text = "Available categories:";
			// 
			// removeCategoryFromResourceButton
			// 
			this.removeCategoryFromResourceButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.removeCategoryFromResourceButton.Location = new System.Drawing.Point(176, 280);
			this.removeCategoryFromResourceButton.Name = "removeCategoryFromResourceButton";
			this.removeCategoryFromResourceButton.Size = new System.Drawing.Size(32, 24);
			this.removeCategoryFromResourceButton.TabIndex = 13;
			this.removeCategoryFromResourceButton.Text = ">";
			// 
			// categoriesTab
			// 
			this.categoriesTab.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.currentPolicyText,
																						this.setPolicyButton,
																						this.availablePoliciesList,
																						this.label5,
																						this.label6,
																						this.newCategoryButton,
																						this.categoriesList,
																						this.label4});
			this.categoriesTab.Location = new System.Drawing.Point(4, 22);
			this.categoriesTab.Name = "categoriesTab";
			this.categoriesTab.Size = new System.Drawing.Size(384, 371);
			this.categoriesTab.TabIndex = 1;
			this.categoriesTab.Text = "Categories";
			// 
			// currentPolicyText
			// 
			this.currentPolicyText.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.currentPolicyText.Location = new System.Drawing.Point(8, 232);
			this.currentPolicyText.Name = "currentPolicyText";
			this.currentPolicyText.ReadOnly = true;
			this.currentPolicyText.Size = new System.Drawing.Size(160, 20);
			this.currentPolicyText.TabIndex = 22;
			this.currentPolicyText.Text = "";
			// 
			// setPolicyButton
			// 
			this.setPolicyButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.setPolicyButton.Location = new System.Drawing.Point(176, 232);
			this.setPolicyButton.Name = "setPolicyButton";
			this.setPolicyButton.Size = new System.Drawing.Size(32, 24);
			this.setPolicyButton.TabIndex = 21;
			this.setPolicyButton.Text = "<";
			// 
			// availablePoliciesList
			// 
			this.availablePoliciesList.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.availablePoliciesList.Location = new System.Drawing.Point(216, 232);
			this.availablePoliciesList.Name = "availablePoliciesList";
			this.availablePoliciesList.Size = new System.Drawing.Size(160, 82);
			this.availablePoliciesList.TabIndex = 16;
			// 
			// label5
			// 
			this.label5.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.label5.Location = new System.Drawing.Point(8, 216);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(152, 16);
			this.label5.TabIndex = 18;
			this.label5.Text = "Current Policy";
			// 
			// label6
			// 
			this.label6.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.label6.Location = new System.Drawing.Point(216, 216);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(152, 16);
			this.label6.TabIndex = 19;
			this.label6.Text = "Available policies:";
			// 
			// newCategoryButton
			// 
			this.newCategoryButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.newCategoryButton.Location = new System.Drawing.Point(8, 328);
			this.newCategoryButton.Name = "newCategoryButton";
			this.newCategoryButton.Size = new System.Drawing.Size(104, 24);
			this.newCategoryButton.TabIndex = 15;
			this.newCategoryButton.Text = "New category...";
			// 
			// categoriesList
			// 
			this.categoriesList.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.categoriesList.Location = new System.Drawing.Point(8, 24);
			this.categoriesList.Name = "categoriesList";
			this.categoriesList.Size = new System.Drawing.Size(368, 186);
			this.categoriesList.TabIndex = 13;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(152, 16);
			this.label4.TabIndex = 14;
			this.label4.Text = "Categories:";
			// 
			// policiesTab
			// 
			this.policiesTab.Controls.AddRange(new System.Windows.Forms.Control[] {
																					  this.policyIDText,
																					  this.label8,
																					  this.label7,
																					  this.policiesList,
																					  this.evaluatePolicyButton,
																					  this.newPolicyButton,
																					  this.policyText,
																					  this.modifyPolicyButton});
			this.policiesTab.Location = new System.Drawing.Point(4, 22);
			this.policiesTab.Name = "policiesTab";
			this.policiesTab.Size = new System.Drawing.Size(384, 371);
			this.policiesTab.TabIndex = 2;
			this.policiesTab.Text = "Policies";
			// 
			// policyIDText
			// 
			this.policyIDText.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.policyIDText.Location = new System.Drawing.Point(64, 128);
			this.policyIDText.Name = "policyIDText";
			this.policyIDText.ReadOnly = true;
			this.policyIDText.Size = new System.Drawing.Size(312, 20);
			this.policyIDText.TabIndex = 17;
			this.policyIDText.Text = "";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(8, 130);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(104, 16);
			this.label8.TabIndex = 16;
			this.label8.Text = "Policy ID:";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(8, 8);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(152, 16);
			this.label7.TabIndex = 15;
			this.label7.Text = "Policies:";
			// 
			// policiesList
			// 
			this.policiesList.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.policiesList.Location = new System.Drawing.Point(8, 24);
			this.policiesList.Name = "policiesList";
			this.policiesList.Size = new System.Drawing.Size(368, 95);
			this.policiesList.TabIndex = 9;
			// 
			// evaluatePolicyButton
			// 
			this.evaluatePolicyButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.evaluatePolicyButton.Location = new System.Drawing.Point(288, 333);
			this.evaluatePolicyButton.Name = "evaluatePolicyButton";
			this.evaluatePolicyButton.Size = new System.Drawing.Size(88, 24);
			this.evaluatePolicyButton.TabIndex = 8;
			this.evaluatePolicyButton.Text = "Evaluate";
			// 
			// newPolicyButton
			// 
			this.newPolicyButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.newPolicyButton.Location = new System.Drawing.Point(8, 333);
			this.newPolicyButton.Name = "newPolicyButton";
			this.newPolicyButton.Size = new System.Drawing.Size(80, 24);
			this.newPolicyButton.TabIndex = 7;
			this.newPolicyButton.Text = "New Policy...";
			this.newPolicyButton.Click += new System.EventHandler(this.newPolicyButton_Click);
			// 
			// policyText
			// 
			this.policyText.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.policyText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.policyText.Location = new System.Drawing.Point(8, 160);
			this.policyText.Multiline = true;
			this.policyText.Name = "policyText";
			this.policyText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.policyText.Size = new System.Drawing.Size(368, 165);
			this.policyText.TabIndex = 6;
			this.policyText.Text = "";
			// 
			// modifyPolicyButton
			// 
			this.modifyPolicyButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.modifyPolicyButton.Location = new System.Drawing.Point(96, 333);
			this.modifyPolicyButton.Name = "modifyPolicyButton";
			this.modifyPolicyButton.Size = new System.Drawing.Size(80, 24);
			this.modifyPolicyButton.TabIndex = 7;
			this.modifyPolicyButton.Text = "Modify";
			// 
			// PolicyEditor
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 397);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.tabs});
			this.Name = "PolicyEditor";
			this.Text = "PolicyEditor";
			this.tabs.ResumeLayout(false);
			this.resourcesTab.ResumeLayout(false);
			this.categoriesTab.ResumeLayout(false);
			this.policiesTab.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new PolicyEditor());
		}

		private void evaluatePolicyButton_Click(object sender, System.EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			EvaluationResult result = PolicyManager.evaluatePolicy(policyText.Text, 
				new TestContext());
			this.Cursor = Cursors.Default;

			StringWriter writer = new StringWriter();
			result.dumpResults(writer);

			MessageBox.Show(this, writer.ToString());
		}

		private void tabs_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (tabs.SelectedTab == resourcesTab) 
			{
				RefreshResourceList();
				RefreshCategoryLists();
			}
			else if (tabs.SelectedTab == categoriesTab) 
			{
				RefreshCategoryLists();
			}
		}

		private void newPolicyButton_Click(object sender, System.EventArgs e)
		{
			string id = policyDAO.AddNewPolicy("test policy", policyText.Text);
			MessageBox.Show(this, "New policy created with id " + id);
		}

		private void resourcesList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			RefreshResourceCategoryList();
		}

		private void addCategoryToResourceButton_Click(object sender, System.EventArgs e)
		{
			Resource resource = resourcesList.SelectedItem;
			ListBox.SelectedObjectCollection categories = availableCategoriesList.SelectedItems;

			if (resource == null || categories.Count == 0) 
			{
				return;
			}

			IEnumerator e = categories.GetEnumerator();
			while (e.MoveNext()) 
			{
				Category cat = e.Current;
				resourceDAO.AddResourceToCategory(resource, cat.Name);

				resourceCategoriesList.Items.Add(cat);
			}

		}

		

		
	}
}
