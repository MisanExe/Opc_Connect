using CodeSysAppNs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Opc.UaFx.Client;
using Opc.UaFx;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using OPCua_APP.ViewModel.PopupViewModels;

namespace OPCua_APP.ViewModel
{
    public class MainWindowViewModel : ObservableObject
    {
        private ObservableCollection<string> _tagList = new ObservableCollection<string>();
        public ObservableCollection<string> TagList { get => _tagList; set => SetProperty(ref _tagList, value); }

        private Dictionary<int, bool>  _machineState = new Dictionary<int, bool>();
        public Dictionary<int, bool> MachineState { get => _machineState; set => SetProperty(ref _machineState, value); }

        private string _serverURL = Properties.Settings.Default.ServerURL;
        public string ServerURL { get => _serverURL; set => SetProperty(ref _serverURL, value); }

        private string _programName = Properties.Settings.Default.ProgramName;
        public string ProgramName { get => _programName; set => SetProperty(ref _programName, value); }

        private bool _connectionState;
        public bool ConnectionState { get => _connectionState; set => SetProperty(ref _connectionState, value); }

        private CodeSysApp app = new CodeSysApp();

        public RelayCommand ConnectCommand { get; set; }

        private void HandleDataChanged(
      object sender,
      OpcDataChangeReceivedEventArgs e)
        {
            // Your code to execute on each data change.
            // The 'sender' variable contains the OpcMonitoredItem with the NodeId.
            OpcMonitoredItem item = (OpcMonitoredItem)sender;
            //bool? result = (bool?)app.ConvertOpcValue(app.ReadTag("UA_Proceed"));

            int? state = (int?)app.ConvertOpcValue(e.Item.Value);

            app?.WriteTag("UA_Proceed", true);
            if (state != null)
            {
                app.MachineStateList[state.Value + 1] = true;
                Application.Current.Dispatcher.Invoke(() => MachineState = new(app.MachineStateList));
            }
            //reset state
            if (app.MachineStateList[9])
            {
                app.ClearState();
            }



            Debug.WriteLine(
                    "Data Change from NodeId '{0}': {1}",
                    item.NodeId,
                    e.Item.Value);
        }

        public MainWindowViewModel()
        {
            ConnectCommand = new RelayCommand(connect);
            MachineState[1] = false;
            MachineState[2] = false;
            MachineState[3] = false;
            MachineState[4] = false;
            MachineState[5] = false;
            MachineState[6] = false;
            MachineState[7] = false;
            MachineState[8] = false;
            MachineState[9] = false;
            MachineState[10] = false;
        }

        private void saveSettings()
        {
            Properties.Settings.Default.ServerURL = ServerURL;
            Properties.Settings.Default.ProgramName =  ProgramName;
            Properties.Settings.Default.Save();
        }

        private async void connect()
        {
            if (string.IsNullOrEmpty(ServerURL) && string.IsNullOrEmpty(ProgramName))
            {
                await MaterialDesignThemes.Wpf.DialogHost.Show(new DialogViewModel { Label = "Server url and Program name required" }, "RootDialogHost");
                return;
            }

        _=    Task.Run(() => {

                //string opcUrl = "opc.tcp://192.168.100.42:4840";
                //PLC_PRG
                saveSettings();
                var nodeId = "ns=4;s=|var|CODESYS Control for Raspberry Pi 64 SL.Application.PLC_PRG.wizzy";

                try
                {
                    using (var client = new OpcClient(ServerURL))
                    {
                        client.Connect();
                        var root_info = client.BrowseNode(OpcObjectTypes.ObjectsFolder);

                        app = new CodeSysApp(client) { ProgramName = ProgramName, RootNodeInfo = root_info };
                        client.Connected += Client_Connected;
                        client.Disconnected += Client_Disconnected;
                        app.Run();

                        var state = client.State;
                        app.ReadTag("iState");
                    var tags = app.TagTuple.Values.Select(obj=>obj.Name.Value).ToList();
                    TagList = new ObservableCollection<string>(tags);
                    if (state == Opc.UaFx.Client.OpcClientState.Connected)
                            Application.Current.Dispatcher.Invoke(() => { 
                                ConnectionState = true;
                                //TagList = new ObservableCollection<string>(app.TagNodes.);
                            });
                        app.SubscribeTo("iState", HandleDataChanged);
                        app.WriteTag("UA_PB_Start", true);
                        while (ConnectionState == true)
                        {

                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

            });
        }

        private void Client_Disconnected(object? sender, EventArgs e)
        {
            ConnectionState = false;
        }

        private void Client_Connected(object? sender, EventArgs e)
        {
            ConnectionState = true;
        }
    }
}
