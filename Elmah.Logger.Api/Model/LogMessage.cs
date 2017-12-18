﻿    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel;
using Elmah.Net;

namespace Elmah.Net.Logger.Web
{
	public enum HydrationLevel { Basic, Detailed }

    public class Item
	{
		public string Key { get; set; }
		public string Value { get; set; }
	}

	public class LogObject
	{
		public long Id { get; set; }
		public string Name { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public string Icon { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public string Color { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        public LogObject() { }
		public LogObject(Data.INameId obj) : this() 
		{ 
			this.Id = obj.Id;
			this.Name = obj.Name;
		} 

		public LogObject(Data.LogMessageType obj)  
		{
			this.Id = obj.Id;
			this.Name = obj.Name;
			this.Icon = obj.Icon;
			this.Color = obj.Color;		
		}

        public LogObject(Elmah.Net.Logger.Data.SearchSpan span)
        {
            this.Id = (long)span;
            this.Name = span.ToString();
            this.Description = EnumHelper.GetEnumDescription(span);
        }

        public LogObject(LogSeverity value)
        {
            this.Id = (long)value;
            this.Name = value.ToString();
            this.Description = EnumHelper.GetEnumDescription(value);
        }
    }

	public class LogMessage
	{
		public long Id { get; set; }
		public DateTime DateTime { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public string Detail { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public string Hostname { get; set; }		
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public string Severity { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public int? StatusCode { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public string Title { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public string Url { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public string IpAddress { get; set; }

		public long LogId { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public long? ApplicationId { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public long? UserId { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public long? MessageTypeId { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public long? SourceId { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public long? SessionId { get; set; }

        public int LogCount { get; set; }

        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public long? OrderId { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public long? PersonId { get; set; }








		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public List<Elmah.Io.Client.Item> Form { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public List<Elmah.Io.Client.Item> QueryString { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public List<Elmah.Io.Client.Item> ServerVariables { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public List<Elmah.Io.Client.Item> Cookies { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public List<Elmah.Io.Client.Item> Data { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public string HowLongAgo { get; set; }
						
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public LogObject Log { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public LogObject Application { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public LogObject MessageType { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public LogObject User { get; set; }
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public LogObject Source { get; set; }

		public LogMessage(Elmah.Net.Logger.Data.LogMessage obj, HydrationLevel level = HydrationLevel.Basic)
		{
			this.Id = obj.Id;
			this.DateTime = obj.DateTime;
			this.Hostname = obj.Hostname;
			this.Severity = obj.Severity.ToString();
			this.StatusCode = obj.StatusCode;
			this.Title = obj.Title;
			this.Url = obj.Url;
			this.IpAddress = obj.IpAddress;
			this.LogId = obj.LogId;
			this.ApplicationId = obj.ApplicationId;
			this.UserId = obj.UserId;
			this.MessageTypeId = obj.MessageTypeId;
			this.SourceId = obj.SourceId;
			this.HowLongAgo = obj.HowLongAgo;
            this.LogCount = obj.LogCount;

            if (level == HydrationLevel.Detailed)
            {
                // get object detail
                if (obj.Log != null) this.Log = new LogObject(obj.Log);
                if (obj.Application != null) this.Application = new LogObject(obj.Application);
                if (obj.User != null) this.User = new LogObject(obj.User);
                if (obj.Source != null) this.Source = new LogObject(obj.Source);
                if (obj.MessageType != null) this.MessageType = new LogObject(obj.MessageType);

                if (obj.Detail != null)
                {
                    this.Detail = obj.Detail.Content;
                }

                var data = obj.Original;
                if (data != null)
                {
                    this.Form = data.Form;
                    this.QueryString = data.QueryString;
                    this.ServerVariables = data.ServerVariables;
                    this.Cookies = data.Cookies;
                    this.Data = data.Data;
                }

                if (this.Form != null && this.Form.Any())
                {
                    foreach (var item in this.Form)
                    {
                        if (item.Key != null)
                        {
                            if (item.Key.ToLower().Contains("password"))
                            {
                                item.Value = new String('*', item.Value.Length);
                            }
                        }
                    }
                }
			}
            else
            {
                // still load some data
                var data = obj.Original;
                if (data != null)
                {
                    this.Data = data.Data;
                }
            }

        }
	}
}