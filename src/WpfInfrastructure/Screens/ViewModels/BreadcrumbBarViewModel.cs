/* Copyright 2017 Cimpress

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License. */


using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels
{
    using CaliburnIntegration;

    public class BreadcrumbBarViewModel : PropertyChangedBase
    {
        private void UseTestData()
        {
            var items = new List<BreadcrumbBarItem>();
            items.Add(new BreadcrumbBarItem("Overview123456789123456789123456789") { IconKey = "Home" });
            items.Add(new BreadcrumbBarItem("Heatpress 2") { IconKey = "Heatpress" });
            items.Add(new BreadcrumbBarItem("Details") { IconKey = "Detail" });
            BreadcrumbBarItems = items;
        }

        public BreadcrumbBarViewModel()
        {
            if (DesignTimeHelper.IsInDesignModeStatic)
            {
                UseTestData();
            }
        }

        public BreadcrumbBarViewModel(IModuleScreen homeScreen)
        {
            HomeScreen = homeScreen;
            ToHomeScreen();
        }

        public IModuleScreen HomeScreen { get; private set; }

        private List<BreadcrumbBarItem> _breadcrumbBarArray;

        public List<BreadcrumbBarItem> BreadcrumbBarItems
        {
            get { return _breadcrumbBarArray; }
            set { _breadcrumbBarArray = value; NotifyOfPropertyChange(() => BreadcrumbBarItems); }
        }

        private List<BreadcrumbBarItem> AddBreadcrumbBarItems(IModuleScreen moduleScreen)
        {
            var items = new List<BreadcrumbBarItem>();

            if (HomeScreen != null)
                items.Add(new BreadcrumbBarItem(HomeScreen));

            if (moduleScreen != null && !items.Any(x => x.DisplayName.Contains(moduleScreen.DisplayName)))
                items.Add(new BreadcrumbBarItem(moduleScreen));
            return items;
        }

        public void ToHomeScreen()
        {
            BreadcrumbBarItems = AddBreadcrumbBarItems(null);
        }

        public void ToModuleScreen(IModuleScreen moduleScreen)
        {
            BreadcrumbBarItems = AddBreadcrumbBarItems(moduleScreen);
        }

        public void ToDetailScreen(IModuleScreen moduleScreen, GenericPlcViewModel detailScreen)
        {
            var items = AddBreadcrumbBarItems(moduleScreen);
            items.Add(new BreadcrumbBarItem(detailScreen));
            BreadcrumbBarItems = items;
        }

    }
}
