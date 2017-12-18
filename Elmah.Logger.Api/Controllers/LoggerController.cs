﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Elmah.Io.Client;
using Elmah.Net.Logger.Data;
using Elmah.Net;

namespace Elmah.Net.Logger.Web
{
    [RoutePrefix("api.logger")]
    public class LoggerController : ApiController
    {
        //protected SiteInstance Site { get; set; }
        protected LoggerContext db { get; set; }
		//protected ISiteInstance SiteInstance { get; set; }

		public LoggerController(LoggerContext loggerContext) // , ISiteInstance siteInstance//SiteInstance site,
		{
            //this.Site = site;
            this.db = loggerContext;
			//this.SiteInstance = siteInstance;
        }

		[HttpGet]
        [Route("messages/{id}")]
        public LogMessage Detail(long id, HydrationLevel level = HydrationLevel.Detailed)
        {
            var message = db.LogMessages.Where(x => x.Id == id)
                    .Include(x => x.Log)
                    .Include(x => x.User)
                    .Include(x => x.Source)
                    .Include(x => x.MessageType)
                    .Include(x => x.Application)
                    .Include(x => x.Detail)
                    .FirstOrDefault();

            return new LogMessage(message, level);
        }

        [HttpDelete]
        [Route("messages/{id}")]
        public LogMessage Delete(long id)
        {
            //using (LoggerContext db = new LoggerContext())
            //{
            var message = db.LogMessages.Where(x => x.Id == id).FirstOrDefault();

            db.LogMessages.Remove(message);
            db.SaveChanges();

            return new LogMessage(message, HydrationLevel.Basic);
            //}
        }


        [HttpGet]
        [Route("messages/init")]
        public LogSearchResponseModel Init()
        {			
            LogSearchResponseModel model = new LogSearchResponseModel();
			model.Logs = db.Logs.ToList().Select(y => new LogObject(y)).ToList();
            model.Applications = db.LogApplications.ToList().Select(y => new LogObject(y)).ToList();
            //model.Users = db.Users.Select(y => new LogObject(y)).ToList();
            model.Types = db.MessageTypes.ToList().Select(y => new LogObject(y)).ToList();
            model.Sources = db.MessageSources.ToList().Select(y => new LogObject(y)).ToList();
            model.Severities = EnumHelper.GetValues<LogSeverity>().Select(y => new LogObject(y)).ToList();
            model.Spans = EnumHelper.GetValues<SearchSpan>().Select(y => new LogObject(y)).OrderBy(x => x.Id).ToList();
            return model;
        }

        /// <summary>
        /// Search and Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("messages/search/{id}")]
        public LogSearchResponseModel Search(string id, Filter filter)
        {
            var obj = db.Filters.Where(x => x.Name == id).FirstOrDefault();            
            if (obj == null)
            {
                obj = new Data.Filter() { Name = id };
                db.Filters.Add(obj);
            }
            obj.Title = filter.Title;
            obj.Description = filter.Description;
            obj.Distribution = filter.Distribution;
            obj.Query = filter.Query;
            db.SaveChanges();

            return Search(filter.Query);
        }

        /// <summary>
        /// Search By Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("messages/search/{name}")]
        public LogSearchResponseModel SearchByName(string name)
        {
            var filter = db.Filters.Where(x => x.Name == name).FirstOrDefault();
            if (filter != null)
            {
                return Search(filter.Query);
            }
            return null;
        }

        /// <summary>
        /// Delete Search By Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("messages/search/{name}")]
        public LogSearchResponseModel DeleteByName(string name)
        {
            var filter = db.Filters.Where(x => x.Name == name).FirstOrDefault();
            if (filter != null)
            {
                db.Filters.Remove(filter);
                db.SaveChanges();
            }
            return Search(new Query());
        }


