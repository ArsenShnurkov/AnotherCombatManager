﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using AnotherCM.Library.Common;
using BrightIdeasSoftware;
using WeifenLuo.WinFormsUI.Docking;

namespace AnotherCM.WinForms.DockWindows {
    public partial class CombatantListWindow<T> : DockContent where T : Combatant {
        public event EventHandler<CombatantsSelectionChangedEventArgs<T>> SelectionChanged;

        private TypedObjectListView<T> typedView;
        private ObservableKeyedCollection<string, T> combatants;
        private IDisposable textChanged;

        public CombatantListWindow () {
            InitializeComponent();
            this.typedView = new TypedObjectListView<T>(this.objectListView);

            // setup text search throttle
            var textChanged = Observable.FromEventPattern(this.searchTextBox, "TextChanged").Select(x => ((TextBox)x.Sender).Text);
            this.textChanged = textChanged.Throttle(TimeSpan.FromMilliseconds(300))
                                          .DistinctUntilChanged()
                                          .ObserveOn(SynchronizationContext.Current)
                                          .Subscribe(text => {
                                              this.Search(text);
                                          });
        }

        public ObservableKeyedCollection<string, T> Combatants {
            get {
                return this.combatants;
            }
            set {
                this.objectListView.EmptyListMsg = null;
                if (this.combatants != null) {
                    this.combatants.CollectionChanged -= combatants_CollectionChanged;
                }
                this.combatants = value;
                this.objectListView.SetObjects(this.combatants);
                this.combatants.CollectionChanged += combatants_CollectionChanged;
            }
        }

        protected virtual void OnSelectionChanged (CombatantsSelectionChangedEventArgs<T> e) {
            var changed = this.SelectionChanged;
            if (changed != null) {
                changed(this, e); 
            }
        }

        private void CombatantListWindow_FormClosing (object sender, FormClosingEventArgs e) {
            this.textChanged.Dispose();
        }

        private void combatants_CollectionChanged (object sender, NotifyCollectionChangedEventArgs e) {
            this.objectListView.SetObjects(this.combatants);
        }

        private void objectListView_KeyPress (object sender, KeyPressEventArgs e) {

        }

        private void objectListView_SelectionChanged (object sender, EventArgs e) {
            var selected = this.typedView.SelectedObjects;
            if (selected.Any()) {
                this.OnSelectionChanged(new CombatantsSelectionChangedEventArgs<T>(selected));
            }
        }

        private void searchTextBox_Cleared (object sender, EventArgs e) {
            this.objectListView.AdditionalFilter = null;
        }

        private void Search (string text) {
            var filter = TextMatchFilter.Contains(this.objectListView, text);
            this.objectListView.AdditionalFilter = filter;
        }
    }

    public class CombatantsSelectionChangedEventArgs<T> : EventArgs where T : Combatant {
        public CombatantsSelectionChangedEventArgs (IEnumerable<T> combatants) {
            this.Combatants = combatants;
        }

        public IEnumerable<T> Combatants { get; private set; }
    }
}
