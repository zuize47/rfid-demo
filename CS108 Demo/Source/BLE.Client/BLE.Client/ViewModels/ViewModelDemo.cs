using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using BLE.Client.Data;
using MvvmCross.Core.ViewModels;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Xamarin.Forms;

namespace BLE.Client.ViewModels
{
    public class ViewModelDemo : BaseViewModel
    {
        public static int TabIndex { get; set; } = 0;
        public static string EPCCopy { get; set; } = "";

        private IUserDialogs _userDialogs;
        private IApiClient _apiClient;

        public string Title1 { get; private set; } = "Issue Product";
        public string Title2 { get; private set; } = "Checking Stock";

        public ICommand OnStartInventoryButtonCommand { protected set; get; }
        public ICommand OnClearButtonCommand { protected set; get; }
        public ICommand OnSaveButtonCommand { protected set; get; }
        public ICommand OnRefreshButtonCommand { protected set; get; }
        public ICommand OnResetButtonCommand { protected set; get; }
        public ICommand OnCopyButtonCommand { protected set; get; }

        //public IList<KhoResultDTO> fromServer = new List<KhoResultDTO>();

        public IList<KhoResultDTO> fromServer { get; set; } = new List<KhoResultDTO>();

        public ObservableCollection<KhoResultDTO> CheckList { get; set; } = new ObservableCollection<KhoResultDTO>();



        public ViewModelDemo(IAdapter adapter, IUserDialogs userDialogs, IApiClient apiClient) : base(adapter)
        {
            _userDialogs = userDialogs;
            _apiClient = apiClient;

            OnStartInventoryButtonCommand = new Command(StartInventoryClick);
            OnClearButtonCommand = new Command(ClearClick);
            OnSaveButtonCommand = new Command(SaveClick);
            OnRefreshButtonCommand = new Command(RefreshClick);
            OnResetButtonCommand = new Command(ResetClick);
            OnCopyButtonCommand = new Command(CopyClick);

            GetDataFromServer();
            updateCheckList();
            InventorySetting();

            Staffs.Add(new Starff()
            {
                Name = "Nam",
                Code = "Nam"
            });
            Staffs.Add(new Starff()
            {
                Name = "Quốc",
                Code = "Quốc"
            });
            Staffs.Add(new Starff()
            {
                Name = "Lan",
                Code = "Lan"
            });
            Staffs.Add(new Starff()
            {
                Name = "Ngọc",
                Code = "Ngọc"
            });

            Customers.Add(new Customer()
            {
                Name = "A.Khoa",
                Code = "A.Khoa"
            });

            Customers.Add(new Customer()
            {
                Name = "C.Cúc",
                Code = "C.Cúc"
            });

            Customers.Add(new Customer()
            {
                Name = "Thương",
                Code = "Thương"
            });



        }

        private void ResetClick(object obj)
        {
            _apiClient.ResetKiem();
            GetDataFromServer();
            updateCheckList();
        }

        private void CopyClick(object obj)
        {
            try { EPCCopy = YourSelectedItem.RfId; }
            catch { }

        }

        private void RefreshClick(object obj)
        {
            GetDataFromServer();
            updateCheckList();
        }

        private void updateCheckList()
        {
            this.CheckList.Clear();
            int iTotal = 0;
            int iQuan = 0;
            int iCheckedQua = 0;
            foreach (var o in this.fromServer)
            {
                this.CheckList.Add(o);
               if (o.KiemKe)
                    iCheckedQua++;
                iTotal += o.GiaBan;
                iQuan++;
            }
            _TotalAmountCheck = iTotal.ToString("#,###");
            _TotalQuantityCheck = iQuan.ToString("#,###");
            _CheckedTotalQuantity = iCheckedQua.ToString("#,###");
            RaisePropertyChanged(() => TotalAmountCheck);
            RaisePropertyChanged(() => TotalQuantityCheck);
            RaisePropertyChanged(() => CheckedTotalQuantity);
        }

