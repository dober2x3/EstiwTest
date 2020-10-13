using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstiwTest.DB.Model
{
    public class ItemCollection<T> : ObservableCollection<T> where T : class, ITrackItem, INotifyPropertyChanged
    {
        public delegate void CollectionChangeStateEvent(object sender, bool IsValid,bool IsChanged);
        public event CollectionChangeStateEvent OnCollectionChangeStateEvent;
        public void Susbsribe()
        {
            foreach (var item in this)
                item.PropertyChanged += HandleChangeStateEvent;
        }

        public ItemCollection() : base()
        {
        }
        public ItemCollection(IEnumerable<T> items): base(items)
        {
            foreach (var item in this)
                item.PropertyChanged += HandleChangeStateEvent;
        }
        //public new void Add(T item)
        //{
        //    base.Add(item);
        //    item.PropertyChanged += HandleEvent;
        //}

        //public new void Remove(T item)
        //{
        //    base.Remove(item);
        //    item.PropertyChanged -= HandleEvent;
        //}

        public new void Clear()
        {
            foreach (var item in this)
                item.PropertyChanged -= HandleChangeStateEvent;
            base.Clear();
        }
        private void HandleChangeStateEvent(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsValid"|| e.PropertyName == "IsChanged")
                OnCollectionChangeStateEvent(this, CollectionIsValid, CollectionIsChanged);
        }

        public bool CollectionIsValid => this.All(p => p.IsValid);
        public bool CollectionIsChanged => this.All(p => p.IsChanged);

        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.Cast<T>().ToList().ForEach(x=>x.PropertyChanged += HandleChangeStateEvent);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    e.OldItems.Cast<T>().ToList().ForEach(x => x.PropertyChanged -= HandleChangeStateEvent);
                    break;
                case NotifyCollectionChangedAction.Replace:

                    break;
                case NotifyCollectionChangedAction.Reset:

                    break;
            }
            base.OnCollectionChanged(e);
        }

    }

    
  
}
