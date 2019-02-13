using System.Windows.Forms;
using RobotOM;

namespace SamplePlugin
{
    public class SamplePlugin : IRobotAddIn
    {
        public double GetExpectedVersion()
        {
            return 30.0;
        }

        public bool Connect(RobotApplication robotApp, int addInId, bool firstTime)
        {
            System.Diagnostics.Debugger.Launch();

            Robapp = robotApp;
            AddInId = addInId;

            return true;
        }

        public int AddInId { get; set; }

        public RobotApplication Robapp { get; set; }

        public int InstallCommands(RobotCmdList cmdList)
        {
            try
            {
                cmdList.New(1, "Hello World");
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public bool Disconnect()
        {
            return true;
        }

        public void DoCommand(int cmdId)
        {
            if (cmdId == 1)
            {
                MessageBox.Show("Hello World");
            }
        }
    }

    // public interface IRobotAddIn;
    //Definition of the Robot program extension. 
    //This interface must implement each component that is integrated 
    //with the Robot program and that extends its functionality

    public class CadsRebarApplication
    {
        public IRobotCmdInfo Robcmdinfo;
        public int AddInId;
        public RobotApplication Robapp;
        public RobotCmdList CmdList;
        public IRobotAddInMngr AddInMngr = default(IRobotAddInMngr);
        public IRobotAddInRegistrar Reg = default(IRobotAddInRegistrar);

        //command list
        public void Commandlist()
        {
            long num = CmdList.New(1, "CADS Rebar");
            Robcmdinfo = CmdList.Get((int)num);
            Robcmdinfo.MenuEnabled = true;
            Robcmdinfo.Id = 1;
            Robcmdinfo.Name = "CADS Rebar";
        }

        //general dll data for registry
        public void Programdata()
        {
            Reg.Guid = "BB9150B2-6D89-4946-84EB-7A864F4F0B84";
            //GUID identifier of the main extension component (presented interchangeably with Progld) 
            Reg.ProductName = "CADS Rebar";
            //Extension name
            Reg.ProgId = "CadsRebarApplication";
            //Identifier of the main extension component (presented interchangeably with Guid)
            Reg.ProviderName = "n/a";
            //Name of the extension supplier 
            Reg.InstallMenu("CADS Rebar", CmdList);
            //Function adds menu with the specified name to all Robot program views. 
            //The commands specified in the list will are the menu options. 
            //They will be supported by the extension whose Progld or Guid have been set earlier. 
            //If identifier of the extension supporting commands from the list is not specified, 
            //the function returns zero value (False).
        }

        public bool Connect(RobotApplication robotApp, int addInId, bool firstTime)
        //Function wil be called up by the Robot application when the extension is connected to the program: 
        //for the first time - upon user's request or during first activation of the program after installing the extension;
        //for the next time - while activating the Robot program again. 
        //The extension should remember the access interface to the Robot application
        //(RobotApplication) so that the extension can use its functionality. The extension, while being connected, 
        //is ascribed its identifier which should be remembered since it will be needed during subsequent communication 
        //with the Robot program. While connecting the extension to the application for the first time it is possible 
        //to perform additional initializing operations. If all is performed correctly, 
        //then function should return a value different from zero (True).

        {
            try
            {
                Robapp = robotApp;
                AddInId = addInId;
                Commandlist();

                if (firstTime == true)
                {
                    //Programdata();
                    //Reg.Register();
                    // Function saves information about the extension to the register. 
                    //If registration is performed successfully, 
                    //a value different from zero is returned (True). 
                    //Registration failure may result from lack of settings concerning suppler name 
                    //or product as well as identifier (Guid or Progld).
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool Disconnect()
        //This function will be called up before the extension is disconnected with the Robot program 
        //(e.g. while exiting the application or upon user's request who wants no longer to use the extension).
        // If there is any reason for the extension not to be disconnected with the program at the given moment, 
        //the function should return zero value (False). 
        {
            Programdata();
            //REG.Unregister();
            //Function deletes information about the extension from the register.
            return false;
        }

        public void DoCommand(int cmdId)
        //Function will be called up by the Robot program if a user selects the menu option linked with the 
        //command supported by this extension. 
        {
            if (cmdId == 1) MessageBox.Show("CADS Rebar");
        }

        public double GetExpectedVersion()
        // Function should return the number of the RobotOS model version which is used by the extension. 
        //It will enable warning a user in a situation when the extension that is being installed by the 
        //user expects a newer Robot program version than the one currently used by the user. .
        {
            return 9.5; //RSA 2009
        }

        public int InstallCommands(RobotCmdList cmdList)
        //Function will be called up by the application to obtain information about the commands that are made 
        //available by the extension. List specified by the parameter should be filled out with the information 
        //about all the commands defined by the extension. If the operation is performed successfully, 
        //the function should return a value different from zero (True). 
        {
            try
            {
                for (int i = 1; i <= cmdList.Count; i++)
                {
                    AddInMngr.InstallCommand(AddInId, i);
                    //Add-ins Manager keeps tracks to all add-ins currently registered in Robot. 
                }
                return 1;
            }
            catch
            {
                return 0;
            }


        }
    }
}
