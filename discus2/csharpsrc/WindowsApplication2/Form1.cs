
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Text;
using PSL.TotalRecall;
using PSL.Web.Services.DynamicProxy;
using PSL.Web.Services.DynamicInvoke;
using Microsoft.Web.Services;
using Microsoft.Web.Services.Security;
using Microsoft.Web.Services.Security.X509;
using System.Net;
using System.Xml;
using PSL.TotalRecall.Util;

namespace WindowsApplication2
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button m_btnRecall;
		private System.Windows.Forms.Button m_btnSend;
		private System.Windows.Forms.Button m_btnList;
		private System.Windows.Forms.Button m_btnSelectMtg;
		private System.Windows.Forms.Button m_btnRefMtg;
		private System.Windows.Forms.ComboBox m_boxMtgs;
		private System.Windows.Forms.ComboBox m_boxParticipants;
		private System.Windows.Forms.ListBox m_boxResources;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		/**
		 * this is the constructor for the GUI, it does the following:
		 * 1.  set up the DAO objects to access the local database
		 * 2.  add any active meetings in the database to the field in the GUI
		 * 3.  extract my X.509 certificate from the local store
		 * 4.  instantiate 5 "dummy" resources, add them to resource list
		* */
		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			dbConnect = "DSN=TotalRecall;UID=TotalRecallUser;PWD=totalrecall;DATABASE=TotalRecall";

			mDAO = new MeetingDAO(dbConnect);
			pDAO = new ParticipantDAO(dbConnect);
			rDAO = new ResourceDAO(dbConnect);
			cDAO = new ContactDAO(dbConnect);
			cmDAO = new ContextMsgDAO(dbConnect);

			strSelectedMtg = "";
			ArrayList lstMtgs = mDAO.GetMeetingIDs(enuMeetingState.Active);
			foreach (string s in lstMtgs)
			{
				m_boxMtgs.Items.Add(s);
			}

			strMyUrl = "http://localhost/TotalRecall/InfoAgent.asmx?wsdl";

			store = X509CertificateStore.LocalMachineStore(X509CertificateStore.MyStore);
			store.OpenRead();

			strMyName = "CN=Omar";
			certCol = store.FindCertificateBySubjectName( strMyName );

			cert = (X509Certificate) certCol[0];
			certToken = new X509SecurityToken(cert);

			lstResources = new ArrayList();
			for (int i=0; i<5; i++)
			{
				Resource res = new Resource();
				res.ID="res"+(i+1);
				res.Name = "Foo" + (i+1)+ ".txt";
				res.Url = "file:///c:\\"+res.Name;
				rDAO.AddNewResource(res);
				lstResources.Add(res);
			}

			foreach (Resource r in lstResources)
			{
				m_boxResources.Items.Add(r.ID);
			}
		}

		private ArrayList lstResources;
		private ContextMsgDAO cmDAO;
		private MeetingDAO mDAO;
		private ParticipantDAO pDAO;
		private ResourceDAO rDAO;
		private ContactDAO cDAO;

		private string strSelectedMtg;
		private string dbConnect;
		private string strMyName;
		private string strMyUrl;

		private X509CertificateStore store;
		private X509CertificateCollection certCol;
		private X509Certificate cert;
		private X509SecurityToken certToken;


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
			this.m_boxResources = new System.Windows.Forms.ListBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.m_btnRecall = new System.Windows.Forms.Button();
			this.m_boxParticipants = new System.Windows.Forms.ComboBox();
			this.m_btnSend = new System.Windows.Forms.Button();
			this.m_btnList = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.m_btnRefMtg = new System.Windows.Forms.Button();
			this.m_btnSelectMtg = new System.Windows.Forms.Button();
			this.m_boxMtgs = new System.Windows.Forms.ComboBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_boxResources
			// 
			this.m_boxResources.Location = new System.Drawing.Point(32, 24);
			this.m_boxResources.Name = "m_boxResources";
			this.m_boxResources.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
			this.m_boxResources.Size = new System.Drawing.Size(240, 251);
			this.m_boxResources.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.m_boxResources});
			this.groupBox1.Location = new System.Drawing.Point(352, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(312, 280);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "AVAILABLE RESOURCES";
			// 
			// m_btnRecall
			// 
			this.m_btnRecall.Location = new System.Drawing.Point(376, 288);
			this.m_btnRecall.Name = "m_btnRecall";
			this.m_btnRecall.Size = new System.Drawing.Size(256, 56);
			this.m_btnRecall.TabIndex = 5;
			this.m_btnRecall.Text = "recall selected resource(s)";
			this.m_btnRecall.Click += new System.EventHandler(this.m_btnRecall_Click);
			// 
			// m_boxParticipants
			// 
			this.m_boxParticipants.Location = new System.Drawing.Point(16, 24);
			this.m_boxParticipants.Name = "m_boxParticipants";
			this.m_boxParticipants.Size = new System.Drawing.Size(304, 21);
			this.m_boxParticipants.TabIndex = 2;
			this.m_boxParticipants.Text = "Current Participants";
			// 
			// m_btnSend
			// 
			this.m_btnSend.Location = new System.Drawing.Point(8, 288);
			this.m_btnSend.Name = "m_btnSend";
			this.m_btnSend.Size = new System.Drawing.Size(336, 56);
			this.m_btnSend.TabIndex = 3;
			this.m_btnSend.Text = "check for resource requests and send selected resource(s)";
			this.m_btnSend.Click += new System.EventHandler(this.m_btnSend_Click);
			// 
			// m_btnList
			// 
			this.m_btnList.Location = new System.Drawing.Point(24, 72);
			this.m_btnList.Name = "m_btnList";
			this.m_btnList.Size = new System.Drawing.Size(144, 23);
			this.m_btnList.TabIndex = 4;
			this.m_btnList.Text = "REFRESH LIST";
			this.m_btnList.Click += new System.EventHandler(this.m_btnList_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.m_boxParticipants,
																					this.m_btnList});
			this.groupBox2.Location = new System.Drawing.Point(16, 160);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(328, 120);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "CURRENT PARTICIPANTS";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.m_btnRefMtg,
																					this.m_btnSelectMtg,
																					this.m_boxMtgs});
			this.groupBox3.Location = new System.Drawing.Point(8, 8);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(336, 144);
			this.groupBox3.TabIndex = 6;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "MEETINGS";
			// 
			// m_btnRefMtg
			// 
			this.m_btnRefMtg.Location = new System.Drawing.Point(32, 88);
			this.m_btnRefMtg.Name = "m_btnRefMtg";
			this.m_btnRefMtg.Size = new System.Drawing.Size(128, 23);
			this.m_btnRefMtg.TabIndex = 2;
			this.m_btnRefMtg.Text = "refresh meetings";
			this.m_btnRefMtg.Click += new System.EventHandler(this.m_btnRefMtg_Click);
			// 
			// m_btnSelectMtg
			// 
			this.m_btnSelectMtg.Location = new System.Drawing.Point(200, 88);
			this.m_btnSelectMtg.Name = "m_btnSelectMtg";
			this.m_btnSelectMtg.Size = new System.Drawing.Size(128, 23);
			this.m_btnSelectMtg.TabIndex = 1;
			this.m_btnSelectMtg.Text = "SELECT MEETING";
			this.m_btnSelectMtg.Click += new System.EventHandler(this.m_btnSelectMtg_Click);
			// 
			// m_boxMtgs
			// 
			this.m_boxMtgs.Location = new System.Drawing.Point(16, 24);
			this.m_boxMtgs.Name = "m_boxMtgs";
			this.m_boxMtgs.Size = new System.Drawing.Size(312, 21);
			this.m_boxMtgs.TabIndex = 0;
			this.m_boxMtgs.Text = "m_boxMtgs";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(672, 357);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox3,
																		  this.groupBox2,
																		  this.m_btnSend,
																		  this.groupBox1,
																		  this.m_btnRecall});
			this.Name = "Form1";
			this.Text = "Form1";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		/**
		 * this method sends a resource
		 * first, it checks to see if any new resource request messages have arrived
		 * into the ContextMessageDAO.  If so, then for each resource request, it
		 * sends a resource response to the sender.  For each selected resource in the GUI,
		 * it adds the resource to the resource response.  Next, it creates an Executor / Executor
		 * Context to communicate with the InfoAgent that sent the message.  It then calls the Recommend
		 * method on that InfoAgent that requested the resource, passing the selected resources (serialized)
		 * in the parameter
		 * */
		private void m_btnSend_Click(object sender, System.EventArgs e)
		{

			//first i check for resource request messages
			ArrayList lstNew = cmDAO.GetNewReceivedContextMessages();
			IEnumerator it = lstNew.GetEnumerator();
			while (it.MoveNext())
			{
				if (it.Current is RecommendationRequestCtxMsg)
				{
					RecommendationRequestCtxMsg recReq = (RecommendationRequestCtxMsg)it.Current;
					RecommendationResponseCtxMsg respctxmsg = new RecommendationResponseCtxMsg(recReq);

					ResourceMsg msg = new ResourceMsg();
					msg.MeetingID = recReq.MeetingID;
					msg.Sender = respctxmsg.Sender;
					msg.SenderUrl = respctxmsg.SenderUrl;

					foreach (string strSel in m_boxResources.SelectedItems)
					{
						foreach (Resource r in lstResources)
						{
							if (r.ID == strSel)
							{
								msg.m_lstResources.Add(r);
							}
						}

					}

						respctxmsg.ResourceMessage = msg;

						string strResponse = Serializer.Serialize(respctxmsg.ToXml());
						string strInfoagenturl = recReq.SenderUrl;

						ProxyGenRequest pxyreq = new ProxyGenRequest();
						pxyreq.ProxyPath = "";
						pxyreq.ServiceName = "InfoAgent";
						pxyreq.WsdlUrl = strInfoagenturl;

						ProxyPolicyMutator mymutator = new ProxyPolicyMutator();
						mymutator.ProxyName = pxyreq.ServiceName;

						// Ensure the name of the file generated is unique
						string strMySuffix = "";
						int nMyCode = Guid.NewGuid().GetHashCode();
						if( nMyCode < 0 )
							nMyCode = nMyCode * -1;
						strMySuffix = nMyCode.ToString();
						pxyreq.ServiceName = pxyreq.ServiceName + "_" + strMySuffix;

						ProxyGen myPxyGen = new ProxyGen();
						myPxyGen.Mutator = mymutator;

						string strMyAssembly = "";
						try
						{
							strMyAssembly = myPxyGen.GenerateAssembly(pxyreq);
						}
						catch (Exception excep)
						{
							string strString = excep.Message;
						}

						ExecContext myctx = new ExecContext();
						myctx.ServiceName = pxyreq.Namespace + "." + mymutator.ProxyName;
						myctx.Assembly = strMyAssembly;
		
						Executor myexec = new Executor();
						myexec.Settings.ExpectSignedResponse = false;
						myexec.Settings.SigningCertificate = cert;
		
						myctx.MethodName = "Recommend";
						myctx.AddParameter(strResponse);
						
						object objectRes = null;
				
						try
						{
							objectRes = myexec.Execute(myctx);
						}
						catch( Exception exc )
						{
							string strBsg = exc.Message;
							return;
						}
					}				
			}
		}
		/**
		 * this method simply refreshes the participant list by
		 * checking the ParticipantDAO for the selected MeetingID
		* */
		private void m_btnList_Click(object sender, System.EventArgs e)
		{
			UpdateParticipantsList();
		}

		/**
		 * this method selects between existing meetings.  When a meeting is
		 * selected, the GUI updates itself with the newly selected meeting's
		 * participant list
		 * */
		private void m_btnSelectMtg_Click(object sender, System.EventArgs e)
		{
			strSelectedMtg = (string)m_boxMtgs.SelectedItem;
			UpdateParticipantsList();
		}

		/**
		 * much like the method to send resources, this method is used to recall resources
		 * that are being shared.  The resources to be recalled are the resources that are
		 * selected in the GUI.  First, it creates a ResourceContextMsg and an Executor to
		 * the local InfoAgent.  It then calls the RecallMyResources method on the local InfoAgent.
		 * */
		private void m_btnRecall_Click(object sender, System.EventArgs e)
		{
			ResourceCtxMsg rctx = new ResourceCtxMsg();
			rctx.MeetingID = strSelectedMtg;
			rctx.Sender = strMyName;
			rctx.Type = enuContextMsgType.ResourceRecalled;
			rctx.SenderUrl = strMyUrl;

			foreach (string strSel in m_boxResources.SelectedItems)
			{
				rctx.AddResourceID(strSel);
			}

			string strResponse = Serializer.Serialize(rctx.ToXml());

			ProxyGenRequest pxyreq = new ProxyGenRequest();
			pxyreq.ProxyPath = "";
			pxyreq.ServiceName = "InfoAgent";
			pxyreq.WsdlUrl = strMyUrl;

			ProxyPolicyMutator mymutator = new ProxyPolicyMutator();
			mymutator.ProxyName = pxyreq.ServiceName;

			// Ensure the name of the file generated is unique
			string strMySuffix = "";
			int nMyCode = Guid.NewGuid().GetHashCode();
			if( nMyCode < 0 )
				nMyCode = nMyCode * -1;
			strMySuffix = nMyCode.ToString();
			pxyreq.ServiceName = pxyreq.ServiceName + "_" + strMySuffix;

			ProxyGen myPxyGen = new ProxyGen();
			myPxyGen.Mutator = mymutator;

			string strMyAssembly = "";
			try
			{
				strMyAssembly = myPxyGen.GenerateAssembly(pxyreq);
			}
			catch (Exception excep)
			{
				string strString = excep.Message;
			}

			ExecContext myctx = new ExecContext();
			myctx.ServiceName = pxyreq.Namespace + "." + mymutator.ProxyName;
			myctx.Assembly = strMyAssembly;
		
			Executor myexec = new Executor();
			myexec.Settings.ExpectSignedResponse = false;
			myexec.Settings.SigningCertificate = cert;
		
			myctx.MethodName = "RecallMyResources";
			myctx.AddParameter(strResponse);
						
			object objectRes = null;
				
			try
			{
				objectRes = myexec.Execute(myctx);
			}
			catch( Exception exc )
			{
				string strBsg = exc.Message;
				return;
			}
		}

		/**
		 * this method checks the MeetingDAO and refreshes the list of active meetings
		 * */
		private void m_btnRefMtg_Click(object sender, System.EventArgs e)
		{
			ArrayList lstMtgs = mDAO.GetMeetingIDs(enuMeetingState.Active);
			foreach (string s in lstMtgs)
			{
				if (m_boxMtgs.Items.Contains(s))
					continue;

				m_boxMtgs.Items.Add(s);
			}
		}

		private void UpdateParticipantsList()
		{
			m_boxParticipants.Items.Clear();

			ArrayList lstParticipants = pDAO.GetParticipants(strSelectedMtg);
			foreach (MeetingParticipant mp in lstParticipants)
			{
				StringBuilder strB = new StringBuilder(mp.Name);
				strB.Append(" @ ");
				strB.Append(mp.Location);

				m_boxParticipants.Items.Add(strB.ToString());
			}
		}
	}
}
