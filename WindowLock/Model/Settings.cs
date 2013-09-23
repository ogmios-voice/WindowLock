using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace WindowLock.Model {
    public class Settings {
        public ObservableCollection<FilterData> FilterList { get; protected set; }
        public List<Regex> Filters { get; protected set; }

        public Settings() {
            FilterList = new ObservableCollection<FilterData>();
            FilterList.CollectionChanged += OnFilterListChanged;
            Filters = new List<Regex>();
        }

        public void LoadFilterData() {
            foreach(string filter in Properties.Settings.Default.Filters) {
                FilterList.Add(new FilterData(filter));
            }
        }
        public void SaveFilterData() {
            Properties.Settings.Default.Filters.Clear();
            foreach(FilterData filter in FilterList) {
                Properties.Settings.Default.Filters.Add(filter.Value);
            }
        }

        public void RefreshFilters() {
            UpdateFilters();
            ((App)Application.Current).RefreshApps(true);
        }

        protected void UpdateFilters() {
            Filters.Clear();
            string s;
            //foreach(string filter in Properties.Settings.Default.Filters) {
            foreach(FilterData filter in FilterList) {
                s = TrimComment(filter.Value);
                if(!string.IsNullOrEmpty(s)) {
                    Filters.Add(new Regex(s, RegexOptions.IgnoreCase));
                }
            }
        }

        protected string TrimComment(string s) {
            int idxComment = s.IndexOf('#'); // comment
            return idxComment >= 0 ? s.Substring(0, idxComment).Trim() : s.Trim();
        }

        public void AddEmptyFilterDataAfter(int row) {
            FilterList.Insert(row + 1, new FilterData(""));
        }

        protected void OnFilterListChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if(e.NewItems != null) {
                //Add listener for each item on PropertyChanged event
                foreach(FilterData newItem in e.NewItems) {
                    newItem.PropertyChanged += OnFilterDataChanged;
                }
            }
            if(e.OldItems != null) {
                foreach(FilterData oldItem in e.OldItems) {
                    oldItem.PropertyChanged -= OnFilterDataChanged;
                }
            }
            if(e.NewItems != null || e.OldItems != null) {
                RefreshFilters();
            }
        }

        protected void OnFilterDataChanged(object sender, PropertyChangedEventArgs e) {
            var item = sender as FilterData;
            if(item != null) {
                RefreshFilters();
            }
        }
    }
}
