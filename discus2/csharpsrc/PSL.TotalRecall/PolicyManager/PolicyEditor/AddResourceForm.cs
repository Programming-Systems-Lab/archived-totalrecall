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
	public class AddResourceForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox nameText;
		private System.Windows.Forms.TextBox urlText;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Button cancelButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AddResourceForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public void SetModify(string resourceName, string resourceUrl) 
		{
			nameText.Text = resourceName;
			urlText.Text = resourceUrl;

			this.Text = "Modify Resource";
			addButton.Text = "Modify";
		}


		public string ResourceName 
		{
			get 
			{
				return nameText.Text;
			}
		}

		public string ResourceUrl 
		{
			get 
			{
				return urlText.Text;
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
			this.label2 = new System.Windows.Forms.Label();
			this.nameText = new System.Windows.Forms.TextBox();
			this.urlText = new System.Windows.Forms.TextBox();
			this.addButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
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
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(32, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "URL:";
			// 
			// nameText
			// 
			this.nameText.Location = new System.Drawing.Point(48, 16);
			this.nameText.Name = "nameText";
			this.nameText.Size = new System.Drawing.Size(216, 20);
			this.nameText.TabIndex = 1;
			this.nameText.Text = "";
			// 
			// urlText
			// 
			this.urlText.Location = new System.Drawing.Point(48, 40);
			this.urlText.Name = "urlText";
			this.urlText.Size = new System.Drawing.Size(216, 20);
			this.urlText.TabIndex = 1;
			this.urlText.Text = "";
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
			// AddResourceForm
			// 
			this.AcceptButton = this.addButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(290, 111);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.addButton,
																		  this.nameText,
																		  this.label1,
																		  this.label2,
																		  this.urlText,
																		  this.cancelButton});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MinimizeBox = false;
			this.Name = "AddResourceForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add Resource";
			this.ResumeLayout(false);

		}
		#endregion

		private void addButton_Click(object sender, System.EventArgs e)
		{
			if (ResourceName.Length == 0 || ResourceUrl.Length == 0) 
			{
				MessageBox.Show(this, "Please enter a resource name and URL.",
					"Add Resource", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			else 
			{
				this.DialogResult = DialogResult.OK;
				Hide();
			}
		}
	}
}