		/// <summary>
		/// Defaulting to [FromUri] for GET and HEAD requests
		/// http://www.strathweb.com/2013/04/asp-net-web-api-parameter-binding-part-1-understanding-binding-from-uri/
		/// </summary>
		/// <param name="query"></param>
		/// <param name="level"></param>
		/// <param name="log"></param>
		/// <param name="application"></param>
		/// <returns></returns>
		[HttpGet]
        [Route("messages/search")]
        public LogSearchResponseModel Search(Query query, HydrationLevel level = HydrationLevel.Basic) //, string log = null, string application = null
		{
			var search = new SearchQuery(db);

			var messages = search.Search(query);
			
			//// convert to utc
			//if (query.Start.HasValue) query.Start.Value.ToUniversalTime();
   //         if (query.End.HasValue) query.End.Value.ToUniversalTime();

   //         // convert to span
   //         if (!query.Start.HasValue && query.Span > 0)
   //         {
   //             query.Start = DateTime.UtcNow.AddMinutes((int)query.Span * -1);
   //         }

   //         IQueryable<Elmah.Net.Logger.Data.LogMessage> messages = null;

   //         if (query.Start.HasValue)
   //         {
   //             if (query.End.HasValue)
   //             {
			//		messages = (IQueryable<Elmah.Net.Logger.Data.LogMessage>)db.LogMessages.Where(x => x.DateTime >= query.Start && x.DateTime <= query.End);
			//	}
   //             else
   //             {
			//		messages = (IQueryable<Elmah.Net.Logger.Data.LogMessage>)db.LogMessages.Where(x => x.DateTime >= query.Start);
			//	}
   //         }
   //         else
   //         {
			//	messages = (IQueryable<Elmah.Net.Logger.Data.LogMessage>)db.LogMessages;
			//}

			////Log theLog = null;
			//if (!string.IsNullOrWhiteSpace(log))
   //         {
   //             //var start = DateTime.Now.Date;
   //             //var allLogs = (IQueryable<Elmah.Net.Logger.Data.Log>)db.Logs.OrderBy(x => x.Name);
   //             var theLog = db.Logs.FirstOrDefault(x => x.Name == log);
   //             if (theLog != null)
   //             {
   //                 messages = messages.Where(x => x.LogId == theLog.Id);
   //             }
   //         }

   //         if (query.Logs != null && query.Logs.Any())
   //         {
   //             messages = messages.Where(x => query.Logs.Any(y => y == x.LogId));
   //         }

   //         // application
   //         if (!string.IsNullOrWhiteSpace(application))
   //         {
   //             var obj = db.LogApplications.FirstOrDefault(x => x.Name == application);
   //             if (obj != null)
   //             {
   //                 messages = messages.Where(x => x.ApplicationId == obj.Id);
   //                 //logs = logs.Where(x => x.Applications.Any(a => a.Id == obj.Id));						
   //             }
   //         }

   //         // applications
   //         if (query.Applications != null && query.Applications.Any())
   //         {
   //             //var list = types.Select(x => x.Id);
   //             messages = messages.Where(x => query.Applications.Any(y => y == x.ApplicationId));
   //         }

   //         //// types
   //         //if (!string.IsNullOrWhiteSpace(type))
   //         //{
   //         //    var obj = db.MessageTypes.FirstOrDefault(x => x.Name == type);
   //         //    if (obj != null)
   //         //    {
   //         //        messages = messages.Where(x => x.MessageTypeId == obj.Id);
   //         //    }
   //         //}

   //         if (query.Types != null && query.Types.Any())
   //         {
   //             //var list = types.Select(x => x.Id);
   //             messages = messages.Where(x => query.Types.Any(y => y == x.MessageTypeId));
   //         }

   //         if (query.Sources != null && query.Sources.Any())
   //         {
   //             //var list = sources.Select(x => x.Id);
   //             messages = messages.Where(x => query.Sources.Any(y => y == x.SourceId));
   //         }

   //         if (query.Users != null && query.Users.Any())
   //         {
   //             //var list = users.Select(x => x.Id);
   //             messages = messages.Where(x => query.Users.Any(y => y == x.UserId));
   //         }

   //         //// sources
   //         //if (!string.IsNullOrWhiteSpace(source))
   //         //{
   //         //    var obj = db.MessageSources.FirstOrDefault(x => x.Name == source);
   //         //    if (obj != null)
   //         //    {
   //         //        messages = messages.Where(x => x.SourceId == obj.Id);
   //         //    }
   //         //}


   //         //// users
   //         //if (!string.IsNullOrWhiteSpace(user))
   //         //{
   //         //    var obj = db.Users.FirstOrDefault(x => x.Name == user);
   //         //    if (obj != null)
   //         //    {
   //         //        messages = messages.Where(x => x.UserId == obj.Id);
   //         //    }
   //         //}

   //         // severities
   //         if (query.Severities != null && query.Severities.Any())
   //         {
   //             //var list = users.Select(x => x.Id);
   //             messages = messages.Where(x => query.Severities.Any(y => y == x.Severity));
   //         }

			//// severity
			////if (severity.HasValue) { messages = messages.Where(x => x.Severity == severity); }

			////if (query.Limit > 0)
   ////         {
   ////             messages = messages.Take(query.Limit);
   ////         }


			//// Includes and order by
			//messages = messages
			//	.Include(x => x.Log)
			//	.Include(x => x.User)
			//	.Include(x => x.Source)
			//	.Include(x => x.MessageType)
			//	.Include(x => x.Application)
			//	.OrderByDescending(x => x.DateTime);


			// get filters
			var filters = db.Filters.OrderBy(x => x.Name).ToList();

			//var counts = messages.GroupBy(x => x.MessageType).Select(z => new { type = z.Key, count = z.Count() }).ToList();
			//var counts = messages.GroupBy(x => new { Year = x.DateTime.Year, Month = x.DateTime.Month } ).Select(z => new { type = z.Key, count = z.Count() }).ToList();

			//var counts = messages.GroupBy(x => new { Year = x.DateTime.Year, Month = x.DateTime.Month, Day = x.DateTime.Day, Hour = x.DateTime.Hour }).Select(z => new { type = z.Key, count = z.Count() }).ToList();
			var counts = messages.GroupBy(x => new { Year = x.DateTime.Year, Month = x.DateTime.Month, Day = x.DateTime.Day }).Select(z => new { type = z.Key, count = z.Count() }).ToList();
			var typeCounts = messages.GroupBy(x => new { TypeId = x.MessageTypeId, Year = x.DateTime.Year, Month = x.DateTime.Month, Day = x.DateTime.Day }).Select(z => new { type = z.Key, count = z.Count() }).ToList();

			if (query.Limit > 0)
			{
				messages = messages.Take(query.Limit);
			}

			//var newCount = counts.Select(x => new LogCount() { Id = x.type.Id, Name = x.type.Name, Count = x.count }).ToList();

			// Setup model
			var model = new LogSearchResponseModel(query, messages.ToList(), filters, level);

            // always return all logs and applications...
            model.Logs = db.Logs.ToList().Select(y => new LogObject(y)).ToList();
            model.Applications = db.LogApplications.ToList().Select(y => new LogObject(y)).ToList();
            model.Severities = EnumHelper.GetValues<LogSeverity>().Select(y => new LogObject(y)).ToList();
            model.Types = db.MessageTypes.ToList().Select(y => new LogObject(y)).ToList();
			//model.Sources = db.MessageSources.ToList().Select(y => new LogObject(y)).ToList();
			//model.Spans = EnumHelper.GetValues<SearchSpan>().Select(y => new LogObject(y)).ToList();
						

			//model.TypeCount = counts.OrderBy(x=>x.type.Year * 1000000 + x.type.Month * 10000 + x.type.Day * 100  + x.type.Hour).Select(x => new LogCount() { Id = 0, Name = new DateTime(x.type.Year, x.type.Month, x.type.Day, x.type.Hour, 0, 0).ToString(), Count = x.count }).ToList();
			//model.TypeCount = counts
			//	.OrderBy(x => x.type.Year * 1000000 + x.type.Month * 10000 + x.type.Day * 100)
			//	.Select(x => new LogCount() { Id = 0, Name = new DateTime(x.type.Year, x.type.Month, x.type.Day).ToString(), Count = x.count }).ToList();

			// count by type / id
			model.TypeCount2 = typeCounts
				.OrderBy(x => x.type.Year * 1000000 + x.type.Month * 10000 + x.type.Day * 100)
				.Select(x => new LogCount() { Id = x.type.TypeId ?? 0, Name = new DateTime(x.type.Year, x.type.Month, x.type.Day).ToString(), Count = x.count }).ToList();

			//model.TypeCount = counts.OrderBy(x => x.type.Year * 1000000 + x.type.Month * 10000 + x.type.Day + x.type.Hour).Select(x => new LogCount() { Id = 0, Name = new DateTime(x.type.Year, x.type.Month, x.type.Day).ToString(), Count = x.count }).ToList();
			//model.TypeCount = counts.OrderBy(x => x.type).Select(x => new LogCount() { Id = 0, Name = new DateTime(x.type.Year, x.type.Month, x.type.Day, x.type.Hour, 0, 0).ToString(), Count = x.count }).ToList();
			//model.TypeCount = counts.OrderBy(x => x.type).Select(x => new LogCount() { Id = 0, Name = new x.ToString(), Count = x.count }).ToList();
			//model.TypeCount = model.TypeCount.OrderBy(x => x.Name).ToList();

			// Make sure Query Items are in lists!
			foreach (var type in query.Types)
            {
                if (!model.Types.Any(x => x.Id == type))
                {
                    var obj = db.MessageTypes.Find(type);
                    if (obj != null) { model.Types.Add(new LogObject(obj)); }
                }
            }
            foreach (var key in query.Logs)
            {
                if (!model.Logs.Any(x => x.Id == key))
                {
                    var obj = db.Logs.Find(key);
                    if (obj != null) { model.Logs.Add(new LogObject(obj)); }
                }
            }
            foreach (var key in query.Applications)
            {
                if (!model.Applications.Any(x => x.Id == key))
                {
                    var obj = db.LogApplications.Find(key);
                    if (obj != null) { model.Applications.Add(new LogObject(obj)); }
                }
            }
            foreach (var key in query.Sources)
            {
                if (!model.Sources.Any(x => x.Id == key))
                {
                    var obj = db.MessageSources.Find(key);
                    if (obj != null) { model.Sources.Add(new LogObject(obj)); }
                }
            }
            foreach (var key in query.Users)
            {
                if (!model.Users.Any(x => x.Id == key))
                {
                    var obj = db.Users.Find(key);
                    if (obj != null) { model.Users.Add(new LogObject(obj)); }
                }
            }
            foreach (var key in query.Severities)
            {
                if (!model.Severities.Any(x => x.Id == (long)key))
                {
                    model.Severities.Add(new LogObject(key));
                }
            }

            return model;
        }
    }
}