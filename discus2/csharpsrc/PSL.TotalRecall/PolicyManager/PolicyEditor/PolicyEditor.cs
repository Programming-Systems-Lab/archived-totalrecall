using System;
using System.Drawing;
using System.Collections;
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
		private System.Windows.Forms.TextBox policyText;
		private System.Windows.Forms.Button addPolicyButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button evaluatePolicyButton;

		private PolicyDAO policyDAO;

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
			this.policyText = new System.Windows.Forms.TextBox();
			this.addPolicyButton = new System.Windows.Forms.Button();
			this.evaluatePolicyButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// policyText
			// 
			this.policyText.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.policyText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.policyText.Location = new System.Drawing.Point(16, 24);
			this.policyText.Multiline = true;
			this.policyText.Name = "policyText";
			this.policyText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.policyText.Size = new System.Drawing.Size(432, 304);
			this.policyText.TabIndex = 0;
			this.policyText.Text = "";
			// 
			// addPolicyButton
			// 
			this.addPolicyButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.addPolicyButton.Location = new System.Drawing.Point(16, 336);
			this.addPolicyButton.Name = "addPolicyButton";
			this.addPolicyButton.Size = new System.Drawing.Size(88, 24);
			this.addPolicyButton.TabIndex = 1;
			this.addPolicyButton.Text = "Add Policy";
			this.addPolicyButton.Click += new System.EventHandler(this.addPolicyButton_Click);
			// 
			// evaluatePolicyButton
			// 
			this.evaluatePolicyButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.evaluatePolicyButton.Location = new System.Drawing.Point(120, 336);
			this.evaluatePolicyButton.Name = "evaluatePolicyButton";
			this.evaluatePolicyButton.Size = new System.Drawing.Size(88, 24);
			this.evaluatePolicyButton.TabIndex = 2;
			this.evaluatePolicyButton.Text = "Evaluate";
			this.evaluatePolicyButton.Click += new System.EventHandler(this.evaluatePolicyButton_Click);
			// 
			// PolicyEditor
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(456, 373);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.evaluatePolicyButton,
																		  this.addPolicyButton,
																		  this.policyText});
			this.Name = "PolicyEditor";
			this.Text = "PolicyEditor";
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

		private void addPolicyButton_Click(object sender, System.EventArgs e)
		{
			string id = policyDAO.AddNewPolicy("test policy", policyText.Text);
			MessageBox.Show(this, "New policy created with id " + id);
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
	}
}
