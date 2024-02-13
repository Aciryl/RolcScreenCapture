using CaptureSampleCore;
using Macro;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCaptureSample
{
    internal class MainWindowViewModel
    {
        public class TabItem
        {
            public class Item
            {
                public static string[] ItemsSource { get; } = new string[]
                {
                    "None",
                    "Left1",
                    "Left2",
                    "Right1",
                    "Right2",
                    "Right3",
                };

                public string ItemName { get; }
                public ReactivePropertySlim<string> SelectedItem { get; set; }

                public Item(string itemName, Macro.Macro.RewardAction defaultAction, int id)
                {
                    ItemName = itemName;
                    SelectedItem = new ReactivePropertySlim<string>(defaultAction.ToString());

                    SelectedItem.Subscribe(x =>
                    {
                        var item = x;
                        Macro.Macro.RewardAction action = Macro.Macro.RewardAction.None;
                        if (item.Equals("None"))
                            action = Macro.Macro.RewardAction.None;
                        if (item.Equals("Left1"))
                            action = Macro.Macro.RewardAction.Left1;
                        if (item.Equals("Left2"))
                            action = Macro.Macro.RewardAction.Left2;
                        if (item.Equals("Right1"))
                            action = Macro.Macro.RewardAction.Right1;
                        if (item.Equals("Right2"))
                            action = Macro.Macro.RewardAction.Right2;
                        if (item.Equals("Right3"))
                            action = Macro.Macro.RewardAction.Right3;

                        Macro.Macro.ChangeRewardActions(id, ItemName, action);
                    });
                }
            }

            public ObservableCollection<Item> Items { get; } = new ObservableCollection<Item>();
            public string Name { get; }
            private int Id { get; }
            
            public TabItem(int id)
            {
                Id = id;
                Name = id == 0 ? "Main" : $"Sub{id}";

                foreach (var pair in Macro.Macro.RewardActions[Id].OrderBy(x => x.Key))
                {
                    Items.Add(new Item(pair.Key, pair.Value, Id));
                }
            }

            public void AddItem(string itemName)
            {
                Items.Add(new Item(itemName, Macro.Macro.RewardAction.None, Id));
            }
        }

        
        public ObservableCollection<TabItem> TabItems { get; } = new ObservableCollection<TabItem>();

        public MainWindowViewModel()
        {
            for (int i = 0; i < BasicSampleApplication.ScreenCount; ++i)
            {
                TabItems.Add(new TabItem(i));
            }
        }

        public void AddItem(string itemName, BasicCapture[] captures, (int width, int heigh) vec)
        {
            for (int i = 0; i < BasicSampleApplication.ScreenCount; ++i)
            {
                var tabItem = TabItems[i];
                var screenShot = captures[i].ScreenShot;
                var width = screenShot.ClientWidth;
                var height = screenShot.ClientHeight;
                if (width == vec.width && height == vec.heigh)
                {
                    if (Macro.Macro.ItemCheckPoints.ContainsKey((width, height)) &&
                        Macro.Macro.ItemCheckPoints[(width, height)].ContainsKey(itemName) &&
                        !Macro.Macro.RewardActions[i].ContainsKey(itemName))
                        tabItem.AddItem(itemName);
                }
            }
        }
    }
}
