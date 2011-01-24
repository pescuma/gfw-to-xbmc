using System;
using System.Windows.Forms;
using Growl.Destinations;

namespace org.pescuma.XbmcForwarder
{
	public class XbmcInputs : DestinationSettingsPanel
	{
		private Label serverLabel;
		private HighlightTextBox serverText;
		private Label labelInfo;

		public XbmcInputs()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			serverText = new Growl.Destinations.HighlightTextBox();
			serverLabel = new System.Windows.Forms.Label();
			labelInfo = new System.Windows.Forms.Label();
			panelDetails.SuspendLayout();
			SuspendLayout();
			// 
			// panelDetails
			// 
			panelDetails.Controls.Add(serverLabel);
			panelDetails.Controls.Add(serverText);
			panelDetails.Controls.Add(labelInfo);
			// 
			// serverText
			// 
			serverText.HighlightColor = System.Drawing.Color.FromArgb(((((254)))), ((((250)))), ((((184)))));
			serverText.Location = new System.Drawing.Point(108, 20);
			serverText.Name = "serverText";
			serverText.Size = new System.Drawing.Size(211, 20);
			serverText.TabIndex = 2;
			serverText.TextChanged += serverText_TextChanged;
			// 
			// serverLabel
			// 
			serverLabel.AutoSize = true;
			serverLabel.Location = new System.Drawing.Point(19, 23);
			serverLabel.Name = "serverLabel";
			serverLabel.Size = new System.Drawing.Size(83, 13);
			serverLabel.TabIndex = 1;
			serverLabel.Text = "XBMC machine:";
			// 
			// labelInfo
			// 
			labelInfo.Location = new System.Drawing.Point(33, 66);
			labelInfo.Name = "labelInfo";
			labelInfo.Size = new System.Drawing.Size(280, 68);
			labelInfo.TabIndex = 7;
			labelInfo.Text =
				"Remember to enable \'Allow programs on this system to control XBMC\' in XBMC option"
				+ "s.\r\n\r\nIf XBMC is in a different machine you also need to enable \'Allow programs "
				+ "on other systems to control XBMC\'.";
			// 
			// XbmcInputs
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			Name = "XbmcInputs";
			panelDetails.ResumeLayout(false);
			panelDetails.PerformLayout();
			ResumeLayout(false);
		}

		/// <summary>
		/// Initializes the configuration UI when a forwarder is being added or edited.
		/// </summary>
		/// <param name="isSubscription">will always be <c>false</c> for <see cref="ForwardDestination"/>s</param>
		/// <param name="dli">The <see cref="DestinationListItem"/> that the user selected</param>
		/// <param name="db">The <see cref="DestinationBase"/> of the item if it is being edited;<c>null</c> otherwise</param>
		/// <remarks>
		/// When an instance is being edited (<paramref name="dli"/> != null), make sure to repopulate any
		/// inputs with the current values.
		/// 
		/// By default, the 'Save' button is disabled and you must call <see cref="DestinationSettingsPanel.OnValidChanged"/>
		/// in order to enable it when appropriate.
		/// </remarks>
		public override void Initialize(bool isSubscription, DestinationListItem dli, DestinationBase db)
		{
			// set text box values
			serverText.Text = "";
			serverText.Enabled = true;

			XbmcDestination xbmc = db as XbmcDestination;
			if (xbmc != null)
			{
				serverText.Text = xbmc.Server;
			}

			ValidateInputs();

			serverText.Focus();
		}

		/// <summary>
		/// Creates a new instance of the forwarder.
		/// </summary>
		/// <returns>New <see cref="XbmcDestination"/></returns>
		/// <remarks>
		/// This is called when the user is adding a new destination and clicks the 'Save' button.
		/// </remarks>
		public override DestinationBase Create()
		{
			return new XbmcDestination(GetServer());
		}

		private string GetServer()
		{
			string ret = serverText.Text;
			ret = ret.Trim();
			if (ret == "")
				ret = "localhost";
			return ret;
		}

		/// <summary>
		/// Updates the specified instance.
		/// </summary>
		/// <param name="db">The <see cref="XbmcDestination"/> to update</param>
		/// <remarks>
		/// This is called when a user is editing an existing forwarder and clicks the 'Save' button.
		/// </remarks>
		public override void Update(DestinationBase db)
		{
			XbmcDestination xbmc = (XbmcDestination) db;
			xbmc.Server = GetServer();
		}

		private void ValidateInputs()
		{
			bool valid = true;

			// Server
			if (String.IsNullOrEmpty(serverText.Text))
			{
				serverText.Highlight();
				valid = false;
			}
			else
			{
				serverText.Unhighlight();
			}

			OnValidChanged(valid);
		}

		private void serverText_TextChanged(object sender, EventArgs e)
		{
			ValidateInputs();
		}
	}
}
