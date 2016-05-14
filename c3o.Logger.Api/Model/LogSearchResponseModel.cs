﻿using System;
using System.Collections.Generic;
using System.Linq;
using c3o.Core;

namespace c3o.Logger.Web
{
    public class Filter
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Distribution { get; set; }

        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Span { get; set; }
        public int Limit { get; set; }
        
        public List<LogObject> FilterTypes { get; set; }
        public List<LogObject> FilterSources { get; set; }
        public Filter()
        {
            this.FilterTypes = new List<LogObject>();
            this.FilterSources = new List<LogObject>();
        }
        public Filter(c3o.Logger.Data.Filter obj): this()
        {
            this.Id = obj.Id;
            this.Name = obj.Name;
            this.Distribution = obj.Distribution;
            this.Start = obj.Start;
            this.End = obj.End;
            this.Span = obj.Span.ToString();
            this.Limit = obj.Limit;
            this.FilterTypes = obj.FilterTypes.Select(y => new LogObject(y)).ToList();
            this.FilterSources = obj.FilterSources.Select(y => new LogObject(y)).ToList();
        }
    }
    
    public class LogSearchResponseModel
	{
		//public string Log { get; set; }
		//public string Application { get; set; }
		//public string Type { get; set; }
		//public string Source { get; set; }
		//public Elmah.Io.Client.Severity? Severity { get; set; }
		//public string User { get; set; }
		//public SearchSpan Span { get; set; }

		//public List<string> Severities { get; set; }
		public List<LogMessage> Messages { get; set; }

		public List<LogObject> Severities { get; set; }
		public List<LogObject> Logs { get; set; }
		public List<LogObject> Applications { get; set; }
		public List<LogObject> Users { get; set; }
		public List<LogObject> Types { get; set; }
		public List<LogObject> Sources { get; set; }
		public List<LogObject> Spans { get; set; }

        public List<Filter> Filters { get; set; }

        public LogSearchResponseModel() {
			this.Messages = new List<LogMessage>();
			this.Logs = new List<LogObject>();
			this.Applications = new List<LogObject>();
			this.Users = new List<LogObject>();
			this.Types = new List<LogObject>();
			this.Sources = new List<LogObject>();
            this.Filters = new List<Filter>();
		}

		public LogSearchResponseModel(List<c3o.Logger.Data.LogMessage> list, List<c3o.Logger.Data.Filter> Filters, HydrationLevel level = HydrationLevel.Basic) : this()
		{
			this.Logs =				list.Where(x=>x.Log != null).Select(x => x.Log).Distinct().Select(y => new LogObject(y)).ToList();
			this.Applications =		list.Where(x=>x.Application != null).Select(x => x.Application).Distinct().Select(y => new LogObject(y)).ToList();
			this.Users =			list.Where(x=>x.User != null).Select(x => x.User).Distinct().Select(y => new LogObject(y)).ToList();
			this.Types =			list.Where(x=>x.MessageType != null).Select(x => x.MessageType).Distinct().Select(y => new LogObject(y)).ToList();
			this.Sources =			list.Where(x=>x.Source != null).Select(x => x.Source).Distinct().Select(y => new LogObject(y)).ToList();
			this.Severities =		list.Select(x=>x.Severity).Distinct().Select(y => new LogObject { Id = 0, Name = y.ToString() }).ToList();
			//this.Spans =			EnumHelper.GetValues<SearchSpan>().Select(y => new LogObject { Id = (long)y, Name = y.ToString() }).ToList();
            this.Spans = EnumHelper.GetValues<c3o.Logger.Data.SearchSpan>().Select(y => new LogObject(y)).ToList();

            foreach (var item in list)
			{
				this.Messages.Add(new LogMessage(item));
			}

            foreach (var item in Filters)
            {
                this.Filters.Add(new Filter(item));
            }
        }
	}
}