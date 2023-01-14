using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ExtraFunctions.Extras;
using SFPCalculator;

namespace SFP_Planner.Data
{
    public class PlanData : INotifyPropertyChanged
    {
        static readonly SFPPlanner SFP = new();
        public bool bFirst = true;

        public static List<string> GetRecipes
        {
            get { return SFPCalculator.Recipes.Data.GetRecipes().Select(x => x.ToString()).ToList(); }
        }
        public static Process? FullPlan { get; private set; }
        public Process? ProductionPlan
        {
            get { return FullPlan; }
            set
            {
                FullPlan = value;
                SelectionEnabled = value == null;
                Resepies = Array.Empty<string>();
                ActivePlan = null;
                Selected = false;
                AllPropChanged();
            }
        }

        public class KeyValue 
        {
            public string Key { get; set; }
            public double Value { get; set; }

            public KeyValue(string key, double value)
            {
                Key = key;
                Value = value;
            }
        }

        public ObservableCollection<KeyValue> SumIn
        {
            get
            {
                ObservableCollection<KeyValue> IN = new();
                if (ProductionPlan == null || !ProductionPlan.Inputs.Any()) return IN;
                Loop(ProductionPlan);
                return IN;
                void Loop(Process Item)
                {
                    foreach (var Child in Item.Children)
                        if (Child.Production) Loop(Child); else Add(Child);
                    if (!Item.Children.Any()) Add(Item);
                }

                void Add(Process Item)
                {
                    if (IN.Select(x => x.Key).Contains(Item.Product.Item.Name))
                        IN.First(x => x.Key == Item.Product.Item.Name).Value +=
                            (double)ExMath.Round((decimal)Item.Product.PerMin, 2);
                    else
                        IN.Add(new(Item.Product.Item.Name, (double)ExMath.Round((decimal)Item.Product.PerMin, 2)));
                }
            }
        }

        public ObservableCollection<KeyValue> SumOut
        {
            get
            {
                ObservableCollection<KeyValue> Out = new();
                if (ProductionPlan == null || !ProductionPlan.Inputs.Any()) return Out;
                Loop(ProductionPlan);
                return Out;
                void Loop(Process Item)
                {
                    foreach (var Child in Item.Children)
                        if(Child.Production) Loop(Child);
                    if (Item.Outputs.Count() >= 2)
                        for (int i = 1; i < Item.Outputs.Count(); i++)
                            if (Out.Select(x => x.Key).Contains(Item.Outputs.ToList()[i].Item.Name))
                                Out.First(x => x.Key == Item.Outputs.ToList()[i].Item.Name).Value +=
                                    (double)ExMath.Round((decimal)Item.Outputs.ToList()[i].PerMin, 2);
                            else
                                Out.Add(new(Item.Outputs.ToList()[i].Item.Name, (double)ExMath.Round((decimal)Item.Outputs.ToList()[i].PerMin, 2)));
                }
            }
        }

        public static int PlanID
        {
            get { return FullPlan == null ? -1 : FullPlan.MainID; }
        }
        public string PlanName
        {
            get { return ProductionPlan == null ? "" : ProductionPlan.Recipe.ToString(); }
            set { ProductionPlan = MakePlan(value, PlanPerMin); AllPropChanged(); }
        }
        public double PlanPerMin
        {
            get { if (ProductionPlan == null) return 0; return ProductionPlan.Product.PerMin; }
            set
            {
                //if (ProductionPlan == null || bFirst) { bFirst = false; return; }
                    
                if (ProductionPlan != null && ProductionPlan.Product.PerMin != value)
                    ProductionPlan = SFP.UpdatePerMin(ProductionPlan, value).Result;
                    //ProductionPlan = MakePlan(ProductionPlan.Recipe.ToString(), value);
            }
        }
        public bool SelectionEnabled { get; private set; } = true;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public static Process MakePlan(string RPName, double PerMin) =>
           SFP.Produce(RPName, PerMin).Result;

        public Process? ActivePlan { get; set; }
        public string[] Resepies { get; set; } = Array.Empty<string>();
        public bool Selected { get; set; }
        public void AllPropChanged()
        {
            OnPropertyChanged(nameof(ProductionPlan));
            OnPropertyChanged(nameof(SelectionEnabled));
            OnPropertyChanged(nameof(PlanID));
            OnPropertyChanged(nameof(PlanName));
            OnPropertyChanged(nameof(PlanPerMin));
            OnPropertyChanged(nameof(ActivePlan));
            OnPropertyChanged(nameof(Resepies));
            OnPropertyChanged(nameof(Selected));
            OnPropertyChanged(nameof(SumIn));
            OnPropertyChanged(nameof(SumOut));
        }
    }
}
