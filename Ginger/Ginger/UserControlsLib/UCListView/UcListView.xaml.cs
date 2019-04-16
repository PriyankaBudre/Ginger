﻿using Amdocs.Ginger.Common;
using Amdocs.Ginger.Common.Enums;
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Ginger.UserControlsLib.UCListView
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UcListView : UserControl
    {
        IObservableList mObjList;

        public delegate void UcListViewEventHandler(UcListViewEventArgs EventArgs);
        public event UcListViewEventHandler UcListViewEvent;
        private void OnUcListViewEvent(UcListViewEventArgs.eEventType eventType, Object eventObject = null)
        {
            UcListViewEventHandler handler = UcListViewEvent;
            if (handler != null)
            {
                handler(new UcListViewEventArgs(eventType, eventObject));
            }
        }

        public UcListView()
        {
            InitializeComponent();            
        }

        public ListView List
        {
            get
            {
                return xListView;
            }
            set
            {
                xListView = value;
            }
        }

        public IObservableList DataSourceList
        {
            set
            {
                try
                {
                    if (mObjList != null)
                    {
                        mObjList.PropertyChanged -= ObjListPropertyChanged;                        
                    }

                    mObjList = value;                    
                    //mCollectionView = CollectionViewSource.GetDefaultView(mObjList);

                    //if (mCollectionView != null)
                    //{
                    //    try
                    //    {
                    //        CollectFilterData();
                    //        mCollectionView.Filter = FilterGridRows;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        grdMain.CommitEdit();
                    //        grdMain.CancelEdit();
                    //        mCollectionView.Filter = FilterGridRows;
                    //        Reporter.ToLog(eLogLevel.ERROR, $"Method - {MethodBase.GetCurrentMethod().Name}, Error - {ex.Message}", ex);
                    //    }
                    //}
                    this.Dispatcher.Invoke(() =>
                    {
                        xListView.ItemsSource = mObjList;

                        // Make the first row selected
                        if (value != null && value.Count > 0)
                        {                            
                            xListView.SelectedIndex = 0;
                            xListView.SelectedItem = value[0];
                            // Make sure that in case we have only one item it will be the current - otherwise gives err when one record
                            mObjList.CurrentItem = value[0];
                        }
                    });
                }
                catch (Exception ex)
                {
                    Reporter.ToLog(eLogLevel.ERROR, "Failed to set ucListView.DataSourceList", ex);
                }

                if (mObjList != null)
                {
                    mObjList.PropertyChanged += ObjListPropertyChanged;
                    BindingOperations.EnableCollectionSynchronization(mObjList, mObjList);//added to allow collection changes from other threads
                    mObjList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CollectionChangedMethod);
                }
            }

            get
            {
                return mObjList;
            }
        }

        private void ObjListPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            GingerCore.General.DoEvents();
            if (e.PropertyName == nameof(IObservableList.CurrentItem))
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (mObjList.CurrentItem != xListView.SelectedItem)
                    {
                        xListView.SelectedItem = mObjList.CurrentItem;
                        int index = xListView.Items.IndexOf(mObjList.CurrentItem);
                        xListView.SelectedIndex = index;
                    }
                });
            }
        }

        private void CollectionChangedMethod(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                //different kind of changes that may have occurred in collection
                if (e.Action == NotifyCollectionChangedAction.Add ||
                e.Action == NotifyCollectionChangedAction.Replace ||
                e.Action == NotifyCollectionChangedAction.Remove ||
                e.Action == NotifyCollectionChangedAction.Move)
                {
                    OnUcListViewEvent(UcListViewEventArgs.eEventType.UpdateIndex);
                }
            });
        }

        public object CurrentItem
        {
            get
            {
                object o = null;
                this.Dispatcher.Invoke(() =>
                {
                    o = xListView.SelectedItem;
                });
                return o;
            }
        }

        public int CurrentItemIndex
        {
            get
            {
                if (xListView.Items != null && xListView.Items.Count > 0)
                {
                    return xListView.Items.IndexOf(xListView.SelectedItem);
                }
                else
                {
                    return -1;
                }
            }
        }

        public Visibility ExpandCollapseBtnVisiblity
        {
            get
            {
                return xExpandCollapseBtn.Visibility;
            }
            set
            {
                xExpandCollapseBtn.Visibility = value;
            }
        }

        public Visibility ListOperationsBarPnlVisiblity
        {
            get
            {
                return xListOperationsBarPnl.Visibility;
            }
            set
            {
                xListOperationsBarPnl.Visibility = value;
            }
        }

        public Visibility AddBtnVisiblity
        {
            get
            {
                return xAddBtn.Visibility;
            }
            set
            {
                xAddBtn.Visibility = value;
            }
        }

        public Visibility DeleteAllBtnVisiblity
        {
            get
            {
                return xDeleteAllBtn.Visibility;
            }
            set
            {
                xDeleteAllBtn.Visibility = value;
            }
        }

        public Visibility MoveBtnsVisiblity
        {
            get
            {
                return xMoveBtns.Visibility;
            }
            set
            {
                xMoveBtns.Visibility = value;
            }
        }

        public string Title
        {
            get
            {
                if (xListTitleLbl.Content != null)
                {
                    return xListTitleLbl.Content.ToString(); ;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                xListTitleLbl.Content = value;
            }
        }

        public eImageType ListImageType
        {
            get
            {
                return xListTitleImage.ImageType;
            }
            set
            {
                xListTitleImage.ImageType = value;
            }
        }

        public RoutedEventHandler AddItemHandler
        {
            set
            {
                xAddBtn.Click += value;
            }
        }

        private void xDeleteAllBtn_Click(object sender, RoutedEventArgs e)
        {
            if (mObjList.Count == 0)
            {
                Reporter.ToUser(eUserMsgKey.NoItemToDelete);
                return;
            }

            if ((Reporter.ToUser(eUserMsgKey.SureWantToDeleteAll)) == Amdocs.Ginger.Common.eUserMsgSelection.Yes)
            {
                mObjList.SaveUndoData();
                mObjList.ClearAll();
            }
        }

        private void xMoveUpBtn_Click(object sender, RoutedEventArgs e)
        {
            int currentIndx = CurrentItemIndex;
            if (currentIndx >= 1)
            {
                mObjList.Move(currentIndx, currentIndx - 1);
                ScrollToViewCurrentItem();
            }
        }

        private void xMoveDownBtn_Click(object sender, RoutedEventArgs e)
        {
            int currentIndx = CurrentItemIndex;
            if (currentIndx >= 0)
            {
                mObjList.Move(currentIndx, currentIndx + 1);
                ScrollToViewCurrentItem();
            }
        }

        public void ScrollToViewCurrentItem()
        {
            if (mObjList.CurrentItem != null)
            {
                xListView.ScrollIntoView(mObjList.CurrentItem);
            }
        }

        private void xListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (SkipItemSelection)//avoid user item selection in run time 
            //{
            //    SkipItemSelection = false;
            //    return;
            //}

            if (mObjList == null) return;

            if (mObjList.CurrentItem == xListView.SelectedItem) return;

            if (mObjList != null)
            {
                mObjList.CurrentItem = xListView.SelectedItem;
                ScrollToViewCurrentItem();
            }

            e.Handled = true;
        }

        private void XExpandCollapseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (xExpandCollapseBtn.ButtonImageType == eImageType.ExpandAll)
            {
                OnUcListViewEvent(UcListViewEventArgs.eEventType.ExpandAllItems);
                xExpandCollapseBtn.ButtonImageType = eImageType.CollapseAll;
            }
            else
            {
                OnUcListViewEvent(UcListViewEventArgs.eEventType.CollapseAllItems);
                xExpandCollapseBtn.ButtonImageType = eImageType.ExpandAll;
            }
        }

        public void SetDefaultListDataTemplate(object listItemInfo)
        {
            DataTemplate dataTemp = new DataTemplate();
            FrameworkElementFactory listItemFac = new FrameworkElementFactory(typeof(UcListViewItem));
            listItemFac.SetBinding(UcListViewItem.ItemProperty, new Binding());
            listItemFac.SetValue(UcListViewItem.ItemInfoProperty, listItemInfo);
            dataTemp.VisualTree = listItemFac;
            xListView.ItemTemplate = dataTemp;
        }
    }

    public class UcListViewEventArgs
    {
        public enum eEventType
        {
            ExpandAllItems,
            CollapseAllItems,
            UpdateIndex,
        }

        public eEventType EventType;
        public Object EventObject;

        public UcListViewEventArgs(eEventType eventType, object eventObject = null)
        {
            this.EventType = eventType;
            this.EventObject = eventObject;
        }
    }
}