using System;
using System.Windows.Forms;
using Growl.Destinations;

namespace org.pescuma.XbmcForwarder
{
	public class XbmcInputs : DestinationSettingsPanel
	{
		private Label portLabel;
		private Label serverLabel;
		private HighlightTextBox serverText;
		private Label labelInfo;
		private ComboBox connectionTypeCombo;
		private Label connectionTypeLabel;
		private HighlightTextBox portText;

		public XbmcInputs()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XbmcInputs));
			this.portText = new Growl.Destinations.HighlightTextBox();
			this.portLabel = new System.Windows.Forms.Label();
			this.serverText = new Growl.Destinations.HighlightTextBox();
			this.serverLabel = new System.Windows.Forms.Label();
			this.labelInfo = new System.Windows.Forms.Label();
			this.connectionTypeLabel = new System.Windows.Forms.Label();
			this.connectionTypeCombo = new System.Windows.Forms.ComboBox();
			this.panelDetails.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelDetails
			// 
			this.panelDetails.Controls.Add(this.serverLabel);
			this.panelDetails.Controls.Add(this.serverText);
			this.panelDetails.Controls.Add(this.portLabel);
			this.panelDetails.Controls.Add(this.portText);
			this.panelDetails.Controls.Add(this.connectionTypeCombo);
			this.panelDetails.Controls.Add(this.connectionTypeLabel);
			this.panelDetails.Controls.Add(this.labelInfo);
			// 
			// portText
			// 
			this.portText.HighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(250)))), ((int)(((byte)(184)))));
			this.portText.Location = new System.Drawing.Point(108, 35);
			this.portText.Name = "portText";
			this.portText.Size = new System.Drawing.Size(211, 20);
			this.portText.TabIndex = 4;
			// 
			// portLabel
			// 
			this.portLabel.AutoSize = true;
			this.portLabel.Location = new System.Drawing.Point(19, 38);
			this.portLabel.Name = "portLabel";
			this.portLabel.Size = new System.Drawing.Size(29, 13);
			this.portLabel.TabIndex = 3;
			this.portLabel.Text = "Port:";
			// 
			// serverText
			// 
			this.serverText.HighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(250)))), ((int)(((byte)(184)))));
			this.serverText.Location = new System.Drawing.Point(108, 9);
			this.serverText.Name = "serverText";
			this.serverText.Size = new System.Drawing.Size(211, 20);
			this.serverText.TabIndex = 2;
			// 
			// serverLabel
			// 
			this.serverLabel.AutoSize = true;
			this.serverLabel.Location = new System.Drawing.Point(19, 12);
			this.serverLabel.Name = "serverLabel";
			this.serverLabel.Size = new System.Drawing.Size(83, 13);
			this.serverLabel.TabIndex = 1;
			this.serverLabel.Text = "XBMC machine:";
			// 
			// labelInfo
			// 
			this.labelInfo.Location = new System.Drawing.Point(22, 94);
			this.labelInfo.Name = "labelInfo";
			this.labelInfo.Size = new System.Drawing.Size(296, 68);
			this.labelInfo.TabIndex = 7;
			this.labelInfo.Text = resources.GetString("labelInfo.Text");
			// 
			// connectionTypeLabel
			// 
			this.connectionTypeLabel.AutoSize = true;
			this.connectionTypeLabel.Location = new System.Drawing.Point(19, 66);
			this.connectionTypeLabel.Name = "connectionTypeLabel";
			this.connectionTypeLabel.Size = new System.Drawing.Size(87, 13);
			this.connectionTypeLabel.TabIndex = 5;
			this.connectionTypeLabel.Text = "Connection type:";
			// 
			// connectionTypeCombo
			// 
			this.connectionTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.connectionTypeCombo.FormattingEnabled = true;
			this.connectionTypeCombo.Items.AddRange(new object[] {
            "Event client",
            "HTTP"});
			this.connectionTypeCombo.Location = new System.Drawing.Point(108, 63);
			this.connectionTypeCombo.Name = "connectionTypeCombo";
			this.connectionTypeCombo.Size = new System.Drawing.Size(211, 21);
			this.connectionTypeCombo.TabIndex = 6;
			// 
			// XbmcInputs
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.Name = "XbmcInputs";
			this.panelDetails.ResumeLayout(false);
			this.panelDetails.PerformLayout();
			this.ResumeLayout(false);

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
			portText.Text = "";
			portText.Enabled = true;
			connectionTypeCombo.SelectedIndex = 0;
			connectionTypeCombo.Enabled = true;

			XbmcDestination xbmc = db as XbmcDestination;
			if (xbmc != null)
			{
				serverText.Text = xbmc.Server;
				portText.Text = (xbmc.Port <= 0 ? "" : xbmc.Port.ToString());
				connectionTypeCombo.SelectedIndex = xbmc.UseHTTP ? 1 : 0;
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
			return new XbmcDestination(GetServer(), GetPort(), GetUseHTTP());
		}

		private bool GetUseHTTP()
		{
			return connectionTypeCombo.SelectedIndex == 1;
		}

		private string GetServer()
		{
			string ret = serverText.Text;
			ret = ret.Trim();
			if (ret == "")
				ret = "localhost";
			return ret;
		}

		private int GetPort()
		{
			int port;
			if (!int.TryParse(portText.Text, out port))
				port = 0;
			return port;
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
			xbmc.Port = GetPort();
			xbmc.UseHTTP = GetUseHTTP();
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

			// Port
			if (String.IsNullOrEmpty(portText.Text))
			{
				// Use default (aka valid)
			}
			else
			{
				int port;
				if (!int.TryParse(portText.Text, out port))
				{
					portText.Highlight();
					valid = false;
				}
				else
				{
					portText.Unhighlight();
				}
			}

			OnValidChanged(valid);
		}

		private void serverText_TextChanged(object sender, EventArgs e)
		{
			ValidateInputs();
		}

		private void portText_TextChanged(object sender, EventArgs e)
		{
			ValidateInputs();
		}

		private void connectionTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			ValidateInputs();
		}
	}
}
