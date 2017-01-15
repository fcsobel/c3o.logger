﻿using System;
using System.Linq;
using Elmah.Io.Client;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elmah.Net.Logger.Data
{
	public static class ElamhIoHelper
	{
		public static string GetValue(this List<Item> list, string key)
		{
			if (list != null && list.Any())
			{
				var item = list.Find(x => x.Key == key);
				if (item != null)
				{
					return item.Value;
				}
			}
			return null;
		}

		public static string GetDelta(this DateTime dt)
		{
			var ts = new TimeSpan(DateTime.UtcNow.Ticks - dt.Ticks);
			double delta = Math.Abs(ts.TotalSeconds);

			if (delta < 60)
			{
				return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
			}
			if (delta < 120)
			{
				return "a minute ago";
			}
			if (delta < 2700) // 45 * 60
			{
				return ts.Minutes + " minutes ago";
			}
			if (delta < 5400) // 90 * 60
			{
				return "an hour ago";
			}
			if (delta < 86400) // 24 * 60 * 60
			{
				return ts.Hours + " hours ago";
			}
			if (delta < 172800) // 48 * 60 * 60
			{
				return "yesterday";
			}
			if (delta < 2592000) // 30 * 24 * 60 * 60
			{
				return ts.Days + " days ago";
			}
			if (delta < 31104000) // 12 * 30 * 24 * 60 * 60
			{
				int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
				return months <= 1 ? "one month ago" : months + " months ago";
			}
			int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
			return years <= 1 ? "one year ago" : years + " years ago";
		}
	}
}