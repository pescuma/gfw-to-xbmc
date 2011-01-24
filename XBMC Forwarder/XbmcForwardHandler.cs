using System;
using System.Collections.Generic;
using Growl.Destinations;

namespace org.pescuma.XbmcForwarder
{
	/// <summary>
	/// Manages the creation and operation of the forwarder.
	/// </summary>
	public class XbmcForwardHandler : IForwardDestinationHandler
	{
		#region IDestinationHandler Members

		/// <summary>
		/// The name of the instance
		/// </summary>
		/// <value>string</value>
		public string Name
		{
			get { return "XBMC"; }
		}

		/// <summary>
		/// Registers the forwarder with Growl.
		/// </summary>
		/// <returns><see cref="List[Type]"/></returns>
		/// <remarks>
		/// A single handler can register multiple forwarder types if desired.
		/// However, most of the time, you will return a list with just a single
		/// item in it.
		/// </remarks>
		public List<Type> Register()
		{
			List<Type> list = new List<Type>();
			list.Add(typeof (XbmcDestination));
			return list;
		}

		/// <summary>
		/// Gets the list of <see cref="DestinationListItem"/>s to display as choices when
		/// the user chooses 'Add Forward'.
		/// </summary>
		/// <returns><see cref="List[DestinationListItem]"/></returns>
		/// <remarks>
		/// A single handler can return multiple list entries if appropriate (for example, the Bonjour forwarder
		/// detects other computers on the network and returns each as a separate list item).
		/// However, most of the time, you will return a list with just a single
		/// item in it.
		/// </remarks>
		public List<DestinationListItem> GetListItems()
		{
			ForwardDestinationListItem item =
				new ForwardDestinationListItem("Click here to forward notifications to\r\nXBMC", GetIcon(), this);
			List<DestinationListItem> list = new List<DestinationListItem>();
			list.Add(item);
			return list;
		}

		/// <summary>
		/// Gets the settings panel associated with this forwarder.
		/// </summary>
		/// <param name="dbli">The <see cref="DestinationListItem"/> as selected by the user</param>
		/// <returns><see cref="DestinationSettingsPanel"/></returns>
		/// <remarks>
		/// This is called when a user is adding a new forwarding destination.
		/// </remarks>
		public DestinationSettingsPanel GetSettingsPanel(DestinationListItem dbli)
		{
			return new XbmcInputs();
		}

		/// <summary>
		/// Gets the settings panel associated with this forwarder.
		/// </summary>
		/// <param name="db">The <see cref="DestinationBase"/> of an exiting forwarder</param>
		/// <returns><see cref="DestinationSettingsPanel"/></returns>
		/// <remarks>
		/// This is called when a user is editing an existing forwarder.
		/// </remarks>
		public DestinationSettingsPanel GetSettingsPanel(DestinationBase db)
		{
			return new XbmcInputs();
		}

		#endregion

		/// <summary>
		/// Gets the icon associated with this forwarder.
		/// </summary>
		/// <returns><see cref="System.Drawing.Image"/></returns>
		internal static System.Drawing.Image GetIcon()
		{
			return new System.Drawing.Bitmap(Properties.Resources.xbmc);
		}
	}
}