        public KhoResultDTO YourSelectedItem { get; set; }

        ~ViewModelDemo()
        {
            BleMvxApplication._reader.barcode.Stop();
        }


        private void GetDataFromServer()
        {
            InvokeOnMainThread(() => {
                this.fromServer.Clear();
                foreach (var o in _apiClient.GetKhoResults().OrderBy(p => p.Name).ToList())
                {
                    this.fromServer.Add(o);
                }
            });

        }

        #region Issue Product


        private void SetEvent(bool enable)
        {
            // Cancel RFID event handler
            BleMvxApplication._reader.rfid.ClearEventHandler();

            // Cancel Barcode event handler
            BleMvxApplication._reader.barcode.ClearEventHandler();

            // Key Button event handler
            BleMvxApplication._reader.notification.ClearEventHandler();

            if (enable)
            {
                // RFID event handler
                //BleMvxApplication._reader.rfid.OnAsyncCallback += new EventHandler<CSLibrary.Events.OnAsyncCallbackEventArgs>(TagInventoryEvent);
                //BleMvxApplication._reader.rfid.OnStateChanged += new EventHandler<CSLibrary.Events.OnStateChangedEventArgs>(StateChangedEvent);


                // Key Button event handler
                BleMvxApplication._reader.notification.OnKeyEvent += new EventHandler<CSLibrary.Notification.HotKeyEventArgs>(HotKeys_OnKeyEvent);
                //BleMvxApplication._reader.notification.OnVoltageEvent += new EventHandler<CSLibrary.Notification.VoltageEventArgs>(VoltageEvent);
            }
        }

        //private DateTime _dateSale = DateTime.Now;
        public DateTime SelectedDate { get; set; } = DateTime.Now;

        //public string TotalAmount { get; set; } = "0";s
        private string _TotalAmountt = "0";
        public string TotalAmount { get { return _TotalAmountt; } }
        private string _TotalQuantity = "0";
        public string TotalQuantity { get { return _TotalQuantity; } }

        private string _TotalAmountCheck = "0";
        public string TotalAmountCheck { get { return _TotalAmountCheck; } }

        private string _TotalQuantityCheck = "0";
        public string TotalQuantityCheck { get { return _TotalQuantityCheck; } }

        private string _CheckedTotalQuantity = "0";
        public string CheckedTotalQuantity { get { return _CheckedTotalQuantity; } }


        public ObservableCollection<KhoResultDTO> Items { get; set; } = new ObservableCollection<KhoResultDTO>();


        public ObservableCollection<Starff> Staffs { get; private set; } = new ObservableCollection<Starff>();

        public ObservableCollection<Customer> Customers { get; private set; } = new ObservableCollection<Customer>();


        public Customer SelectedCustomer { get; set; }

        public Starff SelectedNhanVien { get; set; }


        private ObservableCollection<TagInfoViewModel> _TagInfoList = new ObservableCollection<TagInfoViewModel>();
        public ObservableCollection<TagInfoViewModel> TagInfoList { get { return _TagInfoList; } set { SetProperty(ref _TagInfoList, value); } }

        private System.Collections.Generic.SortedDictionary<string, int> TagInfoListSpeedup = new SortedDictionary<string, int>();


        public bool _InventoryScanning = false;

        private string _startInventoryButtonText = "Scan";
        public string startInventoryButtonText { get { return _startInventoryButtonText; } }

        private int _ListViewRowHeight = -1;
        public int ListViewRowHeight { get { return _ListViewRowHeight; } }

        DateTime InventoryStartTime;

        bool _tagCount = false;

        bool _waitingRFIDIdle = false;

        // Tag Counter for Inventory Alert
        uint _tagCount4Display = 0;
        uint _tagCount4BeepSound = 0;
        uint _newtagCount4BeepSound = 0;
        uint _newtagCount4Vibration = 0;
        bool _Vibrating = false;
        uint _noNewTag = 0;
        uint _newTagPerSecond = 0;

        bool _cancelVoltageValue;

