using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Core.Geoprocessing;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;

namespace FloorPlanAddIn
{
    internal class Dockpane1ViewModel : DockPane
    {
        private const string _dockPaneID = "FloorPlanAddIn_Dockpane1";

        protected Dockpane1ViewModel() { }


        private bool _buildingChb;
        public bool BuildingChb
        {
            get
            {
                if (_buildingChb)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            set
            {
                _buildingChb = value;
                Debug.WriteLine("set (_buildingChb)");
                NotifyPropertyChanged("BuildingChb");
            }
        }

        private bool _floorChb;
        public bool FloorChb
        {
            get
            {
                if (_floorChb)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            set
            {
                _floorChb = value;
                Debug.WriteLine("set (_floorChb)");
                NotifyPropertyChanged("FloorChb");
            }
        }

        private bool _groupIDChb;
        public bool GroupIDChb
        {
            get
            {
                if (_groupIDChb)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            set
            {
                _groupIDChb = value;
                Debug.WriteLine("set (_groupIDChb)");
                NotifyPropertyChanged("GrupIDChb");
            }
        }



        private ObservableCollection<SiteData> _sites = new ObservableCollection<SiteData>();
        public ObservableCollection<SiteData> Sites
        {
            get {
                return _sites; }
            set
            {
                _sites = value;
            }
        }

        public String SelectedSitesWhereClause
        {
            get {
                    if ((Sites.Where(o => o.IsSelected).Select(x => x.Site)).Count() > 1)
                    {
                        return "SITE in ('" + string.Join("','", (Sites.Where(o => o.IsSelected).Select(x => x.Site))) + "')";
                    }
                    if ((Sites.Where(o => o.IsSelected).Select(x => x.Site)).Count() == 1)
                    {
                        return "SITE in ('" + string.Join("", (Sites.Where(o => o.IsSelected).Select(x => x.Site))) + "')";
                    }
                    else
                    {
                        return "SITE IS NOT NULL";
                    }

            }
        }


        public String SelectedBuildingsWhereClause
        {
            get
            {
                if ((Buildings.Where(o => o.IsSelected).Select(x => x.Building)).Count() > 1)
                {
                    return "BUILDING in ('" + string.Join("','", (Buildings.Where(o => o.IsSelected).Select(x => x.Building))) + "')";
                }
                if ((Buildings.Where(o => o.IsSelected).Select(x => x.Building)).Count() == 1)
                {
                    return "BUILDING in ('" + string.Join("", (Buildings.Where(o => o.IsSelected).Select(x => x.Building))) + "')";
                }
                else
                {
                    return "BUILDING IS NOT NULL";
                }
            }
        }

        public String SelectedFloorsWhereClause
        {
            get
            {
                    if ((Floors.Where(o => o.IsSelected).Select(x => x.Floor)).Count() > 1)
                    {
                        return "FLOOR in ('" + string.Join("','", (Floors.Where(o => o.IsSelected).Select(x => x.Floor))) + "')";
                    }
                    if ((Floors.Where(o => o.IsSelected).Select(x => x.Floor)).Count() == 1)
                    {
                        return "FLOOR in ('" + string.Join("", (Floors.Where(o => o.IsSelected).Select(x => x.Floor))) + "')";
                    }
                    else
                    {
                        return "FLOOR IS NOT NULL";
                    }
            }
        }

        public String SelectedGroupIDsWhereClause
        {
            get
            {
                    if ((GroupIDs.Where(o => o.IsSelected).Select(x => x.GroupID)).Count() > 1)
                    {
                        return "GROUP_ID in ('" + string.Join("','", (GroupIDs.Where(o => o.IsSelected).Select(x => x.GroupID))) + "')";
                    }
                    if ((GroupIDs.Where(o => o.IsSelected).Select(x => x.GroupID)).Count() == 1)
                    {
                        return "GROUP_ID in ('" + string.Join("", (GroupIDs.Where(o => o.IsSelected).Select(x => x.GroupID))) + "')";
                    }
                    else
                    {
                        return "GROUP_ID IS NOT NULL";
                    }
            }
        }

        private ObservableCollection<BuildingData> _buildings = new ObservableCollection<BuildingData>();
        public ObservableCollection<BuildingData> Buildings
        {
            get { return _buildings; }
            set
            {
                _buildings = value;
            }
        }
        private ObservableCollection<FloorData> _floors = new ObservableCollection<FloorData>();
        public ObservableCollection<FloorData> Floors
        {
            get { return _floors; }
            set
            {
                _floors = value;
            }
        }
        private ObservableCollection<GroupIDData> _groupIDs = new ObservableCollection<GroupIDData>();
        public ObservableCollection<GroupIDData> GroupIDs
        {
            get { return _groupIDs; }
            set
            {
                _groupIDs = value;
            }
        }

        private readonly object _lockListOfSites = new object();
        private ICommand _cmdRefreshSiteData;
        public ICommand CmdRefreshSiteData => _cmdRefreshSiteData ?? (_cmdRefreshSiteData = new RelayCommand(async () =>
        {
            BindingOperations.EnableCollectionSynchronization(Sites, _lockListOfSites); //needed to avoid multithreading related error because a different thread is used to modify the collection than was used to create the collection.
            await QueuedTask.Run(() =>
            {

                //using (Geodatabase geodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\UMN_BLDG_ROOM.gdb"))))
                using (Geodatabase geodatabase = new Geodatabase(new DatabaseConnectionFile(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\EFLOORPLAN TO TEST.sde"))))
                {
                    QueryDef queryDef = new QueryDef
                    {
                        Tables = "EFLOORPLAN.UMN_BLDG_ROOM",
                        SubFields = "SITE",
                        WhereClause = "",
                        PrefixClause = "DISTINCT",
                        PostfixClause = "ORDER BY SITE"
                    };

                    Sites.Clear();

                    using (RowCursor rowCursor = geodatabase.Evaluate(queryDef, false))
                    {
                        while (rowCursor.MoveNext())
                        {
                            using (Row row = rowCursor.Current)
                            {
                                Feature feature = row as Feature;
                                Sites.Add(new SiteData()
                                {
                                    Site = Convert.ToString(row["SITE"]),
                                    IsSelected = false
                                });
                                Debug.WriteLine("added row");
                            }
                        }
                    }
                }
            });
        }, true //end queuedtask
                 )); // end relaycommand//end return

        private ICommand _cmdSelectAllSites;
        public ICommand CmdSelectAllSites => _cmdSelectAllSites ?? (_cmdSelectAllSites = new RelayCommand(async () =>
        {
            BindingOperations.EnableCollectionSynchronization(Sites, _lockListOfSites); //needed to avoid multithreading related error because a different thread is used to modify the collection than was used to create the collection.
            await QueuedTask.Run(() =>
            {
                //using (Geodatabase geodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\UMN_BLDG_ROOM.gdb"))))
                using (Geodatabase geodatabase = new Geodatabase(new DatabaseConnectionFile(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\EFLOORPLAN TO TEST.sde"))))
                {
                    QueryDef queryDef = new QueryDef
                    {
                        Tables = "EFLOORPLAN.UMN_BLDG_ROOM",
                        SubFields = "SITE",
                        WhereClause = "",
                        PrefixClause = "DISTINCT",
                        PostfixClause = "ORDER BY SITE"
                    };

                    Sites.Clear();

                    using (RowCursor rowCursor = geodatabase.Evaluate(queryDef, false))
                    {
                        while (rowCursor.MoveNext())
                        {
                            using (Row row = rowCursor.Current)
                            {
                                Feature feature = row as Feature;
                                Sites.Add(new SiteData()
                                {
                                    Site = Convert.ToString(row["SITE"]),
                                    IsSelected = true
                                });
                                Debug.WriteLine("added row");
                            }
                        }
                    }
                }
            });
        }, true //end queuedtask
                 )); // end relaycommand//end return


        private readonly object _lockListOfBuildings = new object();
        private ICommand _cmdRefreshBuildingData;
        public ICommand CmdRefreshBuildingData => _cmdRefreshBuildingData ?? (_cmdRefreshBuildingData = new RelayCommand(async () =>
        {
            BindingOperations.EnableCollectionSynchronization(Buildings, _lockListOfBuildings); //needed to avoid multithreading related error because a different thread is used to modify the collection than was used to create the collection.
            await QueuedTask.Run(() =>
            {
            //using (Geodatabase geodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\UMN_BLDG_ROOM.gdb"))))
            using (Geodatabase geodatabase = new Geodatabase(new DatabaseConnectionFile(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\EFLOORPLAN TO TEST.sde"))))
            {
                    QueryDef queryDef = new QueryDef();
                    queryDef.Tables = "EFLOORPLAN.UMN_BLDG_ROOM";
                    queryDef.SubFields = "BUILDING, BLDG_DESC";
                    queryDef.PrefixClause = "DISTINCT";
                    queryDef.PostfixClause = "ORDER BY BUILDING";
                    if (BuildingChb == true) {
                        queryDef.WhereClause = SelectedSitesWhereClause;
                    }
                    else{
                        queryDef.WhereClause = "";
                    }
                    Debug.WriteLine(queryDef.WhereClause);



                    Buildings.Clear();

                    using (RowCursor rowCursor = geodatabase.Evaluate(queryDef, false))
                    {
                        while (rowCursor.MoveNext())
                        {
                            using (Row row = rowCursor.Current)
                            {
                                Feature feature = row as Feature;
                                Buildings.Add(new BuildingData()
                                {
                                    Building = Convert.ToString(row["BUILDING"]),
                                    DisplayText = Convert.ToString(row["BUILDING"]) + " (" + Convert.ToString(row["BLDG_DESC"]) + ")",
                                    IsSelected = false
                                });
                            }
                        }
                    }
                }
            });
        }, true //end queuedtask
                 )); // end relaycommand//end return

        private ICommand _cmdSelectAllBuildings;
        public ICommand CmdSelectAllBuildings => _cmdSelectAllBuildings ?? (_cmdSelectAllBuildings = new RelayCommand(async () =>
        {
            BindingOperations.EnableCollectionSynchronization(Buildings, _lockListOfBuildings); //needed to avoid multithreading related error because a different thread is used to modify the collection than was used to create the collection.
            await QueuedTask.Run(() =>
            {
                //using (Geodatabase geodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\UMN_BLDG_ROOM.gdb"))))
                using (Geodatabase geodatabase = new Geodatabase(new DatabaseConnectionFile(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\EFLOORPLAN TO TEST.sde"))))
                {
                    Debug.WriteLine(SelectedSitesWhereClause);

                    //QueryDef queryDef = new QueryDef
                    //{
                    //    Tables = "EFLOORPLAN.UMN_BLDG_ROOM",
                    //    SubFields = "BUILDING",
                    //    WhereClause = SelectedSitesWhereClause,
                    //    PrefixClause = "DISTINCT",
                    //    PostfixClause = "ORDER BY BUILDING"
                    //};

                    QueryDef queryDef = new QueryDef();
                    queryDef.Tables = "EFLOORPLAN.UMN_BLDG_ROOM";
                    queryDef.SubFields = "BUILDING, BLDG_DESC";
                    queryDef.PrefixClause = "DISTINCT";
                    queryDef.PostfixClause = "ORDER BY BUILDING";
                    if (BuildingChb == true)
                    {
                        queryDef.WhereClause = SelectedSitesWhereClause;
                    }
                    else
                    {
                        queryDef.WhereClause = "";
                    }
                    Debug.WriteLine(queryDef.WhereClause);


                    Buildings.Clear();

                    using (RowCursor rowCursor = geodatabase.Evaluate(queryDef, false))
                    {
                        while (rowCursor.MoveNext())
                        {
                            using (Row row = rowCursor.Current)
                            {
                                Feature feature = row as Feature;
                                Buildings.Add(new BuildingData()
                                {
                                    Building = Convert.ToString(row["BUILDING"]),
                                    DisplayText = Convert.ToString(row["BUILDING"]) + " ("+Convert.ToString(row["BLDG_DESC"]) + ")",
                                    IsSelected = true
                                });
                            }
                        }
                    }
                }
            });
        }, true //end queuedtask
                 )); // end relaycommand//end return


        private readonly object _lockListOfFloors = new object();
        private ICommand _cmdRefreshFloorData;
        public ICommand CmdRefreshFloorData => _cmdRefreshFloorData ?? (_cmdRefreshFloorData = new RelayCommand(async () =>
        {
            BindingOperations.EnableCollectionSynchronization(Floors, _lockListOfFloors); //needed to avoid multithreading related error because a different thread is used to modify the collection than was used to create the collection.
            await QueuedTask.Run(() =>
            {
                //using (Geodatabase geodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\UMN_BLDG_ROOM.gdb"))))
                using (Geodatabase geodatabase = new Geodatabase(new DatabaseConnectionFile(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\EFLOORPLAN TO TEST.sde"))))
                {
                    //QueryDef queryDef = new QueryDef
                    //{
                    //    Tables = "EFLOORPLAN.UMN_BLDG_ROOM",
                    //    SubFields = "FLOOR",
                    //    WhereClause = SelectedBuildingsWhereClause,
                    //    PrefixClause = "DISTINCT",
                    //    PostfixClause = "ORDER BY FLOOR"
                    //};

                    QueryDef queryDef = new QueryDef();
                    queryDef.Tables = "EFLOORPLAN.UMN_BLDG_ROOM";
                    queryDef.SubFields = "FLOOR";
                    queryDef.PrefixClause = "DISTINCT";
                    queryDef.PostfixClause = "ORDER BY FLOOR";
                    if (FloorChb == true)
                    {
                        queryDef.WhereClause = SelectedBuildingsWhereClause + " AND " + SelectedSitesWhereClause;
                    }
                    else
                    {
                        queryDef.WhereClause = "";
                    }
                    Debug.WriteLine(queryDef.WhereClause);

                    Floors.Clear();

                    using (RowCursor rowCursor = geodatabase.Evaluate(queryDef, false))
                    {
                        while (rowCursor.MoveNext())
                        {
                            using (Row row = rowCursor.Current)
                            {
                                Feature feature = row as Feature;
                                Floors.Add(new FloorData()
                                {
                                    Floor = Convert.ToString(row["FLOOR"]),
                                    IsSelected = false
                                });
                            }
                        }
                    }
                }
            });
        }, true //end queuedtask
                 )); // end relaycommand//end return

        private ICommand _cmdSelectAllFloors;
        public ICommand CmdSelectAllFloors => _cmdSelectAllFloors ?? (_cmdSelectAllFloors = new RelayCommand(async () =>
        {
            BindingOperations.EnableCollectionSynchronization(Floors, _lockListOfFloors); //needed to avoid multithreading related error because a different thread is used to modify the collection than was used to create the collection.
            await QueuedTask.Run(() =>
            {
                //using (Geodatabase geodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\UMN_BLDG_ROOM.gdb"))))
                using (Geodatabase geodatabase = new Geodatabase(new DatabaseConnectionFile(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\EFLOORPLAN TO TEST.sde"))))
                {
                    //QueryDef queryDef = new QueryDef
                    //{
                    //    Tables = "EFLOORPLAN.UMN_BLDG_ROOM",
                    //    SubFields = "FLOOR",
                    //    WhereClause = SelectedBuildingsWhereClause,
                    //    PrefixClause = "DISTINCT",
                    //    PostfixClause = "ORDER BY FLOOR"
                    //};
                    QueryDef queryDef = new QueryDef();
                    queryDef.Tables = "EFLOORPLAN.UMN_BLDG_ROOM";
                    queryDef.SubFields = "FLOOR";
                    queryDef.PrefixClause = "DISTINCT";
                    queryDef.PostfixClause = "ORDER BY FLOOR";
                    if (FloorChb == true)
                    {
                        queryDef.WhereClause = SelectedBuildingsWhereClause + " AND " + SelectedSitesWhereClause;
                    }
                    else
                    {
                        queryDef.WhereClause = "";
                    }
                    Debug.WriteLine(queryDef.WhereClause);

                    Floors.Clear();

                    using (RowCursor rowCursor = geodatabase.Evaluate(queryDef, false))
                    {
                        while (rowCursor.MoveNext())
                        {
                            using (Row row = rowCursor.Current)
                            {
                                Feature feature = row as Feature;
                                Floors.Add(new FloorData()
                                {
                                    Floor = Convert.ToString(row["FLOOR"]),
                                    IsSelected = true
                                });
                            }
                        }
                    }
                }
            });
        }, true //end queuedtask
                 )); // end relaycommand//end return

        private readonly object _lockListOfGroupIDs = new object();
        private ICommand _cmdRefreshGroupIDData;
        public ICommand CmdRefreshGroupIDData => _cmdRefreshGroupIDData ?? (_cmdRefreshGroupIDData = new RelayCommand(async () =>
        {
            BindingOperations.EnableCollectionSynchronization(GroupIDs, _lockListOfGroupIDs); //needed to avoid multithreading related error because a different thread is used to modify the collection than was used to create the collection.
            await QueuedTask.Run(() =>
            {
                //using (Geodatabase geodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\UMN_BLDG_ROOM.gdb"))))
                using (Geodatabase geodatabase = new Geodatabase(new DatabaseConnectionFile(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\EFLOORPLAN TO TEST.sde"))))
                {
                    //QueryDef queryDef = new QueryDef
                    //{
                    //    Tables = "EFLOORPLAN.UMN_BLDG_ROOM",
                    //    SubFields = "GROUP_ID",
                    //    WhereClause = SelectedFloorsWhereClause,
                    //    PrefixClause = "DISTINCT",
                    //    PostfixClause = "ORDER BY GROUP_ID"
                    //};
                    QueryDef queryDef = new QueryDef();
                    queryDef.Tables = "EFLOORPLAN.UMN_BLDG_ROOM";
                    queryDef.SubFields = "GROUP_ID, GROUP_DESC";
                    queryDef.PrefixClause = "DISTINCT";
                    queryDef.PostfixClause = "ORDER BY GROUP_ID";
                    if (GroupIDChb == true)
                    {
                        queryDef.WhereClause = SelectedBuildingsWhereClause + " AND " + SelectedSitesWhereClause + " AND " + SelectedFloorsWhereClause;

                    }
                    else
                    {
                        queryDef.WhereClause = "";
                    }
                    Debug.WriteLine(queryDef.WhereClause);

                    GroupIDs.Clear();

                    using (RowCursor rowCursor = geodatabase.Evaluate(queryDef, false))
                    {
                        while (rowCursor.MoveNext())
                        {
                            using (Row row = rowCursor.Current)
                            {
                                Feature feature = row as Feature;
                                GroupIDs.Add(new GroupIDData()
                                {
                                    GroupID = Convert.ToString(row["GROUP_ID"]),
                                    DisplayText = Convert.ToString(row["GROUP_DESC"]),
                                    IsSelected = false
                                });
                            }
                        }
                    }
                }
            });
        }, true //end queuedtask
                 )); // end relaycommand//end return

        private ICommand _cmdSelectAllGroupIDs;
        public ICommand CmdSelectAllGroupIDs => _cmdSelectAllGroupIDs ?? (_cmdSelectAllGroupIDs = new RelayCommand(async () =>
        {
            BindingOperations.EnableCollectionSynchronization(GroupIDs, _lockListOfGroupIDs); //needed to avoid multithreading related error because a different thread is used to modify the collection than was used to create the collection.
            await QueuedTask.Run(() =>
            {
                //using (Geodatabase geodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\UMN_BLDG_ROOM.gdb"))))
                using (Geodatabase geodatabase = new Geodatabase(new DatabaseConnectionFile(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\EFLOORPLAN TO TEST.sde"))))
                {
                    //QueryDef queryDef = new QueryDef
                    //{
                    //    Tables = "EFLOORPLAN.UMN_BLDG_ROOM",
                    //    SubFields = "GROUP_ID",
                    //    WhereClause = SelectedFloorsWhereClause,
                    //    PrefixClause = "DISTINCT",
                    //    PostfixClause = "ORDER BY GROUP_ID"
                    //};
                    QueryDef queryDef = new QueryDef();
                    queryDef.Tables = "EFLOORPLAN.UMN_BLDG_ROOM";
                    queryDef.SubFields = "GROUP_ID, GROUP_DESC";
                    queryDef.PrefixClause = "DISTINCT";
                    queryDef.PostfixClause = "ORDER BY GROUP_ID";
                    if (GroupIDChb == true)
                    {
                        queryDef.WhereClause = SelectedBuildingsWhereClause + " AND " + SelectedSitesWhereClause + " AND " + SelectedFloorsWhereClause;
                    }
                    else
                    {
                        queryDef.WhereClause = "";
                    }
                    Debug.WriteLine(queryDef.WhereClause);



                    GroupIDs.Clear();

                    using (RowCursor rowCursor = geodatabase.Evaluate(queryDef, false))
                    {
                        while (rowCursor.MoveNext())
                        {
                            using (Row row = rowCursor.Current)
                            {
                                Feature feature = row as Feature;
                                GroupIDs.Add(new GroupIDData()
                                {
                                    GroupID = Convert.ToString(row["GROUP_ID"]),
                                    DisplayText = Convert.ToString(row["GROUP_DESC"]),
                                    IsSelected = true
                                });
                            }
                        }
                    }
                }
            });
        }, true //end queuedtask
                 )); // end relaycommand//end return


        List<string> uniqueSiteBuildingFloors = new List<string>(); 
        private ICommand _cmdValidate;
        public ICommand CmdValidate => _cmdValidate ?? (_cmdValidate = new RelayCommand(async () =>
        {
            await QueuedTask.Run(() =>
            {
                //using (Geodatabase geodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\UMN_BLDG_ROOM.gdb"))))
                using (Geodatabase geodatabase = new Geodatabase(new DatabaseConnectionFile(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\EFLOORPLAN TO TEST.sde"))))
                {

                    string combinedWhereClause = SelectedSitesWhereClause + " AND " + SelectedBuildingsWhereClause + " AND " + SelectedFloorsWhereClause + " AND " + SelectedGroupIDsWhereClause;
                    Debug.WriteLine("combinedWhereClause:");

                    Debug.WriteLine(combinedWhereClause);

                    QueryDef queryDef = new QueryDef
                    {
                        Tables = "EFLOORPLAN.UMN_BLDG_ROOM",
                        SubFields = "SITE, BUILDING, FLOOR",
                        WhereClause = combinedWhereClause,
                        PrefixClause = "DISTINCT",
                        PostfixClause = "ORDER BY SITE, BUILDING, FLOOR"
                    };
                    using (RowCursor rowCursor = geodatabase.Evaluate(queryDef, false))
                    {
                        uniqueSiteBuildingFloors.Clear();
                        while (rowCursor.MoveNext())
                        {
                            using (Row row = rowCursor.Current)
                            {
                                Feature feature = row as Feature;
                                var site = Convert.ToString(row["SITE"]);
                                var building = Convert.ToString(row["BUILDING"]);
                                var floor = Convert.ToString(row["FLOOR"]);
                                uniqueSiteBuildingFloors.Add(site + building + floor);
                            }
                        }
                    }
                    Debug.WriteLine(String.Join(", ", uniqueSiteBuildingFloors.ToArray()));
                }

            });

            //MessageBox.Show("Using your currently selected filter options, the query will return " + uniqueSiteBuildingFloors.Count().ToString() + " unique floors. The Create Layout button will create a page layout for each floor." + Environment.NewLine + Environment.NewLine + "Click OK to close this dialog. Then either modify your filter selections and re-validate the query or proceed by clicking the Create Layout button.", "Query Validation Results");

            string messagetitle = "Confirm Query Validation Results";
            string messagetext = "Based on the selected filter options ArcGIS Pro will create " + uniqueSiteBuildingFloors.Count().ToString() + " floor plan maps and layouts." + Environment.NewLine + Environment.NewLine + "Click Ok to proceed.";

            var messageresult = ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(messagetext, messagetitle, MessageBoxButton.OKCancel, MessageBoxImage.Information);

            Debug.WriteLine(messageresult);

            if (uniqueSiteBuildingFloors.Count() > 0) { 
            if (messageresult.ToString() == "OK")
            {
                CmdCreateMapsLiveData.Execute(true);
            }
            }

        }, true));

        private ICommand _cmdCreateMapsLiveData;
        public ICommand CmdCreateMapsLiveData => _cmdCreateMapsLiveData ?? (_cmdCreateMapsLiveData = new RelayCommand(async () =>
        {
            Debug.WriteLine("CmdCreateMapsLiveData");

            //await QueuedTask.Run(async () =>
            //{
            //using (Geodatabase geodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri("C:\\Users\\fimpe\\Documents\\ArcGIS\\Projects\\MyProject58\\UMN_BLDG_ROOM.gdb"))))
            //using (Geodatabase geodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\UMN_BLDG_ROOM.gdb"))))
            using (Geodatabase geodatabase = new Geodatabase(new DatabaseConnectionFile(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\EFLOORPLAN TO TEST.sde"))))
            {

                //for each site-building-floor combo floor open a new map and add the layers
                foreach (var item in uniqueSiteBuildingFloors)
                {
                    //create new layout
                    Layout layout = await QueuedTask.Run(() =>
                   {
                            //*** CREATE A NEW LAYOUT ***

                            //Set up a page
                            CIMPage newPage = new CIMPage();
                            //required properties
                            newPage.Width = 17;
                       newPage.Height = 11;
                       newPage.Units = LinearUnit.Inches;


                            //optional rulers
                            newPage.ShowRulers = true;
                       newPage.SmallestRulerDivision = 0.5;

                       layout = LayoutFactory.Instance.CreateLayout(newPage);
                       layout.SetName("FL" + item);

                            //*** INSERT MAP FRAME ***

                            // create a new map
                            Map map = MapFactory.Instance.CreateMap("FL" + item, MapType.Map, MapViewingMode.Map, Basemap.None);

                            //Build map frame geometry
                            Coordinate2D ll = new Coordinate2D(4.0, 0.5);
                       Coordinate2D ur = new Coordinate2D(16.5, 10.5);
                       Envelope env = EnvelopeBuilder.CreateEnvelope(ll, ur);
                       Debug.WriteLine("env");

                            //Create map frame and add to layout
                            MapFrame mfElm = LayoutElementFactory.Instance.CreateMapFrame(layout, env, map);
                       Debug.WriteLine("0");

                       mfElm.SetName("FL" + item);

                            //var title = @"<dyn type = ""page"" property = ""name"" />";
                            var title = item;
                            //Debug.WriteLine(item.Length);
                            //var title = "Campus: " + item.Substring(0, 2) + "& vbnewline &" + "Building: <br>" + item.Substring(3, 4) + "& vbnewline &" + "Floor: "; //+ item.Substring(5, item.Length-1);
                            //Coordinate2D llTitle = new Coordinate2D(1, 9.5);
                            //Note: call within QueuedTask.Run()
                            //var titleGraphics = LayoutElementFactory.Instance.CreatePointTextGraphicElement(layout, llTitle, null) as ArcGIS.Desktop.Layouts.TextElement;
                            //titleGraphics.SetName("MapTitle");
                            //titleGraphics.SetTextProperties(new TextProperties(title, "Arial", 36, "Bold"));
                            Debug.WriteLine("1");
                       Coordinate2D llcampus = new Coordinate2D(0.5, 10.03);
                       var campusGraphics = LayoutElementFactory.Instance.CreatePointTextGraphicElement(layout, llcampus, null) as ArcGIS.Desktop.Layouts.TextElement;
                       campusGraphics.SetName("MapCampus");
                       string c = "Campus: " + title.Substring(0, 2);
                       campusGraphics.SetTextProperties(new TextProperties(c, "Arial", 30, "Bold"));
                       Debug.WriteLine("2");

                       Coordinate2D llbuilding = new Coordinate2D(0.5, 9.47);
                       var buildingGraphics = LayoutElementFactory.Instance.CreatePointTextGraphicElement(layout, llbuilding, null) as ArcGIS.Desktop.Layouts.TextElement;
                       buildingGraphics.SetName("MapBuilding");
                       string b = "Building: " + title.Substring(2, 3);
                       buildingGraphics.SetTextProperties(new TextProperties(b, "Arial", 30, "Bold"));
                       Debug.WriteLine("3");

                       Coordinate2D llfloor = new Coordinate2D(0.5, 8.9);
                       var floorGraphics = LayoutElementFactory.Instance.CreatePointTextGraphicElement(layout, llfloor, null) as ArcGIS.Desktop.Layouts.TextElement;
                            //int l = (item.Length) - 4;
                            string fl = title.Substring(5); //title.Remove(4, title.Length - (4+1) );
                            floorGraphics.SetName("MapFloor");
                       string f = "Floor: " + fl;
                       floorGraphics.SetTextProperties(new TextProperties(f, "Arial", 30, "Bold"));
                       Debug.WriteLine("4");


                       string date = "Created date: " + System.DateTime.Today.ToString("dd/MM/yy");
                       Coordinate2D llDate = new Coordinate2D(0.5, 8.52);
                       var dateGraphics = LayoutElementFactory.Instance.CreatePointTextGraphicElement(layout, llDate, null) as ArcGIS.Desktop.Layouts.TextElement;
                       dateGraphics.SetName("MapDate");
                       dateGraphics.SetTextProperties(new TextProperties(date, "Arial", 12, "Bold"));

                       string user = "Created by: " + Environment.UserName;
                       Coordinate2D llUser = new Coordinate2D(0.5, 8.22);
                       var userGraphics = LayoutElementFactory.Instance.CreatePointTextGraphicElement(layout, llUser, null) as ArcGIS.Desktop.Layouts.TextElement;
                       userGraphics.SetName("MapUser");
                       userGraphics.SetTextProperties(new TextProperties(user, "Arial", 12, "Bold"));


                       return layout;
                   });

                    //*** OPEN LAYOUT VIEW (must be in the GUI thread) ***
                    MapProjectItem mapPrjItem = Project.Current.GetItems<MapProjectItem>().FirstOrDefault(i => i.Name.Equals("FL" + item));
                    Map map2 = await QueuedTask.Run<Map>(() => { return mapPrjItem.GetMap(); });
                    //FeatureLayer rooms = await QueuedTask.Run<FeatureLayer>(() => { return (FeatureLayer)LayerFactory.Instance.CreateLayer(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\UMN_BLDG_ROOM.gdb\\" + "FL" + item + "_ROOMS"), map2); });
                    //FeatureLayer rooms = await QueuedTask.Run<FeatureLayer>(() => { return (FeatureLayer)LayerFactory.Instance.CreateLayer(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\EFLOORPLAN TO TEST.sde\\EFLOORPLAN." + "FL" + item + "_ROOMS"), map2); });
                    //FeatureLayer details = await QueuedTask.Run<FeatureLayer>(() => { return (FeatureLayer)LayerFactory.Instance.CreateLayer(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\UMN_BLDG_ROOM.gdb\\" + "FL" + item + "_DETAILS"), map2); });
                    FeatureLayer details = await QueuedTask.Run<FeatureLayer>(() => { return (FeatureLayer)LayerFactory.Instance.CreateLayer(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\EFLOORPLAN TO TEST.sde\\EFLOORPLAN." + "FL" + item + "_DETAILS"), map2); });


                    await QueuedTask.Run(async() =>
                    {
                        MapProjectItem mapPrjItem3 = Project.Current.GetItems<MapProjectItem>().FirstOrDefault(i => i.Name.Equals("FL" + item));
                        Map map3 = await QueuedTask.Run<Map>(() => { return mapPrjItem.GetMap(); });

                        using (Geodatabase g = new Geodatabase(new DatabaseConnectionFile(new Uri("C:\\ArcGIS Pro Floor Plan Tools Add-In\\EFLOORPLAN TO TEST.sde"))))
                        {

                            //Geodatabase g = new Geodatabase(new DatabaseConnectionFile(new Uri(@"C:\\ArcGIS Pro Floor Plan Tools Add-In\\EFLOORPLAN TO TEST.sde")));
                            var rooms_fc_name = "EFLOORPLAN." + "FL" + item + "_ROOMS";
                            var site_string = item.Substring(0, 2);
                            Debug.WriteLine(site_string);

                            var bldg_string = item.Substring(2, 3);
                            Debug.WriteLine(bldg_string);

                            var floor_string = item.Substring(5);
                            Debug.WriteLine(floor_string);

                            //var q = "Select b.objectid, a.room_id, a.shape, b.bldg_desc, b.group_desc, c.group_desc_count, concat(a.room_id, concat(' - ', b.group_desc)) as room_id_dash_group_desc from EFLOORPLAN.UMN_BLDG_ROOM b, " + rooms_fc_name + " a left outer join (SELECT room_id, count(1) as group_desc_count FROM (select room_id, group_desc from EFLOORPLAN.UMN_BLDG_ROOM where site = '" + site_string + "' and building = '" + bldg_string + "' and floor = '" + floor_string + "'  GROUP BY room_id, group_desc ORDER BY room_id) group by room_id) c on (c.room_id = a.room_id) where (a.room_id = b.room_id)";

                            //var q = "Select objectid, room_id, shape, bldg_desc, group_desc, group_desc_count, GROUP_DESC_OR_MULITPLE, sumgroupsqft, case when GROUP_DESC_OR_MULITPLE = 'Multiple' then 'Multiple' else (concat(GROUP_DESC_OR_MULITPLE, concat( ' - ', concat(to_char(sumgroupsqft),'sf')))) end as group_descritpion from (SELECT b.objectid,   a.room_id,   a.shape,  b.bldg_desc,  b.group_desc,  c.group_desc_count,  CASE when c.group_desc_count > 1 Then 'Multiple'   Else to_char(b.group_desc) end as GROUP_DESC_OR_MULITPLE, d.sumgroupsqft FROM   efloorplan.umn_bldg_room b, (select group_desc,sum(sqr_feet) as sumgroupsqft FROM   efloorplan.umn_bldg_room WHERE  site = '" + site_string + "' AND building = '" + bldg_string + "' AND floor = '" + floor_string + "' GROUP  BY group_desc) d, " + rooms_fc_name + " a LEFT OUTER JOIN (SELECT room_id, Count(1) AS group_desc_count FROM   (SELECT room_id, group_desc FROM   efloorplan.umn_bldg_room WHERE  site = '" + site_string + "' AND building = '" + bldg_string + "' AND floor = '" + floor_string + "' GROUP  BY room_id, group_desc ORDER  BY room_id) GROUP  BY room_id ORDER  BY room_id) c ON ( c.room_id = a.room_id ) WHERE  ( a.room_id = b.room_id  and d.group_desc = b.group_desc) )";
                            //var q = "SELECT objectid, room_id, shape, room, bldg_desc, sqr_feet, group_desc, group_desc_count, CASE WHEN GROUP_DESC_OR_MULITPLE = 'Multiple' THEN 'Multiple' ELSE(concat(GROUP_DESC_OR_MULITPLE, concat( ' - ', concat(TO_CHAR(sumgroupsqft),'sf')))) END AS group_description, function_desc, function_desc_count, CASE WHEN function_desc_OR_MULITPLE = 'Multiple' THEN 'Multiple' ELSE (concat(function_desc_OR_MULITPLE, concat( ' - ', concat(TO_CHAR(sumfunctionsqft),'sf')))) END AS function_description, use_code_desc, use_code_desc_count, CASE WHEN use_code_desc_OR_MULITPLE = 'Multiple' THEN 'Multiple' ELSE (concat(use_code_desc_OR_MULITPLE, concat( ' - ', concat(TO_CHAR(sumusecodesqft),'sf')))) END AS use_code_description FROM ( SELECT b.objectid, a.room_id, a.shape, b.room, b.bldg_desc, b.sqr_feet, b.group_desc, c.group_desc_count, CASE WHEN c.group_desc_count > 1 THEN 'Multiple' ELSE TO_CHAR(b.group_desc) END AS GROUP_DESC_OR_MULITPLE, d.sumgroupsqft, b.function_desc, c2.function_desc_count, CASE WHEN c2.function_desc_count > 1 THEN 'Multiple' ELSE TO_CHAR(b.function_desc) END AS function_desc_OR_MULITPLE, d2.sumfunctionsqft, b.use_code_desc, c3.use_code_desc_count, CASE WHEN c3.use_code_desc_count > 1 THEN 'Multiple' ELSE TO_CHAR(b.use_code_desc) END AS use_code_desc_OR_MULITPLE, d3.sumusecodesqft FROM " + rooms_fc_name + " a, efloorplan.umn_bldg_room b left outer join ( SELECT group_desc, SUM(sqr_feet) AS sumgroupsqft FROM efloorplan.umn_bldg_room WHERE site = '" + site_string + "' AND building = '" + bldg_string + "' AND floor = '" + floor_string + "' GROUP BY group_desc) d on (d.group_desc = b.group_desc ) left outer join ( SELECT function_desc, SUM(sqr_feet) AS sumfunctionsqft FROM efloorplan.umn_bldg_room WHERE site = '" + site_string + "' AND building = '" + bldg_string + "' AND floor = '" + floor_string + "' GROUP BY function_desc ) d2 on (d2.function_desc = b.function_desc ) left outer join ( SELECT use_code_desc, SUM(sqr_feet) AS sumusecodesqft FROM efloorplan.umn_bldg_room WHERE site = '" + site_string + "' AND building = '" + bldg_string + "' AND floor = '" + floor_string + "' GROUP BY use_code_desc ) d3 on (d3.use_code_desc = b.use_code_desc ) LEFT OUTER JOIN (SELECT room_id, COUNT(1) AS group_desc_count FROM (SELECT room_id, group_desc FROM efloorplan.umn_bldg_room WHERE site = '" + site_string + "' AND building = '" + bldg_string + "' AND floor = '" + floor_string + "' GROUP BY room_id, group_desc ORDER BY room_id ) GROUP BY room_id ORDER BY room_id ) c ON ( c.room_id = b.room_id) LEFT OUTER JOIN (SELECT room_id, COUNT(1) AS function_desc_count FROM (SELECT room_id, function_desc FROM efloorplan.umn_bldg_room WHERE site = '" + site_string + "' AND building = '" + bldg_string + "' AND floor = '" + floor_string + "' GROUP BY room_id, function_desc ORDER BY room_id ) GROUP BY room_id ORDER BY room_id ) c2 ON (c2.room_id = b.room_id) LEFT OUTER JOIN (SELECT room_id, COUNT(1) AS use_code_desc_count FROM (SELECT room_id, use_code_desc FROM efloorplan.umn_bldg_room WHERE site = '" + site_string + "' AND building = '" + bldg_string + "' AND floor = '" + floor_string + "' GROUP BY room_id, use_code_desc ORDER BY room_id ) GROUP BY room_id ORDER BY room_id ) c3 ON (c3.room_id = b.room_id) where (a.room_id = b.room_id) ) ";
                            var q = "SELECT objectid, room_id, shape, room, bldg_desc, sqr_feet, group_desc, group_desc_count, CASE WHEN GROUP_DESC_OR_MULITPLE = 'Multiple' THEN 'Multiple' ELSE(concat(GROUP_DESC_OR_MULITPLE, concat( ' - ', concat(TO_CHAR(sumgroupsqft),'sf')))) END AS group_description, function_desc, function_desc_count, CASE WHEN function_desc_OR_MULITPLE = 'Multiple' THEN 'Multiple' ELSE (concat(function_desc_OR_MULITPLE, concat( ' - ', concat(TO_CHAR(sumfunctionsqft),'sf')))) END AS function_description, use_code_desc, use_code_desc_count, CASE WHEN use_code_desc_OR_MULITPLE = 'Multiple' THEN 'Multiple' ELSE (concat(use_code_desc_OR_MULITPLE, concat( ' - ', concat(TO_CHAR(sumusecodesqft),'sf')))) END AS use_code_description FROM ( SELECT b.objectid, a.room_id, a.shape, b.room, b.bldg_desc, b.sqr_feet, b.group_desc, c.group_desc_count, CASE WHEN c.group_desc_count > 1 THEN 'Multiple' ELSE TO_CHAR(b.group_desc) END AS GROUP_DESC_OR_MULITPLE, d.sumgroupsqft, b.function_desc, c2.function_desc_count, CASE WHEN c2.function_desc_count > 1 THEN 'Multiple' ELSE TO_CHAR(b.function_desc) END AS function_desc_OR_MULITPLE, d2.sumfunctionsqft, b.use_code_desc, c3.use_code_desc_count, CASE WHEN c3.use_code_desc_count > 1 THEN 'Multiple' ELSE TO_CHAR(b.use_code_desc) END AS use_code_desc_OR_MULITPLE, d3.sumusecodesqft FROM " + rooms_fc_name + " a, efloorplan.umn_bldg_room b left outer join ( SELECT group_desc, SUM(sqr_feet) AS sumgroupsqft FROM efloorplan.umn_bldg_room WHERE site = '" + site_string + "' AND building = '" + bldg_string + "' AND floor = '" + floor_string + "' GROUP BY group_desc) d on (d.group_desc = b.group_desc ) left outer join ( SELECT function_desc, SUM(sqr_feet) AS sumfunctionsqft FROM efloorplan.umn_bldg_room WHERE site = '" + site_string + "' AND building = '" + bldg_string + "' AND floor = '" + floor_string + "' GROUP BY function_desc ) d2 on (d2.function_desc = b.function_desc ) left outer join ( SELECT use_code_desc, SUM(sqr_feet) AS sumusecodesqft FROM efloorplan.umn_bldg_room WHERE site = '" + site_string + "' AND building = '" + bldg_string + "' AND floor = '" + floor_string + "' GROUP BY use_code_desc ) d3 on (d3.use_code_desc = b.use_code_desc ) LEFT OUTER JOIN (SELECT room_id, COUNT(1) AS group_desc_count FROM (SELECT room_id, group_desc FROM efloorplan.umn_bldg_room WHERE site = '" + site_string + "' AND building = '" + bldg_string + "' AND floor = '" + floor_string + "' GROUP BY room_id, group_desc ORDER BY room_id ) GROUP BY room_id ORDER BY room_id ) c ON ( c.room_id = b.room_id) LEFT OUTER JOIN (SELECT room_id, COUNT(1) AS function_desc_count FROM (SELECT room_id, function_desc FROM efloorplan.umn_bldg_room WHERE site = '" + site_string + "' AND building = '" + bldg_string + "' AND floor = '" + floor_string + "' GROUP BY room_id, function_desc ORDER BY room_id ) GROUP BY room_id ORDER BY room_id ) c2 ON (c2.room_id = b.room_id) LEFT OUTER JOIN (SELECT room_id, COUNT(1) AS use_code_desc_count FROM (SELECT room_id, use_code_desc FROM efloorplan.umn_bldg_room WHERE site = '" + site_string + "' AND building = '" + bldg_string + "' AND floor = '" + floor_string + "' GROUP BY room_id, use_code_desc ORDER BY room_id ) GROUP BY room_id ORDER BY room_id ) c3 ON (c3.room_id = b.room_id) where (a.room_id = b.room_id) ) ";

                            Debug.WriteLine(q);

                            CIMSqlQueryDataConnection sqldc = new CIMSqlQueryDataConnection()
                            {
                                WorkspaceConnectionString = g.GetConnectionString(),
                                GeometryType = esriGeometryType.esriGeometryPolygon,
                                OIDFields = "[OBJECTID]",

                                //this seems to be needed for unknown sr
                                SpatialReference = null,
                                Srid = "9",

                                //SqlQuery = "Select OBJECTID, SHAPE from " + rooms_fc_name,
                                //SqlQuery = "Select b.objectid, a.room_id, a.shape,b.bldg_desc,b.group_desc,concat(a.room_id, concat(' - ', b.group_desc)) as room_id_dash_group_desc from EFLOORPLAN.UMN_BLDG_ROOM b, " + rooms_fc_name + " a where(a.room_id = b.room_id)",
                                SqlQuery = q,
                                Dataset = rooms_fc_name,
                            };
                            Debug.WriteLine(sqldc.ToXml().ToString());
                            Debug.WriteLine(sqldc.ToString());

                            FeatureLayer rooms = (FeatureLayer)LayerFactory.Instance.CreateFeatureLayer(sqldc, map3, ArcGIS.Desktop.Mapping.LayerPosition.AddToBottom);
                        }


                    });


                    //Legend legend =  await QueuedTask.Run(() =>{Coordinate2D leg_ll = new Coordinate2D(1, 1); Coordinate2D leg_ur = new Coordinate2D(3, 7); Envelope leg_env = EnvelopeBuilder.CreateEnvelope(leg_ll, leg_ur); MapFrame mf = layout.FindElement("FL" + item) as MapFrame; Legend legendElm = LayoutElementFactory.Instance.CreateLegend(layout, leg_env, mf); legendElm.SetName("New Legend"); return legendElm;});
                    Legend legend = await QueuedTask.Run(() => { Coordinate2D leg_ll = new Coordinate2D(0.5, 0.5); Coordinate2D leg_ur = new Coordinate2D(3.8, 8); Envelope leg_env = EnvelopeBuilder.CreateEnvelope(leg_ll, leg_ur); MapFrame mf = layout.FindElement("FL" + item) as MapFrame; Legend legendElm = LayoutElementFactory.Instance.CreateLegend(layout, leg_env, mf); legendElm.SetName("New Legend"); return legendElm; });
                    await QueuedTask.Run(() =>
                    {

                        CIMLegend cimlegend = legend.GetDefinition() as CIMLegend;
                        foreach (var legendItem in cimlegend.Items)
                        {
                            if (legendItem.Name.EndsWith("DETAILS")){
                                legendItem.IsVisible = false;
                            }

                        }
                        legend.SetDefinition(cimlegend);
                    });

                    var layoutPane = await ProApp.Panes.CreateLayoutPaneAsync(layout);
                    var sel = layoutPane.LayoutView.GetSelectedElements();
                    if (sel.Count > 0)
                    {
                        layoutPane.LayoutView.ClearElementSelection();
                    }
                }
            }

            }));


        /// <summary>
        /// Show the DockPane.
        /// </summary>
        internal static void Show()
        {
            DockPane pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
            if (pane == null)
                return;

            pane.Activate();
        }
    }

    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class Dockpane1_ShowButton : Button
    {
        protected override void OnClick()
        {
            Dockpane1ViewModel.Show();
        }
    }

    internal class SiteData
    {
        public string Site { get; set; }
        public bool IsSelected { get; set; }

    }

    //here we coudl add a displaytext etc.
    internal class BuildingData
    {
        public string Building { get; set; }
        public string DisplayText { get; set; }
        public bool IsSelected { get; set; }
    }
    internal class FloorData
    {
        public string Floor { get; set; }
        public bool IsSelected { get; set; }
    }
    internal class GroupIDData
    {
        public string GroupID { get; set; }
        public string DisplayText { get; set; }
        public bool IsSelected { get; set; }
    }


}
