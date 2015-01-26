using System.Collections.Generic;
using Kohl.Framework;
using Kohl.Framework.Lists;
using Terminals.Configuration.Serialization;

namespace Terminals.Configuration.Files.History
{
    public class HistoryByFavorite : SerializableSortedDictionary<string, List<HistoryItem>>
    {
        public const string TODAY = "Today";
        public const string YESTERDAY = "Yesterday";
        public const string WEEK = "Less than 1 Week";
        public const string TWOWEEKS = "Less than 2 Weeks";
        public const string MONTH = "Less than 1 Month";
        public const string OVERONEMONTH = "Over 1 Month";
        public const string HALFYEAR = "Over 6 Months";
        public const string YEAR = "Over 1 Year";

        public SerializableDictionary<string, SortableList<HistoryItem>> GroupByDate()
        {
            Kohl.Framework.Logging.Log.Info("Grouping history items by date.");

            SerializableDictionary<string, SortableList<HistoryItem>> groupedByDate = this.InitializeGroups();

            foreach (string name in this.Keys) //name is the favorite name
            {
                foreach (HistoryItem item in this[name]) //each history item per favorite
                {
                    SortableList<HistoryItem> timeIntervalItems = this.GetTimeIntervalItems(item.DateGroup,
                                                                                            groupedByDate);
                    if (!timeIntervalItems.Contains(item))
                        timeIntervalItems.Add(item);
                }
            }
            return groupedByDate;
        }

        private SerializableDictionary<string, SortableList<HistoryItem>> InitializeGroups()
        {
            SerializableDictionary<string, SortableList<HistoryItem>> groupedByDate =
                new SerializableDictionary<string, SortableList<HistoryItem>>();
            groupedByDate.Add(TODAY, new SortableList<HistoryItem>());
            groupedByDate.Add(YESTERDAY, new SortableList<HistoryItem>());
            groupedByDate.Add(WEEK, new SortableList<HistoryItem>());
            groupedByDate.Add(TWOWEEKS, new SortableList<HistoryItem>());
            groupedByDate.Add(MONTH, new SortableList<HistoryItem>());
            groupedByDate.Add(OVERONEMONTH, new SortableList<HistoryItem>());
            groupedByDate.Add(HALFYEAR, new SortableList<HistoryItem>());
            groupedByDate.Add(YEAR, new SortableList<HistoryItem>());
            return groupedByDate;
        }

        /// <summary>
        ///     this will contain each name in each bin of grouped by time.
        ///     Returns not null list of items.
        /// </summary>
        private SortableList<HistoryItem> GetTimeIntervalItems(string timeKey,
                                                               SerializableDictionary<string, SortableList<HistoryItem>>
                                                                   groupedByDate)
        {
            if (!groupedByDate.ContainsKey(timeKey))
            {
                SortableList<HistoryItem> timeIntervalItems = new SortableList<HistoryItem>();
                groupedByDate.Add(timeKey, timeIntervalItems);
            }

            return groupedByDate[timeKey];
        }
    }
}