        private double _InventoryTime = 0;
        public string InventoryTime { get { return ((uint)_InventoryTime).ToString() + "s"; } }

        public string _DebugMessage = "";
        public string DebugMessage { get { return _DebugMessage; } }

        private string _numberOfTagsText = "     0 tags";
        public string numberOfTagsText { get { return _numberOfTagsText; } }

        private string _tagPerSecondText = "0 tags/s     ";
        public string tagPerSecondText { get { return _tagPerSecondText; } }


        private void SaveClick()
        {
            if (_InventoryScanning)
            {
                _userDialogs.ShowError("Please stop the scanning", 2000);
                return;
            }

            if (SelectedNhanVien == null || SelectedCustomer == null)
            {
                _userDialogs.ShowError("Please choose Employee and Customer", 2000);
                return;
            }
            if (this.Items.Count == 0)
            {
                _userDialogs.ShowError("Please add product to sale", 2000);
                return;
            }

            Device.BeginInvokeOnMainThread(() => this._userDialogs.ShowLoading("Loading ...", MaskType.Black));
            Task.Run(() =>
            {
                //var step = 100 / this.Items.Count;
                foreach (var o in this.Items)
                {
                    //progress.PercentComplete += step;
                    KhoDTO kho = _apiClient.XuatKho(new XuatkhoDTO()
                    {
                        NhanVien = SelectedNhanVien.Code,
                        KhachHang = SelectedCustomer.Code,
                        Rfid = o.RfId
                    });


                }
                lock (fromServer)
                {
                    var all = Items.Select(e => e.RfId.ToUpper()).ToList();
                    var removed = fromServer.Where(e => !all.Contains(e.RfId.ToUpper())).ToList();

                    foreach(var o in removed)
                    {
                        fromServer.Remove(o);
                    }


                }
                lock (this.Items)
                {
                    Items.Clear();
                }
                _TotalAmountt = "0";
                _TotalQuantity = "0";
                RaisePropertyChanged(() => TotalAmount);
                RaisePropertyChanged(() => TotalQuantity);

            }).ContinueWith(result => Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.HideLoading()));

        }

        void InventorySetting()
        {
            BleMvxApplication._reader.rfid.Options.TagRanging.flags = CSLibrary.Constants.SelectFlags.ZERO;

            // Setting 1

            BleMvxApplication._reader.rfid.SetInventoryTimeDelay((uint)BleMvxApplication._config.RFID_InventoryDelayTime);
            BleMvxApplication._reader.rfid.SetInventoryCycleDelay(BleMvxApplication._config.RFID_InventoryCycleDelayTime);
            BleMvxApplication._reader.rfid.SetInventoryDuration((uint)BleMvxApplication._config.RFID_DWellTime);
            BleMvxApplication._reader.rfid.SetPowerLevel((uint)BleMvxApplication._config.RFID_Power);

            // Setting 3
            BleMvxApplication._reader.rfid.SetDynamicQParms(BleMvxApplication._config.RFID_DynamicQParms);

            // Setting 4
            BleMvxApplication._reader.rfid.SetFixedQParms(BleMvxApplication._config.RFID_FixedQParms);

            // Setting 2
            BleMvxApplication._reader.rfid.SetOperationMode(BleMvxApplication._config.RFID_OperationMode);
            BleMvxApplication._reader.rfid.SetTagGroup(BleMvxApplication._config.RFID_TagGroup);
            BleMvxApplication._reader.rfid.SetCurrentSingulationAlgorithm(BleMvxApplication._config.RFID_Algorithm);
            BleMvxApplication._reader.rfid.SetCurrentLinkProfile(BleMvxApplication._config.RFID_Profile);

            // Select Criteria filter
            if (BleMvxApplication._PREFILTER_Enable)
            {
                BleMvxApplication._reader.rfid.Options.TagSelected.flags = CSLibrary.Constants.SelectMaskFlags.ENABLE_TOGGLE;
                BleMvxApplication._reader.rfid.Options.TagSelected.bank = CSLibrary.Constants.MemoryBank.EPC;
                BleMvxApplication._reader.rfid.Options.TagSelected.epcMask = new CSLibrary.Structures.S_MASK(BleMvxApplication._PREFILTER_MASK_EPC);
                BleMvxApplication._reader.rfid.Options.TagSelected.epcMaskOffset = 0;
                BleMvxApplication._reader.rfid.Options.TagSelected.epcMaskLength = (uint)(BleMvxApplication._PREFILTER_MASK_EPC.Length) * 4;
                BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_PREFILTER);

                BleMvxApplication._reader.rfid.Options.TagRanging.flags |= CSLibrary.Constants.SelectFlags.SELECT;
            }

