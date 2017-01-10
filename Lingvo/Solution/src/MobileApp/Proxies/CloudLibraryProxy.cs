﻿using System;
using System.Threading.Tasks;
using Lingvo.Common;
using Lingvo.Common.Entities;

namespace MobileApp.Proxies
{
	/// <summary>
	/// Cloud library proxy.
	/// </summary>
	public class CloudLibraryProxy
	{
		private static CloudLibraryProxy instance;

		private CloudLibraryProxy()
		{
		}

		/// <summary>
		/// Gets the instance of cloud library proxy (singleton pattern).
		/// </summary>
		/// <returns>The instance.</returns>
		public static CloudLibraryProxy Instance => instance ?? (instance = new CloudLibraryProxy());

		/// <summary>
		/// Downloads a single page.
		/// </summary>
		/// <returns>The page.</returns>
		/// <param name="proxy">A proxy for the page that is downloaded</param>
		internal async Task<Page> DownloadSinglePage(PageProxy proxy)
		{
			// to do: get correspondent page from server
			return null;
		}

		/// <summary>
		/// Downloads a workbook.
		/// </summary>
		/// <returns>The workbook.</returns>
		/// <param name="workbookID">Workbook identifier.</param>
		public async Task<Workbook> DownloadWorkbook(int workbookID)
		{
			// to do: get correspondent workbook from server
			return null;
		}

		public async Task<Workbook[]> FetchAllWorkbooks()
		{
			// TODO
			return null;
		}
	}
}
