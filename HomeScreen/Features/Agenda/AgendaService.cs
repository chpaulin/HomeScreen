using GalaSoft.MvvmLight.Messaging;
using HomeScreen.Common;
using HomeScreen.Common.Configuration;
using HomeScreen.Messages;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.System.Threading;

namespace HomeScreen.Features.Agenda
{
    public class AgendaService
    {
        private const string EVENT_PATTERN = @"BEGIN:VEVENT([]\s\S\w]*?)END:VEVENT";
        private const string SUMMARY_KEY = "SUMMARY";
        private const string START_DATE_KEY = "DTSTART";
        private const string END_DATE_KEY = "DTEND";

        private readonly FeatureConfig _configuration;

        public AgendaService(FeatureConfig configuration)
        {
            _configuration = configuration;
        }

        public async Task<IReadOnlyCollection<EventData>> RetrieveCalendarData()
        {
            var outcome = await PollyUtility.ExecuteWebRequest(async () =>
            {
                var startTime = DateTime.Today;
                var endTime = startTime.AddHours(23).AddMinutes(59).AddSeconds(59);

                var url = _configuration.Settings["agendaDataUrl"];

                var request = WebRequest.CreateHttp(url);
                var response = await request.GetResponseAsync();

                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var calendarText = reader.ReadToEnd();

                    var regEx = new Regex(EVENT_PATTERN);

                    var matches = regEx.Matches(calendarText);

                    var events = new List<EventData>();

                    foreach (Match match in matches)
                    {
                        using (var eventReader = new StringReader(match.Value))
                        {
                            string line = null;
                            var values = new Dictionary<string, string>();

                            do
                            {
                                line = eventReader.ReadLine();

                                if (line != null)
                                {
                                    var parts = line.Split(new[] { ':' }, 2);

                                    if (parts.Length != 2)
                                        continue;

                                    var key = parts[0].Split(';')[0];

                                    values[key] = parts[1];
                                }
                            }
                            while (line != null);

                            var @event = new EventData
                            {
                                Subject = values[SUMMARY_KEY],
                                Start = GetDate(values[START_DATE_KEY]),
                                End = GetDate(values[END_DATE_KEY]),
                                IsAllDay = GetIsAllDay(values)
                            };

                            events.Add(@event);
                        }
                    }

                    return events;
                }
            });

            if (outcome.Result == null)
                return new List<EventData>();

            return outcome.Result;
        }

        private bool GetIsAllDay(Dictionary<string, string> values)
        {
            var start = GetDate(values[START_DATE_KEY]);
            var end = GetDate(values[END_DATE_KEY]);

            return end.Subtract(start).TotalHours >= 24;
        }

        public async Task<DateSignificanceData> RetrieveDateSigificanceData(DateTime date)
        {
            var outcome = await PollyUtility.ExecuteWebRequest(async () =>
            {
                var urlTemplate = _configuration.Settings["dateSignificanceUrl"];
                var url = string.Format(urlTemplate, date.ToString("yyyy/M/d", CultureInfo.InvariantCulture));

                var request = WebRequest.CreateHttp(url);
                var response = await request.GetResponseAsync();

                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var data = JsonConvert.DeserializeObject<DateSignificanceData>(await reader.ReadToEndAsync());

                    return data;
                }
            });

            if (outcome.Result == null)
                return DateSignificanceData.Empty;

            return outcome.Result;
        }

        private static DateTime GetDate(string dateString)
        {
            var regEx = new Regex(@"\/\/Microsoft\/Utc" + "\"" + @":([\dT]*)");

            var match = regEx.Match(dateString);

            if (match.Success)
                dateString = match.Groups[1].Value;

            if (dateString.Length == 16)
                return DateTime.ParseExact(dateString, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            else if (dateString.Length == 15)
                return DateTime.ParseExact(dateString, "yyyyMMddTHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
            else if (dateString.Length == 8)
                return DateTime.ParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture);            
            else
                throw new FormatException($"Invalid Format for DateTime: {dateString}");
        }
    }
}