            // Post Match Criteria filter
            if (BleMvxApplication._POSTFILTER_MASK_Enable)
            {
                BleMvxApplication._reader.rfid.Options.TagSelected.epcMask = new CSLibrary.Structures.S_MASK(BleMvxApplication._POSTFILTER_MASK_EPC);

                CSLibrary.Structures.SingulationCriterion[] sel = new CSLibrary.Structures.SingulationCriterion[1];
                sel[0] = new CSLibrary.Structures.SingulationCriterion();
                sel[0].match = BleMvxApplication._POSTFILTER_MASK_MatchNot ? 0U : 1U;
                sel[0].mask = new CSLibrary.Structures.SingulationMask(BleMvxApplication._POSTFILTER_MASK_Offset, (uint)(BleMvxApplication._POSTFILTER_MASK_EPC.Length * 4), BleMvxApplication._reader.rfid.Options.TagSelected.epcMask.ToBytes());
                BleMvxApplication._reader.rfid.SetPostMatchCriteria(sel);
                BleMvxApplication._reader.rfid.Options.TagRanging.flags |= CSLibrary.Constants.SelectFlags.POSTMATCH;
            }

            // Multi bank inventory
            BleMvxApplication._reader.rfid.Options.TagRanging.multibanks = 0;
            BleMvxApplication._reader.rfid.Options.TagRanging.compactmode = true;
            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_PRERANGING);

            //ShowDialog("Configuring RFID");
        }

        public override void Resume()
        {
            base.Resume();
            //SetEvent(true);
            BleMvxApplication._reader.rfid.OnAsyncCallback += new EventHandler<CSLibrary.Events.OnAsyncCallbackEventArgs>(TagInventoryEvent);
            BleMvxApplication._reader.rfid.OnStateChanged += new EventHandler<CSLibrary.Events.OnStateChangedEventArgs>(StateChangedEvent);


            // Key Button event handler
            BleMvxApplication._reader.notification.OnKeyEvent += new EventHandler<CSLibrary.Notification.HotKeyEventArgs>(HotKeys_OnKeyEvent);
            //BleMvxApplication._reader.notification.OnVoltageEvent += new EventHandler<CSLibrary.Notification.VoltageEventArgs>(VoltageEvent);

            BleMvxApplication._reader.barcode.FastBarcodeMode(false);



        }

        public override void Suspend()
        {
            _InventoryScanning = false;
            StopInventory();
            ClassBattery.SetBatteryMode(ClassBattery.BATTERYMODE.IDLE);
            if (BleMvxApplication._config.RFID_Vibration)
                BleMvxApplication._reader.barcode.VibratorOff();
            BleMvxApplication._reader.rfid.OnAsyncCallback -= new EventHandler<CSLibrary.Events.OnAsyncCallbackEventArgs>(TagInventoryEvent);
            BleMvxApplication._reader.rfid.OnStateChanged -= new EventHandler<CSLibrary.Events.OnStateChangedEventArgs>(StateChangedEvent);



            // Key Button event handler
            BleMvxApplication._reader.notification.OnKeyEvent -= new EventHandler<CSLibrary.Notification.HotKeyEventArgs>(HotKeys_OnKeyEvent);
            //BleMvxApplication._reader.notification.OnVoltageEvent -= new EventHandler<CSLibrary.Notification.VoltageEventArgs>(VoltageEvent);

            BleMvxApplication._reader.barcode.FastBarcodeMode(false);

            // don't turn off event handler is you need program work in sleep mode.
            //SetEvent(false);
            base.Suspend();
        }

        protected override void InitFromBundle(IMvxBundle parameters)
        {
            base.InitFromBundle(parameters);
        }


        void TagInventoryEvent(object sender, CSLibrary.Events.OnAsyncCallbackEventArgs e)
        {
            if (e.type != CSLibrary.Constants.CallbackType.TAG_RANGING)
                return;

            if (_waitingRFIDIdle) // ignore display tags
                return;

            InvokeOnMainThread(() =>
            {
                _tagCount4Display++;
                _tagCount4BeepSound++;

                if (_tagCount4BeepSound == 1)
                {
                    if (BleMvxApplication._config.RFID_InventoryAlertSound)
                    {
                        if (_newtagCount4BeepSound > 0)
                            Xamarin.Forms.DependencyService.Get<ISystemSound>().SystemSound(3);
                        else
                            Xamarin.Forms.DependencyService.Get<ISystemSound>().SystemSound(2);
                        _newtagCount4BeepSound = 0;
                    }
                }
                else if (_tagCount4BeepSound >= 40) // from 5
                    _tagCount4BeepSound = 0;

                AddOrUpdateTagData(e.info);
            });
        }

        private void AddOrUpdateTagData(CSLibrary.Structures.TagCallbackInfo info)
        {
            InvokeOnMainThread(() =>
            {
               lock (TagInfoList)
                {
                    string epcstr = info.epc.ToString();

                    try
                    {
                        if (TabIndex == 0)
                        {
                            TagInfoListSpeedup.Add(epcstr, TagInfoList.Count);
                            TagInfoViewModel item = new TagInfoViewModel();

                            item.timeOfRead = DateTime.Now;
                            item.EPC = info.epc.ToString();
                            item.Bank1Data = CSLibrary.Tools.Hex.ToString(info.Bank1Data);
                            item.Bank2Data = CSLibrary.Tools.Hex.ToString(info.Bank2Data);
                            item.RSSI = info.rssi;
                            //item.Phase = info.phase;
                            //item.Channel = (byte)info.freqChannel;
                            item.PC = info.pc.ToUshorts()[0];

                            //TagInfoList.Add(item);
                            TagInfoList.Insert(0, item);

                        }

                        var dto = this.fromServer.Where(e => e.RfId.Equals(info.epc.ToString(), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                        if (dto != null && TabIndex == 0)
                        {

                            this.Items.Add(dto);
                            ObservableCollection<KhoResultDTO> ListOut = new ObservableCollection<KhoResultDTO>(this.Items.OrderBy(x => x.Name));

                            _TotalAmountt = (Convert.ToInt32(
                                                        String.IsNullOrEmpty(_TotalAmountt) ? "0" : _TotalAmountt.Replace(",", "")) + dto.GiaBan).ToString("#,###");
                            _TotalQuantity = (Convert.ToInt32(String.IsNullOrEmpty(_TotalQuantity) ? "0" : TotalQuantity) + 1).ToString();

                            RaisePropertyChanged(() => TotalAmount);
                            RaisePropertyChanged(() => TotalQuantity);

                            InvokeOnMainThread(() =>
                            {

                                lock (Items)
                                {
                                    Items.Clear();
                                    }
                                });

                           foreach (var v in ListOut)
                            {
                                this.Items.Add(v);
                            }
                            //this.Items.Add(dto);
                            //this.Items.OrderBy(a => a.Name);
                            //this.Items = this.Items.OrderBy(a => a.Name).ToList<KhoResultDTO>;

                            //this.Items = itemTemp;
                            //this.Items.Clear();
                            //this.Items = new ObservableCollection<KhoResultDTO>(itemTemp.OrderByDescending(x => x.Name));
                        }
                        else if(dto != null && !dto.KiemKe && TabIndex == 1)
                        {
                            dto.KiemKe = true;
                            _apiClient.KiemOk(dto.RfId);
                            updateCheckList();
                        }


                        _newtagCount4BeepSound++;
                        _newtagCount4Vibration++;
                        _newTagPerSecond++;



                        //_newTag = true;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error:" + ex.StackTrace);
                        int index;

                        if (TagInfoListSpeedup.TryGetValue(epcstr, out index))
                        {
                            index = TagInfoList.Count - index;
                            index--;

                            TagInfoList[index].Bank1Data = CSLibrary.Tools.Hex.ToString(info.Bank1Data);
                            TagInfoList[index].Bank2Data = CSLibrary.Tools.Hex.ToString(info.Bank2Data);
                            TagInfoList[index].RSSI = info.rssi;
                        }
                        else
                        {
                            // error found epc
                        }

                    }

                }
            });
        }


        void StateChangedEvent(object sender, CSLibrary.Events.OnStateChangedEventArgs e)
        {
            //InvokeOnMainThread(() =>
            //{
            switch (e.state)
            {
                case CSLibrary.Constants.RFState.IDLE:
                    ClassBattery.SetBatteryMode(ClassBattery.BATTERYMODE.IDLE);
                    _cancelVoltageValue = true;
                    _waitingRFIDIdle = false;
                    switch (BleMvxApplication._reader.rfid.LastMacErrorCode)
                    {
                        case 0x00:  // normal end
                            break;

                        case 0x0309:    // 
                            _userDialogs.Alert("Too near to metal, please move CS108 away from metal and start inventory again.");
                            break;

                        default:
                            _userDialogs.Alert("Mac error : 0x" + BleMvxApplication._reader.rfid.LastMacErrorCode.ToString("X4"));
                            break;
                    }

                    //InventoryStopped();
                    break;
            }
            //});
        }


        void StartInventoryClick()
        {
            if (!_InventoryScanning)
            {
                StartInventory();
            }
            else
            {
                StopInventory();
            }
        }

        void StartInventory()
        {
            if (BleMvxApplication._reader.BLEBusy)
            {
                _userDialogs.ShowSuccess("Configuring Reader, Please Wait", 1000);
                return;
            }

            StartTagCount();
            _InventoryScanning = true;
            _startInventoryButtonText = "Stop";

            _ListViewRowHeight = 40 + (int)(BleMvxApplication._reader.rfid.Options.TagRanging.multibanks * 10);
            RaisePropertyChanged(() => ListViewRowHeight);

            InventoryStartTime = DateTime.Now;

            _Vibrating = false;
            _noNewTag = 0;
            if (BleMvxApplication._config.RFID_Vibration && BleMvxApplication._config.RFID_VibrationTag)
                BleMvxApplication._reader.barcode.VibratorOn(CSLibrary.BarcodeReader.VIBRATORMODE.INVENTORYON, BleMvxApplication._config.RFID_VibrationTime);

            BleMvxApplication._reader.rfid.StartOperation(CSLibrary.Constants.Operation.TAG_EXERANGING);
            ClassBattery.SetBatteryMode(ClassBattery.BATTERYMODE.INVENTORY);
            _cancelVoltageValue = true;

            RaisePropertyChanged(() => startInventoryButtonText);
        }
        void StartTagCount()
        {
            _tagCount = true;

            _tagCount4Display = 0;
            _tagCount4BeepSound = 0;
            _newtagCount4BeepSound = 0;
            //_tagCount4Vibration = 0;
            _newtagCount4Vibration = 0;

            // Create a timer that waits one second, then invokes every second.
            Xamarin.Forms.Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
            {
                if (BleMvxApplication._config.RFID_Vibration && !BleMvxApplication._config.RFID_VibrationTag)
                {
                    if (_newtagCount4Vibration > 0)
                    {
                        _newtagCount4Vibration = 0;
                        _noNewTag = 0;
                        if (!_Vibrating)
                        {
                            _Vibrating = true;
                            BleMvxApplication._reader.barcode.VibratorOn(CSLibrary.BarcodeReader.VIBRATORMODE.INVENTORYON, BleMvxApplication._config.RFID_VibrationTime);
                        }
                    }
                    else
                    {
                        if (_Vibrating)
                        {
                            _noNewTag++;

                            if (_noNewTag > BleMvxApplication._config.RFID_VibrationWindow)
                            {
                                _Vibrating = false;
                                BleMvxApplication._reader.barcode.VibratorOff();
                            }
                        }
                    }
                }

                _InventoryTime = (DateTime.Now - InventoryStartTime).TotalSeconds;
                RaisePropertyChanged(() => InventoryTime);

                _DebugMessage = CSLibrary.InventoryDebug._inventoryPacketCount.ToString() + " OK, " + CSLibrary.InventoryDebug._inventorySkipPacketCount.ToString() + " Fail";
                RaisePropertyChanged(() => DebugMessage);

                _tagCount4BeepSound = 0;

                //_numberOfTagsText = "  " + newTagPerSecond.ToString() + @"\" + _TagInfoList.Count.ToString() + " tags";
                _numberOfTagsText = "     " + _TagInfoList.Count.ToString() + " tags";
                RaisePropertyChanged(() => numberOfTagsText);

                _tagPerSecondText = _newTagPerSecond.ToString() + @"\" + _tagCount4Display.ToString() + " tags/s     ";
                //_tagPerSecondText = _tagCount4Display.ToString() + " tags/s     ";
                RaisePropertyChanged(() => tagPerSecondText);
                _tagCount4Display = 0;
                _newTagPerSecond = 0;

                if (_tagCount)
                    return true;

                return false;
            });
        }

        async void StopInventory()
        {
            BleMvxApplication._reader.rfid.StopOperation();
            if (BleMvxApplication._config.RFID_Vibration)
                BleMvxApplication._reader.barcode.VibratorOff();
            _waitingRFIDIdle = true;
            _InventoryScanning = false;
            _tagCount = false;
            _startInventoryButtonText = "Scan";
            RaisePropertyChanged(() => startInventoryButtonText);
        }

        void StopInventoryClick()
        {
            BleMvxApplication._reader.rfid.StopOperation();
            _Vibrating = false;
            _waitingRFIDIdle = true;
        }

        private void ClearClick()
        {
            InvokeOnMainThread(() =>
            {
                lock (TagInfoList)
                {
                    _InventoryTime = 0;
                    RaisePropertyChanged(() => InventoryTime);

                    _DebugMessage = "";
                    RaisePropertyChanged(() => DebugMessage);

                    TagInfoList.Clear();
                    TagInfoListSpeedup.Clear();
                    _numberOfTagsText = "     " + _TagInfoList.Count.ToString() + " tags";
                    RaisePropertyChanged(() => numberOfTagsText);

                    _tagCount4Display = 0;
                    _tagPerSecondText = _tagCount4Display.ToString() + " tags/s     ";

                    RaisePropertyChanged(() => tagPerSecondText);
                }
                lock (Items)
                {
                    Items.Clear();

                }
                _TotalAmountt = "0";
                _TotalQuantity = "0";
                RaisePropertyChanged(() => TotalAmount);
                RaisePropertyChanged(() => TotalQuantity);
            });
        }

        #endregion

        #region Key_event

        void HotKeys_OnKeyEvent(object sender, CSLibrary.Notification.HotKeyEventArgs e)
        {
            //Page currentPage;

            Trace.Message("Receive Key Event");

            // try to get current page

            if (e.KeyCode == CSLibrary.Notification.Key.BUTTON)
            {
                if (e.KeyDown)
                {
                    if (!_InventoryScanning)
                        StartInventory();
                }
                else
                {
                    StopInventory();
                }
            }

        }
        #endregion
    }

   
}
