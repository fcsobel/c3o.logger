﻿namespace Elmah.Net.Logger.Data
{
	//public interface ISiteInstanceContext
	//{

	//}

	public interface ISiteInstance
	{
		string SiteName { get; }
		ISiteRecord Site { get; }
		string CreateSite(ISiteRecord site);
	}
}