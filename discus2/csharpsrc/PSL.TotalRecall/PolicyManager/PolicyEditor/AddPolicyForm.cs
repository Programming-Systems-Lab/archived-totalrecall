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
	public class AddPolicyForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox nameText;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox policyText;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AddPolicyForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}


		public string PolicyName 
		{
			get 
			{
				return nameText.Text;
			}
		}

		public string PolicyDoc
		{
			get 
			{
				return policyText.Text;
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
			this.policyText = new System.Windows.Forms.TextBox();
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
			this.addButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.addButton.Location = new System.Drawing.Point(127, 270);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(64, 24);
			this.addButton.TabIndex = 2;
			this.addButton.Text = "Add";
			this.addButton.Click += new System.EventHandler(this.addButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(199, 270);
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
			// policyText
			// 
			this.policyText.AcceptsReturn = true;
			this.policyText.AcceptsTab = true;
			this.policyText.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.policyText.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.policyText.Location = new System.Drawing.Point(8, 64);
			this.policyText.Multiline = true;
			this.policyText.Name = "policyText";
			this.policyText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.policyText.Size = new System.Drawing.Size(350, 198);
			this.policyText.TabIndex = 4;
			this.policyText.Text = "";
			// 
			// AddPolicyForm
			// 
			this.AcceptButton = this.addButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(368, 301);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.policyText,
																		  this.label2,
																		  this.addButton,
																		  this.nameText,
																		  this.label1,
																		  this.cancelButton});
			this.MinimizeBox = false;
			this.Name = "AddPolicyForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add Policy";
			this.ResumeLayout(false);

		}
		#endregion

		private void addButton_Click(object sender, System.EventArgs e)
		{
			if (PolicyName.Length == 0 || PolicyDoc.Length == 0) 
			{
				MessageBox.Show(this, "Please enter a policy name and a policy document.",
					"Add Policy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			else 
			{
				this.DialogResult = DialogResult.OK;
				Hide();
			}
		}
	}
}
