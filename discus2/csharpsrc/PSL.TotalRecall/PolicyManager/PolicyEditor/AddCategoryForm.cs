using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PSL.TotalRecall.PolicyManager
{
	/// <summary>
	/// Summary description for AddResourceForm.
	/// </summary>
	public class AddCategoryForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox nameText;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Label label2;
		internal System.Windows.Forms.ComboBox policyList;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AddCategoryForm(AccessPolicy[] policies)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			policyList.Items.AddRange(policies);
		}


		public void SetModify(string categoryName, AccessPolicy selectedPolicy) 
		{
			nameText.Text = categoryName;
			policyList.SelectedItem = selectedPolicy;

			this.Text = "Modify Category";
			addButton.Text = "Modify";
		}

		public string CategoryName 
		{
			get 
			{
				return nameText.Text;
			}
		}

		public AccessPolicy SelectedPolicy
		{
			get 
			{
				return (AccessPolicy) policyList.SelectedItem;
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
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
			this.label1 = new System.Windows.Forms.Label();
			this.nameText = new System.Windows.Forms.TextBox();
			this.addButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.policyList = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name:";
			// 
			// nameText
			// 
			this.nameText.Location = new System.Drawing.Point(48, 16);
			this.nameText.Name = "nameText";
			this.nameText.Size = new System.Drawing.Size(216, 20);
			this.nameText.TabIndex = 1;
			this.nameText.Text = "";
			// 
			// addButton
			// 
			this.addButton.Location = new System.Drawing.Point(88, 80);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(64, 24);
			this.addButton.TabIndex = 2;
			this.addButton.Text = "Add";
			this.addButton.Click += new System.EventHandler(this.addButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(160, 80);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(64, 24);
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Text = "Cancel";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "Policy:";
			// 
			// policyList
			// 
			this.policyList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.policyList.Location = new System.Drawing.Point(48, 46);
			this.policyList.Name = "policyList";
			this.policyList.Size = new System.Drawing.Size(216, 21);
			this.policyList.TabIndex = 4;
			// 
			// AddCategoryForm
			// 
			this.AcceptButton = this.addButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(290, 111);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.policyList,
																		  this.label2,
																		  this.addButton,
																		  this.nameText,
																		  this.label1,
																		  this.cancelButton});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MinimizeBox = false;
			this.Name = "AddCategoryForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add Category";
			this.ResumeLayout(false);

		}
		#endregion

		private void addButton_Click(object sender, System.EventArgs e)
		{
			if (CategoryName.Length == 0 || SelectedPolicy == null) 
			{
				MessageBox.Show(this, "Please enter a category name and choose a policy.",
					"Add Category", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			else 
			{
				this.DialogResult = DialogResult.OK;
				Hide();
			}
		}
	}
}
