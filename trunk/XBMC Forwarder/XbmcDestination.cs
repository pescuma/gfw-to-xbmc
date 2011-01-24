using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Growl.Connector;
using Growl.Destinations;
using XBMC;

namespace org.pescuma.XbmcForwarder
{
	[Serializable]
	public class XbmcDestination : ForwardDestination
	{
		public XbmcDestination(string server, int port, bool useHTTP)
			: base("XBMC", true)
		{
			Server = server;
			Port = port;
			UseHTTP = useHTTP;
		}

		public string Server { get; set; }

		public int Port { get; set; }

		public bool UseHTTP { get; set; }

		public bool IsLocalhost
		{
			get
			{
				return "localhost".Equals(Server, StringComparison.CurrentCultureIgnoreCase)
				       || "127.0.0.1".Equals(Server, StringComparison.CurrentCultureIgnoreCase);
			}
		}

		/// <summary>
		/// Gets the address display.
		/// </summary>
		/// <value>The address display.</value>
		/// <remarks>
		/// This is shown in GfW as the second line of the item in the 'Forwards' list view.
		/// </remarks>
		public override string AddressDisplay
		{
			get { return Server; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="XbmcDestination"/> is available.
		/// </summary>
		/// <value>Always returns <c>true</c>.</value>
		/// <remarks>
		/// This value is essentially read-only. Setting the value will have no effect.
		/// </remarks>
		public override bool Available
		{
			get { return true; }
			protected set
			{
				//throw new Exception("The method or operation is not implemented.");
			}
		}

		/// <summary>
		/// Called when a notification is received by GfW.
		/// </summary>
		/// <param name="notification">The notification information</param>
		/// <param name="callbackContext">The callback context.</param>
		/// <param name="requestInfo">The request info.</param>
		/// <param name="isIdle"><c>true</c> if the user is currently idle;<c>false</c> otherwise</param>
		/// <param name="callbackFunction">The function GfW will run if this notification is responded to on the forwarded computer</param>
		/// <remarks>
		/// Unless your forwarder is going to handle socket-style callbacks from the remote computer, you should ignore
		/// the <paramref name="callbackFunction"/> parameter.
		/// </remarks>
		public override void ForwardNotification(Growl.Connector.Notification notification,
		                                         Growl.Connector.CallbackContext callbackContext,
		                                         Growl.Connector.RequestInfo requestInfo, bool isIdle,
		                                         ForwardedNotificationCallbackHandler callbackFunction)
		{
			try
			{
				Image img = null;

				if (UseHTTP)
				{
					StringBuilder url = new StringBuilder();

					url.Append("http://").Append(Server);
					if (Port > 0)
						url.Append(":").Append(Port);

					url.Append("/xbmcCmds/xbmcHttp?command=ExecBuiltIn(Notification(").Append(
						Encode(notification.Title)).Append(",").Append(Encode(notification.Text));

					url.Append(",").Append(notification.Sticky ? 20000 : 4000);

					if (IsLocalhost)
					{
						img = GetImage(notification);
						if (img != null)
						{
							var filename = GetTmpFile(".png");
							img.Save(filename, ImageFormat.Png);
							url.Append(",").Append(filename);
						}
					}

					url.Append("))");

					Growl.CoreLibrary.WebClientEx wc = new Growl.CoreLibrary.WebClientEx();
					using (wc)
					{
						wc.Headers.Add(HttpRequestHeader.UserAgent, "Growl for Windows XBMC Plugin/1.0");
						string result = wc.UploadString(url.ToString(), "");

						if (result.IndexOf("OK") < 0)
							Growl.CoreLibrary.DebugInfo.WriteLine("XBMC forwarding failed: " + result);
					}
				}
				else
				{
					string tempFile = "";

					try
					{
						var iconType = IconType.ICON_NONE;
						img = GetImage(notification);
						if (img != null)
						{
							tempFile = Path.GetTempFileName();
							iconType = IconType.ICON_PNG;
							img.Save(tempFile, ImageFormat.Png);
						}

						EventClient eventClient = new EventClient();

						if (!eventClient.Connect(Server, Port > 0 ? Port : EventClient.STD_PORT))
						{
							Growl.CoreLibrary.DebugInfo.WriteLine("Could not connect to XBMC server at " + Server);
							return;
						}

						if (
							!eventClient.SendNotification(ToSingleLine(notification.Title),
							                              ToSingleLine(notification.Text), iconType, tempFile))
							Growl.CoreLibrary.DebugInfo.WriteLine("Error sending notification");

						eventClient.Disconnect();
					}
					finally
					{
						if (img != null)
							img.Dispose();

						Delete(tempFile);
					}
				}
			}
			catch (Exception ex)
			{
				Growl.CoreLibrary.DebugInfo.WriteLine("XBMC forwarding failed: " + ex.Message);
			}
		}

		private Image GetImage(Notification notification)
		{
			Image img = notification.Icon;
			if (img != null)
			{
				if (img.Size.Width > 48 || img.Size.Height > 48)
				{
					var tmp = ResizeImage(img, new Size(48, 48));
					img.Dispose();
					img = tmp;
				}
			}
			return img;
		}

		private static Image ResizeImage(Image imgToResize, Size size)
		{
			int sourceWidth = imgToResize.Width;
			int sourceHeight = imgToResize.Height;

			float nPercent = 0;
			float nPercentW = 0;
			float nPercentH = 0;

			nPercentW = (size.Width / (float) sourceWidth);
			nPercentH = (size.Height / (float) sourceHeight);

			if (nPercentH < nPercentW)
				nPercent = nPercentH;
			else
				nPercent = nPercentW;

			int destWidth = (int) (sourceWidth * nPercent);
			int destHeight = (int) (sourceHeight * nPercent);

			Bitmap b = new Bitmap(destWidth, destHeight);
			Graphics g = Graphics.FromImage(b);
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;

			g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
			g.Dispose();

			return b;
		}

		private string Encode(String text)
		{
			return HttpUtility.UrlEncode(ToSingleLine(text));
		}

		private string ToSingleLine(string text)
		{
			return text.Replace("\r\n", "  ").Replace("\n", "  ");
		}

		/// <summary>
		/// Called when an application registration is received by GfW.
		/// </summary>
		/// <param name="application">The application.</param>
		/// <param name="notificationTypes">The notification types.</param>
		/// <param name="requestInfo">The request info.</param>
		/// <param name="isIdle"><c>true</c> if the user is currently idle;<c>false</c> otherwise</param>
		/// <remarks>
		/// Many types of forwarders can just ignore this event.
		/// </remarks>
		public override void ForwardRegistration(Growl.Connector.Application application,
		                                         List<Growl.Connector.NotificationType> notificationTypes,
		                                         Growl.Connector.RequestInfo requestInfo, bool isIdle)
		{
			// do nothing
		}

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns><see cref="XbmcDestination"/></returns>
		public override DestinationBase Clone()
		{
			return new XbmcDestination(Server, Port, UseHTTP);
		}

		/// <summary>
		/// Gets the icon that represents this type of forwarder.
		/// </summary>
		/// <returns><see cref="System.Drawing.Image"/></returns>
		public override Image GetIcon()
		{
			return XbmcForwardHandler.GetIcon();
		}

		private class TmpFile
		{
			public readonly string TempFile;
			public readonly string ImageFile;
			public readonly DateTime CreationTime;

			public TmpFile(string tempFile, string extension)
			{
				TempFile = tempFile;
				ImageFile = tempFile + extension;
				CreationTime = DateTime.Now;
			}
		}

		private static readonly List<TmpFile> TmpFiles = new List<TmpFile>();

		private static string GetTmpFile(string extension)
		{
			DeleteOldFiles();

			TmpFile file = new TmpFile(Path.GetTempFileName(), extension);
			TmpFiles.Add(file);

			return file.ImageFile;
		}

		private static void DeleteOldFiles()
		{
			DateTime now = DateTime.Now;
			for (int i = TmpFiles.Count - 1; i >= 0; i--)
			{
				var file = TmpFiles[i];

				if ((now - file.CreationTime).TotalSeconds > 30)
				{
					Delete(file.ImageFile);
					Delete(file.TempFile);
					TmpFiles.RemoveAt(i);
				}
			}
		}

		private static void Delete(string tmpFile)
		{
			if (string.IsNullOrEmpty(tmpFile))
				return;

			try
			{
				File.Delete(tmpFile);
			}
			catch (Exception e)
			{
				Growl.CoreLibrary.DebugInfo.WriteLine("Error deleting tmp file: " + e.Message);
			}
		}
	}
}
