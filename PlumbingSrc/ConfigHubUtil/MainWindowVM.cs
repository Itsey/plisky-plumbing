using Plisky.Plumbing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ConfigHubUtil {
    public class MainWindowVM : BaseVM {
        private string actualConfigFilename;
        private ICommand newFileCommand;
        private ICommand addNewSettingCommand;
         
        public string PathToConfigFile {
            get {
                return actualConfigFilename;
            }
            set {
                actualConfigFilename = value;
            }
        }

        public string SelectedSettingName { get; set; }
        public string SelectedSettingValue { get; set; }

        public ICommand CreateNewFileCommand {
            get {
                return newFileCommand;
            }
        }
        public ICommand AddNewSettingCommand {
            get {
                return addNewSettingCommand;
            }
        }


        public ObservableCollection<ConfigHubSetting> AllSettings { get; set; }
        public bool CanCreateNewFile { get; private set; }
        public bool CanAddNewSetting { get; private set; }

        public MainWindowVM() {
            AllSettings = new ObservableCollection<ConfigHubSetting>();
            CreateCommands();
        }

        private void CreateCommands() {
            newFileCommand = new RelayCommand(param => this.PerformCreateNewFile(param), param => this.CanCreateNewFile);
            addNewSettingCommand = new RelayCommand(param => this.PerformAddNewSetting(param), param => this.CanAddNewSetting);
        }

        private void PerformAddNewSetting(object param) {
            throw new NotImplementedException();
        }

        private void PerformCreateNewFile(object param) {
            throw new NotImplementedException();
        }
    }
}
