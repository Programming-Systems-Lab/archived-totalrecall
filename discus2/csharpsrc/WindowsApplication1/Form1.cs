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

namespace WindowsApplication1
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Button m_btnCreate;
		private System.Windows.Forms.TextBox m_boxCreate;
		private System.Windows.Forms.Button m_btnSelect;
		private System.Windows.Forms.Button m_btnEnd;
		private System.Windows.Forms.Button m_btnRefresh;
		private System.Windows.Forms.Button m_btnReq;
		private System.Windows.Forms.Button m_btnRef;
		private System.Windows.Forms.Button m_btnTopic;
		private System.Windows.Forms.Button m_btnInvite;
		private System.Windows.Forms.ComboBox m_boxMtgs;
		private System.Windows.Forms.ListBox m_boxParticipants;
		private System.Windows.Forms.ListBox m_boxResources;
		private System.Windows.Forms.TextBox m_boxTopic;
		private System.Windows.Forms.TextBox m_boxInvite;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/**
		 * this constructor does the following:
		 * 1.  read in the X.509 certificate from the machine store
		 * 2.  get the localhost IP address
		 * 3.  create an executor / executor context for the local InfoAgent
		 * 4.  create the DAO objects to access contents in the database
		 * 5.  update the existing meetings with all meetings in the database
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
			store = X509CertificateStore.LocalMachineStore(X509CertificateStore.MyStore);
			store.OpenRead();

			strMyId = "CN=Omar";
			certCol = store.FindCertificateBySubjectName( strMyId );

			cert = (X509Certificate) certCol[0];
			certToken = new X509SecurityToken(cert);

			me = new MeetingParticipant();
			me.Name = cert.GetName();
			me.Role = enuMeetingParticipantRole.Organizer;

			strFileLocation = "TotalRecall/InfoAgent.asmx?wsdl";
			string strHost = Dns.GetHostName();
			IPHostEntry entry = Dns.Resolve(strHost);
			string strIP = "";
			if (entry.AddressList.Length > 0)
			{
				IPAddress addr = new IPAddress(entry.AddressList[0].Address);
				strIP = addr.ToString();
			}
			else
			{
				m_boxInvite.Text = "ERROR getting host IP";
				return;
			}
			StringBuilder strbldUrl = new StringBuilder(strIP);
			strbldUrl.Append(strFileLocation);
			me.Location = strbldUrl.ToString();


			//create my infoagent
			strMyUrl = "http://localhost/TotalRecall/InfoAgent.asmx?wsdl";

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

			myctx = new ExecContext();
			myctx.ServiceName = pxyreq.Namespace + "." + mymutator.ProxyName;
			myctx.Assembly = strMyAssembly;
		

			myexec = new Executor();
			myexec.Settings.ExpectSignedResponse = true;
			myexec.Settings.SigningCertificate = cert;

			dbConnect = "DSN=TotalRecall;UID=TotalRecallUser;PWD=totalrecall;DATABASE=TotalRecall";

			mDAO = new MeetingDAO(dbConnect);
			pDAO = new ParticipantDAO(dbConnect);
			rDAO = new ResourceDAO(dbConnect);
			cDAO = new ContactDAO(dbConnect);

			strSelectedMtg = "";
			ArrayList lstMtgs = mDAO.GetMeetingIDs(enuMeetingState.Active);
				foreach (string s in lstMtgs)
				{
					m_boxMtgs.Items.Add(s);
				}

		}


		private Executor myexec;
		private ExecContext myctx;
		private MeetingParticipant me;

		private X509CertificateStore store;
		private X509CertificateCollection certCol;
		private X509Certificate cert;
		private X509SecurityToken certToken;

		private string strMyUrl;
		private string strFileLocation;
		private string dbConnect;
		private MeetingDAO mDAO;
		private ParticipantDAO pDAO;
		private ResourceDAO rDAO;
		private ContactDAO cDAO;
		private string strMyId;

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
			this.m_btnRefresh = new System.Windows.Forms.Button();
			this.m_btnInvite = new System.Windows.Forms.Button();
			this.m_btnRef = new System.Windows.Forms.Button();
			this.m_boxInvite = new System.Windows.Forms.TextBox();
			this.m_boxResources = new System.Windows.Forms.ListBox();
			this.m_boxTopic = new System.Windows.Forms.TextBox();
			this.m_boxParticipants = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.m_btnReq = new System.Windows.Forms.Button();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.m_btnTopic = new System.Windows.Forms.Button();
			this.m_boxMtgs = new System.Windows.Forms.ComboBox();
			this.m_btnCreate = new System.Windows.Forms.Button();
			this.m_boxCreate = new System.Windows.Forms.TextBox();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.m_btnEnd = new System.Windows.Forms.Button();
			this.m_btnSelect = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_btnRefresh
			// 
			this.m_btnRefresh.Location = new System.Drawing.Point(96, 200);
			this.m_btnRefresh.Name = "m_btnRefresh";
			this.m_btnRefresh.TabIndex = 0;
			this.m_btnRefresh.Text = "REFRESH";
			this.m_btnRefresh.Click += new System.EventHandler(this.m_btnRefresh_Click);
			// 
			// m_btnInvite
			// 
			this.m_btnInvite.Location = new System.Drawing.Point(480, 32);
			this.m_btnInvite.Name = "m_btnInvite";
			this.m_btnInvite.TabIndex = 1;
			this.m_btnInvite.Text = "INVITE";
			this.m_btnInvite.Click += new System.EventHandler(this.m_btnInvite_Click);
			// 
			// m_btnRef
			// 
			this.m_btnRef.Location = new System.Drawing.Point(152, 208);
			this.m_btnRef.Name = "m_btnRef";
			this.m_btnRef.Size = new System.Drawing.Size(112, 23);
			this.m_btnRef.TabIndex = 2;
			this.m_btnRef.Text = "REFRESH";
			this.m_btnRef.Click += new System.EventHandler(this.m_btnRef_Click);
			// 
			// m_boxInvite
			// 
			this.m_boxInvite.Location = new System.Drawing.Point(40, 32);
			this.m_boxInvite.Name = "m_boxInvite";
			this.m_boxInvite.Size = new System.Drawing.Size(376, 20);
			this.m_boxInvite.TabIndex = 3;
			this.m_boxInvite.Text = "ENTER PARTICIPANT URL HERE";
			// 
			// m_boxResources
			// 
			this.m_boxResources.HorizontalScrollbar = true;
			this.m_boxResources.Location = new System.Drawing.Point(72, 24);
			this.m_boxResources.Name = "m_boxResources";
			this.m_boxResources.ScrollAlwaysVisible = true;
			this.m_boxResources.Size = new System.Drawing.Size(120, 160);
			this.m_boxResources.TabIndex = 4;
			// 
			// m_boxTopic
			// 
			this.m_boxTopic.Location = new System.Drawing.Point(32, 32);
			this.m_boxTopic.Name = "m_boxTopic";
			this.m_boxTopic.Size = new System.Drawing.Size(232, 20);
			this.m_boxTopic.TabIndex = 6;
			this.m_boxTopic.Text = "m_boxTopic";
			// 
			// m_boxParticipants
			// 
			this.m_boxParticipants.HorizontalScrollbar = true;
			this.m_boxParticipants.Location = new System.Drawing.Point(72, 24);
			this.m_boxParticipants.Name = "m_boxParticipants";
			this.m_boxParticipants.ScrollAlwaysVisible = true;
			this.m_boxParticipants.Size = new System.Drawing.Size(120, 147);
			this.m_boxParticipants.TabIndex = 7;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Century Gothic", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(960, 72);
			this.label1.TabIndex = 9;
			this.label1.Text = "CONTEXT MANAGER";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.m_btnInvite,
																					this.m_boxInvite});
			this.groupBox1.Location = new System.Drawing.Point(40, 440);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(592, 100);
			this.groupBox1.TabIndex = 10;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "INVITE PARTICIPANT";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.m_btnRefresh,
																					this.m_boxParticipants});
			this.groupBox2.Location = new System.Drawing.Point(368, 160);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(256, 256);
			this.groupBox2.TabIndex = 11;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "CURRENT PARTICIPANTS";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.m_btnRef,
																					this.m_boxResources,
																					this.m_btnReq});
			this.groupBox3.Location = new System.Drawing.Point(672, 80);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(280, 256);
			this.groupBox3.TabIndex = 12;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "MEETING RESOURCES";
			// 
			// m_btnReq
			// 
			this.m_btnReq.Location = new System.Drawing.Point(16, 208);
			this.m_btnReq.Name = "m_btnReq";
			this.m_btnReq.Size = new System.Drawing.Size(112, 23);
			this.m_btnReq.TabIndex = 19;
			this.m_btnReq.Text = "REQUEST";
			this.m_btnReq.Click += new System.EventHandler(this.m_btnReq_Click);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.m_btnTopic,
																					this.m_boxTopic});
			this.groupBox4.Location = new System.Drawing.Point(640, 384);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(304, 160);
			this.groupBox4.TabIndex = 13;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "CURRENT TOPIC";
			// 
			// m_btnTopic
			// 
			this.m_btnTopic.Location = new System.Drawing.Point(104, 88);
			this.m_btnTopic.Name = "m_btnTopic";
			this.m_btnTopic.Size = new System.Drawing.Size(104, 23);
			this.m_btnTopic.TabIndex = 7;
			this.m_btnTopic.Text = "CHANGE TOPIC";
			this.m_btnTopic.Click += new System.EventHandler(this.m_btnTopic_Click);
			// 
			// m_boxMtgs
			// 
			this.m_boxMtgs.Location = new System.Drawing.Point(32, 32);
			this.m_boxMtgs.Name = "m_boxMtgs";
			this.m_boxMtgs.Size = new System.Drawing.Size(121, 21);
			this.m_boxMtgs.TabIndex = 14;
			this.m_boxMtgs.Text = "m_boxMtgs";
			// 
			// m_btnCreate
			// 
			this.m_btnCreate.Location = new System.Drawing.Point(240, 64);
			this.m_btnCreate.Name = "m_btnCreate";
			this.m_btnCreate.TabIndex = 15;
			this.m_btnCreate.Text = "CREATE";
			this.m_btnCreate.Click += new System.EventHandler(this.m_btnCreate_Click);
			// 
			// m_boxCreate
			// 
			this.m_boxCreate.Location = new System.Drawing.Point(24, 24);
			this.m_boxCreate.Name = "m_boxCreate";
			this.m_boxCreate.Size = new System.Drawing.Size(272, 20);
			this.m_boxCreate.TabIndex = 16;
			this.m_boxCreate.Text = "Enter text here";
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.m_boxCreate,
																					this.m_btnCreate});
			this.groupBox5.Location = new System.Drawing.Point(24, 88);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(328, 100);
			this.groupBox5.TabIndex = 17;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "CREATE MEETING";
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.m_btnEnd,
																					this.m_btnSelect,
																					this.m_boxMtgs});
			this.groupBox6.Location = new System.Drawing.Point(88, 256);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(200, 168);
			this.groupBox6.TabIndex = 18;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "SELECT MEETING";
			// 
			// m_btnEnd
			// 
			this.m_btnEnd.Location = new System.Drawing.Point(56, 128);
			this.m_btnEnd.Name = "m_btnEnd";
			this.m_btnEnd.TabIndex = 16;
			this.m_btnEnd.Text = "END MTG";
			this.m_btnEnd.Click += new System.EventHandler(this.m_btnEnd_Click);
			// 
			// m_btnSelect
			// 
			this.m_btnSelect.Location = new System.Drawing.Point(56, 88);
			this.m_btnSelect.Name = "m_btnSelect";
			this.m_btnSelect.TabIndex = 15;
			this.m_btnSelect.Text = "SELECT";
			this.m_btnSelect.Click += new System.EventHandler(this.m_btnSelect_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(960, 573);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox6,
																		  this.groupBox5,
																		  this.groupBox4,
																		  this.groupBox3,
																		  this.groupBox2,
																		  this.groupBox1,
																		  this.label1});
			this.Name = "Form1";
			this.Text = "ContextManager";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
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

		private void vScrollBar2_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
		
		}

		/**
		 * 
		 * this method occurs when the "update topic" button is pressed....the topic is updated
		 * in the Database using the MeetingDAO, and if that is successful, the new topic is
		 * displayed in the topic field
		 * 
		 * */
		private void m_btnTopic_Click(object sender, System.EventArgs e)
		{
			string strNewTopic = m_boxTopic.Text;
			
			if (mDAO.UpdateMeetingTopic(strSelectedMtg, strNewTopic))
			{
				//let us know if the update worked...
				StringBuilder strTopic = new StringBuilder();
				strTopic.Append("the new topic is: ");
				strTopic.Append(strNewTopic);
				UpdateTopic(strTopic.ToString());
			}
		}


		private void UpdateTopic(string strT)
		{
			m_boxTopic.Text = strT.ToString();
		}

		/**
		 * this method checks the ResourceDAO for all resources that are currently
		 * listed for the current meeting, and refreshes the resource field in the GUI
		 * */
		private void m_btnRef_Click(object sender, System.EventArgs e)
		{
			UpdateResourcesList();
		}

		/**
		 * this method checks the ParticipantDAO for all current participants in the
		 * selected meeting, and refreshes the participant field
		 * */
		private void m_btnRefresh_Click(object sender, System.EventArgs e)
		{
			UpdateParticipantsList();
		}

		/**
		 * this method invites a new participant into the selected meeting.  First, the participant
		 * URL is grabbed from the Invite field and serialized.  Then the serialized URL is passed to
		 * the InviteAgent method in the local InfoAgent.  The InviteAgent method first gets the signatures
		 * of all of the existing participants, and then signs the message and sends it to the invitee
		 * */
		private void m_btnInvite_Click(object sender, System.EventArgs e)
		{			
			//invite new participant to meeting:  strSelectedMtg
			string strNewUrl = m_boxInvite.Text;
			StringBuilder strTest = new StringBuilder();
			strTest.Append("we are inviting: ");
			strTest.Append(strNewUrl);
			string strNewUrlSerialized = Serializer.Serialize(strNewUrl);
			m_boxInvite.Text = strTest.ToString();
	

			myctx.ClearParameters();
			myctx.MethodName = "InviteAgent";
		
			MeetingRequestMsg newMsg = new MeetingRequestMsg();
			newMsg.Sender = cert.GetName();

			newMsg.SenderUrl = strMyUrl;

			ArrayList lstParticipants = pDAO.GetParticipants(strSelectedMtg);
			foreach (MeetingParticipant mp in lstParticipants)
			{
				newMsg.m_lstParticipants.Add(mp);
			}

			newMsg.MeetingID = strSelectedMtg;
			newMsg.MeetingTopic = mDAO.GetMeetingTopic(strSelectedMtg);

			string strNewMsgSerialized = Serializer.Serialize(newMsg.ToXml());

			myctx.AddParameter(strNewMsgSerialized);
			myctx.AddParameter(strNewUrlSerialized);

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
			
			StringBuilder sbResult = new StringBuilder("Added ");
			sbResult.Append(strNewUrl).Append(" to meeting list");
			m_boxInvite.Text = sbResult.ToString();
			UpdateParticipantsList();

		}

		/**
		 * this button creates a new meeting
		 * first it gets the meeting name from the appropriate field, and the
		 * topic from the appropriate field.  Then, it calls the CreateMeeting method
		 * on the local InfoAgent to create the meeting.  Upon success, it updates
		 * the text fields with the new meeting.
		* */
		private void m_btnCreate_Click(object sender, System.EventArgs e)
		{
			string strMtgName = m_boxCreate.Text;
			string strMtgTopic = m_boxTopic.Text;

			string strMtgNameSerialized = Serializer.Serialize(strMtgName);
			string strMtgTopicSerialized = Serializer.Serialize(strMtgTopic);

			myctx.MethodName = "CreateMeeting";
			myctx.AddParameter(strMtgNameSerialized);
			myctx.AddParameter(strMtgTopicSerialized);

			object objectResult = null;

			try
				{
					objectResult = myexec.Execute(myctx);
				}
				catch( Exception except )
				{
					string strExcMsg = except.Message;
					m_boxCreate.Text = strExcMsg;
					return;
				}

			
			StringBuilder strBuild = new StringBuilder("Created meeting, name = ");
			strBuild.Append(strMtgName);
			strBuild.Append(", topic = ");
			strBuild.Append(m_boxTopic.Text);
			m_boxCreate.Text = strBuild.ToString();

			m_boxMtgs.Items.Add(strMtgName);
		}

		private string strSelectedMtg;

		/**
		 * this button selects between available meetings.  First, if I am not
		 * in the meeting that I have selected, I add myself to the meeting.  If that
		 * does not work the GUI displays an error.  Otherwise, the new meeting is
		 * selected and the participants list, resources list, and meeting topic are
		 * updated
		 * */
		private void m_btnSelect_Click(object sender, System.EventArgs e)
		{
			strSelectedMtg = (string)m_boxMtgs.SelectedItem;

			//make sure i'm in the meeting
			if (!pDAO.IsInMeeting(strSelectedMtg, me))
			{
				if (!pDAO.AddMeetingParticipant(strSelectedMtg, me))
				{
					m_boxCreate.Text = "ERROR";
					return;
				}
			}
			StringBuilder strbuild = new StringBuilder ("selected ");
			strbuild.Append(strSelectedMtg);
			m_boxCreate.Text = strbuild.ToString();

			UpdateParticipantsList();
			UpdateResourcesList();
			
			string strFoundTopic = mDAO.GetMeetingTopic(strSelectedMtg);
			UpdateTopic(strFoundTopic);
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

		private void UpdateResourcesList()
		{
			m_boxResources.Items.Clear();

			Hashtable lstResources = rDAO.GetMeetingResources( strSelectedMtg );// rDAO.GetSharedResources(strSelectedMtg);
			IDictionaryEnumerator it = lstResources.GetEnumerator();
			while (it.MoveNext())
			{
				MeetingResource mr = (MeetingResource)it.Value;
				StringBuilder strB2 = new StringBuilder(mr.Name);
				strB2.Append(", ID = ");
				strB2.Append(mr.ID);
				strB2.Append(", URL = ");
				strB2.Append(mr.Url);
				strB2.Append(", owned by ");
				strB2.Append(mr.Owner);
				strB2.Append(", state = ");
				strB2.Append(mr.State.ToString());
				m_boxResources.Items.Add(strB2.ToString());
			}

		}

		/**
		 * this button, at this time, simply ends the selected meeting in the MeetingDAO
		 * it does not, however, inform the meeting participants that the meeting 
		 * has ended (that would have been my next task)
		 * */
		private void m_btnEnd_Click(object sender, System.EventArgs e)
		{
			string strMtgtoEnd = (string)m_boxMtgs.SelectedItem;

			if (mDAO.EndMeeting(strMtgtoEnd))
			{
				m_boxMtgs.Items.Remove(strMtgtoEnd);
			}
		}

		/**
		 * this method requests a resource to be added to the meeting, by sending
		 * out a RecommendationRequest Context Message to the local InfoAgent
		 * */
		private void m_btnReq_Click(object sender, System.EventArgs e)
		{
			MeetingRequestMsg mtgReq = new MeetingRequestMsg();
			mtgReq.MeetingID = strSelectedMtg;
			mtgReq.MeetingTopic = mDAO.GetMeetingTopic(strSelectedMtg);
			mtgReq.Sender = me.Name;
			mtgReq.SenderUrl = strMyUrl;
			mtgReq.m_lstParticipants = pDAO.GetParticipants(strSelectedMtg);
			RecommendationRequestCtxMsg recReqCtxMsg = new RecommendationRequestCtxMsg(mtgReq);
			recReqCtxMsg.Type = enuContextMsgType.RecommendationRequest;

			string strS = recReqCtxMsg.ToXml();
			myctx.ClearParameters();
			myctx.MethodName = "RequestRecommendation";
			myctx.AddParameter(Serializer.Serialize(strS));

			Object objRes = null;
			try
			{
				objRes = myexec.Execute(myctx);
			}
			catch (Exception ex)
			{
				string strExceptionMsg = ex.Message;
			}
		}

	}
}
